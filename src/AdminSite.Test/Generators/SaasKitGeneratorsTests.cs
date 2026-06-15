// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FsCheck;
using FsCheck.Xunit;
using Marketplace.SaaS.Accelerator.AdminSite.Test.Fixtures;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.Services.Models;
using Xunit;

namespace Marketplace.SaaS.Accelerator.AdminSite.Test.Generators;

/// <summary>
/// Trustworthiness tests for <see cref="SaasKitGenerators"/> and
/// <see cref="InMemorySaasKitContextFactory"/>. The property tests in 1.4 and
/// task 2 lean on these fixtures, so we verify the fixtures themselves before
/// any production-code property runs through them.
///
/// Tests in this file are intentionally fast: they exercise corpora drawn from
/// <see cref="SaasKitGenerators.DefaultSubscriptionCounts"/> (max 50 subs).
/// The 50_000-row case is gated behind <c>[Trait("category", "large")]</c>
/// so the default <c>dotnet test --filter "Category!=large"</c> excludes it.
/// </summary>
[Properties(Arbitrary = new[] { typeof(SaasKitArbitraries) })]
public class SaasKitGeneratorsTests
{
    /// <summary>
    /// Helper: sample a single corpus deterministically using FsCheck's
    /// `Gen.Sample` so we can shape assertions in non-property tests.
    /// </summary>
    private static SubscriptionCorpus SampleCorpus(int count) =>
        Gen.Sample(50, 1, SaasKitGenerators.Corpus(count)).Single();

    [Fact]
    public void Corpus_HasNoDanglingForeignKeys_ForRepresentativeCount()
    {
        var corpus = SampleCorpus(50);

        // Every plan's OfferId resolves to an offer's OfferGuid.
        var offerGuids = corpus.Offers.Select(o => o.OfferGuid).ToHashSet();
        Assert.All(corpus.Plans, p =>
            Assert.True(offerGuids.Contains(p.OfferId),
                $"Plan {p.PlanId} references unknown OfferGuid {p.OfferId}"));

        // Every subscription's AmpplanId resolves to a plan's PlanId.
        var planIds = corpus.Plans.Select(p => p.PlanId).ToHashSet();
        Assert.All(corpus.Subscriptions, s =>
            Assert.True(planIds.Contains(s.AmpplanId),
                $"Subscription {s.AmpsubscriptionId} references unknown PlanId {s.AmpplanId}"));

        // Every subscription's AmpOfferId resolves to an offer's OfferId.
        var offerIds = corpus.Offers.Select(o => o.OfferId).ToHashSet();
        Assert.All(corpus.Subscriptions, s =>
            Assert.True(offerIds.Contains(s.AmpOfferId),
                $"Subscription {s.AmpsubscriptionId} references unknown OfferId {s.AmpOfferId}"));

        // Every subscription's UserId (if set) resolves to a user.
        var userIds = corpus.Users.Select(u => u.UserId).ToHashSet();
        Assert.All(corpus.Subscriptions, s =>
        {
            if (s.UserId.HasValue)
            {
                Assert.True(userIds.Contains(s.UserId.Value),
                    $"Subscription {s.AmpsubscriptionId} references unknown UserId {s.UserId.Value}");
            }
        });
    }

    [Property(MaxTest = 10)]
    public Property Corpus_FromGenerator_HasNoDanglingForeignKeys(SubscriptionCorpus corpus)
    {
        var offerGuids = corpus.Offers.Select(o => o.OfferGuid).ToHashSet();
        var planIds = corpus.Plans.Select(p => p.PlanId).ToHashSet();
        var offerIds = corpus.Offers.Select(o => o.OfferId).ToHashSet();
        var userIds = corpus.Users.Select(u => u.UserId).ToHashSet();

        return (
            corpus.Plans.All(p => offerGuids.Contains(p.OfferId)) &&
            corpus.Subscriptions.All(s =>
                planIds.Contains(s.AmpplanId) &&
                offerIds.Contains(s.AmpOfferId) &&
                (!s.UserId.HasValue || userIds.Contains(s.UserId.Value)))
        ).ToProperty();
    }

    [Fact]
    public void CreateAndSeed_RoundTripsCorpusCounts()
    {
        var corpus = SampleCorpus(50);

        using var ctx = InMemorySaasKitContextFactory.CreateAndSeed(corpus);

        Assert.Equal(corpus.Offers.Count, ctx.Offers.Count());
        Assert.Equal(corpus.Plans.Count, ctx.Plans.Count());
        Assert.Equal(corpus.Users.Count, ctx.Users.Count());
        Assert.Equal(corpus.Subscriptions.Count, ctx.Subscriptions.Count());
        Assert.Equal(corpus.AuditLogs.Count, ctx.SubscriptionAuditLogs.Count());
    }

    [Fact]
    public void Create_WithDifferentNames_ProducesIsolatedDatabases()
    {
        using var ctxA = InMemorySaasKitContextFactory.Create();
        using var ctxB = InMemorySaasKitContextFactory.Create();

        ctxA.Offers.Add(new Offers
        {
            OfferId = "isolated-a",
            OfferName = "Isolated A",
            OfferGuid = Guid.NewGuid(),
            CreateDate = DateTime.UtcNow,
        });
        ctxA.SaveChanges();

        Assert.Equal(1, ctxA.Offers.Count());
        Assert.Equal(0, ctxB.Offers.Count());
    }

    [Fact]
    public void Create_WithSharedName_ShowsChangesAcrossInstances()
    {
        var name = $"shared-{Guid.NewGuid()}";

        using (var ctxA = InMemorySaasKitContextFactory.Create(name))
        {
            ctxA.Offers.Add(new Offers
            {
                OfferId = "shared-a",
                OfferName = "Shared A",
                OfferGuid = Guid.NewGuid(),
                CreateDate = DateTime.UtcNow,
            });
            ctxA.SaveChanges();
        }

        using var ctxB = InMemorySaasKitContextFactory.Create(name);
        Assert.Equal(1, ctxB.Offers.Count());
    }

    [Fact]
    public void Clone_ProducesDeepCopy_ThatDoesNotShareReferences()
    {
        var original = SampleCorpus(5);
        var clone = original.Clone();

        Assert.Equal(original.Offers.Count, clone.Offers.Count);
        Assert.Equal(original.Subscriptions.Count, clone.Subscriptions.Count);
        for (int i = 0; i < original.Offers.Count; i++)
        {
            Assert.NotSame(original.Offers[i], clone.Offers[i]);
            Assert.Equal(original.Offers[i].OfferId, clone.Offers[i].OfferId);
            Assert.Equal(original.Offers[i].OfferGuid, clone.Offers[i].OfferGuid);
        }

        // Mutating the clone does not affect the original.
        if (clone.Subscriptions.Count > 0)
        {
            clone.Subscriptions[0].SubscriptionStatus = "TamperedStatus";
            Assert.NotEqual("TamperedStatus", original.Subscriptions[0].SubscriptionStatus);
        }
    }

    [Fact]
    public void Clone_AllowsTwoContextsToBeSeededWithoutEfTrackingClash()
    {
        // The preservation tests in task 2 will seed F and F' from the same
        // logical corpus shape. Verify we can seed two contexts from a corpus
        // and its clone without EF tracking blowing up on shared references.
        var corpus = SampleCorpus(5);
        using var ctxF = InMemorySaasKitContextFactory.CreateAndSeed(corpus);
        using var ctxFprime = InMemorySaasKitContextFactory.CreateAndSeed(corpus.Clone());

        Assert.Equal(ctxF.Subscriptions.Count(), ctxFprime.Subscriptions.Count());
    }

    [Fact]
    public void Status_GeneratorCoversAllAuditedStatuses()
    {
        // Sample enough times that we expect every audited status to appear.
        var samples = Gen.Sample(50, 200, SaasKitGenerators.Status()).ToHashSet();
        foreach (var expected in SaasKitGenerators.AuditedStatuses)
        {
            Assert.Contains(expected, samples);
        }
    }

    [Fact]
    public void SpecSubscriptionCount_OnlyDrawsFromSpecValues()
    {
        var samples = Gen.Sample(50, 100, SaasKitGenerators.SpecSubscriptionCount()).ToHashSet();
        foreach (var n in samples)
        {
            Assert.Contains(n, SaasKitGenerators.SpecSubscriptionCounts);
        }
    }

    [Fact]
    public void Corpus_With200Subscriptions_GeneratesAndSeedsUnderFiveSeconds()
    {
        // Loose sanity check that the generator scales to the spec's middle
        // size band. Five seconds is generous to avoid CI flakiness; on a
        // dev machine this completes in well under a second.
        var stopwatch = Stopwatch.StartNew();
        var corpus = SampleCorpus(200);
        using var ctx = InMemorySaasKitContextFactory.CreateAndSeed(corpus);
        stopwatch.Stop();

        Assert.Equal(200, ctx.Subscriptions.Count());
        Assert.True(
            stopwatch.Elapsed < TimeSpan.FromSeconds(5),
            $"Generating + seeding 200 subscriptions took {stopwatch.ElapsedMilliseconds}ms (expected < 5000ms)");
    }

    /// <summary>
    /// 50_000-row corpus generation. Excluded from the default test run via
    /// the "large" trait — task 1.4 / 1.5 will opt in by running with
    /// <c>--filter "Category=large"</c>.
    /// </summary>
    [Fact]
    [Trait("category", "large")]
    public void Corpus_With50000Subscriptions_GeneratesAndSeeds()
    {
        // No timing assertion: this case exists to confirm the generator can
        // produce the spec's largest size on demand. The bug-condition test
        // uses it to drive the unbounded EF-query path.
        var corpus = SampleCorpus(50_000);
        using var ctx = InMemorySaasKitContextFactory.CreateAndSeed(corpus);
        Assert.Equal(50_000, ctx.Subscriptions.Count());
    }
}
