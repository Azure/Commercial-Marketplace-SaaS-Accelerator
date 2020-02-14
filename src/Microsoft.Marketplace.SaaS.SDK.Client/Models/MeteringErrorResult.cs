namespace Microsoft.Marketplace.SaasKit.Models
{
    using Microsoft.Marketplace.SaasKit.Models;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    /// <summary>Metering API Exception Response</summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Models.SaaSApiResult"/>
    public class MeteringErrorResult : SaaSApiResult
    {
        /// <summary>Gets or sets the error message.</summary>
        /// <value>The error message.</value>
        [JsonProperty("message")]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the additional information.
        /// </summary>
        /// <value>
        /// The additional information.
        /// </value>
        [JsonProperty("additionalInfo")]
        public AdditionalInfo AdditionalInfo { get; set; }

        /// <summary>
        /// Gets or sets the error detail.
        /// </summary>
        /// <value>
        /// The error detail.
        /// </value>
        [JsonProperty("details")]
        public List<ErrorDetail> ErrorDetail { get; set; }
        
        /// <summary>Gets or sets the error code.</summary>
        /// <value>The error code.</value>
        [JsonProperty("code")]
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
        [JsonProperty("acceptedMessage")]
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
        [JsonProperty("message")]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the target.
        /// </summary>
        /// <value>
        /// The target.
        /// </value>
        [JsonProperty("target")]
        public string Target { get; set; }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        [JsonProperty("code")]
        public string ErrorCode { get; set; }
    }
}
