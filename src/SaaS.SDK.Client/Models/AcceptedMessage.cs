namespace Microsoft.Marketplace.SaasKit.Models
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// Accepted Message.
    /// </summary>
    public class AcceptedMessage
    {
        /// <summary>
        /// Gets or sets the usage event identifier.
        /// </summary>
        /// <value>
        /// The usage event identifier.
        /// </value>
        [JsonProperty("usageEventId")]
        public string UsageEventId { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the message time.
        /// </summary>
        /// <value>
        /// The message time.
        /// </value>
        [JsonProperty("messageTime")]
        public string MessageTime { get; set; }

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
        public string Quantity { get; set; }

        /// <summary>
        /// Gets or sets the dimension.
        /// </summary>
        /// <value>
        /// The dimension.
        /// </value>
        [JsonProperty("dimension")]
        public string Dimension { get; set; }

        /// <summary>
        /// Gets or sets the effective start time.
        /// </summary>
        /// <value>
        /// The effective start time.
        /// </value>
        [JsonProperty("effectiveStartTime")]
        public string EffectiveStartTime { get; set; }

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