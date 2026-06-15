// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.Services.Configurations;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Marketplace.SaaS.Accelerator.Services.Services;

// ---------------------------------------------------------------------------
// FetchResult and SubscriptionFailure — value objects returned by the pipeline
// ---------------------------------------------------------------------------

/// <summary>
/// Summary of a single <see cref="SubscriptionFetchPipeline.ExecuteAsync"/> run.
/// </summary>
public sealed class FetchResult
{
    /// <summary>Total number of subscriptions returned by the Marketplace API.</summary>
    public int Total { get; init; }

    /// <summary>Number of subscriptions processed without error.</summary>
    public int Succeeded { get; init; }

    /// <summary>Number of subscriptions that could not be processed due to an error.</summary>
    public int Failed { get; init; }

    /// <summary>Wall-clock duration of the fetch operation in milliseconds.</summary>
    public long DurationMs { get; init; }

    /// <summary>Per-subscription failures recorded during the run.</summary>
    public IReadOnlyList<SubscriptionFailure> Failures { get; init; } =
        Array.Empty<SubscriptionFailure>();
}

/// <summary>
/// Details about a single per-subscription failure within a fetch run.
/// </summary>
public sealed class SubscriptionFailure
{
    /// <summary>The Marketplace subscription identifier that failed.</summary>
    public Guid SubscriptionId { get; init; }

    /// <summary>The name of the operation that failed (e.g. "GetAllPlansForSubscriptionAsync").</summary>
    public string Operation { get; init; }

    /// <summary>The error message from the exception.</summary>
    public string ErrorMessage { get; init; }
}

// ---------------------------------------------------------------------------
// SubscriptionFetchPipeline
// ---------------------------------------------------------------------------

/// <summary>
/// Orchestrates a full Marketplace-to-database subscription sync:
///
/// <list type="bullet">
///   <item>Fetches all subscriptions from the Marketplace API (bulk list).</item>
///   <item>For each active subscription, fetches available plans concurrently
///         under a <see cref="SemaphoreSlim"/> bound by
///         <see cref="MarketplaceResilienceOptions.MaxConcurrentPlanFetches"/>.</item>
///   <item>Upserts offers, plans, users, subscriptions, and writes audit-log
///         rows for status / plan / quantity changes.</item>
///   <item>Isolates per-subscription errors: a failure for one subscription
///         is logged and the remaining subscriptions continue processing
///         (Requirement 2.4).</item>
///   <item>Returns a <see cref="FetchResult"/> summarising total, succeeded,
///         failed, duration, and a list of per-subscription failures.</item>
/// </list>
///
/// This service is registered as scoped and is shared by
/// <c>HomeController.FetchAllSubscriptions</c> (task 3.6) and the hosted
/// background sync service (task 3.8).
/// </summary>
public sealed class SubscriptionFetchPipeline
{
    private readonly IFulfillmentApiService fulfillApiService;
    private readonly ISubscriptionsRepository subscriptionsRepository;
    private readonly IPlansRepository plansRepository;
    private readonly IOffersRepository offersRepository;
    private readonly IUsersRepository usersRepository;
    private readonly ISubscriptionLogRepository subscriptionLogRepository;
    private readonly MarketplaceResilienceOptions options;
    private readonly ILogger<SubscriptionFetchPipeline> logger;

    /// <summary>
    /// Initializes a new instance of <see cref="SubscriptionFetchPipeline"/>.
    /// </summary>
    public SubscriptionFetchPipeline(
        IFulfillmentApiService fulfillApiService,
        ISubscriptionsRepository subscriptionsRepository,
        IPlansRepository plansRepository,
        IOffersRepository offersRepository,
        IUsersRepository usersRepository,
        ISubscriptionLogRepository subscriptionLogRepository,
        IOptions<MarketplaceResilienceOptions> options,
        ILogger<SubscriptionFetchPipeline> logger)
    {
        this.fulfillApiService = fulfillApiService ?? throw new ArgumentNullException(nameof(fulfillApiService));
        this.subscriptionsRepository = subscriptionsRepository ?? throw new ArgumentNullException(nameof(subscriptionsRepository));
        this.plansRepository = plansRepository ?? throw new ArgumentNullException(nameof(plansRepository));
        this.offersRepository = offersRepository ?? throw new ArgumentNullException(nameof(offersRepository));
        this.usersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
        this.subscriptionLogRepository = subscriptionLogRepository ?? throw new ArgumentNullException(nameof(subscriptionLogRepository));
        this.options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Runs the full subscription sync pipeline and returns a summary.
    /// </summary>
    /// <param name="currentUserId">
    ///   The ID of the user initiating the sync (used for audit log rows and
    ///   as the <c>CreateBy</c> value on new entities).
    /// </param>
    /// <param name="ct">Cancellation token (hooked to the HTTP request or the hosted service's stop token).</param>
    /// <returns>A <see cref="FetchResult"/> describing the outcome of the sync.</returns>
    public async Task<FetchResult> ExecuteAsync(int currentUserId, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        var failures = new List<SubscriptionFailure>();

        // Build helper services scoped to the current user.
        var subscriptionService = new SubscriptionService(
            this.subscriptionsRepository,
            this.plansRepository,
            currentUserId);

        var userService = new UserService(this.usersRepository);

        // Step 1: Get all subscriptions from the Marketplace API.
        // The call is already wrapped by the Polly resilience pipeline via
        // FulfillmentApiServiceWithPolicy (task 3.3), so transient failures
        // are retried automatically and sustained failures open the circuit.
        // If the bulk list call fails after all retries (or the circuit is
        // open), we return an empty FetchResult rather than letting the
        // exception propagate — a failed bulk fetch is not a 5xx response
        // (Requirement 2.1 "SHALL retry ... before reporting failure").
        List<SubscriptionResult> subscriptions;
        try
        {
            subscriptions = await this.fulfillApiService
                .GetAllSubscriptionAsync()
                .ConfigureAwait(false);
        }
        catch (Exception bulkEx)
        {
            sw.Stop();
            var bulkPayload = System.Text.Json.JsonSerializer.Serialize(new
            {
                @event = "bulk_list_failure",
                operation = "GetAllSubscriptionAsync",
                errorMessage = bulkEx.Message,
                exceptionType = bulkEx.GetType().Name,
            });
            this.logger.LogError(bulkEx, "{StructuredPayload}", bulkPayload);

            // Surface as a zero-total result so the controller returns non-5xx.
            return new FetchResult
            {
                Total = 0,
                Succeeded = 0,
                Failed = 0,
                DurationMs = sw.ElapsedMilliseconds,
                Failures = Array.Empty<SubscriptionFailure>(),
            };
        }

        if (subscriptions == null || subscriptions.Count == 0)
        {
            sw.Stop();
            return new FetchResult
            {
                Total = 0,
                Succeeded = 0,
                Failed = 0,
                DurationMs = sw.ElapsedMilliseconds,
                Failures = Array.Empty<SubscriptionFailure>(),
            };
        }

        int total = subscriptions.Count;

        // Step 2a: Fetch plans concurrently, bounded by MaxConcurrentPlanFetches.
        // EF Core's DbContext is NOT thread-safe, so DB writes (step 2b) must be
        // serialized. We therefore split the work into two phases:
        //   Phase A (concurrent, semaphore-gated): Marketplace API plan fetches.
        //   Phase B (sequential):                  All DB upsert operations.
        //
        // This satisfies:
        //   - Requirement 2.2: fully async, no sync-over-async.
        //   - Requirement 2.4: per-subscription error isolation.
        //   - design.md: "Gate concurrent GetAllPlansForSubscriptionAsync calls
        //     with SemaphoreSlim(MaxConcurrentPlanFetches)".
        using var semaphore = new SemaphoreSlim(
            initialCount: Math.Max(1, this.options.MaxConcurrentPlanFetches));

        var planFetchResults = await FetchPlansForAllSubscriptionsAsync(
            subscriptions, semaphore, ct).ConfigureAwait(false);

        // Step 2b: Sequential DB upserts — one subscription at a time so the
        // shared DbContext is never accessed from concurrent threads.
        foreach (var (subscription, fetchedPlans, fetchError) in planFetchResults)
        {
            try
            {
                UpsertSubscription(
                    subscription,
                    fetchedPlans,
                    fetchError,
                    subscriptionService,
                    userService,
                    currentUserId);
            }
            catch (Exception ex)
            {
                // Log a structured JSON payload for every per-subscription failure
                // so operators can trace errors back to a specific subscription
                // (Requirement 2.8).
                var payload = JsonSerializer.Serialize(new
                {
                    @event = "per_subscription_failure",
                    operation = "UpsertSubscription",
                    subscriptionId = subscription.Id.ToString(),
                    errorMessage = ex.Message,
                    exceptionType = ex.GetType().Name,
                });

                this.logger.LogWarning(ex, "{StructuredPayload}", payload);

                failures.Add(new SubscriptionFailure
                {
                    SubscriptionId = subscription.Id,
                    Operation = "UpsertSubscription",
                    ErrorMessage = ex.Message,
                });
                // continue — other subscriptions must not be affected
            }
        }

        sw.Stop();

        int failed = failures.Count;
        int succeeded = total - failed;

        return new FetchResult
        {
            Total = total,
            Succeeded = succeeded,
            Failed = failed,
            DurationMs = sw.ElapsedMilliseconds,
            Failures = failures.AsReadOnly(),
        };
    }

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

    /// <summary>
    /// Phase A: concurrently fetch the available plans for every subscription,
    /// bounded by the supplied <paramref name="semaphore"/>.  Returns one tuple
    /// per subscription: the subscription itself, the fetched plans (or null if
    /// they were not needed / the fetch failed), and any exception that occurred.
    /// </summary>
    private async Task<IReadOnlyList<(SubscriptionResult subscription, List<PlanDetailResultExtension> plans, Exception error)>>
        FetchPlansForAllSubscriptionsAsync(
            List<SubscriptionResult> subscriptions,
            SemaphoreSlim semaphore,
            CancellationToken ct)
    {
        var results = new (SubscriptionResult, List<PlanDetailResultExtension>, Exception)[subscriptions.Count];

        await Task.WhenAll(subscriptions.Select(async (subscription, index) =>
        {
            // Unsubscribed subscriptions don't need a plan fetch from the API.
            if (subscription.SaasSubscriptionStatus == SubscriptionStatusEnum.Unsubscribed)
            {
                results[index] = (subscription, null, null);
                return;
            }

            await semaphore.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                var plans = await this.fulfillApiService
                    .GetAllPlansForSubscriptionAsync(subscription.Id)
                    .ConfigureAwait(false);
                results[index] = (subscription, plans, null);
            }
            catch (Exception ex)
            {
                results[index] = (subscription, null, ex);
            }
            finally
            {
                semaphore.Release();
            }
        })).ConfigureAwait(false);

        return results;
    }

    /// <summary>
    /// Phase B (called sequentially): perform all DB upsert operations for
    /// a single subscription, using the pre-fetched plans from Phase A.
    /// Mirrors the body of the original <c>HomeController.FetchAllSubscriptions</c>
    /// foreach with all <c>.GetAwaiter().GetResult()</c> calls replaced.
    /// </summary>
    private void UpsertSubscription(
        SubscriptionResult subscription,
        List<PlanDetailResultExtension> fetchedPlans,
        Exception planFetchError,
        SubscriptionService subscriptionService,
        UserService userService,
        int currentUserId)
    {
        // If the plan fetch itself failed, propagate the exception so the
        // outer try/catch records it as a per-subscription failure.
        if (planFetchError != null)
        {
            throw new InvalidOperationException(
                $"Plan fetch failed for subscription {subscription.Id}: {planFetchError.Message}",
                planFetchError);
        }

        var customerUserId = 0;
        var currentSubscription = subscriptionService
            .GetSubscriptionsBySubscriptionId(subscription.Id);

        // Step 2: Check if subscription exists in DB — create if it doesn't.
        if (currentSubscription.Name == null)
        {
            // Step 3: Add/Update the Offer.
            Guid offerId = this.offersRepository.Add(new Offers
            {
                OfferId = subscription.OfferId,
                OfferName = subscription.OfferId,
                UserId = currentUserId,
                CreateDate = DateTime.Now,
                OfferGuid = Guid.NewGuid(),
            });

            // Step 4: Add/Update the Plans.
            // For Unsubscribed subscriptions, only add the current plan from
            // subscription information (the ListAvailablePlans API is unavailable).
            if (subscription.SaasSubscriptionStatus == SubscriptionStatusEnum.Unsubscribed)
            {
                var planDetails = new PlanDetailResultExtension
                {
                    PlanId = subscription.PlanId,
                    DisplayName = subscription.PlanId,
                    Description = "",
                    OfferId = offerId,
                    PlanGUID = Guid.NewGuid(),
                    IsPerUserPlan = subscription.Quantity > 0,
                };
                subscriptionService.AddPlanDetailsForSubscription(planDetails);
            }
            else
            {
                // fetchedPlans was populated by the concurrent Phase A.
                if (fetchedPlans != null)
                {
                    fetchedPlans.ForEach(x =>
                    {
                        x.OfferId = offerId;
                        x.PlanGUID = Guid.NewGuid();
                    });
                    subscriptionService.AddUpdateAllPlanDetailsForSubscription(fetchedPlans);
                }
            }

            // Step 5: Add/Update the current user from Subscription information.
            customerUserId = userService.AddUser(new PartnerDetailViewModel
            {
                FullName = subscription.Beneficiary.EmailId,
                EmailAddress = subscription.Beneficiary.EmailId,
            });
        }

        // Step 6: Add / update the Subscription row (upsert).
        var subscriptionId = subscriptionService
            .AddOrUpdatePartnerSubscriptions(subscription, customerUserId);

        // Step 7: Write audit-log entries for detected status / plan / quantity changes.
        if (currentSubscription != null
            && subscription.SaasSubscriptionStatus.ToString()
                != currentSubscription.SubscriptionStatus.ToString())
        {
            this.subscriptionLogRepository.Save(new SubscriptionAuditLogs
            {
                Attribute = $"{Convert.ToString(SubscriptionLogAttributes.Status)}-Refresh",
                SubscriptionId = subscriptionId,
                NewValue = subscription.SaasSubscriptionStatus.ToString(),
                OldValue = currentSubscription.SubscriptionStatus.ToString(),
                CreateBy = currentUserId,
                CreateDate = DateTime.Now,
            });
        }

        if (currentSubscription != null
            && subscription.PlanId != currentSubscription.PlanId)
        {
            this.subscriptionLogRepository.Save(new SubscriptionAuditLogs
            {
                Attribute = $"{Convert.ToString(SubscriptionLogAttributes.Plan)}-Refresh",
                SubscriptionId = subscriptionId,
                NewValue = subscription.PlanId,
                OldValue = currentSubscription.PlanId,
                CreateBy = currentUserId,
                CreateDate = DateTime.Now,
            });
        }

        if (currentSubscription != null
            && subscription.Quantity != currentSubscription.Quantity)
        {
            this.subscriptionLogRepository.Save(new SubscriptionAuditLogs
            {
                Attribute = $"{Convert.ToString(SubscriptionLogAttributes.Quantity)}-Refresh",
                SubscriptionId = subscriptionId,
                NewValue = subscription.Quantity.ToString(),
                OldValue = currentSubscription.Quantity.ToString(),
                CreateBy = currentUserId,
                CreateDate = DateTime.Now,
            });
        }
    }
}
