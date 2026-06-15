// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.AdminSite.Test.Fixtures;

/// <summary>
/// Deterministic, in-memory corpus of EF entities used to seed the stub
/// <see cref="DataAccess.Context.SaasKitContext"/> for the bug-condition and
/// preservation tests in tasks 1.4 and 2.x.
///
/// The corpus carries the five entity sets that the unfixed
/// <c>HomeController.FetchAllSubscriptions</c> reads or writes:
/// <see cref="Offers"/>, <see cref="Plans"/>, <see cref="Users"/>,
/// <see cref="Subscriptions"/>, and (initially empty) <see cref="AuditLogs"/>.
///
/// The <see cref="Clone"/> helper produces a deep copy with no EF tracking
/// state, so two parallel contexts can be seeded with the same generated
/// shape (this is what enables the F vs F' preservation comparison in task 2).
/// </summary>
public sealed class SubscriptionCorpus
{
    /// <summary>The set of offers in the corpus. Each has a unique <see cref="DataAccess.Entities.Offers.OfferId"/> and <see cref="DataAccess.Entities.Offers.OfferGuid"/>.</summary>
    public IReadOnlyList<Offers> Offers { get; init; } = new List<Offers>();

    /// <summary>The set of plans. Each plan's <see cref="DataAccess.Entities.Plans.OfferId"/> resolves to an offer's <see cref="DataAccess.Entities.Offers.OfferGuid"/>.</summary>
    public IReadOnlyList<Plans> Plans { get; init; } = new List<Plans>();

    /// <summary>
    /// The set of users referenced by <see cref="Subscriptions"/>.
    /// Note: the production <c>IUsersRepository</c> binds <c>Users</c>
    /// (the EF-mapped purchaser/beneficiary table), not <c>KnownUsers</c>
    /// (the admin auth list), so the corpus uses <see cref="DataAccess.Entities.Users"/>.
    /// </summary>
    public IReadOnlyList<Users> Users { get; init; } = new List<Users>();

    /// <summary>The set of subscriptions. Each references a plan, offer, and user from the lists above.</summary>
    public IReadOnlyList<Subscriptions> Subscriptions { get; init; } = new List<Subscriptions>();

    /// <summary>Audit logs, initially empty — populated by the production fetch pipeline on detected change.</summary>
    public IReadOnlyList<SubscriptionAuditLogs> AuditLogs { get; init; } = new List<SubscriptionAuditLogs>();

    /// <summary>
    /// Returns a deep copy of the corpus with freshly-allocated entities and
    /// no EF-tracking state. Use before seeding a second context for a parallel
    /// F vs F' comparison so the two contexts do not share entity references.
    /// </summary>
    public SubscriptionCorpus Clone()
    {
        return new SubscriptionCorpus
        {
            Offers = Offers.Select(CloneOffer).ToList(),
            Plans = Plans.Select(ClonePlan).ToList(),
            Users = Users.Select(CloneUser).ToList(),
            Subscriptions = Subscriptions.Select(CloneSubscription).ToList(),
            AuditLogs = AuditLogs.Select(CloneAuditLog).ToList(),
        };
    }

    private static Offers CloneOffer(Offers o) => new Offers
    {
        Id = o.Id,
        OfferId = o.OfferId,
        OfferName = o.OfferName,
        CreateDate = o.CreateDate,
        UserId = o.UserId,
        OfferGuid = o.OfferGuid,
    };

    private static Plans ClonePlan(Plans p) => new Plans
    {
        Id = p.Id,
        PlanId = p.PlanId,
        Description = p.Description,
        DisplayName = p.DisplayName,
        IsmeteringSupported = p.IsmeteringSupported,
        IsPerUser = p.IsPerUser,
        PlanGuid = p.PlanGuid,
        OfferId = p.OfferId,
    };

    private static Users CloneUser(Users u) => new Users
    {
        UserId = u.UserId,
        EmailAddress = u.EmailAddress,
        CreatedDate = u.CreatedDate,
        FullName = u.FullName,
    };

    private static Subscriptions CloneSubscription(Subscriptions s) => new Subscriptions
    {
        Id = s.Id,
        AmpsubscriptionId = s.AmpsubscriptionId,
        SubscriptionStatus = s.SubscriptionStatus,
        AmpplanId = s.AmpplanId,
        AmpOfferId = s.AmpOfferId,
        IsActive = s.IsActive,
        CreateBy = s.CreateBy,
        CreateDate = s.CreateDate,
        ModifyDate = s.ModifyDate,
        UserId = s.UserId,
        Name = s.Name,
        Ampquantity = s.Ampquantity,
        PurchaserEmail = s.PurchaserEmail,
        PurchaserTenantId = s.PurchaserTenantId,
        Term = s.Term,
        StartDate = s.StartDate,
        EndDate = s.EndDate,
    };

    private static SubscriptionAuditLogs CloneAuditLog(SubscriptionAuditLogs a) => new SubscriptionAuditLogs
    {
        Id = a.Id,
        SubscriptionId = a.SubscriptionId,
        Attribute = a.Attribute,
        OldValue = a.OldValue,
        NewValue = a.NewValue,
        CreateDate = a.CreateDate,
        CreateBy = a.CreateBy,
    };
}
