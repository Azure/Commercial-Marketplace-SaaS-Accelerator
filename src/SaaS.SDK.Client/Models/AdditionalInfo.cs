namespace Microsoft.Marketplace.SaasKit.Models
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Additional  Info.
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
}
