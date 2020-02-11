namespace Microsoft.Marketplace.SaasKit.Models
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.ComponentModel;


    /// <summary>
    /// the subscription list result
    /// </summary>
    public class SubscriptionListResult : SaaSApiResult
    {

        /// <summary>
        /// Gets or sets the subscriptions result.
        /// </summary>
        /// <value>
        /// The subscriptions result.
        /// </value>
        [JsonProperty("Subscriptions")]
        [DisplayName("Subscriptions")]
        public List<SubscriptionResult> SubscriptionsResult { get; set; }
    }
}
