// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace Marketplace.SaaS.Accelerator.AdminSite.Test.BugCondition;

/// <summary>
/// Bug-condition input shape derived from <c>bugfix.md</c> and the
/// <c>isBugCondition</c> pseudocode in <c>design.md</c>.
///
/// Each field maps to a branch of <c>isBugCondition</c>. The combination
/// drives the test fakes (Marketplace SaaS client + InMemory SaasKit
/// context) to reproduce one of the documented "Examples" from
/// <c>design.md</c>:
///   - transient 429 on bulk list
///   - large fan-out causing thread-pool saturation
///   - per-subscription 500 inside the fetch loop
///   - sustained 503 (consecutive failures)
///   - empty-database first run (Property 5).
///
/// The DB-timeout branch is illustrative on the InMemory provider —
/// <see cref="DbQueryExceedsTimeout"/> is true only when the seeded
/// subscription count exceeds the spec threshold (10_000). With the scoped
/// generator in <see cref="BugConditionInputArbitrary"/> this is never true,
/// so the DB-timeout branch is documented but not exercised at runtime.
/// </summary>
public sealed record BugConditionInputs(
    bool ApiExperiencesTransientFailure,
    int TransientFailureCount,
    int SubscriptionCount,
    int ConsecutiveApiFailures,
    bool DbQueryExceedsTimeout,
    int? SingleSubscriptionFailureIndex,
    int SimulatedSdkDelayMs);
