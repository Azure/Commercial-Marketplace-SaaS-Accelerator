using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.Provisioning.Webjob.Models
{
    public enum SubscriptionWebJobStatusEnum
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

        PendingUnsubscribe,

        ActivationFailed,

        UnsubscribeFailed,

        DeploymentPending,
        DeploymentSuccessful,
        DeploymentFailed,
        DeleteResourcePendign,
        DeleteResourceSuccess,
        DeleteResourceFailed


    }
}
