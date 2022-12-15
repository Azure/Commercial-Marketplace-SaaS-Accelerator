// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.
namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// Sets Subscription Operation Status..
/// </summary>
public enum SubscriptionStatusEnum
{
    /// <summary>
    /// The pending fulfillment start
    /// </summary>
    PendingFulfillmentStart,

    /// <summary>
    /// The subscribed
    /// </summary>
    Subscribed,

    /// <summary>
    /// The unsubscribed
    /// </summary>
    Unsubscribed,

    /// <summary>
    /// Pending Activation
    /// </summary>
    PendingActivation,

    /// <summary>
    /// Pending Activation
    /// </summary>
    Suspended,
}