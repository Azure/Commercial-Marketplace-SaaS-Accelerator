// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using FsCheck;
using Marketplace.SaaS.Accelerator.AdminSite.Test.Fixtures;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.Services.Models;
using Microsoft.FSharp.Core;

namespace Marketplace.SaaS.Accelerator.AdminSite.Test.Generators;

/// <summary>
/// LINQ helpers for <see cref="Gen{T}"/>. FsCheck 2.16's C# surface exposes
/// <c>Select</c> but not <c>SelectMany</c>, so we route monadic bind through
/// the public F# operator <c>op_GreaterGreaterEquals</c> (a.k.a. <c>&gt;&gt;=</c>).
/// Using the underlying bind keeps shrinking semantics intact and lets the
/// corpus generators below read as natural LINQ query expressions.
/// </summary>
internal static class GenLinqExtensions
{
    public static Gen<TResult> SelectMany<T, TResult>(
        this Gen<T> source,
        Func<T, Gen<TResult>> selector)
    {
        // FsCheck 2.x's `>>=` (Bind) operator takes an FSharpFunc. The
        // FSharp.Core version pulled in by FsCheck 2.16 (4.2.3) does not
        // expose FuncConvert.ToFSharpFunc(Converter<,>), so we wrap the C#
        // delegate in a small adapter that derives directly from FSharpFunc.
        var fsharpFunc = new CSharpFSharpFunc<T, Gen<TResult>>(selector);
        return Gen<T>.op_GreaterGreaterEquals<T, TResult>(source, fsharpFunc);
    }

    private sealed class CSharpFSharpFunc<TArg, TResult> : FSharpFunc<TArg, TResult>
    {
        private readonly Func<TArg, TResult> inner;
        public CSharpFSharpFunc(Func<TArg, TResult> inner) => this.inner = inner;
        public override TResult Invoke(TArg arg) => inner(arg);
    }

    public static Gen<TResult> SelectMany<T, TIntermediate, TResult>(
        this Gen<T> source,
        Func<T, Gen<TIntermediate>> intermediateSelector,
        Func<T, TIntermediate, TResult> resultSelector)
    {
        return source.SelectMany(t =>
            intermediateSelector(t).Select(i => resultSelector(t, i)));
    }
}

/// <summary>
/// FsCheck generators that build <see cref="SubscriptionCorpus"/> shapes and
/// the EF entity types they contain. These generators are the single source of
/// random subscription input for both the bug-condition exploration test
/// (task 1.4) and the preservation property tests (tasks 2.4–2.7).
///
/// The generators are deliberately conservative about which fields they
/// populate — only the columns that the unfixed
/// <c>HomeController.FetchAllSubscriptions</c> reads or writes are filled.
/// Every other column is left at its default so any future code path that
/// relies on a different field will surface as a clear NullReference rather
/// than silent randomized noise.
/// </summary>
public static class SaasKitGenerators
{
    /// <summary>
    /// Subscription counts called out in <c>tasks.md</c> 1.3 — discrete uniform
    /// over the spec's representative sizes. The 50_000 case is opt-in via the
    /// "large" trait; tests that take a default count should prefer
    /// <see cref="DefaultSubscriptionCounts"/>.
    /// </summary>
    public static readonly int[] SpecSubscriptionCounts = { 50, 200, 50_000 };

    /// <summary>Smaller boundary set for fast tests: zero, one, a few, fifty.</summary>
    public static readonly int[] DefaultSubscriptionCounts = { 0, 1, 5, 50 };

    /// <summary>Subscription statuses the existing audit-log writes round-trip through.</summary>
    public static readonly SubscriptionStatusEnumExtension[] AuditedStatuses =
    {
        SubscriptionStatusEnumExtension.PendingFulfillmentStart,
        SubscriptionStatusEnumExtension.PendingActivation,
        SubscriptionStatusEnumExtension.Subscribed,
        SubscriptionStatusEnumExtension.PendingUnsubscribe,
        SubscriptionStatusEnumExtension.Unsubscribed,
        SubscriptionStatusEnumExtension.ActivationFailed,
        SubscriptionStatusEnumExtension.UnsubscribeFailed,
        SubscriptionStatusEnumExtension.Suspend,
    };

    /// <summary>Subscription count generator — discrete uniform over the spec sizes.</summary>
    public static Gen<int> SpecSubscriptionCount() => Gen.Elements(SpecSubscriptionCounts);

    /// <summary>Subscription count generator — small/boundary sizes for fast tests.</summary>
    public static Gen<int> DefaultSubscriptionCount() => Gen.Elements(DefaultSubscriptionCounts);

    /// <summary>Status generator — uniform over the audit-log statuses.</summary>
    public static Gen<SubscriptionStatusEnumExtension> Status() => Gen.Elements(AuditedStatuses);

    /// <summary>Generate a non-empty alphanumeric token of the given length.</summary>
    private static Gen<string> Token(int length) =>
        Gen.ArrayOf(length, Gen.Elements("abcdefghijklmnopqrstuvwxyz0123456789".ToCharArray()))
            .Select(chars => new string(chars));

    /// <summary>Generate a positive int up to <paramref name="max"/> inclusive.</summary>
    private static Gen<int> PositiveIntUpTo(int max) => Gen.Choose(1, max);

    /// <summary>
    /// Generate one <see cref="Offers"/> row with a non-empty OfferId, OfferName,
    /// CreateDate in the recent past, and a fresh OfferGuid. The
    /// <paramref name="seed"/> is folded into the OfferId so multiple offers
    /// generated as part of the same corpus do not collide.
    /// </summary>
    public static Gen<Offers> Offer(int seed) =>
        Token(8).Select(suffix => new Offers
        {
            OfferId = $"offer-{seed:D5}-{suffix}",
            OfferName = $"Offer {seed}",
            OfferGuid = Guid.NewGuid(),
            CreateDate = DateTime.UtcNow,
            UserId = null,
        });

    /// <summary>
    /// Generate one <see cref="Plans"/> row whose <c>OfferId</c> (Guid) resolves
    /// to one of the supplied <paramref name="offers"/>. <paramref name="seed"/>
    /// disambiguates plan ids so multiple plans per offer do not collide.
    /// </summary>
    public static Gen<Plans> Plan(IReadOnlyList<Offers> offers, int seed) =>
        Gen.Elements<Offers>(offers).Zip(Token(6), (offer, suffix) => new Plans
        {
            PlanId = $"plan-{seed:D5}-{suffix}",
            DisplayName = $"Plan {seed}",
            Description = "Generated by SaasKitGenerators",
            PlanGuid = Guid.NewGuid(),
            OfferId = offer.OfferGuid,
            IsmeteringSupported = false,
            IsPerUser = false,
        });

    /// <summary>
    /// Generate one <see cref="Users"/> row with a non-empty email and full name.
    /// </summary>
    public static Gen<Users> User(int seed) =>
        Token(8).Select(suffix => new Users
        {
            EmailAddress = $"user-{seed:D5}-{suffix}@example.test",
            FullName = $"User {seed}",
            CreatedDate = DateTime.UtcNow,
        });

    /// <summary>
    /// Generate one <see cref="Subscriptions"/> row referencing a plan, the offer
    /// the plan belongs to, and a user from the supplied collections. The
    /// generated row populates every field the unfixed FetchAllSubscriptions
    /// path reads or writes.
    /// </summary>
    public static Gen<Subscriptions> Subscription(
        IReadOnlyList<Offers> offers,
        IReadOnlyList<Plans> plans,
        IReadOnlyList<Users> users) =>
        from plan in Gen.Elements<Plans>(plans)
        from user in Gen.Elements<Users>(users)
        from status in Status()
        from quantity in PositiveIntUpTo(100)
        from name in Token(10)
        select BuildSubscription(plan, user, status, quantity, name, offers);

    private static Subscriptions BuildSubscription(
        Plans plan,
        Users user,
        SubscriptionStatusEnumExtension status,
        int quantity,
        string name,
        IReadOnlyList<Offers> offers)
    {
        var offer = offers.First(o => o.OfferGuid == plan.OfferId);
        return new Subscriptions
        {
            AmpsubscriptionId = Guid.NewGuid(),
            Name = $"sub-{name}",
            AmpplanId = plan.PlanId,
            AmpOfferId = offer.OfferId,
            Ampquantity = quantity,
            SubscriptionStatus = status.ToString(),
            IsActive = status != SubscriptionStatusEnumExtension.Unsubscribed,
            CreateBy = 1,
            CreateDate = DateTime.UtcNow,
            ModifyDate = DateTime.UtcNow,
            UserId = user.UserId == 0 ? null : user.UserId,
            PurchaserEmail = user.EmailAddress,
            PurchaserTenantId = Guid.NewGuid(),
            Term = "Month",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(1),
        };
    }

    /// <summary>
    /// Generate a complete <see cref="SubscriptionCorpus"/> with the given
    /// number of subscriptions. Composes <see cref="Offer"/>, <see cref="Plan"/>,
    /// <see cref="User"/>, and <see cref="Subscription"/> so plans reference
    /// offers, subscriptions reference plans, offers, and users, and there are
    /// no dangling foreign keys.
    /// </summary>
    public static Gen<SubscriptionCorpus> Corpus(int subscriptionCount)
    {
        if (subscriptionCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(subscriptionCount));
        }

        // Heuristic: a few offers, ~3 plans per offer, a handful of users.
        // The exact ratios don't matter for the property tests so long as the
        // graph is connected and FK-clean. We clamp the lower bound so even a
        // zero-subscription corpus still has a parent table to write into.
        int offerCount = Math.Max(1, Math.Min(5, 1 + subscriptionCount / 100));
        int planCount = offerCount * 3;
        int userCount = Math.Max(1, Math.Min(20, 1 + subscriptionCount / 25));

        var offerGens = Enumerable.Range(0, offerCount).Select(Offer);
        var plansGen = Gen.Sequence(offerGens).Select(seq =>
        {
            var offers = seq.ToList();
            AssignSurrogateKeys(offers);
            return offers as IReadOnlyList<Offers>;
        });

        return
            from offers in plansGen
            from plans in BuildPlans(offers, planCount)
            from users in BuildUsers(userCount)
            from subscriptions in BuildSubscriptions(offers, plans, users, subscriptionCount)
            select new SubscriptionCorpus
            {
                Offers = offers,
                Plans = plans,
                Users = users,
                Subscriptions = subscriptions,
                AuditLogs = Array.Empty<SubscriptionAuditLogs>(),
            };
    }

    private static Gen<IReadOnlyList<Plans>> BuildPlans(IReadOnlyList<Offers> offers, int planCount) =>
        Gen.Sequence(Enumerable.Range(0, planCount).Select(i => Plan(offers, i)))
            .Select(seq =>
            {
                var plans = seq.ToList();
                AssignSurrogateKeys(plans);
                return plans as IReadOnlyList<Plans>;
            });

    private static Gen<IReadOnlyList<Users>> BuildUsers(int userCount) =>
        Gen.Sequence(Enumerable.Range(0, userCount).Select(User))
            .Select(seq =>
            {
                var users = seq.ToList();
                AssignSurrogateKeys(users);
                return users as IReadOnlyList<Users>;
            });

    private static Gen<IReadOnlyList<Subscriptions>> BuildSubscriptions(
        IReadOnlyList<Offers> offers,
        IReadOnlyList<Plans> plans,
        IReadOnlyList<Users> users,
        int subscriptionCount) =>
        Gen.Sequence(Enumerable.Range(0, subscriptionCount)
                .Select(_ => Subscription(offers, plans, users)))
            .Select(seq => seq.ToList() as IReadOnlyList<Subscriptions>);

    /// <summary>
    /// Generate a corpus whose subscription count is drawn from
    /// <see cref="DefaultSubscriptionCounts"/>. Use this in property tests
    /// that should run quickly by default.
    /// </summary>
    public static Gen<SubscriptionCorpus> DefaultCorpus() =>
        DefaultSubscriptionCount().SelectMany(Corpus);

    /// <summary>
    /// Generate a corpus whose subscription count is drawn from the
    /// spec sizes (50, 200, 50_000). The 50_000 case is heavyweight and should
    /// be run from <c>[Trait("category", "large")]</c> tests only.
    /// </summary>
    public static Gen<SubscriptionCorpus> SpecCorpus() =>
        SpecSubscriptionCount().SelectMany(Corpus);

    /// <summary>
    /// In-memory EF databases require non-zero surrogate keys for navigation
    /// to round-trip; the production code uses identity columns when the
    /// generator runs against the real provider. We assign sequential keys
    /// here so the generator output is FK-clean before SaveChanges.
    /// </summary>
    private static void AssignSurrogateKeys(IList<Offers> offers)
    {
        for (int i = 0; i < offers.Count; i++) offers[i].Id = i + 1;
    }

    private static void AssignSurrogateKeys(IList<Plans> plans)
    {
        for (int i = 0; i < plans.Count; i++) plans[i].Id = i + 1;
    }

    private static void AssignSurrogateKeys(IList<Users> users)
    {
        for (int i = 0; i < users.Count; i++) users[i].UserId = i + 1;
    }
}

/// <summary>
/// Arbitraries wrapper that <see cref="FsCheck.Xunit.PropertyAttribute"/> /
/// <see cref="FsCheck.Xunit.PropertiesAttribute"/> consume to auto-register
/// the generators above. Apply
/// <c>[Properties(Arbitrary = new[] { typeof(SaasKitArbitraries) })]</c> at
/// the test-class level (or the same option on individual <c>[Property]</c>
/// attributes) to take <see cref="SubscriptionCorpus"/> /
/// <see cref="SubscriptionStatusEnumExtension"/> arguments without per-test
/// boilerplate.
/// </summary>
public static class SaasKitArbitraries
{
    /// <summary>Arbitrary for <see cref="SubscriptionCorpus"/> using <see cref="SaasKitGenerators.DefaultCorpus"/>.</summary>
    public static Arbitrary<SubscriptionCorpus> Corpus() =>
        Arb.From(SaasKitGenerators.DefaultCorpus());

    /// <summary>Arbitrary for <see cref="SubscriptionStatusEnumExtension"/> using <see cref="SaasKitGenerators.Status"/>.</summary>
    public static Arbitrary<SubscriptionStatusEnumExtension> Status() =>
        Arb.From(SaasKitGenerators.Status());
}
