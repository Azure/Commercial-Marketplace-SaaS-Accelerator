// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Marketplace.SaaS.Accelerator.AdminSite.Test.Doubles;

/// <summary>
/// Concurrency-safe counters shared between the builder and the resulting
/// <see cref="FakeMarketplaceSaaSClient"/>. Every Moq setup callback that mutates
/// state goes through this object via <see cref="Interlocked"/> so PBT-driven
/// concurrent calls produce accurate peak/in-flight numbers.
/// </summary>
internal sealed class FakeMarketplaceSaaSClientObservers
{
    private int bulkListCallCount;
    private int currentInFlight;
    private int peakInFlight;
    private readonly ConcurrentDictionary<string, int> perSubscriptionCallCounts = new();
    private readonly ConcurrentQueue<DateTime> callTimestamps = new();

    public int BulkListCallCount => Volatile.Read(ref bulkListCallCount);

    public int PeakInFlight => Volatile.Read(ref peakInFlight);

    public IReadOnlyList<DateTime> CallTimestamps => callTimestamps.ToArray();

    public int PerSubscriptionCallCount(string subscriptionId) =>
        perSubscriptionCallCounts.TryGetValue(subscriptionId, out var count) ? count : 0;

    public InFlightScope EnterBulkList()
    {
        Interlocked.Increment(ref bulkListCallCount);
        callTimestamps.Enqueue(DateTime.UtcNow);
        return EnterInFlight();
    }

    public InFlightScope EnterPerSubscription(string subscriptionId)
    {
        perSubscriptionCallCounts.AddOrUpdate(subscriptionId, 1, (_, v) => v + 1);
        callTimestamps.Enqueue(DateTime.UtcNow);
        return EnterInFlight();
    }

    private InFlightScope EnterInFlight()
    {
        var newCount = Interlocked.Increment(ref currentInFlight);
        UpdateWatermark(newCount);
        return new InFlightScope(this);
    }

    private void Exit() => Interlocked.Decrement(ref currentInFlight);

    private void UpdateWatermark(int candidate)
    {
        // Lock-free CAS loop to record the maximum seen value.
        while (true)
        {
            var current = Volatile.Read(ref peakInFlight);
            if (candidate <= current)
            {
                return;
            }

            if (Interlocked.CompareExchange(ref peakInFlight, candidate, current) == current)
            {
                return;
            }
        }
    }

    /// <summary>
    /// Disposable scope that ensures the in-flight counter is decremented even
    /// when the wrapped operation throws.
    /// </summary>
    internal readonly struct InFlightScope : IDisposable
    {
        private readonly FakeMarketplaceSaaSClientObservers owner;

        public InFlightScope(FakeMarketplaceSaaSClientObservers owner)
        {
            this.owner = owner;
        }

        public void Dispose() => owner?.Exit();
    }
}
