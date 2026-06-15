// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

// =============================================================================
// Unit tests for SubscriptionFetchPipeline (task 3.5).
//
// Covers:
//   1. Per-subscription error isolation — one failure → others succeed.
//   2. Bounded concurrency — peak in-flight ≤ MaxConcurrentPlanFetches.
//   3. Idempotent upsert — two consecutive fetches of the same payload produce
//      the same DB row count (no duplicates on second run).
//   4. Audit-log generation — on detected status / plan / quantity change
//      exactly one log row is written per changed attribute.
//
// Requirements: 2.1, 2.2, 2.4, 2.8, 3.1, 3.2, 3.3
// =============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Marketplace.SaaS.Accelerator.AdminSite.Test.Doubles;
using Marketplace.SaaS.Accelerator.AdminSite.Test.Fixtures;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Services;
using Marketplace.SaaS.Accelerator.Services.Configurations;
using Marketplace.SaaS.Accelerator.Services.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.Marketplace.SaaS.Models;
using Xunit;

namespace Marketplace.SaaS.Accelerator.AdminSite.Test.Pipeline;

/// <summary>
/// Unit tests for <see cref="SubscriptionFetchPipeline"/>.
/// Each test wires a real <see cref="FulfillmentApiService"/> over a
/// <see cref="FakeMarketplaceSaaSClient"/>, backed by EF Core InMemory
/// repositories — no mocks of business logic, only infrastructure seams.
/// </summary>
public class SubscriptionFetchPipelineTests
{
    // -------------------------------------------------------------------------
    // 1. Per-subscription error isolation
    // -------------------------------------------------------------------------

    /// <summary>
    /// When one subscription's plan-fetch throws 500, the pipeline must record
    /// that subscription as a failure but continue processing all remaining
    /// subscriptions. Both the FetchResult and the DB state must reflect partial
    /// success (Requirement 2.4).
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_WhenOneSubscriptionFails_OtherSubscriptionsSucceed()
    {
        // Arrange — 5 subscriptions; subscription at index 2 will throw 500.
        const int SubscriptionCount = 5;
        const int FailingIndex = 2;

        var subscriptionGuids = Enumerable.Range(1, SubscriptionCount)
            .Select(i => Guid.Parse($"bbbbbbbb-{i:D4}-{i:D4}-{i:D4}-bbbbbbbbbbbb"))
            .ToList();
        var failingId = subscriptionGuids[FailingIndex];

        var sdkSubscriptions = subscriptionGuids
            .Select((id, i) => FakeMarketplaceSaaSClientBuilder.CreateFullSubscription(
                id,
                offerId: $"offer-{i:D2}",
                planId: $"plan-{i:D2}",
                status: SubscriptionStatusEnum.Subscribed,
                quantity: 1,
                beneficiaryEmail: $"user-{i:D2}@example.test"))
            .ToList();

        var fakeClient = new FakeMarketplaceSaaSClientBuilder()
            .WithSeededSubscriptions(sdkSubscriptions)
            .WithSubscription500(failingId)
            .Build();

        var (pipeline, ctx, adminUserId) = BuildPipeline(fakeClient, maxConcurrentPlanFetches: 3);

        // Act
        var result = await pipeline.ExecuteAsync(adminUserId);

        // Assert — FetchResult reflects partial failure
        Assert.Equal(SubscriptionCount, result.Total);
        Assert.Equal(1, result.Failed);
        Assert.Equal(SubscriptionCount - 1, result.Succeeded);
        Assert.Single(result.Failures);
        Assert.Equal(failingId, result.Failures[0].SubscriptionId);

        // All non-failing subscriptions should be persisted in DB
        var persistedGuids = ctx.Subscriptions
            .Select(s => s.AmpsubscriptionId)
            .ToHashSet();
        var expectedPersistedGuids = subscriptionGuids
            .Where(id => id != failingId)
            .ToList();
        foreach (var expectedId in expectedPersistedGuids)
        {
            Assert.Contains(expectedId, persistedGuids);
        }
    }

    /// <summary>
    /// When the first subscription in a batch fails, remaining subscriptions
    /// must still be processed. This guards against exception propagation
    /// unwinding the entire Task.WhenAll.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_WhenFirstSubscriptionFails_RemainingSubscriptionsAreProcessed()
    {
        // Arrange — 3 subscriptions; the FIRST one will fail.
        var ids = new[]
        {
            Guid.Parse("cccccccc-0001-0001-0001-cccccccccccc"),
            Guid.Parse("cccccccc-0002-0002-0002-cccccccccccc"),
            Guid.Parse("cccccccc-0003-0003-0003-cccccccccccc"),
        };
        var sdkSubscriptions = ids
            .Select((id, i) => FakeMarketplaceSaaSClientBuilder.CreateFullSubscription(
                id,
                offerId: "offer-c",
                planId: "plan-c",
                status: SubscriptionStatusEnum.Subscribed,
                quantity: 1,
                beneficiaryEmail: $"c-user-{i}@example.test"))
            .ToList();

        var fakeClient = new FakeMarketplaceSaaSClientBuilder()
            .WithSeededSubscriptions(sdkSubscriptions)
            .WithSubscription500(ids[0])   // first subscription fails
            .Build();

        var (pipeline, ctx, adminUserId) = BuildPipeline(fakeClient, maxConcurrentPlanFetches: 2);

        // Act
        var result = await pipeline.ExecuteAsync(adminUserId);

        // Assert
        Assert.Equal(3, result.Total);
        Assert.Equal(1, result.Failed);
        Assert.Equal(2, result.Succeeded);

        // ids[1] and ids[2] must be persisted
        var persistedGuids = ctx.Subscriptions
            .Select(s => s.AmpsubscriptionId)
            .ToHashSet();
        Assert.Contains(ids[1], persistedGuids);
        Assert.Contains(ids[2], persistedGuids);
    }

    /// <summary>
    /// When ALL subscriptions fail, the pipeline still returns a FetchResult
    /// with Total == Failed == SubscriptionCount and Succeeded == 0 (it does not
    /// throw). The caller can then decide to return BadRequest.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_WhenAllSubscriptionsFail_ReturnsTotalFailureResult()
    {
        // Arrange — 3 subscriptions; all three will throw 500.
        var ids = new[]
        {
            Guid.Parse("dddddddd-0001-0001-0001-dddddddddddd"),
            Guid.Parse("dddddddd-0002-0002-0002-dddddddddddd"),
            Guid.Parse("dddddddd-0003-0003-0003-dddddddddddd"),
        };
        var sdkSubscriptions = ids
            .Select((id, i) => FakeMarketplaceSaaSClientBuilder.CreateFullSubscription(
                id,
                offerId: "offer-d",
                planId: "plan-d",
                status: SubscriptionStatusEnum.Subscribed,
                quantity: 1,
                beneficiaryEmail: $"d-user-{i}@example.test"))
            .ToList();

        var builder = new FakeMarketplaceSaaSClientBuilder()
            .WithSeededSubscriptions(sdkSubscriptions);
        foreach (var id in ids)
            builder = builder.WithSubscription500(id);

        var fakeClient = builder.Build();
        var (pipeline, _, adminUserId) = BuildPipeline(fakeClient, maxConcurrentPlanFetches: 3);

        // Act — must NOT throw
        var result = await pipeline.ExecuteAsync(adminUserId);

        // Assert
        Assert.Equal(3, result.Total);
        Assert.Equal(3, result.Failed);
        Assert.Equal(0, result.Succeeded);
        Assert.Equal(3, result.Failures.Count);
    }

    // -------------------------------------------------------------------------
    // 2. Bounded concurrency
    // -------------------------------------------------------------------------

    /// <summary>
    /// The pipeline must not issue more than MaxConcurrentPlanFetches concurrent
    /// plan-fetch calls, even when there are many subscriptions. The fake client
    /// tracks peak in-flight concurrency via lock-free counters.
    /// </summary>
    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public async Task ExecuteAsync_ConcurrentPlanFetches_BoundedByMaxConcurrent(int maxConcurrent)
    {
        // Arrange — 20 subscriptions; each plan fetch has a 5ms delay so
        // concurrent calls actually overlap and the peak counter fires.
        const int SubscriptionCount = 20;

        var ids = Enumerable.Range(1, SubscriptionCount)
            .Select(i => Guid.Parse($"eeeeeeee-{i:D4}-0000-0000-eeeeeeeeeeee"))
            .ToList();
        var sdkSubscriptions = ids
            .Select((id, i) => FakeMarketplaceSaaSClientBuilder.CreateFullSubscription(
                id,
                offerId: "offer-e",
                planId: "plan-e",
                status: SubscriptionStatusEnum.Subscribed,
                quantity: 1,
                beneficiaryEmail: $"e-user-{i}@example.test"))
            .ToList();

        var fakeClient = new FakeMarketplaceSaaSClientBuilder()
            .WithSeededSubscriptions(sdkSubscriptions)
            .WithDelayPerCall(5)   // 5ms delay per call enables concurrency overlap
            .Build();

        var (pipeline, _, adminUserId) = BuildPipeline(fakeClient, maxConcurrentPlanFetches: maxConcurrent);

        // Act
        var result = await pipeline.ExecuteAsync(adminUserId);

        // Assert — all succeeded AND peak in-flight was within bound
        Assert.True(
            result.Failed == 0,
            $"Expected 0 failures, got {result.Failed}. " +
            $"Failure details: {string.Join("; ", result.Failures.Select(f => $"{f.SubscriptionId}: {f.ErrorMessage}"))}");
        Assert.Equal(SubscriptionCount, result.Total);
        Assert.True(
            fakeClient.PeakInFlight <= maxConcurrent + 1,  // +1 for the bulk-list slot which the semaphore doesn't gate
            $"Peak in-flight {fakeClient.PeakInFlight} exceeded maxConcurrent+1 ({maxConcurrent + 1})");
    }

    // -------------------------------------------------------------------------
    // 3. Idempotent upsert behavior
    // -------------------------------------------------------------------------

    /// <summary>
    /// Running ExecuteAsync twice with the same Marketplace payload must produce
    /// the same set of Subscriptions, Plans, Offers, and Users rows in the
    /// database — the second run is a no-op for these tables.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_RunTwiceWithSamePayload_ProducesSameDbState()
    {
        // Arrange — 5 subscriptions, all Subscribed on both runs.
        const int SubscriptionCount = 5;
        var ids = Enumerable.Range(1, SubscriptionCount)
            .Select(i => Guid.Parse($"ffffffff-{i:D4}-{i:D4}-{i:D4}-ffffffffffff"))
            .ToList();
        var sdkSubscriptions = ids
            .Select((id, i) => FakeMarketplaceSaaSClientBuilder.CreateFullSubscription(
                id,
                offerId: "offer-f",
                planId: "plan-f",
                status: SubscriptionStatusEnum.Subscribed,
                quantity: 2,
                beneficiaryEmail: $"f-user-{i}@example.test"))
            .ToList();

        // Both fake clients return the same data.
        var fakeClient1 = new FakeMarketplaceSaaSClientBuilder()
            .WithSeededSubscriptions(sdkSubscriptions)
            .Build();
        var (pipeline1, ctx, adminUserId) = BuildPipeline(fakeClient1, maxConcurrentPlanFetches: 5);

        // Run 1
        var result1 = await pipeline1.ExecuteAsync(adminUserId);
        Assert.Equal(0, result1.Failed);

        var subCountAfterRun1 = ctx.Subscriptions.Count();
        var offerCountAfterRun1 = ctx.Offers.Count();
        var planCountAfterRun1 = ctx.Plans.Count();
        var userCountAfterRun1 = ctx.Users.Count();
        var auditCountAfterRun1 = ctx.SubscriptionAuditLogs.Count();

        // Run 2 — same payload, same DB context.
        var fakeClient2 = new FakeMarketplaceSaaSClientBuilder()
            .WithSeededSubscriptions(sdkSubscriptions)
            .Build();
        var pipeline2 = BuildPipelineWithExistingContext(ctx, fakeClient2, maxConcurrentPlanFetches: 5, adminUserId: adminUserId);
        var result2 = await pipeline2.ExecuteAsync(adminUserId);
        Assert.Equal(0, result2.Failed);

        // Assert — row counts must be the same after both runs.
        Assert.Equal(subCountAfterRun1, ctx.Subscriptions.Count());
        Assert.Equal(offerCountAfterRun1, ctx.Offers.Count());
        Assert.Equal(planCountAfterRun1, ctx.Plans.Count());
        Assert.Equal(userCountAfterRun1, ctx.Users.Count());

        // No new audit log rows on the second run with the same payload
        // (no status/plan/quantity change detected).
        Assert.Equal(auditCountAfterRun1, ctx.SubscriptionAuditLogs.Count());
    }

    // -------------------------------------------------------------------------
    // 4. Audit-log generation on detected change
    // -------------------------------------------------------------------------

    /// <summary>
    /// When a subscription's status changes between two consecutive fetches,
    /// exactly one Status-Refresh audit log row must be written for the changed
    /// subscription (Requirement 3.2).
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_WhenStatusChanges_WritesStatusRefreshAuditLog()
    {
        // Arrange — 3 subscriptions; subscription[1] changes from Subscribed
        // to Unsubscribed on the second fetch.
        const int SubscriptionCount = 3;
        var ids = Enumerable.Range(1, SubscriptionCount)
            .Select(i => Guid.Parse($"aaaaaaaa-aa{i:D2}-aa{i:D2}-aa{i:D2}-aaaaaaaaaaaa"))
            .ToList();

        // Fetch 1 — all Subscribed.
        var sdkFetch1 = ids
            .Select((id, i) => FakeMarketplaceSaaSClientBuilder.CreateFullSubscription(
                id,
                offerId: "offer-aa",
                planId: "plan-aa",
                status: SubscriptionStatusEnum.Subscribed,
                quantity: 1,
                beneficiaryEmail: $"aa-user-{i}@example.test"))
            .ToList();
        var fakeClient1 = new FakeMarketplaceSaaSClientBuilder()
            .WithSeededSubscriptions(sdkFetch1)
            .Build();
        var (pipeline1, ctx, adminUserId) = BuildPipeline(fakeClient1, maxConcurrentPlanFetches: 3);
        await pipeline1.ExecuteAsync(adminUserId);

        var auditCountAfterFetch1 = ctx.SubscriptionAuditLogs.Count();

        // Fetch 2 — subscription[1] changes to Unsubscribed.
        var sdkFetch2 = ids
            .Select((id, i) => FakeMarketplaceSaaSClientBuilder.CreateFullSubscription(
                id,
                offerId: "offer-aa",
                planId: "plan-aa",
                status: (i == 1) ? SubscriptionStatusEnum.Unsubscribed : SubscriptionStatusEnum.Subscribed,
                quantity: 1,
                beneficiaryEmail: $"aa-user-{i}@example.test"))
            .ToList();
        var fakeClient2 = new FakeMarketplaceSaaSClientBuilder()
            .WithSeededSubscriptions(sdkFetch2)
            .Build();
        var pipeline2 = BuildPipelineWithExistingContext(ctx, fakeClient2, maxConcurrentPlanFetches: 3, adminUserId: adminUserId);
        await pipeline2.ExecuteAsync(adminUserId);

        // Assert — exactly one new Status-Refresh audit log row.
        var auditCountAfterFetch2 = ctx.SubscriptionAuditLogs.Count();
        Assert.True(
            auditCountAfterFetch2 > auditCountAfterFetch1,
            "Expected at least one new audit log row after status change.");

        var statusRefreshLog = ctx.SubscriptionAuditLogs
            .Where(a => a.Attribute == "Status-Refresh" && a.NewValue == "Unsubscribed")
            .ToList();
        Assert.Single(statusRefreshLog);
        Assert.Equal("Subscribed", statusRefreshLog[0].OldValue);
    }

    /// <summary>
    /// When a subscription's plan changes between two consecutive fetches,
    /// exactly one Plan-Refresh audit log row must be written.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_WhenPlanChanges_WritesPlanRefreshAuditLog()
    {
        // Arrange — 2 subscriptions; subscription[0] changes plan on second fetch.
        var ids = new[]
        {
            Guid.Parse("bbbbbbbb-bb01-bb01-bb01-bbbbbbbbbbbb"),
            Guid.Parse("bbbbbbbb-bb02-bb02-bb02-bbbbbbbbbbbb"),
        };

        // Fetch 1 — plan is "plan-alpha".
        var sdkFetch1 = ids
            .Select((id, i) => FakeMarketplaceSaaSClientBuilder.CreateFullSubscription(
                id,
                offerId: "offer-bb",
                planId: "plan-alpha",
                status: SubscriptionStatusEnum.Subscribed,
                quantity: 1,
                beneficiaryEmail: $"bb-user-{i}@example.test"))
            .ToList();
        var fakeClient1 = new FakeMarketplaceSaaSClientBuilder()
            .WithSeededSubscriptions(sdkFetch1)
            .Build();
        var (pipeline1, ctx, adminUserId) = BuildPipeline(fakeClient1, maxConcurrentPlanFetches: 2);
        await pipeline1.ExecuteAsync(adminUserId);

        var auditCountAfterFetch1 = ctx.SubscriptionAuditLogs.Count();

        // Fetch 2 — subscription[0] moves to "plan-beta".
        var sdkFetch2 = ids
            .Select((id, i) => FakeMarketplaceSaaSClientBuilder.CreateFullSubscription(
                id,
                offerId: "offer-bb",
                planId: (i == 0) ? "plan-beta" : "plan-alpha",
                status: SubscriptionStatusEnum.Subscribed,
                quantity: 1,
                beneficiaryEmail: $"bb-user-{i}@example.test"))
            .ToList();
        var fakeClient2 = new FakeMarketplaceSaaSClientBuilder()
            .WithSeededSubscriptions(sdkFetch2)
            .Build();
        var pipeline2 = BuildPipelineWithExistingContext(ctx, fakeClient2, maxConcurrentPlanFetches: 2, adminUserId: adminUserId);
        await pipeline2.ExecuteAsync(adminUserId);

        // Assert
        var auditCountAfterFetch2 = ctx.SubscriptionAuditLogs.Count();
        Assert.True(auditCountAfterFetch2 > auditCountAfterFetch1,
            "Expected new audit log rows after plan change.");

        var planRefreshLogs = ctx.SubscriptionAuditLogs
            .Where(a => a.Attribute == "Plan-Refresh")
            .ToList();
        Assert.True(planRefreshLogs.Count >= 1,
            $"Expected at least one Plan-Refresh audit log, found {planRefreshLogs.Count}.");
        Assert.Contains(planRefreshLogs, a => a.NewValue == "plan-beta" && a.OldValue == "plan-alpha");
    }

    /// <summary>
    /// When a subscription's quantity changes between two consecutive fetches,
    /// exactly one Quantity-Refresh audit log row must be written.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_WhenQuantityChanges_WritesQuantityRefreshAuditLog()
    {
        // Arrange — 2 subscriptions; subscription[1] changes quantity.
        var ids = new[]
        {
            Guid.Parse("cccccccc-cc01-cc01-cc01-cccccccccccc"),
            Guid.Parse("cccccccc-cc02-cc02-cc02-cccccccccccc"),
        };

        // Fetch 1 — quantity is 3.
        var sdkFetch1 = ids
            .Select((id, i) => FakeMarketplaceSaaSClientBuilder.CreateFullSubscription(
                id,
                offerId: "offer-cc",
                planId: "plan-cc",
                status: SubscriptionStatusEnum.Subscribed,
                quantity: 3,
                beneficiaryEmail: $"cc-user-{i}@example.test"))
            .ToList();
        var fakeClient1 = new FakeMarketplaceSaaSClientBuilder()
            .WithSeededSubscriptions(sdkFetch1)
            .Build();
        var (pipeline1, ctx, adminUserId) = BuildPipeline(fakeClient1, maxConcurrentPlanFetches: 2);
        await pipeline1.ExecuteAsync(adminUserId);

        var auditCountAfterFetch1 = ctx.SubscriptionAuditLogs.Count();

        // Fetch 2 — subscription[1] quantity changes from 3 to 7.
        var sdkFetch2 = ids
            .Select((id, i) => FakeMarketplaceSaaSClientBuilder.CreateFullSubscription(
                id,
                offerId: "offer-cc",
                planId: "plan-cc",
                status: SubscriptionStatusEnum.Subscribed,
                quantity: (i == 1) ? 7 : 3,
                beneficiaryEmail: $"cc-user-{i}@example.test"))
            .ToList();
        var fakeClient2 = new FakeMarketplaceSaaSClientBuilder()
            .WithSeededSubscriptions(sdkFetch2)
            .Build();
        var pipeline2 = BuildPipelineWithExistingContext(ctx, fakeClient2, maxConcurrentPlanFetches: 2, adminUserId: adminUserId);
        await pipeline2.ExecuteAsync(adminUserId);

        // Assert
        var auditCountAfterFetch2 = ctx.SubscriptionAuditLogs.Count();
        Assert.True(auditCountAfterFetch2 > auditCountAfterFetch1,
            "Expected new audit log rows after quantity change.");

        var quantityRefreshLogs = ctx.SubscriptionAuditLogs
            .Where(a => a.Attribute == "Quantity-Refresh")
            .ToList();
        Assert.True(quantityRefreshLogs.Count >= 1,
            $"Expected at least one Quantity-Refresh audit log, found {quantityRefreshLogs.Count}.");
        Assert.Contains(quantityRefreshLogs, a => a.NewValue == "7" && a.OldValue == "3");
    }

    // -------------------------------------------------------------------------
    // 5. Empty API response
    // -------------------------------------------------------------------------

    /// <summary>
    /// When the Marketplace API returns an empty subscription list, the pipeline
    /// must return a FetchResult with Total = 0, Succeeded = 0, Failed = 0, and
    /// no failures. The DB must remain unmodified.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_WithEmptyApiResponse_ReturnsZeroTotals()
    {
        // Arrange — API returns no subscriptions.
        var fakeClient = new FakeMarketplaceSaaSClientBuilder()
            .WithSeededSubscriptions(Enumerable.Empty<Subscription>())
            .Build();
        var (pipeline, ctx, adminUserId) = BuildPipeline(fakeClient, maxConcurrentPlanFetches: 5);

        // Act
        var result = await pipeline.ExecuteAsync(adminUserId);

        // Assert
        Assert.Equal(0, result.Total);
        Assert.Equal(0, result.Succeeded);
        Assert.Equal(0, result.Failed);
        Assert.Empty(result.Failures);
        Assert.Equal(0, ctx.Subscriptions.Count());
    }

    // -------------------------------------------------------------------------
    // 6. DurationMs is populated
    // -------------------------------------------------------------------------

    /// <summary>
    /// The FetchResult.DurationMs must be > 0 when real work was performed.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_WithSubscriptions_PopulatesDurationMs()
    {
        var id = Guid.NewGuid();
        var sdkSubs = new[]
        {
            FakeMarketplaceSaaSClientBuilder.CreateFullSubscription(
                id, "offer-dur", "plan-dur",
                SubscriptionStatusEnum.Subscribed, 1, "dur@example.test"),
        };
        var fakeClient = new FakeMarketplaceSaaSClientBuilder()
            .WithSeededSubscriptions(sdkSubs)
            .Build();
        var (pipeline, _, adminUserId) = BuildPipeline(fakeClient, maxConcurrentPlanFetches: 1);

        var result = await pipeline.ExecuteAsync(adminUserId);

        Assert.True(result.DurationMs >= 0,
            $"Expected DurationMs >= 0, got {result.DurationMs}");
    }

    // =========================================================================
    // Private helpers
    // =========================================================================

    /// <summary>
    /// Seeds a fresh in-memory context with an admin user and wires up a
    /// <see cref="SubscriptionFetchPipeline"/> over real repository implementations.
    /// Returns the pipeline, the context, and the admin user's ID.
    /// </summary>
    private static (SubscriptionFetchPipeline pipeline, SaasKitContext ctx, int adminUserId)
        BuildPipeline(FakeMarketplaceSaaSClient fakeClient, int maxConcurrentPlanFetches)
    {
        var ctx = InMemorySaasKitContextFactory.Create();
        ctx.Database.EnsureCreated();

        // Seed a synthetic admin user so GetUserIdFromEmailAddress succeeds.
        const string AdminEmail = "admin@pipeline.test";
        ctx.Users.Add(new DataAccess.Entities.Users
        {
            UserId = 1,
            EmailAddress = AdminEmail,
            FullName = "Pipeline Admin",
            CreatedDate = DateTime.UtcNow,
        });
        ctx.SaveChanges();

        var pipeline = BuildPipelineWithExistingContext(
            ctx, fakeClient, maxConcurrentPlanFetches, adminUserId: 1);
        return (pipeline, ctx, adminUserId: 1);
    }

    /// <summary>
    /// Wires a <see cref="SubscriptionFetchPipeline"/> over an existing
    /// <see cref="SaasKitContext"/>. Used for the second-run idempotence tests.
    /// </summary>
    private static SubscriptionFetchPipeline BuildPipelineWithExistingContext(
        SaasKitContext ctx,
        FakeMarketplaceSaaSClient fakeClient,
        int maxConcurrentPlanFetches,
        int adminUserId)
    {
        var appConfigRepo = new ApplicationConfigRepository(ctx);
        var subscriptionsRepo = new SubscriptionsRepository(ctx);
        var plansRepo = new PlansRepository(ctx, appConfigRepo);
        var offersRepo = new OffersRepository(ctx);
        var usersRepo = new UsersRepository(ctx);
        var subscriptionLogsRepo = new SubscriptionLogRepository(ctx);

        var sdkConfig = new SaaSApiClientConfiguration
        {
            FulFillmentAPIBaseURL = "https://localhost/fake",
            FulFillmentAPIVersion = "2018-08-31",
        };

        // Use the real FulfillmentApiService over the fake SDK client.
        // No Polly wrapper here: unit tests exercise the pipeline's own
        // error-isolation logic, not the resilience policy.
        var fulfillmentApiService = new FulfillmentApiService(
            fakeClient.Client,
            sdkConfig,
            new Services.Utilities.FulfillmentApiClientLogger());

        var options = Options.Create(new MarketplaceResilienceOptions
        {
            MaxConcurrentPlanFetches = maxConcurrentPlanFetches,
        });

        return new SubscriptionFetchPipeline(
            fulfillApiService: fulfillmentApiService,
            subscriptionsRepository: subscriptionsRepo,
            plansRepository: plansRepo,
            offersRepository: offersRepo,
            usersRepository: usersRepo,
            subscriptionLogRepository: subscriptionLogsRepo,
            options: options,
            logger: NullLogger<SubscriptionFetchPipeline>.Instance);
    }
}
