using System;
using System.Collections.Generic;
using System.Text;

namespace Saas.SDK.WebJob.Models
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
        /// The not started
        /// </summary>
        NotStarted,

        /// <summary>
        /// Pending Activation
        /// </summary>
        PendingActivation
    }
}
