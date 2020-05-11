namespace Microsoft.Marketplace.SaasKit.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// Error handle FulfillmentError.
    /// </summary>
    public class FulfillmentErrorResult
    {
        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        [JsonProperty("error")]
        public ErrorResult Error { get; set; }
    }
}
