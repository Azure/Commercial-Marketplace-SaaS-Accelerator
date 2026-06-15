// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Microsoft.Marketplace.SaaS.Models;
using Xunit;

namespace Marketplace.SaaS.Accelerator.AdminSite.Test.Doubles;

/// <summary>
/// Trustworthiness tests for <see cref="FakeMarketplaceSaaSClientBuilder"/>.
/// These verify the fake itself is correctly wired so the property test in 1.4
/// can rely on it without re-checking every Moq detail.
/// </summary>
public class FakeMarketplaceSaaSClientBuilderTests
{
    private static readonly Guid SubA = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid SubB = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    private static readonly Guid SubC = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

    [Fact]
    public async Task SeededSubscriptions_AreReturnedFromBulkList()
    {
        var fake = new FakeMarketplaceSaaSClientBuilder()
            .WithSeededSubscriptions(new[] { SubA, SubB, SubC })
            .Build();

        var subscriptions = new List<Subscription>();
        await foreach (var s in fake.Client.Fulfillment.ListSubscriptionsAsync())
        {
            subscriptions.Add(s);
        }

        Assert.Equal(new[] { SubA, SubB, SubC }, subscriptions.Select(s => s.Id!.Value));
        Assert.Equal(1, fake.BulkListCallCount);
    }

    [Fact]
    public async Task Transient429Once_FailsFirstCall_ThenSucceeds()
    {
        var fake = new FakeMarketplaceSaaSClientBuilder()
            .WithTransient429Once()
            .WithSeededSubscriptions(new[] { SubA })
            .Build();

        // First call: AsyncPageable iteration triggers the failure path on enumeration.
        var firstCall = await Record.ExceptionAsync(async () =>
        {
            await foreach (var _ in fake.Client.Fulfillment.ListSubscriptionsAsync())
            {
                // empty
            }
        });

        var rfe = Assert.IsType<RequestFailedException>(firstCall);
        Assert.Equal(429, rfe.Status);

        // Second call succeeds.
        var subscriptions = new List<Subscription>();
        await foreach (var s in fake.Client.Fulfillment.ListSubscriptionsAsync())
        {
            subscriptions.Add(s);
        }

        Assert.Single(subscriptions);
        Assert.Equal(2, fake.BulkListCallCount);
    }

    [Fact]
    public async Task Subscription500_AppliesOnlyToTargetedId()
    {
        var fake = new FakeMarketplaceSaaSClientBuilder()
            .WithSeededSubscriptions(new[] { SubA, SubB })
            .WithSubscription500(SubA)
            .Build();

        var ex = await Record.ExceptionAsync(() =>
            fake.Client.Fulfillment.ListAvailablePlansAsync(SubA));
        var rfe = Assert.IsType<RequestFailedException>(ex);
        Assert.Equal(500, rfe.Status);

        // SubB succeeds.
        var response = await fake.Client.Fulfillment.ListAvailablePlansAsync(SubB);
        Assert.NotNull(response.Value);

        Assert.Equal(1, fake.PerSubscriptionCallCount(SubA));
        Assert.Equal(1, fake.PerSubscriptionCallCount(SubB));
    }

    [Fact]
    public async Task Sustained503_ThrowsIndefinitely()
    {
        var fake = new FakeMarketplaceSaaSClientBuilder()
            .WithSustained503()
            .WithSeededSubscriptions(new[] { SubA })
            .Build();

        for (int i = 0; i < 5; i++)
        {
            var ex = await Record.ExceptionAsync(() =>
                fake.Client.Fulfillment.ListAvailablePlansAsync(SubA));
            var rfe = Assert.IsType<RequestFailedException>(ex);
            Assert.Equal(503, rfe.Status);
        }

        Assert.Equal(5, fake.PerSubscriptionCallCount(SubA));
    }

    [Fact]
    public async Task Sustained503Bounded_ThrowsForFirstNCallsThenSucceeds()
    {
        var fake = new FakeMarketplaceSaaSClientBuilder()
            .WithSustained503(maxCalls: 2)
            .WithSeededSubscriptions(new[] { SubA })
            .Build();

        // First two calls throw.
        for (int i = 0; i < 2; i++)
        {
            var ex = await Record.ExceptionAsync(() =>
                fake.Client.Fulfillment.ListAvailablePlansAsync(SubA));
            Assert.IsType<RequestFailedException>(ex);
        }

        // Third call succeeds.
        var response = await fake.Client.Fulfillment.ListAvailablePlansAsync(SubA);
        Assert.NotNull(response.Value);
    }

    [Fact]
    public async Task DelayPerCall_IntroducesMeasurableDelay()
    {
        var delay = TimeSpan.FromMilliseconds(80);
        var fake = new FakeMarketplaceSaaSClientBuilder()
            .WithDelayPerCall(delay)
            .WithSeededSubscriptions(new[] { SubA })
            .Build();

        var stopwatch = Stopwatch.StartNew();
        await fake.Client.Fulfillment.ListAvailablePlansAsync(SubA);
        stopwatch.Stop();

        Assert.True(
            stopwatch.Elapsed >= TimeSpan.FromMilliseconds(60),
            $"Expected at least ~60ms delay, observed {stopwatch.ElapsedMilliseconds}ms");
    }

    [Fact]
    public async Task CallTimestamps_RecordOneEntryPerInvocation()
    {
        var fake = new FakeMarketplaceSaaSClientBuilder()
            .WithSeededSubscriptions(new[] { SubA, SubB })
            .Build();

        await foreach (var _ in fake.Client.Fulfillment.ListSubscriptionsAsync())
        {
            // drain
        }

        await fake.Client.Fulfillment.ListAvailablePlansAsync(SubA);
        await fake.Client.Fulfillment.ListAvailablePlansAsync(SubB);

        Assert.Equal(3, fake.CallTimestamps.Count);
        Assert.All(fake.CallTimestamps, ts =>
            Assert.True(ts <= DateTime.UtcNow && ts > DateTime.UtcNow.AddMinutes(-1)));
    }

    [Fact]
    public async Task PeakInFlight_TracksConcurrentCalls()
    {
        var delay = TimeSpan.FromMilliseconds(120);
        var fake = new FakeMarketplaceSaaSClientBuilder()
            .WithDelayPerCall(delay)
            .WithSeededSubscriptions(new[] { SubA, SubB, SubC })
            .Build();

        // Fire three per-subscription calls concurrently. Each holds the
        // in-flight slot for ~120ms, so the watermark should reach 3.
        var tasks = new[]
        {
            fake.Client.Fulfillment.ListAvailablePlansAsync(SubA),
            fake.Client.Fulfillment.ListAvailablePlansAsync(SubB),
            fake.Client.Fulfillment.ListAvailablePlansAsync(SubC),
        };

        await Task.WhenAll(tasks);

        Assert.Equal(3, fake.PeakInFlight);
        Assert.Equal(1, fake.PerSubscriptionCallCount(SubA));
        Assert.Equal(1, fake.PerSubscriptionCallCount(SubB));
        Assert.Equal(1, fake.PerSubscriptionCallCount(SubC));
    }
}
