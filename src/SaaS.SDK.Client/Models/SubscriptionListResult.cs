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

        /// <summary>
        /// Indicates the URL for retrieving more subscriptions.
        /// </summary>
        /// <value>
        /// This value will be null when no further subscriptions can be retrieved.
        /// This value will contain a URL for subsequent queries when most subscriptions
        /// exist to be retrieved.
        /// </value>
        [JsonProperty("@nextLink")]
        [DisplayName("NextLink")]
        public string NextLink { get; set; }
    }
}
