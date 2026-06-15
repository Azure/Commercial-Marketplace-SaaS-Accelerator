// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace Marketplace.SaaS.Accelerator.AdminSite.Test.BugCondition;

/// <summary>
/// Configurable thresholds that gate every branch of
/// <see cref="IsBugConditionExtensions.IsBugCondition"/>. Mirrors the
/// "configurable" defaults called out in <c>design.md</c> (Properties 1 + 4
/// and the <c>MarketplaceResilienceOptions</c> block):
///
///   - <see cref="ThreadPoolSaturationThreshold"/> = 100 (test-realistic;
///     production uses <c>MaxConcurrentPlanFetches=5</c> as the bound and
///     does not gate fan-out by subscription count).
///   - <see cref="CircuitBreakerThreshold"/> = 5 (matches
///     <c>MarketplaceResilienceOptions.ConsecutiveFailureThreshold</c>).
///   - <see cref="DbTimeoutThresholdMs"/> = 500 (illustrative — the
///     InMemory EF provider has no real command timeout; we proxy this via
///     <c>SubscriptionCount &gt; 10_000</c> in the predicate below).
/// </summary>
public sealed record BugConditionThresholds(
    int ThreadPoolSaturationThreshold = 100,
    int CircuitBreakerThreshold = 5,
    int DbTimeoutThresholdMs = 500);

/// <summary>
/// Boolean predicate from <c>design.md</c>'s <c>isBugCondition</c>:
/// returns true when any branch of the bug condition holds for an input.
/// </summary>
public static class IsBugConditionExtensions
{
    /// <summary>
    /// Default thresholds used by <see cref="BugConditionInputArbitrary"/>
    /// and the property test in <c>FetchPipelineBugConditionPropertyTests</c>.
    /// </summary>
    public static readonly BugConditionThresholds DefaultThresholds = new();

    /// <summary>
    /// Returns true when <paramref name="i"/> matches any branch of
    /// <c>isBugCondition</c> from <c>design.md</c>:
    ///
    ///   1. The Marketplace API experiences transient failures.
    ///   2. The subscription count exceeds the thread-pool saturation
    ///      threshold (sync-over-async fan-out).
    ///   3. The DB query for all subscriptions exceeds its timeout
    ///      threshold (modelled by <see cref="BugConditionInputs.DbQueryExceedsTimeout"/>
    ///      which is gated to <c>SubscriptionCount &gt; 10_000</c>).
    ///   4. The Marketplace API has been failing consecutively beyond the
    ///      circuit-breaker threshold.
    ///   5. A single subscription's plan fetch fails inside the loop
    ///      (per-subscription failure isolation; this branch is added to
    ///      cover the "single-subscription plan fetch failure" example
    ///      from <c>design.md</c>).
    /// </summary>
    public static bool IsBugCondition(this BugConditionInputs i, BugConditionThresholds t) =>
        i.ApiExperiencesTransientFailure
        || i.SubscriptionCount > t.ThreadPoolSaturationThreshold
        || i.DbQueryExceedsTimeout
        || i.ConsecutiveApiFailures > t.CircuitBreakerThreshold
        || (i.SingleSubscriptionFailureIndex.HasValue && i.SubscriptionCount > 0);
}
