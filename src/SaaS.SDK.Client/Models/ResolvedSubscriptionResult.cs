namespace Microsoft.Marketplace.SaasKit.Models
{
    using System;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Resolved Subscription Response.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Models.SaaSApiResult" />
    public class ResolvedSubscriptionResult : SaaSApiResult
    {
        /// <summary>
        /// Gets or sets the offer identifier.
        /// </summary>
        /// <value>
        /// The offer identifier.
        /// </value>
        public string OfferId { get; set; }

        /// <summary>
        /// Gets or sets the operation identifier.
        /// </summary>
        /// <value>
        /// The operation identifier.
        /// </value>
        public Guid OperationId { get; set; }

        /// <summary>
        /// Gets or sets the plan identifier.
        /// </summary>
        /// <value>
        /// The plan identifier.
        /// </value>
        public string PlanId { get; set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        /// <value>
        /// The quantity.
        /// </value>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the subscription identifier.
        /// </summary>
        /// <value>
        /// The subscription identifier.
        /// </value>
        [JsonPropertyName("id")]
        public Guid SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the name of the subscription.
        /// </summary>
        /// <value>
        /// The name of the subscription.
        /// </value>
        public string SubscriptionName { get; set; }
    }
}
