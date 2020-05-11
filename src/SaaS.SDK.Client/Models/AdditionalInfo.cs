namespace Microsoft.Marketplace.SaasKit.Models
{
    using Newtonsoft.Json;

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
        [JsonProperty("acceptedMessage")]
        public AcceptedMessage AcceptedMessage { get; set; }
    }
}
