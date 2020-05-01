namespace Microsoft.Marketplace.SaasKit.Models
{
    using Microsoft.Marketplace.SaasKit.Models;
    using Newtonsoft.Json;
    using System;

    /// <summary>
    /// Subscription Usage Result
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Models.SaaSApiResult" />
    public class MeteringUsageResult : SaaSApiResult
    {
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the usage posted date.
        /// </summary>
        /// <value>
        /// The usage posted date.
        /// </value>
        [JsonProperty("effectiveStartTime")]
        public DateTime? UsagePostedDate { get; set; }

        /// <summary>
        /// Gets or sets the usage event identifier.
        /// </summary>
        /// <value>
        /// The usage event identifier.
        /// </value>
        [JsonProperty("usageEventId")]
        public Guid UsageEventId { get; set; }

        /// <summary>
        /// Gets or sets the message time.
        /// </summary>
        /// <value>
        /// The message time.
        /// </value>
        [JsonProperty("messageTime")]
        public DateTime MessageTime { get; set; }

        /// <summary>
        /// Gets or sets the resource identifier.
        /// </summary>
        /// <value>
        /// The resource identifier.
        /// </value>
        [JsonProperty("resourceId")]
        public Guid ResourceId { get; set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        /// <value>
        /// The quantity.
        /// </value>
        [JsonProperty("quantity")]
        public long Quantity { get; set; }

        /// <summary>
        /// Gets or sets the dimension.
        /// </summary>
        /// <value>
        /// The dimension.
        /// </value>
        [JsonProperty("dimension")]
        public string Dimension { get; set; }

        /// <summary>
        /// Gets or sets the plan identifier.
        /// </summary>
        /// <value>
        /// The plan identifier.
        /// </value>
        [JsonProperty("planId")]
        public string PlanId { get; set; }
    }
}
