namespace Microsoft.Marketplace.SaasKit.Models
{
    using Microsoft.Marketplace.SaasKit.Models;
    using System.Text.Json.Serialization;
    using System;
    using System.Collections.Generic;

    /// <summary>Metering API Exception Response</summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Models.SaaSApiResult"/>
    public class MeteringErrorResult : SaaSApiResult
    {
        /// <summary>Gets or sets the error message.</summary>
        /// <value>The error message.</value>
        [JsonPropertyName("message")]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the additional information.
        /// </summary>
        /// <value>
        /// The additional information.
        /// </value>
        [JsonPropertyName("additionalInfo")]
        public AdditionalInfo AdditionalInfo { get; set; }

        /// <summary>
        /// Gets or sets the error detail.
        /// </summary>
        /// <value>
        /// The error detail.
        /// </value>
        [JsonPropertyName("details")]
        public List<ErrorDetail> ErrorDetail { get; set; }
        
        /// <summary>Gets or sets the error code.</summary>
        /// <value>The error code.</value>
        [JsonPropertyName("code")]
        public string ErrorCode { get; set; }
    }

    /// <summary>
    /// Additional  Info
    /// </summary>
    public class AdditionalInfo
    {
        /// <summary>
        /// Gets or sets the accepted message.
        /// </summary>
        /// <value>
        /// The accepted message.
        /// </value>
        [JsonPropertyName("acceptedMessage")]
        public AcceptedMessage AcceptedMessage { get; set; }
    }

    /// <summary>
    /// Accepted Message
    /// </summary>
    public class AcceptedMessage
    {
        /// <summary>
        /// Gets or sets the usage event identifier.
        /// </summary>
        /// <value>
        /// The usage event identifier.
        /// </value>
        [JsonPropertyName("usageEventId")]
        public string UsageEventId { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        [JsonPropertyName("status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the message time.
        /// </summary>
        /// <value>
        /// The message time.
        /// </value>
        [JsonPropertyName("messageTime")]
        public string MessageTime { get; set; }

        /// <summary>
        /// Gets or sets the resource identifier.
        /// </summary>
        /// <value>
        /// The resource identifier.
        /// </value>
        [JsonPropertyName("resourceId")]
        public Guid ResourceId { get; set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        /// <value>
        /// The quantity.
        /// </value>
        [JsonPropertyName("quantity")]
        public string Quantity { get; set; }

        /// <summary>
        /// Gets or sets the dimension.
        /// </summary>
        /// <value>
        /// The dimension.
        /// </value>
        [JsonPropertyName("dimension")]
        public string Dimension { get; set; }

        /// <summary>
        /// Gets or sets the effective start time.
        /// </summary>
        /// <value>
        /// The effective start time.
        /// </value>
        [JsonPropertyName("effectiveStartTime")]
        public string EffectiveStartTime { get; set; }

        /// <summary>
        /// Gets or sets the plan identifier.
        /// </summary>
        /// <value>
        /// The plan identifier.
        /// </value>
        [JsonPropertyName("planId")]
        public string PlanId { get; set; }
    }

    /// <summary>
    /// Error Detail
    /// </summary>
    public class ErrorDetail
    {
        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        [JsonPropertyName("message")]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the target.
        /// </summary>
        /// <value>
        /// The target.
        /// </value>
        [JsonPropertyName("target")]
        public string Target { get; set; }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        [JsonPropertyName("code")]
        public string ErrorCode { get; set; }
    }
}
