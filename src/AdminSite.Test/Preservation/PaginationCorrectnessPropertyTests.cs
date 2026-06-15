// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

// =============================================================================
// PBT-2.2: Pagination Correctness Property (Property 3)
//
// Validates: Requirements 2.3, 3.4
//
// For random (pageIndex, pageSize) and random subscription corpora, assert:
//   - concat(GetPaged(1..N, pageSize)).Items == Get() ordering
//   - TotalCount == seeded count
//
// UNFIXED CODE OBSERVATION:
//   GetPaged does not exist on UNFIXED code (task 3.4 introduces it).
//   The tests that call GetPaged are annotated with
//   [Fact(Skip = "Awaiting paginated repo from task 3.4")] so they do not
//   block the green baseline established in wave 6.
//
//   What DOES exist on unfixed code is Get(), which returns the full ordered
//   set (IEnumerable<Subscriptions> ordered by CreateDate descending, with
//   Include(s => s.User)). The companion tests below verify this observable
//   baseline and constitute the unskipped portions that MUST PASS on unfixed code.
//
//   UNFIXED-CODE OBSERVATION (documented here, per task 2.5 instructions):
//   "Get() returns the full ordered set today — ordered by CreateDate
//   descending, with eager-loaded User navigation properties. There is no
//   Skip/Take, so all rows are materialised in a single query regardless of
//   corpus size. GetPaged does not exist; pagination will be introduced in
//   task 3.4."
//
// Reference: design.md Property 3 (Pagination Correctness)
// Reference: tasks.md 2.5, 3.4
// =============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using FsCheck;
using FsCheck.Xunit;
using Marketplace.SaaS.Accelerator.AdminSite.Test.Doubles;
using Marketplace.SaaS.Accelerator.AdminSite.Test.Fixtures;
using Marketplace.SaaS.Accelerator.AdminSite.Test.Generators;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.DataAccess.Services;
using Microsoft.Marketplace.SaaS.Models;
using Xunit;

namespace Marketplace.SaaS.Accelerator.AdminSite.Test.Preservation;

/// <summary>
/// PBT-2.2: Pagination correctness property test.
///
/// Validates: Requirements 2.3, 3.4
///
/// This class contains two groups of tests:
///
///   GROUP A (unskipped, must PASS on unfixed code):
///     Companion tests that verify the current Get() baseline:
///       - Get() returns a stable, fully-ordered sequence (OrderByDescending CreateDate).
///       - Get() always returns all seeded rows (no implicit pagination).
///       - Get() includes the User navigation property (eager-loaded).
///       - Get() is deterministic across multiple calls with the same data.
///
///   GROUP B (skipped — awaiting task 3.4):
///     The full PBT-2.2 property tests that require GetPaged:
///       - concat(GetPaged(1..N, pageSize)) equals Get() ordering.
///       - TotalCount equals the seeded count.
///     These are annotated [Fact(Skip = "Awaiting paginated repo from task 3.4")].
/// </summary>
[Properties(Arbitrary = new[] { typeof(SaasKitArbitraries) })]
public class PaginationCorrectnessPropertyTests
{
    // ──────────────────────────────────────────────────────────────────────────
    // GROUP A: Companion tests — Get() baseline (must PASS on UNFIXED code)
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// PBT-2.2 companion property (unskipped):
    /// For any subscription corpus, Get() returns all seeded rows ordered
    /// by CreateDate descending with no implicit skip or take.
    ///
    /// This establishes the total-ordered baseline that GetPaged must
    /// slice consistently once it is introduced in task 3.4.
    ///
    /// Validates: Requirements 3.4, 2.3 (baseline observation)
    /// </summary>
    [Property(MaxTest = 50, EndSize = 50)]
    public Property Get_ReturnsAllSubscriptionsInDescendingCreateDateOrder(SubscriptionCorpus corpus)
    {
        // Arrange — seed a fresh in-memory context with the generated corpus.
        var ctx = InMemorySaasKitContextFactory.CreateAndSeed(corpus);
        var repo = new SubscriptionsRepository(ctx);

        // Act — call the existing Get() method.
        var result = repo.Get().ToList();

        // Assert 1: total count matches seeded corpus.
        var countMatches = (result.Count == corpus.Subscriptions.Count).Label(
            $"CountMatches: expected={corpus.Subscriptions.Count}, actual={result.Count}");

        // Assert 2: order is stable — each adjacent pair is CreateDate descending.
        bool orderedCorrectly = true;
        for (int i = 1; i < result.Count; i++)
        {
            // CreateDate may equal the previous (same-millisecond inserts) but
            // must not be strictly greater (ascending) than the previous entry.
            if (result[i].CreateDate > result[i - 1].CreateDate)
            {
                orderedCorrectly = false;
                break;
            }
        }

        var orderProp = orderedCorrectly.Label(
            $"OrderedByCreateDateDescending: count={result.Count}");

        // Assert 3: every AmpsubscriptionId from the corpus appears in result.
        var seededIds = corpus.Subscriptions
            .Select(s => s.AmpsubscriptionId)
            .OrderBy(g => g)
            .ToList();
        var returnedIds = result
            .Select(s => s.AmpsubscriptionId)
            .OrderBy(g => g)
            .ToList();
        var allIdsPresent = seededIds.SequenceEqual(returnedIds);
        var idsProp = allIdsPresent.Label(
            $"AllSeededIdsPresent: seeded={seededIds.Count}, returned={returnedIds.Count}");

        return countMatches.And(orderProp).And(idsProp);
    }

    /// <summary>
    /// PBT-2.2 companion property (unskipped):
    /// Get() is deterministic — two consecutive calls on the same data
    /// return the same sequence of AmpsubscriptionIds in the same order.
    ///
    /// Validates: Requirements 3.4 (stable ordering for pagination baseline)
    /// </summary>
    [Property(MaxTest = 30, EndSize = 50)]
    public Property Get_IsDeterministicAcrossConsecutiveCalls(SubscriptionCorpus corpus)
    {
        var ctx = InMemorySaasKitContextFactory.CreateAndSeed(corpus);
        var repo = new SubscriptionsRepository(ctx);

        var run1 = repo.Get().Select(s => s.AmpsubscriptionId).ToList();
        var run2 = repo.Get().Select(s => s.AmpsubscriptionId).ToList();

        var isDeterministic = run1.SequenceEqual(run2);
        return isDeterministic.Label(
            $"DeterministicOrdering: count={run1.Count}, run1Same={run1.SequenceEqual(run2)}");
    }

    /// <summary>
    /// Deterministic snapshot test (unskipped):
    /// Seeds 10 subscriptions with known CreateDate values and verifies that
    /// Get() returns them in strict CreateDate-descending order.
    ///
    /// This anchors the property test above to a concrete, inspectable example.
    ///
    /// Validates: Requirements 3.4 (column ordering unchanged), 2.3 (baseline)
    /// </summary>
    [Fact]
    public void Get_WithKnownCreateDates_ReturnsSubscriptionsInDescendingOrder()
    {
        // Arrange — build 10 subscriptions with evenly-spaced CreateDate values.
        const int Count = 10;
        var baseDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var subscriptions = Enumerable.Range(0, Count).Select(i => new Subscriptions
        {
            AmpsubscriptionId = Guid.NewGuid(),
            Name = $"sub-{i:D2}",
            AmpplanId = "plan-alpha",
            AmpOfferId = "offer-snap",
            SubscriptionStatus = "Subscribed",
            IsActive = true,
            CreateBy = 1,
            // Ascending create dates so descending order is clearly verifiable.
            CreateDate = baseDate.AddMinutes(i),
            ModifyDate = baseDate.AddMinutes(i),
            Ampquantity = 1,
        }).ToList();

        var corpus = new SubscriptionCorpus
        {
            Offers = new List<Offers>(),
            Plans = new List<Plans>(),
            Users = new List<Users>(),
            Subscriptions = subscriptions,
            AuditLogs = new List<SubscriptionAuditLogs>(),
        };

        var ctx = InMemorySaasKitContextFactory.Create();
        ctx.Database.EnsureCreated();
        // Add subscriptions directly without FK parent tables (InMemory provider
        // does not enforce FK constraints).
        ctx.Subscriptions.AddRange(subscriptions);
        ctx.SaveChanges();

        var repo = new SubscriptionsRepository(ctx);

        // Act
        var result = repo.Get().ToList();

        // Assert — result must be ordered by CreateDate descending.
        Assert.Equal(Count, result.Count);

        // The subscription with the LATEST CreateDate (index 9) must come first.
        Assert.Equal(baseDate.AddMinutes(Count - 1), result[0].CreateDate);

        // Verify strict descending order across all consecutive pairs.
        for (int i = 1; i < result.Count; i++)
        {
            Assert.True(
                result[i].CreateDate <= result[i - 1].CreateDate,
                $"Expected result[{i}].CreateDate <= result[{i - 1}].CreateDate " +
                $"but got {result[i].CreateDate} > {result[i - 1].CreateDate}");
        }
    }

    /// <summary>
    /// Deterministic test (unskipped):
    /// Get() on an empty database returns an empty sequence (not null, not an exception).
    ///
    /// Validates: Requirements 3.4, 2.3 (boundary: empty corpus baseline)
    /// </summary>
    [Fact]
    public void Get_WithEmptyDatabase_ReturnsEmptySequence()
    {
        var ctx = InMemorySaasKitContextFactory.Create();
        ctx.Database.EnsureCreated();

        var repo = new SubscriptionsRepository(ctx);
        var result = repo.Get().ToList();

        Assert.Empty(result);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // GROUP B: Skipped — awaiting GetPaged from task 3.4
    //
    // UNFIXED-CODE OBSERVATION:
    //   GetPaged does not exist in ISubscriptionsRepository or
    //   SubscriptionsRepository on the current codebase. These tests document
    //   the intended contract (Property 3) and will be unskipped in task 3.9
    //   once task 3.4 has introduced GetPaged.
    //
    //   Once unskipped, each test asserts:
    //     concat(GetPaged(pageIndex=1..N, pageSize)).Items == Get() full list
    //     GetPaged(pageIndex, pageSize).TotalCount == seededCount
    //     GetPaged clamps pageIndex < 1 to 1
    //     GetPaged clamps pageSize < 1 to 1
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// PBT-2.2 (Property 3 - pagination correctness).
    ///
    /// For random (pageIndex, pageSize) and random subscription corpora:
    ///   - concat(GetPaged(1..N, pageSize)).Items equals Get() ordering
    ///   - TotalCount equals the seeded count
    ///
    /// Validates: Requirements 2.3, 3.4
    /// </summary>
    [Property(MaxTest = 50, EndSize = 50)]
    public Property GetPaged_ConcatenatedPages_EqualsGetOrdering(SubscriptionCorpus corpus)
    {
        // Use a selection of page sizes that all divide evenly or have remainders
        // to exercise boundary conditions.
        int[] pageSizes = { 1, 5, 10, 100 };

        foreach (int pageSize in pageSizes)
        {
            var ctx = InMemorySaasKitContextFactory.CreateAndSeed(corpus);
            var repo = new SubscriptionsRepository(ctx);

            var fullList = repo.Get().Select(s => s.AmpsubscriptionId).ToList();
            int totalCount = corpus.Subscriptions.Count;

            // Collect all pages by stepping through page index until we have all rows.
            var concatenated = new List<Guid>();
            int pageIndex = 1;
            while (true)
            {
                var page = repo.GetPaged(pageIndex, pageSize);

                // TotalCount must equal seeded count on every page.
                if (page.TotalCount != totalCount)
                {
                    return false.Label(
                        $"TotalCount mismatch on page {pageIndex} with pageSize={pageSize}: " +
                        $"expected={totalCount}, actual={page.TotalCount}");
                }

                concatenated.AddRange(page.Items.Select(s => s.AmpsubscriptionId));

                if (page.Items.Count < pageSize)
                {
                    break; // Last (possibly partial) page reached.
                }

                pageIndex++;
            }

            // Concatenated pages must equal the full ordered Get() result.
            bool equal = concatenated.SequenceEqual(fullList);
            if (!equal)
            {
                return false.Label(
                    $"Concatenated pages differ from Get() ordering. " +
                    $"pageSize={pageSize}, corpus={totalCount}, " +
                    $"concatenated={concatenated.Count}, fullList={fullList.Count}");
            }
        }

        return true.Label("All page sizes produced ordering consistent with Get()");
    }

    /// <summary>
    /// PBT-2.2 (Property 3 - TotalCount correctness).
    ///
    /// For any (pageIndex, pageSize) pair, GetPaged.TotalCount equals the seeded count.
    ///
    /// Validates: Requirements 2.3, 3.4
    /// </summary>
    [Property(MaxTest = 50, EndSize = 50)]
    public Property GetPaged_TotalCount_EqualsSeededCount(SubscriptionCorpus corpus)
    {
        var ctx = InMemorySaasKitContextFactory.CreateAndSeed(corpus);
        var repo = new SubscriptionsRepository(ctx);
        int expected = corpus.Subscriptions.Count;

        // Sample a few different (pageIndex, pageSize) combinations to confirm
        // TotalCount is independent of the specific page requested.
        var pairs = new[] { (1, 1), (1, 10), (2, 5), (100, 100) };
        foreach (var (pi, ps) in pairs)
        {
            var page = repo.GetPaged(pi, ps);
            if (page.TotalCount != expected)
            {
                return false.Label(
                    $"TotalCount mismatch: pageIndex={pi}, pageSize={ps}, " +
                    $"expected={expected}, actual={page.TotalCount}");
            }
        }

        return true.Label($"TotalCount=={expected} for all sampled (pageIndex,pageSize) pairs");
    }

    /// <summary>
    /// PBT-2.2 (Property 3 - clamping).
    ///
    /// GetPaged clamps pageIndex &lt; 1 to 1 and pageSize &lt; 1 to 1.
    ///
    /// Validates: Requirements 2.3, 3.4
    /// </summary>
    [Fact]
    public void GetPaged_WithZeroOrNegativePageIndexOrSize_ClampsToOne()
    {
        // Arrange: seed a small corpus so the results are non-trivial.
        var subscriptions = Enumerable.Range(1, 5).Select(i => new Subscriptions
        {
            AmpsubscriptionId = Guid.NewGuid(),
            Name = $"sub-{i}",
            AmpplanId = "plan-a",
            AmpOfferId = "offer-a",
            SubscriptionStatus = "Subscribed",
            IsActive = true,
            CreateBy = 1,
            CreateDate = DateTime.UtcNow.AddMinutes(-i),
            ModifyDate = DateTime.UtcNow,
            Ampquantity = 1,
        }).ToList();

        var ctx = InMemorySaasKitContextFactory.Create();
        ctx.Database.EnsureCreated();
        ctx.Subscriptions.AddRange(subscriptions);
        ctx.SaveChanges();
        var repo = new SubscriptionsRepository(ctx);

        // pageIndex = 0 → clamped to 1
        var page0 = repo.GetPaged(0, 10);
        Assert.Equal(1, page0.PageIndex);
        Assert.Equal(10, page0.PageSize);

        // pageIndex = -5 → clamped to 1
        var pageNeg = repo.GetPaged(-5, 10);
        Assert.Equal(1, pageNeg.PageIndex);

        // pageSize = 0 → clamped to 1
        var size0 = repo.GetPaged(1, 0);
        Assert.Equal(1, size0.PageSize);

        // pageSize = -3 → clamped to 1
        var sizeNeg = repo.GetPaged(1, -3);
        Assert.Equal(1, sizeNeg.PageSize);

        // Clamped page 1 with size 1 returns the first item (highest CreateDate).
        var firstPage = repo.GetPaged(0, 0);
        Assert.Equal(1, firstPage.PageIndex);
        Assert.Equal(1, firstPage.PageSize);
        Assert.Single(firstPage.Items);
    }

    /// <summary>
    /// PBT-2.2 (Property 3 - User navigation).
    ///
    /// GetPaged eagerly loads the User navigation property on each item,
    /// consistent with the existing Get() behaviour.
    ///
    /// Validates: Requirements 2.3, 3.4
    /// </summary>
    [Fact]
    public void GetPaged_IncludesUserNavigationProperty()
    {
        // Arrange: create a user and subscriptions referencing that user.
        var user = new Users
        {
            UserId = 1,
            EmailAddress = "testuser@example.test",
            FullName = "Test User",
            CreatedDate = DateTime.UtcNow,
        };

        var subscriptions = Enumerable.Range(1, 3).Select(i => new Subscriptions
        {
            AmpsubscriptionId = Guid.NewGuid(),
            Name = $"sub-{i}",
            AmpplanId = "plan-a",
            AmpOfferId = "offer-a",
            SubscriptionStatus = "Subscribed",
            IsActive = true,
            CreateBy = 1,
            CreateDate = DateTime.UtcNow.AddMinutes(-i),
            ModifyDate = DateTime.UtcNow,
            Ampquantity = 1,
            UserId = user.UserId,
            PurchaserEmail = user.EmailAddress,
        }).ToList();

        var ctx = InMemorySaasKitContextFactory.Create();
        ctx.Database.EnsureCreated();
        ctx.Users.Add(user);
        ctx.Subscriptions.AddRange(subscriptions);
        ctx.SaveChanges();

        var repo = new SubscriptionsRepository(ctx);

        // Act
        var page = repo.GetPaged(1, 100);

        // Assert: all items with a UserId must have the User navigation property loaded.
        Assert.Equal(3, page.Items.Count);
        Assert.All(page.Items, s => Assert.NotNull(s.User));
        Assert.All(page.Items, s => Assert.Equal("testuser@example.test", s.User.EmailAddress));
    }
}
