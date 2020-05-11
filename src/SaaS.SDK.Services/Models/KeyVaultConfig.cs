namespace Microsoft.Marketplace.SaaS.SDK.Services.Models
{
    /// <summary>
    /// Key Vault Config.
    /// </summary>
    public class KeyVaultConfig
    {
        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        public string ClientID { get; set; }

        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        /// <value>
        /// The client secret.
        /// </value>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the tenant identifier.
        /// </summary>
        /// <value>
        /// The tenant identifier.
        /// </value>
        public string TenantID { get; set; }

        /// <summary>
        /// Gets or sets the key vault URL.
        /// </summary>
        /// <value>
        /// The key vault URL.
        /// </value>
        public string KeyVaultUrl { get; set; }
    }
}
