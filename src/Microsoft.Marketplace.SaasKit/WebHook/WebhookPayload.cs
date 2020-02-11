namespace Microsoft.Marketplace.SaasKit.WebHook
{
    using System;
    using Microsoft.Marketplace.SaasKit.Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Web hook Payload get or set the API Response Data
    /// </summary>
    public class WebhookPayload
    {
        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        [JsonProperty(PropertyName = "action")]
        [JsonConverter(typeof(StringEnumConverter))]
        public WebhookAction Action { get; set; }

        /// <summary>
        /// Gets or sets the activity identifier.
        /// </summary>
        /// <value>
        /// The activity identifier.
        /// </value>
        [JsonProperty(PropertyName = "activityId")]
        public Guid ActivityId { get; set; }

        /// <summary>
        /// Gets or sets the offer identifier.
        /// </summary>
        /// <value>
        /// The offer identifier.
        /// </value>
        [JsonProperty(PropertyName = "offerId")]
        public string OfferId { get; set; }

        /// <summary>
        /// Gets or sets the operation identifier.
        /// </summary>
        /// <value>
        /// Operation Id is presented as Id property on the JSON payload
        /// </value>
        [JsonProperty(PropertyName = "Id")] public Guid OperationId { get; set; }

        /// <summary>
        /// Gets or sets the plan identifier.
        /// </summary>
        /// <value>
        /// The plan identifier.
        /// </value>
        public string PlanId { get; set; }

        /// <summary>
        /// Gets or sets the publisher identifier.
        /// </summary>
        /// <value>
        /// The publisher identifier.
        /// </value>
        public string PublisherId { get; set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        /// <value>
        /// The quantity.
        /// </value>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        [JsonProperty(PropertyName = "status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public OperationStatusEnum Status { get; set; }

        /// <summary>
        /// Gets or sets the subscription identifier.
        /// </summary>
        /// <value>
        /// The subscription identifier.
        /// </value>
        [JsonProperty(PropertyName = "subscriptionId")]
        public Guid SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the time stamp.
        /// </summary>
        /// <value>
        /// The time stamp.
        /// </value>
        [JsonProperty(PropertyName = "timeStamp")]
        public DateTimeOffset TimeStamp { get; set; }
    }
}
