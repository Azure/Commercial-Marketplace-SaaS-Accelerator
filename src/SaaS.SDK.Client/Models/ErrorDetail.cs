namespace Microsoft.Marketplace.SaasKit.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// Error Detail.
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
