// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

// =============================================================================
// Shared test-infrastructure helper for SubscriptionLazyLoaderHostedService.
//
// Extracted so it can be reused by IdempotentFetchPropertyTests (task 3.8).
// =============================================================================

using System;
using Marketplace.SaaS.Accelerator.AdminSite.Test.Doubles;
using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Services;
using Marketplace.SaaS.Accelerator.Services.Configurations;
using Marketplace.SaaS.Accelerator.Services.Services;
using Marketplace.SaaS.Accelerator.Services.Services.Hosted;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Marketplace.SaaS.Accelerator.AdminSite.Test.Hosted;

/// <summary>
/// Shared builder for <see cref="SubscriptionLazyLoaderHostedService"/> test instances.
/// Wires up a real in-memory EF Core context, real repositories, a
/// <see cref="FakeMarketplaceSaaSClient"/>, and the production
/// <see cref="SubscriptionFetchPipeline"/> so tests exercise real code with
/// only the network/database providers swapped.
/// </summary>
internal static class SubscriptionLazyLoaderHostedServiceTestHelper
{
    /// <summary>
    /// Build a <see cref="SubscriptionLazyLoaderHostedService"/> over a fresh
    /// in-memory database. Returns (service, assertionContext, dbName) so
    /// callers can create additional services sharing the same store via
    /// <see cref="BuildWithDbName"/>.
    /// </summary>
    public static (SubscriptionLazyLoaderHostedService service, SaasKitContext ctx, string dbName) Build(
        FakeMarketplaceSaaSClient fakeClient,
        int intervalSeconds,
        Action onTickComplete = null)
    {
        var dbName = Guid.NewGuid().ToString();
        var seedCtx = CreateContext(dbName);
        seedCtx.Database.EnsureCreated();
        var service = BuildCore(dbName, fakeClient, intervalSeconds, onTickComplete);
        return (service, seedCtx, dbName);
    }

    /// <summary>
    /// Build a <see cref="SubscriptionLazyLoaderHostedService"/> that shares the
    /// same in-memory database as a previously-built service.
    /// </summary>
    public static (SubscriptionLazyLoaderHostedService service, SaasKitContext ctx) BuildWithDbName(
        string dbName,
        FakeMarketplaceSaaSClient fakeClient,
        int intervalSeconds,
        Action onTickComplete = null)
    {
        var ctx = CreateContext(dbName);
        var service = BuildCore(dbName, fakeClient, intervalSeconds, onTickComplete);
        return (service, ctx);
    }

    /// <summary>
    /// Build a <see cref="SubscriptionLazyLoaderHostedService"/> that shares the
    /// same in-memory store as an existing <see cref="SaasKitContext"/>.
    /// The DB name is taken from <see cref="Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade.GetConnectionString"/>.
    /// </summary>
    public static (SubscriptionLazyLoaderHostedService service, SaasKitContext ctx) BuildWithExistingContext(
        SaasKitContext ctx,
        FakeMarketplaceSaaSClient fakeClient,
        int intervalSeconds,
        Action onTickComplete = null)
    {
        // For EF InMemory the "connection string" IS the database name.
        var dbName = ctx.Database.GetConnectionString() ?? Guid.NewGuid().ToString();
        var service = BuildCore(dbName, fakeClient, intervalSeconds, onTickComplete);
        return (service, ctx);
    }

    // -------------------------------------------------------------------------
    // Internal helpers
    // -------------------------------------------------------------------------

    private static SaasKitContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<SaasKitContext>()
            .UseInMemoryDatabase(dbName)
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new SaasKitContext(options);
    }

    private static SubscriptionLazyLoaderHostedService BuildCore(
        string dbName,
        FakeMarketplaceSaaSClient fakeClient,
        int intervalSeconds,
        Action onTickComplete)
    {
        var resilienceOptions = new MarketplaceResilienceOptions
        {
            BackgroundSyncIntervalSeconds = intervalSeconds,
            MaxConcurrentPlanFetches = 5,
            BackgroundSyncEnabled = true,
        };

        var services = new ServiceCollection();

        services.AddOptions();   // Required to support IOptions<T> resolution.
        services.AddLogging();   // Required to support ILogger<T> injection.

        // Register a DbContext factory pointing at the named in-memory database.
        // Each scope creates a fresh DbContext instance sharing the same store —
        // this avoids UsersRepository.Dispose() killing a singleton context.
        services.AddDbContext<SaasKitContext>(o =>
        {
            o.UseInMemoryDatabase(dbName);
            o.ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));
        });

        services.AddScoped<DataAccess.Contracts.ISubscriptionsRepository, SubscriptionsRepository>();
        services.AddScoped<DataAccess.Contracts.IUsersRepository, UsersRepository>();
        services.AddScoped<DataAccess.Contracts.IOffersRepository, OffersRepository>();
        services.AddScoped<DataAccess.Contracts.ISubscriptionLogRepository, SubscriptionLogRepository>();
        services.AddScoped<DataAccess.Contracts.IPlansRepository>(sp =>
        {
            var dbCtx = sp.GetRequiredService<SaasKitContext>();
            var appConfig = new ApplicationConfigRepository(dbCtx);
            return new PlansRepository(dbCtx, appConfig);
        });

        var sdkConfig = new SaaSApiClientConfiguration
        {
            FulFillmentAPIBaseURL = "https://localhost/fake-marketplace",
            FulFillmentAPIVersion = "2018-08-31",
        };
        var fulfillApiService = new FulfillmentApiService(
            fakeClient.Client, sdkConfig, new NullContractsLogger());

        services.AddScoped<Services.Contracts.IFulfillmentApiService>(_ => fulfillApiService);

        services.Configure<MarketplaceResilienceOptions>(o =>
        {
            o.BackgroundSyncIntervalSeconds = resilienceOptions.BackgroundSyncIntervalSeconds;
            o.MaxConcurrentPlanFetches = resilienceOptions.MaxConcurrentPlanFetches;
            o.BackgroundSyncEnabled = resilienceOptions.BackgroundSyncEnabled;
        });

        services.AddScoped<SubscriptionFetchPipeline>();

        var sp = services.BuildServiceProvider();
        var optionsInstance = Options.Create(resilienceOptions);
        var logger = NullLogger<SubscriptionLazyLoaderHostedService>.Instance;

        IServiceProvider rootProvider = onTickComplete != null
            ? new CallbackServiceProvider(sp, onTickComplete)
            : sp;

        return new SubscriptionLazyLoaderHostedService(rootProvider, optionsInstance, logger);
    }

    // -------------------------------------------------------------------------
    // Private support types
    // -------------------------------------------------------------------------

    private sealed class CallbackServiceProvider : IServiceProvider, IServiceScopeFactory
    {
        private readonly IServiceProvider inner;
        private readonly Action onTickComplete;

        public CallbackServiceProvider(IServiceProvider inner, Action callback)
        {
            this.inner = inner;
            this.onTickComplete = callback;
        }

        public object GetService(Type serviceType) =>
            serviceType == typeof(IServiceScopeFactory)
                ? this
                : inner.GetService(serviceType);

        public IServiceScope CreateScope() =>
            new CallbackScope(
                inner.GetRequiredService<IServiceScopeFactory>().CreateScope(),
                onTickComplete);

        private sealed class CallbackScope : IServiceScope
        {
            private readonly IServiceScope inner;
            private readonly Action onTickComplete;

            public CallbackScope(IServiceScope inner, Action callback)
            {
                this.inner = inner;
                this.onTickComplete = callback;
            }

            public IServiceProvider ServiceProvider => inner.ServiceProvider;

            public void Dispose()
            {
                inner.Dispose();
                try { onTickComplete?.Invoke(); } catch { /* swallow */ }
            }
        }
    }

    private sealed class NullContractsLogger : Services.Contracts.ILogger
    {
        public void Debug(string message) { }
        public void Debug(string message, Exception ex) { }
        public void Error(string message) { }
        public void Error(string message, Exception ex) { }
        public void Info(string message) { }
        public void Info(string message, Exception ex) { }
        public void Warn(string message) { }
        public void Warn(string message, Exception ex) { }
    }
}
