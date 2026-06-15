// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Marketplace.SaaS;
using Moq;

namespace Marketplace.SaaS.Accelerator.AdminSite.Test.Doubles;

/// <summary>
/// Result of <see cref="FakeMarketplaceSaaSClientBuilder.Build"/>: the configured
/// <see cref="IMarketplaceSaaSClient"/> mock plus side-channel observers that
/// property tests assert against.
/// </summary>
public sealed class FakeMarketplaceSaaSClient
{
    private readonly FakeMarketplaceSaaSClientObservers observers;

    internal FakeMarketplaceSaaSClient(
        Mock<IMarketplaceSaaSClient> clientMock,
        Mock<FulfillmentOperations> fulfillmentMock,
        Mock<SubscriptionOperations> operationsMock,
        FakeMarketplaceSaaSClientObservers observers)
    {
        ClientMock = clientMock;
        FulfillmentMock = fulfillmentMock;
        OperationsMock = operationsMock;
        this.observers = observers;
    }

    /// <summary>The Moq instance for the top-level Marketplace client.</summary>
    public Mock<IMarketplaceSaaSClient> ClientMock { get; }

    /// <summary>The Moq instance for the <c>Fulfillment</c> operation group.</summary>
    public Mock<FulfillmentOperations> FulfillmentMock { get; }

    /// <summary>The Moq instance for the <c>Operations</c> operation group.</summary>
    public Mock<SubscriptionOperations> OperationsMock { get; }

    /// <summary>Convenience pointer to <see cref="ClientMock"/>.Object.</summary>
    public IMarketplaceSaaSClient Client => ClientMock.Object;

    /// <summary>Number of times the bulk subscription list was invoked.</summary>
    public int BulkListCallCount => observers.BulkListCallCount;

    /// <summary>Peak concurrent in-flight calls observed across the entire client.</summary>
    public int PeakInFlight => observers.PeakInFlight;

    /// <summary>UTC timestamps of every invocation, for backoff-schedule assertions.</summary>
    public IReadOnlyList<DateTime> CallTimestamps => observers.CallTimestamps;

    /// <summary>Number of times the per-subscription plan fetch was invoked for a given id.</summary>
    public int PerSubscriptionCallCount(string subscriptionId) =>
        observers.PerSubscriptionCallCount(subscriptionId);

    /// <summary>Number of times the per-subscription plan fetch was invoked for a given id.</summary>
    public int PerSubscriptionCallCount(Guid subscriptionId) =>
        observers.PerSubscriptionCallCount(subscriptionId.ToString());
}
