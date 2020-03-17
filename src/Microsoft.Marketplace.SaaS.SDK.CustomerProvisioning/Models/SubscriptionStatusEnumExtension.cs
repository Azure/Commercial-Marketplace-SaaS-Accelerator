using Microsoft.Marketplace.SaasKit.Models;
namespace Microsoft.Marketplace.SaasKit.Client.Models
{
    /// <summary>
    /// Sets Subscription Operation Status
    /// </summary>
    public enum SubscriptionStatusEnumExtension
    {
        /// <summary>
        /// The pending fulfillment start
        /// </summary>
        PendingFulfillmentStart = SubscriptionStatusEnum.PendingFulfillmentStart,

        /// <summary>
        /// The subscribed
        /// </summary>
        Subscribed = SubscriptionStatusEnum.Subscribed,

        /// <summary>
        /// The unsubscribed
        /// </summary>
        Unsubscribed = SubscriptionStatusEnum.Unsubscribed,

        /// <summary>
        /// The not started
        /// </summary>
        NotStarted = SubscriptionStatusEnum.NotStarted,
    }
}


