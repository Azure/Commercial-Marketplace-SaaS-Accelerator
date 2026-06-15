// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Microsoft.Marketplace.SaaS;
using Microsoft.Marketplace.SaaS.Models;
using Moq;

namespace Marketplace.SaaS.Accelerator.AdminSite.Test.Doubles;

/// <summary>
/// Fluent builder that produces a configured <see cref="Mock{IMarketplaceSaaSClient}"/>
/// wrapped in a <see cref="FakeMarketplaceSaaSClient"/> with side-channel observers.
///
/// The builder lets each test inject the failure shapes from <c>design.md</c>
/// "Examples": throw <see cref="RequestFailedException"/>(429) once, throw
/// <see cref="RequestFailedException"/>(500) for one specific subscription id,
/// throw <see cref="RequestFailedException"/>(503) indefinitely, and delay N ms
/// per call. Each chained method composes a failure mode; <see cref="Build"/>
/// wires everything onto the Moq instance.
/// </summary>
public sealed class FakeMarketplaceSaaSClientBuilder
{
    private static readonly Response EmptyResponse = Mock.Of<Response>();

    // Subscription's parameterised internal constructor is the only way to set
    // its read-only Id without going through reflection on the property.
    private static readonly ConstructorInfo SubscriptionParameterisedCtor =
        typeof(Subscription)
            .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .Single(c => c.GetParameters().Length > 0);

    // SubscriptionPlans has an internal ctor taking IReadOnlyList<Plan>.
    private static readonly ConstructorInfo SubscriptionPlansInternalCtor =
        typeof(SubscriptionPlans)
            .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .Single(c => c.GetParameters().Length == 1);

    private List<Subscription> seededSubscriptions = new();

    private int transient429RemainingForBulk;
    private readonly HashSet<string> subscription500Ids = new(StringComparer.OrdinalIgnoreCase);
    private bool sustained503Enabled;
    private bool sustained503Bounded;
    private int sustained503Budget;
    private TimeSpan delayPerCall = TimeSpan.Zero;

    /// <summary>Seed the bulk list call with the given subscriptions.</summary>
    public FakeMarketplaceSaaSClientBuilder WithSeededSubscriptions(IEnumerable<Subscription> subscriptions)
    {
        seededSubscriptions = subscriptions?.ToList() ?? new List<Subscription>();
        return this;
    }

    /// <summary>Seed the bulk list call with subscriptions built from id-only test fixtures.</summary>
    public FakeMarketplaceSaaSClientBuilder WithSeededSubscriptions(IEnumerable<Guid> subscriptionIds)
    {
        seededSubscriptions = (subscriptionIds ?? Enumerable.Empty<Guid>())
            .Select(CreateMinimalSubscription)
            .ToList();
        return this;
    }

    /// <summary>First call to the bulk list throws 429; subsequent calls succeed.</summary>
    public FakeMarketplaceSaaSClientBuilder WithTransient429Once() => WithTransient429Times(1);

    /// <summary>The first <paramref name="n"/> calls to the bulk list throw 429.</summary>
    public FakeMarketplaceSaaSClientBuilder WithTransient429Times(int n)
    {
        if (n < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(n), "Transient 429 count must be non-negative.");
        }

        transient429RemainingForBulk = n;
        return this;
    }

    /// <summary>
    /// The per-subscription plan-fetch call for <paramref name="subscriptionId"/>
    /// throws 500. Other subscriptions succeed.
    /// </summary>
    public FakeMarketplaceSaaSClientBuilder WithSubscription500(string subscriptionId)
    {
        if (string.IsNullOrEmpty(subscriptionId))
        {
            throw new ArgumentException("subscriptionId must be non-empty.", nameof(subscriptionId));
        }

        subscription500Ids.Add(subscriptionId);
        return this;
    }

    /// <summary>
    /// The per-subscription plan-fetch call for <paramref name="subscriptionId"/>
    /// throws 500. Other subscriptions succeed.
    /// </summary>
    public FakeMarketplaceSaaSClientBuilder WithSubscription500(Guid subscriptionId) =>
        WithSubscription500(subscriptionId.ToString());

    /// <summary>
    /// Every call throws 503. If <paramref name="maxCalls"/> is null the failure
    /// is unbounded; otherwise the first <paramref name="maxCalls"/> invocations
    /// throw and subsequent calls succeed (modelling a recovering upstream).
    /// </summary>
    public FakeMarketplaceSaaSClientBuilder WithSustained503(int? maxCalls = null)
    {
        if (maxCalls is < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxCalls), "maxCalls must be non-negative.");
        }

        sustained503Enabled = true;
        sustained503Bounded = maxCalls is not null;
        sustained503Budget = maxCalls ?? 0;
        return this;
    }

    /// <summary>Every call awaits <paramref name="delay"/> before returning.</summary>
    public FakeMarketplaceSaaSClientBuilder WithDelayPerCall(TimeSpan delay)
    {
        if (delay < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(delay), "delay must be non-negative.");
        }

        delayPerCall = delay;
        return this;
    }

    /// <summary>Convenience overload taking milliseconds.</summary>
    public FakeMarketplaceSaaSClientBuilder WithDelayPerCall(int delayMs) =>
        WithDelayPerCall(TimeSpan.FromMilliseconds(delayMs));

    /// <summary>Materialise the configured fake.</summary>
    public FakeMarketplaceSaaSClient Build()
    {
        var observers = new FakeMarketplaceSaaSClientObservers();
        var failureState = new FailureState
        {
            Transient429RemainingForBulk = transient429RemainingForBulk,
            Subscription500Ids = new HashSet<string>(subscription500Ids, StringComparer.OrdinalIgnoreCase),
            Sustained503Enabled = sustained503Enabled,
            Sustained503Bounded = sustained503Bounded,
            Sustained503Budget = sustained503Budget,
            DelayPerCall = delayPerCall,
            SubscriptionsSnapshot = seededSubscriptions.ToList(),
        };

        var fulfillmentMock = new Mock<FulfillmentOperations>(MockBehavior.Loose) { CallBase = false };
        var operationsMock = new Mock<SubscriptionOperations>(MockBehavior.Loose) { CallBase = false };
        var clientMock = new Mock<IMarketplaceSaaSClient>(MockBehavior.Loose);

        clientMock.SetupGet(c => c.Fulfillment).Returns(fulfillmentMock.Object);
        clientMock.SetupGet(c => c.Operations).Returns(operationsMock.Object);

        ConfigureBulkList(fulfillmentMock, observers, failureState);
        ConfigurePerSubscriptionPlanFetch(fulfillmentMock, observers, failureState);

        return new FakeMarketplaceSaaSClient(clientMock, fulfillmentMock, operationsMock, observers);
    }

    private static void ConfigureBulkList(
        Mock<FulfillmentOperations> fulfillmentMock,
        FakeMarketplaceSaaSClientObservers observers,
        FailureState state)
    {
        fulfillmentMock
            .Setup(f => f.ListSubscriptionsAsync(
                It.IsAny<string>(),
                It.IsAny<Guid?>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .Returns<string, Guid?, Guid?, CancellationToken>((continuation, requestId, correlationId, ct) =>
            {
                using var scope = observers.EnterBulkList();
                state.ApplySynchronousDelay();
                state.ThrowIfSustained503();
                state.ThrowIfTransient429ForBulk();

                var page = Page<Subscription>.FromValues(state.SubscriptionsSnapshot, null, EmptyResponse);
                return AsyncPageable<Subscription>.FromPages(new[] { page });
            });
    }

    private static void ConfigurePerSubscriptionPlanFetch(
        Mock<FulfillmentOperations> fulfillmentMock,
        FakeMarketplaceSaaSClientObservers observers,
        FailureState state)
    {
        fulfillmentMock
            .Setup(f => f.ListAvailablePlansAsync(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<Guid?>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .Returns<Guid, string, Guid?, Guid?, CancellationToken>(async (subscriptionId, planId, requestId, correlationId, ct) =>
            {
                var key = subscriptionId.ToString();
                using var scope = observers.EnterPerSubscription(key);
                await state.ApplyAsynchronousDelayAsync(ct).ConfigureAwait(false);
                state.ThrowIfSustained503();
                state.ThrowIfPerSubscription500(key);

                var emptyPlans = (SubscriptionPlans)SubscriptionPlansInternalCtor.Invoke(
                    new object[] { Array.Empty<Plan>() });
                return Response.FromValue(emptyPlans, EmptyResponse);
            });
    }

    // AadIdentifier and SubscriptionTerm have internal constructors; use reflection.
    private static readonly ConstructorInfo AadIdentifierCtor =
        typeof(AadIdentifier)
            .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .Single(c => c.GetParameters().Length == 4);

    private static readonly ConstructorInfo SubscriptionTermCtor =
        typeof(SubscriptionTerm)
            .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .Single(c => c.GetParameters().Length == 4);

    /// <summary>
    /// Construct a minimal <see cref="Subscription"/> populated with just an id.
    /// Useful for tests that only care about iteration shape.
    /// </summary>
    public static Subscription CreateMinimalSubscription(Guid id)
    {
        // Constructor signature (verified via reflection in 1.2 prework):
        // (Guid? id, string publisherId, string offerId, string name,
        //  SubscriptionStatusEnum? saasSubscriptionStatus, AadIdentifier beneficiary,
        //  AadIdentifier purchaser, string planId, int? quantity,
        //  SubscriptionTerm term, bool? autoRenew, bool? isTest, bool? isFreeTrial,
        //  IReadOnlyList<AllowedCustomerOperationsEnum> allowedCustomerOperations,
        //  SandboxTypeEnum? sandboxType, DateTimeOffset? created, SessionModeEnum? sessionMode)
        var ctorParams = SubscriptionParameterisedCtor.GetParameters();
        var args = new object[ctorParams.Length];
        args[0] = (Guid?)id;
        // Remaining parameters default to null/0; Activator-style population.
        for (int i = 1; i < ctorParams.Length; i++)
        {
            var pt = ctorParams[i].ParameterType;
            args[i] = pt.IsValueType && Nullable.GetUnderlyingType(pt) is null
                ? Activator.CreateInstance(pt)
                : null;
        }

        return (Subscription)SubscriptionParameterisedCtor.Invoke(args);
    }

    /// <summary>
    /// Construct a fully-populated <see cref="Subscription"/> that passes through
    /// <see cref="Marketplace.SaaS.Accelerator.Services.Helpers.ConversionHelper.subscriptionResult"/>
    /// without throwing. All required fields (Purchaser.ObjectId, Purchaser.TenantId,
    /// Beneficiary.EmailId, Beneficiary.ObjectId, Beneficiary.TenantId, and Term) are
    /// populated.
    /// </summary>
    /// <param name="id">The subscription Guid.</param>
    /// <param name="offerId">The offer id string (e.g. "offer-00001").</param>
    /// <param name="planId">The plan id string (e.g. "plan-alpha").</param>
    /// <param name="status">The subscription status.</param>
    /// <param name="quantity">The seat quantity.</param>
    /// <param name="beneficiaryEmail">Beneficiary / customer email.</param>
    public static Subscription CreateFullSubscription(
        Guid id,
        string offerId,
        string planId,
        SubscriptionStatusEnum status,
        int quantity,
        string beneficiaryEmail)
    {
        // AadIdentifier(string emailId, Guid? objectId, Guid? tenantId, string puid)
        var beneficiary = (AadIdentifier)AadIdentifierCtor.Invoke(new object[]
        {
            beneficiaryEmail,
            (Guid?)Guid.NewGuid(),
            (Guid?)Guid.NewGuid(),
            (string)null,
        });

        var purchaser = (AadIdentifier)AadIdentifierCtor.Invoke(new object[]
        {
            (string)null,
            (Guid?)Guid.NewGuid(),
            (Guid?)Guid.NewGuid(),
            (string)null,
        });

        // SubscriptionTerm(TermUnitEnum? termUnit, DateTimeOffset? startDate, DateTimeOffset? endDate, string chargeDuration)
        var term = (SubscriptionTerm)SubscriptionTermCtor.Invoke(new object[]
        {
            (TermUnitEnum?)TermUnitEnum.P1M,
            (DateTimeOffset?)DateTimeOffset.UtcNow,
            (DateTimeOffset?)DateTimeOffset.UtcNow.AddMonths(1),
            (string)null,
        });

        var ctorParams = SubscriptionParameterisedCtor.GetParameters();
        var args = new object[ctorParams.Length];
        // Position order: id, publisherId, offerId, name, saasSubscriptionStatus,
        //                 beneficiary, purchaser, planId, quantity, term, ...
        args[0] = (Guid?)id;
        args[1] = "test-publisher";
        args[2] = offerId;
        args[3] = $"sub-{id:N}";
        args[4] = (SubscriptionStatusEnum?)status;
        args[5] = beneficiary;
        args[6] = purchaser;
        args[7] = planId;
        args[8] = (int?)quantity;
        args[9] = term;
        // Remaining args: autoRenew, isTest, isFreeTrial, allowedCustomerOperations,
        //                 sandboxType, created, sessionMode — all null.
        for (int i = 10; i < ctorParams.Length; i++)
        {
            var pt = ctorParams[i].ParameterType;
            args[i] = pt.IsValueType && Nullable.GetUnderlyingType(pt) is null
                ? Activator.CreateInstance(pt)
                : null;
        }

        return (Subscription)SubscriptionParameterisedCtor.Invoke(args);
    }

    private sealed class FailureState
    {
        private int transient429Counter;
        private int sustained503Used;

        public int Transient429RemainingForBulk
        {
            get => transient429Counter;
            init => transient429Counter = value;
        }

        public HashSet<string> Subscription500Ids { get; init; } = new();
        public bool Sustained503Enabled { get; init; }
        public bool Sustained503Bounded { get; init; }
        public int Sustained503Budget { get; init; }
        public TimeSpan DelayPerCall { get; init; }
        public List<Subscription> SubscriptionsSnapshot { get; init; } = new();

        public void ApplySynchronousDelay()
        {
            if (DelayPerCall > TimeSpan.Zero)
            {
                Thread.Sleep(DelayPerCall);
            }
        }

        public Task ApplyAsynchronousDelayAsync(CancellationToken ct)
        {
            return DelayPerCall > TimeSpan.Zero
                ? Task.Delay(DelayPerCall, ct)
                : Task.CompletedTask;
        }

        public void ThrowIfTransient429ForBulk()
        {
            // Decrement is atomic. As long as the result is non-negative the
            // caller is within the configured 429 budget and should throw. After
            // the budget is exhausted the counter goes negative and stays there;
            // subsequent callers see < 0 and proceed normally.
            if (Interlocked.Decrement(ref transient429Counter) >= 0)
            {
                throw new RequestFailedException(429, "Too Many Requests");
            }
        }

        public void ThrowIfPerSubscription500(string subscriptionId)
        {
            if (Subscription500Ids.Contains(subscriptionId))
            {
                throw new RequestFailedException(500, "Internal Server Error");
            }
        }

        public void ThrowIfSustained503()
        {
            if (!Sustained503Enabled)
            {
                return;
            }

            if (!Sustained503Bounded)
            {
                throw new RequestFailedException(503, "Service Unavailable");
            }

            // Bounded sustained 503: throw for the first N invocations across the
            // entire client. Use Interlocked so concurrent callers see a stable
            // countdown.
            if (Interlocked.Increment(ref sustained503Used) <= Sustained503Budget)
            {
                throw new RequestFailedException(503, "Service Unavailable");
            }
        }
    }
}
