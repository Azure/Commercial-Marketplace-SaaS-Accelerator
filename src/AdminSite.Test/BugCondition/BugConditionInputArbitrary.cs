// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using FsCheck;
using Marketplace.SaaS.Accelerator.AdminSite.Test.Generators;

namespace Marketplace.SaaS.Accelerator.AdminSite.Test.BugCondition;

/// <summary>
/// FsCheck arbitrary that produces <see cref="BugConditionInputs"/> values
/// biased toward the concrete shapes called out in the <c>design.md</c>
/// "Examples" block. The arbitrary filters generated values to only those
/// where <see cref="IsBugConditionExtensions.IsBugCondition"/> returns true,
/// so the property test method's body can assume the bug condition holds.
///
/// Per task 1.4's "Scoped PBT Approach":
///   - <c>transientFailureCount</c> ∈ {1, 2}
///   - <c>subscriptionCount</c> ∈ {0, 50, 200, 500} (0 included for Property 5)
///   - <c>consecutiveApiFailures</c> = 6 (single value; Examples cite "6 in a row")
///   - <c>singleSubscriptionFailureIndex</c> drawn from the generated
///     subscription-index set (or null with non-zero probability so we sample
///     non-isolation cases too)
///   - <c>simulatedSdkDelayMs</c> ∈ {0, 200}
///
/// The 50_000-row case from <see cref="SaasKitGenerators.SpecSubscriptionCounts"/>
/// is intentionally excluded from this arbitrary; that scale is reserved for
/// the opt-in <c>[Trait("category", "large")]</c> tests so the default
/// property runs stay inside the per-test timeout budget.
/// </summary>
public static class BugConditionInputArbitrary
{
    /// <summary>
    /// Subscription counts the bug-condition arbitrary samples from. Note that
    /// 0 is intentionally included to exercise Property 5 (empty-state
    /// guidance) — when <see cref="BugConditionInputs.SubscriptionCount"/> is 0
    /// the test drives <c>Subscriptions()</c> instead of the fetch loop.
    /// </summary>
    public static readonly int[] BiasedSubscriptionCounts = { 0, 50, 200, 500 };

    /// <summary>Transient-failure counts from the design.md Examples block.</summary>
    public static readonly int[] BiasedTransientFailureCounts = { 1, 2 };

    /// <summary>Per-call SDK delays from the design.md Examples block (ms).</summary>
    public static readonly int[] BiasedSimulatedSdkDelaysMs = { 0, 200 };

    /// <summary>
    /// Single value cited by the design.md Examples block ("6 consecutive
    /// failures"); strictly greater than the default
    /// <see cref="BugConditionThresholds.CircuitBreakerThreshold"/> so the
    /// circuit-breaker branch of <c>isBugCondition</c> always trips when
    /// <see cref="BugConditionInputs.ConsecutiveApiFailures"/> is set.
    /// </summary>
    public const int SustainedFailureCount = 6;

    /// <summary>
    /// FsCheck registers this static method automatically when the test class
    /// is annotated with <c>[Properties(Arbitrary = new[] { typeof(BugConditionInputArbitrary) })]</c>.
    /// </summary>
    public static Arbitrary<BugConditionInputs> BugConditionInputs() =>
        Arb.From(GenInputs())
            .Filter(i => i.IsBugCondition(IsBugConditionExtensions.DefaultThresholds));

    private static Gen<BugConditionInputs> GenInputs()
    {
        // Independent generator for each input axis. We compose them with
        // Gen<T>.Zip(otherGen, mergeFn), which FsCheck 2.x exposes as a public
        // C# extension. (LINQ query syntax over Gen is ambiguous in this
        // assembly because both FsCheck and the local GenLinqExtensions
        // surface a SelectMany.)
        var apiFailureGen = Gen.Elements(true, false);
        var transientCountGen = Gen.Elements(BiasedTransientFailureCounts);
        var subscriptionCountGen = Gen.Elements(BiasedSubscriptionCounts);
        var triggerCircuitBreakerGen = Gen.Elements(true, false);
        var triggerSingleSubFailureGen = Gen.Elements(true, false);
        var simulatedDelayGen = Gen.Elements(BiasedSimulatedSdkDelaysMs);
        var rawIndexGen = Gen.Choose(0, 9999);

        // Build a Draw record progressively via repeated .Zip(...).
        return apiFailureGen
            .Zip(transientCountGen, (api, tc) => new Draw { ApiFailure = api, TransientCount = tc })
            .Zip(subscriptionCountGen, (d, sc) => d with { SubscriptionCount = sc })
            .Zip(triggerCircuitBreakerGen, (d, tcb) => d with { TriggerCircuitBreaker = tcb })
            .Zip(triggerSingleSubFailureGen, (d, tsf) => d with { TriggerSingleSubFailure = tsf })
            .Zip(simulatedDelayGen, (d, ms) => d with { SimulatedDelayMs = ms })
            .Zip(rawIndexGen, (d, idx) => d with { RawIndex = idx })
            .Select(Build);
    }

    private static BugConditionInputs Build(Draw d)
    {
        var consecutiveApiFailures = d.TriggerCircuitBreaker ? SustainedFailureCount : 0;
        var dbQueryExceedsTimeout = d.SubscriptionCount > 10_000;
        int? singleSubscriptionFailureIndex =
            (d.TriggerSingleSubFailure && d.SubscriptionCount > 0)
                ? d.RawIndex % d.SubscriptionCount
                : null;

        return new BugConditionInputs(
            ApiExperiencesTransientFailure: d.ApiFailure,
            TransientFailureCount: d.TransientCount,
            SubscriptionCount: d.SubscriptionCount,
            ConsecutiveApiFailures: consecutiveApiFailures,
            DbQueryExceedsTimeout: dbQueryExceedsTimeout,
            SingleSubscriptionFailureIndex: singleSubscriptionFailureIndex,
            SimulatedSdkDelayMs: d.SimulatedDelayMs);
    }

    /// <summary>
    /// Mutable-by-with record carrying the seven independent draws. Kept
    /// internal because FsCheck's auto-registration only inspects the
    /// surface returned by <see cref="BugConditionInputs"/>.
    /// </summary>
    private sealed record Draw
    {
        public bool ApiFailure { get; init; }
        public int TransientCount { get; init; }
        public int SubscriptionCount { get; init; }
        public bool TriggerCircuitBreaker { get; init; }
        public bool TriggerSingleSubFailure { get; init; }
        public int SimulatedDelayMs { get; init; }
        public int RawIndex { get; init; }
    }
}
