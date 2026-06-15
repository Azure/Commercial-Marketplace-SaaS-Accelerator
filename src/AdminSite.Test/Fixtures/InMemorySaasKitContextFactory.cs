// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Marketplace.SaaS.Accelerator.AdminSite.Test.Fixtures;

/// <summary>
/// Builds <see cref="SaasKitContext"/> instances backed by EF Core's in-memory
/// provider. The factory deliberately returns the production
/// <see cref="SaasKitContext"/> (rather than a subclass or mock) so the
/// preservation tests in task 2 exercise the same DbContext type the live
/// pipeline uses — only the storage provider is swapped.
///
/// Each call to <see cref="Create(string)"/> with no name yields a brand-new
/// in-memory database (via <see cref="Guid.NewGuid"/>) so tests are isolated
/// by default. Pass an explicit name to share state between two contexts when
/// the test specifically needs to.
/// </summary>
public static class InMemorySaasKitContextFactory
{
    /// <summary>
    /// Create a fresh <see cref="SaasKitContext"/>. If <paramref name="databaseName"/>
    /// is null or empty a unique name is generated, isolating this database from
    /// every other test.
    /// </summary>
    public static SaasKitContext Create(string databaseName = null)
    {
        var name = string.IsNullOrEmpty(databaseName) ? Guid.NewGuid().ToString() : databaseName;

        var options = new DbContextOptionsBuilder<SaasKitContext>()
            .UseInMemoryDatabase(name)
            // The InMemory provider has no transaction support; the production
            // SaasKitContext does not enable transactions either, but EF emits a
            // warning the first time SaveChanges runs without one. Suppressing
            // that warning keeps test output focused on real failures.
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        return new SaasKitContext(options);
    }

    /// <summary>
    /// Create a fresh context, ensure its model is materialised, seed it with
    /// the supplied <paramref name="corpus"/>, save, and return the context.
    /// The caller owns the context and is responsible for disposing it.
    /// </summary>
    public static SaasKitContext CreateAndSeed(SubscriptionCorpus corpus, string databaseName = null)
    {
        if (corpus is null)
        {
            throw new ArgumentNullException(nameof(corpus));
        }

        var ctx = Create(databaseName);
        ctx.Database.EnsureCreated();
        Seed(ctx, corpus);
        return ctx;
    }

    /// <summary>
    /// Seed an existing context with the supplied corpus and save changes.
    /// Order matters: parents (Offers, Plans, Users) before children
    /// (Subscriptions) before grandchildren (SubscriptionAuditLogs).
    /// </summary>
    public static void Seed(SaasKitContext ctx, SubscriptionCorpus corpus)
    {
        if (ctx is null)
        {
            throw new ArgumentNullException(nameof(ctx));
        }
        if (corpus is null)
        {
            throw new ArgumentNullException(nameof(corpus));
        }

        if (corpus.Offers.Count > 0) ctx.Offers.AddRange(corpus.Offers);
        if (corpus.Plans.Count > 0) ctx.Plans.AddRange(corpus.Plans);
        if (corpus.Users.Count > 0) ctx.Users.AddRange(corpus.Users);
        if (corpus.Subscriptions.Count > 0) ctx.Subscriptions.AddRange(corpus.Subscriptions);
        if (corpus.AuditLogs.Count > 0) ctx.SubscriptionAuditLogs.AddRange(corpus.AuditLogs);

        ctx.SaveChanges();
    }
}
