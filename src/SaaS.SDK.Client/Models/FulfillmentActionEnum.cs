// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.
namespace Microsoft.Marketplace.SaasKit.Models
{
    /// <summary>
    /// Fulfillment Action Enum.
    /// </summary>
    public enum SaaSResourceActionEnum
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
        /// The List All Plan
        /// </summary>
        LISTALLPLAN,

        /// <summary>
        /// The default
        /// </summary>
        DEFAULT,

        /// <summary>
        /// The operation status
        /// </summary>
        OPERATION_STATUS,

        /// <summary>
        /// The All Subscriptions
        /// </summary>
        ALL_SUBSCRIPTIONS,

        /// <summary>
        /// The subscription usageevent
        /// </summary>
        SUBSCRIPTION_USAGEEVENT,

        /// <summary>
        /// The subscription batch usageevent
        /// </summary>
        SUBSCRIPTION_BATCHUSAGEEVENT,
    }
}