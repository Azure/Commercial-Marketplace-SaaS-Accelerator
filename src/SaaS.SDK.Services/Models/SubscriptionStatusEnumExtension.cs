﻿namespace Microsoft.Marketplace.SaaS.SDK.Services.Models
{
    /// <summary>
    /// Enumerator witrh new  status.
    /// </summary>
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

        /// <summary>
        /// The pending unsubscribe
        /// </summary>
        PendingUnsubscribe,

        /// <summary>
        /// The activation failed
        /// </summary>
        ActivationFailed,

        /// <summary>
        /// The unsubscribe failed
        /// </summary>
        UnsubscribeFailed,
    }
}
