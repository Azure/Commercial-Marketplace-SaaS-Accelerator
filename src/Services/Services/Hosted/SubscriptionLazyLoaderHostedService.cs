// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.Services.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Marketplace.SaaS.Accelerator.Services.Services.Hosted;

/// <summary>
/// Background service that incrementally syncs subscription data from the
/// Marketplace API at a configurable interval (Requirement 2.5).
///
/// On each tick, this service:
///   1. Creates a new DI scope so the scoped <see cref="SubscriptionFetchPipeline"/>
///      is resolved fresh (EF Core DbContext is not safe to reuse across ticks).
///   2. Resolves (or upserts on first run) a synthetic system user so audit-log
///      foreign keys are valid.
///   3. Calls <see cref="SubscriptionFetchPipeline.ExecuteAsync"/> and emits a
///      structured log entry summarising the outcome.
///   4. Catches and logs all exceptions from the loop body so a transient failure
///      does not terminate the service (Requirement 2.8).
///   5. Waits <see cref="MarketplaceResilienceOptions.BackgroundSyncIntervalSeconds"/>
///      before the next tick, respecting the stop token.
/// </summary>
public sealed class SubscriptionLazyLoaderHostedService : BackgroundService
{
    /// <summary>
    /// E-mail address used for the synthetic system user that owns audit-log
    /// rows created by background-sync ticks.
    /// </summary>
    public const string SystemUserEmail = "system@saas-accelerator.local";

    private readonly IServiceProvider serviceProvider;
    private readonly MarketplaceResilienceOptions options;
    private readonly ILogger<SubscriptionLazyLoaderHostedService> logger;

    /// <summary>
    /// Initializes a new instance of <see cref="SubscriptionLazyLoaderHostedService"/>.
    /// </summary>
    /// <param name="serviceProvider">
    ///   Root service provider. A child scope is created per tick so scoped
    ///   dependencies (EF Core DbContext, SubscriptionFetchPipeline) are
    ///   lifetime-safe.
    /// </param>
    /// <param name="options">Resilience/background-sync configuration.</param>
    /// <param name="logger">Structured logger.</param>
    public SubscriptionLazyLoaderHostedService(
        IServiceProvider serviceProvider,
        IOptions<MarketplaceResilienceOptions> options,
        ILogger<SubscriptionLazyLoaderHostedService> logger)
    {
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        this.options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        LogStart();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = this.serviceProvider.CreateScope();

                // Resolve the system user ID (upserting the synthetic user on first run).
                var usersRepo = scope.ServiceProvider.GetRequiredService<IUsersRepository>();
                var systemUserId = ResolveSystemUserId(usersRepo);

                // Execute the fetch pipeline within this scope.
                var pipeline = scope.ServiceProvider.GetRequiredService<SubscriptionFetchPipeline>();
                var start = DateTimeOffset.UtcNow;
                var result = await pipeline.ExecuteAsync(systemUserId, stoppingToken).ConfigureAwait(false);

                LogTickCompleted(start, result);
            }
            catch (OperationCanceledException)
            {
                // Cancellation is expected on shutdown — exit the loop cleanly.
                break;
            }
            catch (Exception ex)
            {
                // Log and swallow: one failed tick must not stop the service.
                LogTickError(ex);
            }

            // Wait for the next tick, exiting cleanly if stopped during the delay.
            // We use at least 1ms so the await always yields to the scheduler, preventing
            // a tight CPU-spin when BackgroundSyncIntervalSeconds is configured as 0.
            try
            {
                await Task.Delay(
                    TimeSpan.FromMilliseconds(
                        Math.Max(1, this.options.BackgroundSyncIntervalSeconds * 1000.0)),
                    stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        LogStop();
    }

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

    /// <summary>
    /// Upserts the synthetic system user via <see cref="IUsersRepository.Save"/>
    /// (which is idempotent: it matches on e-mail and updates in place) and
    /// returns its stable <c>UserId</c>.
    /// </summary>
    private static int ResolveSystemUserId(IUsersRepository usersRepo)
    {
        var systemUser = new Users
        {
            EmailAddress = SystemUserEmail,
            FullName = "Background Sync System",
            CreatedDate = DateTime.UtcNow,
        };

        // IUsersRepository.Save is an upsert: if the e-mail already exists it
        // returns the existing UserId; otherwise it inserts and returns the new one.
        return usersRepo.Save(systemUser);
    }

    private void LogStart()
    {
        var payload = JsonSerializer.Serialize(new
        {
            @event = "background_sync_started",
            intervalSeconds = this.options.BackgroundSyncIntervalSeconds,
        });
        this.logger.LogInformation("{StructuredPayload}", payload);
    }

    private void LogStop()
    {
        var payload = JsonSerializer.Serialize(new
        {
            @event = "background_sync_stopped",
        });
        this.logger.LogInformation("{StructuredPayload}", payload);
    }

    private void LogTickCompleted(DateTimeOffset start, FetchResult result)
    {
        var elapsed = (DateTimeOffset.UtcNow - start).TotalMilliseconds;
        var payload = JsonSerializer.Serialize(new
        {
            @event = "background_sync_tick_completed",
            startUtc = start.ToString("o"),
            durationMs = (long)elapsed,
            total = result.Total,
            succeeded = result.Succeeded,
            failed = result.Failed,
        });
        this.logger.LogInformation("{StructuredPayload}", payload);
    }

    private void LogTickError(Exception ex)
    {
        var payload = JsonSerializer.Serialize(new
        {
            @event = "background_sync_tick_error",
            errorMessage = ex.Message,
            exceptionType = ex.GetType().Name,
        });
        this.logger.LogError(ex, "{StructuredPayload}", payload);
    }
}
