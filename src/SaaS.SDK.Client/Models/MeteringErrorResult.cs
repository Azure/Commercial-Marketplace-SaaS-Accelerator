namespace Microsoft.Marketplace.SaasKit.Models
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>Metering API Exception Response.</summary>
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
}
