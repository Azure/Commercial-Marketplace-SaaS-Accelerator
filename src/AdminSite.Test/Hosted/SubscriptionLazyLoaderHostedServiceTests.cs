// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

// =============================================================================
// Unit tests for SubscriptionLazyLoaderHostedService (task 3.8).
//
// Design references: design.md "SubscriptionLazyLoaderHostedService block"
//
// Covers:
//   1. Exception inside loop body does NOT terminate the service.
//   2. Graceful shutdown on stoppingToken cancellation.
//   3. Multiple ticks execute the pipeline multiple times.
//   4. Idempotence across consecutive ticks with the same Marketplace payload.
//
// Requirements: 2.5, 2.8
// =============================================================================

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Marketplace.SaaS.Accelerator.AdminSite.Test.Doubles;
using Marketplace.SaaS.Accelerator.Services.Services.Hosted;
using Microsoft.Marketplace.SaaS.Models;
using Xunit;

namespace Marketplace.SaaS.Accelerator.AdminSite.Test.Hosted;

/// <summary>
/// Unit tests for <see cref="SubscriptionLazyLoaderHostedService"/>.
///
/// Strategy: use <see cref="SubscriptionLazyLoaderHostedServiceTestHelper"/> to
/// wire a real in-memory EF Core context, real repositories, and the real
/// <see cref="Services.Services.SubscriptionFetchPipeline"/>. Inject a
/// <see cref="FakeMarketplaceSaaSClient"/> so no network calls are made.
/// Use a short (0-ms) BackgroundSyncInterval so tests complete without sleeping.
/// </summary>
public class SubscriptionLazyLoaderHostedServiceTests
{
    // -------------------------------------------------------------------------
    // 1. Exception inside the loop body must NOT terminate the service
    // -------------------------------------------------------------------------

    /// <summary>
    /// When the pipeline throws on the first tick (transient 429), the service
    /// must remain running and execute subsequent ticks.
    ///
    /// Validates: Requirements 2.5, 2.8
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_WhenPipelineThrowsOnFirstTick_ServiceContinuesToNextTick()
    {
        var ids = new[]
        {
            Guid.Parse("11111111-0001-0001-0001-111111111111"),
            Guid.Parse("11111111-0002-0002-0002-111111111111"),
        };
        var sdkSubscriptions = ids
            .Select((id, i) => FakeMarketplaceSaaSClientBuilder.CreateFullSubscription(
                id, "offer-ex", "plan-ex", SubscriptionStatusEnum.Subscribed, 1,
                $"ex-user-{i}@example.test"))
            .ToList();

        // First bulk-list call throws 429; subsequent calls succeed.
        var fakeClient = new FakeMarketplaceSaaSClientBuilder()
            .WithSeededSubscriptions(sdkSubscriptions)
            .WithTransient429Once()
            .Build();

        var (hostedService, _, _) = SubscriptionLazyLoaderHostedServiceTestHelper.Build(
            fakeClient, intervalSeconds: 0);

        await hostedService.StartAsync(CancellationToken.None);
        // Let the service run for 500ms: first tick throws (429), subsequent ticks succeed.
        await Task.Delay(500, CancellationToken.None);
        await hostedService.StopAsync(CancellationToken.None);

        // At least 2 bulk-list calls: first threw 429, second succeeded.
        Assert.True(
            fakeClient.BulkListCallCount >= 2,
            $"Expected >= 2 bulk-list calls after exception, got {fakeClient.BulkListCallCount}.");
    }

    /// <summary>
    /// When the pipeline always throws (sustained 503), the service must keep
    /// running and NOT propagate the exception.
    ///
    /// Validates: Requirements 2.5, 2.8
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_WhenPipelineAlwaysThrows_ServiceDoesNotTerminate()
    {
        var fakeClient = new FakeMarketplaceSaaSClientBuilder()
            .WithSustained503()
            .Build();

        var (hostedService, _, _) = SubscriptionLazyLoaderHostedServiceTestHelper.Build(
            fakeClient, intervalSeconds: 0);

        Exception serviceException = null;

        try
        {
            await hostedService.StartAsync(CancellationToken.None);
            await Task.Delay(300, CancellationToken.None);
        }
        catch (Exception ex)
        {
            serviceException = ex;
        }
        finally
        {
            await hostedService.StopAsync(CancellationToken.None);
        }

        Assert.Null(serviceException);
    }

    // -------------------------------------------------------------------------
    // 2. Graceful shutdown on stoppingToken cancellation
    // -------------------------------------------------------------------------

    /// <summary>
    /// When the stop token fires, the service must exit ExecuteAsync promptly
    /// without throwing an unhandled exception.
    ///
    /// Validates: Requirements 2.5
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_WhenStopTokenCancelled_ExitsCleanly()
    {
        var fakeClient = new FakeMarketplaceSaaSClientBuilder()
            .WithSeededSubscriptions(Enumerable.Empty<Subscription>())
            .Build();

        using var cts = new CancellationTokenSource();
        var (hostedService, _, _) = SubscriptionLazyLoaderHostedServiceTestHelper.Build(
            fakeClient, intervalSeconds: 60 /* long delay — we cancel before it elapses */);

        await hostedService.StartAsync(cts.Token);
        cts.Cancel();

        var stopTask = hostedService.StopAsync(CancellationToken.None);
        var completedInTime = await Task.WhenAny(stopTask, Task.Delay(5000)) == stopTask;

        Assert.True(completedInTime, "StopAsync did not complete within 5 seconds after cancellation.");
    }

    /// <summary>
    /// Cancelling during the Task.Delay between ticks must also exit cleanly.
    ///
    /// Validates: Requirements 2.5
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_CancelDuringDelay_ExitsCleanly()
    {
        var sdkSubscriptions = new[]
        {
            FakeMarketplaceSaaSClientBuilder.CreateFullSubscription(
                Guid.Parse("22222222-0001-0001-0001-222222222222"),
                "offer-delay", "plan-delay", SubscriptionStatusEnum.Subscribed, 1,
                "delay-user@example.test"),
        };

        var fakeClient = new FakeMarketplaceSaaSClientBuilder()
            .WithSeededSubscriptions(sdkSubscriptions)
            .Build();

        bool firstTickDone = false;
        SubscriptionLazyLoaderHostedService serviceRef = null;

        var (hostedService, _, _) = SubscriptionLazyLoaderHostedServiceTestHelper.Build(
            fakeClient, intervalSeconds: 30 /* long delay — cancelled before it elapses */,
            onTickComplete: () =>
            {
                if (!firstTickDone)
                {
                    firstTickDone = true;
                    serviceRef?.StopAsync(CancellationToken.None);
                }
            });

        serviceRef = hostedService;
        await hostedService.StartAsync(CancellationToken.None);
        await WaitForServiceToStop(hostedService, maxWaitSeconds: 5);

        Assert.True(firstTickDone, "First tick did not complete.");
    }

    // -------------------------------------------------------------------------
    // 3. Multiple ticks fire within a reasonable period
    // -------------------------------------------------------------------------

    /// <summary>
    /// With a 0-ms interval and an empty payload the service must tick at least
    /// N times before being stopped.
    ///
    /// Validates: Requirements 2.5
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_WithZeroIntervalAndEmptyPayload_TicksMultipleTimes()
    {
        var fakeClient = new FakeMarketplaceSaaSClientBuilder()
            .WithSeededSubscriptions(Enumerable.Empty<Subscription>())
            .Build();

        var (hostedService, _, _) = SubscriptionLazyLoaderHostedServiceTestHelper.Build(
            fakeClient, intervalSeconds: 0);

        await hostedService.StartAsync(CancellationToken.None);
        // Let the service run for 500ms — at 0ms intervals it should tick many times.
        await Task.Delay(500, CancellationToken.None);
        await hostedService.StopAsync(CancellationToken.None);

        Assert.True(
            fakeClient.BulkListCallCount >= 3,
            $"Expected >= 3 bulk-list calls in 500ms, got {fakeClient.BulkListCallCount}.");
    }

    // -------------------------------------------------------------------------
    // 4. Idempotence — same payload, consecutive ticks, stable DB state
    // -------------------------------------------------------------------------

    /// <summary>
    /// Two consecutive ticks with the same Marketplace payload must not create
    /// duplicate rows or spurious audit log entries.
    ///
    /// Validates: Requirements 2.5, 3.2 (Property 4 - Background Sync Idempotence)
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_TwoTicksSamePayload_DbStateIsIdempotent()
    {
        const int SubCount = 3;
        var ids = Enumerable.Range(1, SubCount)
            .Select(i => Guid.Parse($"33333333-{i:D4}-{i:D4}-{i:D4}-333333333333"))
            .ToList();
        var sdkSubscriptions = ids
            .Select((id, i) => FakeMarketplaceSaaSClientBuilder.CreateFullSubscription(
                id, "offer-idem", "plan-idem", SubscriptionStatusEnum.Subscribed, 2,
                $"idem-user-{i}@example.test"))
            .ToList();

        var fakeClient = new FakeMarketplaceSaaSClientBuilder()
            .WithSeededSubscriptions(sdkSubscriptions)
            .Build();

        var (hostedService, ctx, dbName) = SubscriptionLazyLoaderHostedServiceTestHelper.Build(
            fakeClient, intervalSeconds: 0);

        await hostedService.StartAsync(CancellationToken.None);
        // Let it run for 500ms so at least 2 ticks complete.
        await Task.Delay(500, CancellationToken.None);
        await hostedService.StopAsync(CancellationToken.None);

        // DB row counts must be stable: exactly SubCount subscriptions, no duplicates.
        Assert.Equal(SubCount, ctx.Subscriptions.Count());

        var auditAfterTwoTicks = ctx.SubscriptionAuditLogs.Count();

        // Third tick on the same DB — count must not grow.
        var fakeClient3 = new FakeMarketplaceSaaSClientBuilder()
            .WithSeededSubscriptions(sdkSubscriptions)
            .Build();

        var (hostedService3, _) = SubscriptionLazyLoaderHostedServiceTestHelper.BuildWithDbName(
            dbName, fakeClient3, intervalSeconds: 0);

        await hostedService3.StartAsync(CancellationToken.None);
        await Task.Delay(300, CancellationToken.None);
        await hostedService3.StopAsync(CancellationToken.None);

        Assert.Equal(SubCount, ctx.Subscriptions.Count());
        Assert.Equal(auditAfterTwoTicks, ctx.SubscriptionAuditLogs.Count());
    }

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

    /// <summary>
    /// Waits for the <see cref="SubscriptionLazyLoaderHostedService"/> to stop
    /// (either because <see cref="BackgroundService.ExecuteTask"/> completes or
    /// because <see cref="BackgroundService.StopAsync"/> was called) with a
    /// maximum wait of <paramref name="maxWaitSeconds"/>.
    /// </summary>
    private static async Task WaitForServiceToStop(
        SubscriptionLazyLoaderHostedService service,
        int maxWaitSeconds)
    {
        var deadline = Task.Delay(TimeSpan.FromSeconds(maxWaitSeconds));
        // ExecuteTask is public on IHostedService in .NET 8.
        var execTask = service.ExecuteTask;
        if (execTask != null)
        {
            await Task.WhenAny(execTask, deadline);
        }
        else
        {
            await deadline;
        }

        // Ensure the service is stopped regardless.
        await service.StopAsync(CancellationToken.None);
    }
}
