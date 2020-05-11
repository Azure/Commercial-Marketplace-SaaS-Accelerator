namespace Microsoft.Marketplace.SaaS.SDK.Services.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// Credentials Model.
    /// </summary>
    public class CredentialsModel
    {
        /// <summary>
        /// Gets or sets the tenant identifier.
        /// </summary>
        /// <value>
        /// The tenant identifier.
        /// </value>
        [JsonProperty("Tenant ID")]
        public string TenantID { get; set; }

        /// <summary>
        /// Gets or sets the subscription identifier.
        /// </summary>
        /// <value>
        /// The subscription identifier.
        /// </value>
        [JsonProperty("Subscription ID")]
        public string SubscriptionID { get; set; }

        /// <summary>
        /// Gets or sets the service principal identifier.
        /// </summary>
        /// <value>
        /// The service principal identifier.
        /// </value>
        [JsonProperty("Service Principal ID")]
        public string ServicePrincipalID { get; set; }

        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        /// <value>
        /// The client secret.
        /// </value>
        [JsonProperty("Client Secret")]
        public string ClientSecret { get; set; }
    }
}
