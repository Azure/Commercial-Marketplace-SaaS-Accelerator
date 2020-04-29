﻿using Microsoft.Marketplace.SaasKit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Marketplace.SaasKit.Client.Models
{
    public enum SubscriptionStatusEnumExtension
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
        /// The not started
        /// </summary>
        NotStarted,

        /// <summary>
        /// Pending Activation
        /// </summary>
        PendingActivation,

        PendingUnsubscribe,

        ActivationFailure,

        UnsubscribeFailure,

        DeploymentPending,
        DeploymentSuccessful,
        DeploymentFailed,
        DeleteResourcePendign,
        DeleteResourceSuccess,
        DeleteResourceFailed
    }
}