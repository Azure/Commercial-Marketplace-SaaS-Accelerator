// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.
namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// Fulfillment Action Enum.
/// </summary>
public enum MarketplaceActionEnum
{
    /// <summary>
    /// The resolve
    /// </summary>
    RESOLVE,

    /// <summary>
    /// The activate
    /// </summary>
    ACTIVATE,

    /// <summary>
    /// The delete
    /// </summary>
    DELETE,

    /// <summary>
    /// The change plan
    /// </summary>
    CHANGE_PLAN,

    /// <summary>
    /// The change quantity
    /// </summary>
    CHANGE_QUANTITY,

    /// <summary>
    /// The default
    /// </summary>
    DEFAULT,

    /// <summary>
    /// The operation status
    /// </summary>
    OPERATION_STATUS,

    /// <summary>
    /// The operation status
    /// </summary>
    UPDATE_OPERATION_STATUS,

    /// <summary>
    /// The a Subscription
    /// </summary>
    GET_SUBSCRIPTION,

    /// <summary>
    /// The list plans
    /// </summary>
    GET_ALL_PLANS,

    /// <summary>
    /// The All Subscriptions
    /// </summary>
    GET_ALL_SUBSCRIPTIONS,

    /// <summary>
    /// The subscription usageevent
    /// </summary>
    SUBSCRIPTION_USAGEEVENT,

    /// <summary>
    /// The subscription batch usageevent
    /// </summary>
    SUBSCRIPTION_BATCHUSAGEEVENT,
}