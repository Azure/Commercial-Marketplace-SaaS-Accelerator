namespace Microsoft.Marketplace.SaasKit.Models
{
    using System.Text.Json.Serialization;

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
        [JsonPropertyName("error")]
        public ErrorResult Error { get; set; }
    }
}
