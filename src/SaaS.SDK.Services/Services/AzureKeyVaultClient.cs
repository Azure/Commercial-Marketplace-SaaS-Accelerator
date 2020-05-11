namespace Microsoft.Marketplace.SaaS.SDK.Services.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Azure.KeyVault;
    using Microsoft.Azure.KeyVault.Models;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Microsoft.Marketplace.SaaS.SDK.Services.Contracts;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;

    /// <summary>
    /// Implementation of vault service to store and get data from Azure keyvault.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaaS.SDK.Services.Contracts.IVaultService" />
    public class AzureKeyVaultClient : IVaultService
    {
        /// <summary>
        /// The content type.
        /// </summary>
        protected const string ContentTypeContant = "AMP-SaaS";

        /// <summary>
        /// The key vault configuration.
        /// </summary>
        private KeyVaultConfig keyVaultConfig;

        /// <summary>
        /// The logger.
        /// </summary>
        private ILogger<AzureKeyVaultClient> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureKeyVaultClient" /> class.
        /// </summary>
        /// <param name="keyVaultConfig">The key vault configuration.</param>
        /// <param name="logger">The logger.</param>
        public AzureKeyVaultClient(
                                    KeyVaultConfig keyVaultConfig,
                                    ILogger<AzureKeyVaultClient> logger)
        {
            this.keyVaultConfig = keyVaultConfig;
            this.logger = logger;
        }

        /// <summary>
        /// Writes the key asynchronous.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="val">The value.</param>
        /// <returns>string. </returns>
        public async Task<string> WriteKeyAsync(string key, string val)
        {
            SecretAttributes attribs = new SecretAttributes
            {
                Enabled = true,
            };

            IDictionary<string, string> tags = new Dictionary<string, string>();
            tags.Add("subscriptionId", key);

            string name = key;
            string value = val; // Json
            string contentType = ContentTypeContant;
            try
            {
                var client = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(this.GetAccessToken));
                SecretBundle bundle = await client.SetSecretAsync(this.keyVaultConfig.KeyVaultUrl, name, value, tags, contentType, attribs);
                return bundle.SecretIdentifier.ToString();
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "Error while writing to key vault");
                return ex.Message;
            }
        }

        /// <summary>
        /// Gets the key asynchronous.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns> string.</returns>
        public async Task<string> GetKeyAsync(string key)
        {
            var client = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(this.GetAccessToken));
            SecretBundle secret = await client.GetSecretAsync(key);
            return secret.Value;
        }

        /// <summary>
        /// Validates the user parameters.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns> bool.</returns>
        public bool ValidateUserParameters(IDictionary<string, string> dictionary)
        {
            string authority = string.Empty;
            string resource = string.Empty;
            string clientId = dictionary["Service Principal ID"];
            string clientSecret = dictionary["Client Secret"];
            string tenantId = dictionary["Tenant ID"];
            string subscriptionId = dictionary["Subscription ID"];

            authority = string.Format("https://login.windows.net/{0}", tenantId);
            resource = "https://vault.azure.net";
            KeyVaultClient kvc = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(this.GetAccessToken));
            var authContext = new AuthenticationContext(authority);
            ClientCredential clientCred = new ClientCredential(clientId, clientSecret);

            try
            {
                AuthenticationResult result = authContext.AcquireTokenAsync(resource, clientCred).ConfigureAwait(false).GetAwaiter().GetResult();

                if (result == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "Error while validating deployment parameters");
                return false;
            }
        }

        /// <summary>
        /// Gets the access token.
        /// </summary>
        /// <param name="authority">The authority.</param>
        /// <param name="resource">The resource.</param>
        /// <param name="scope">The scope.</param>
        /// <returns> string.</returns>
        protected async Task<string> GetAccessToken(string authority, string resource, string scope)
        {
            authority = string.Format("https://login.windows.net/{0}", this.keyVaultConfig.TenantID);
            resource = "https://vault.azure.net";
            ClientCredential clientCredential = new ClientCredential(this.keyVaultConfig.ClientID, this.keyVaultConfig.ClientSecret);
            var context = new AuthenticationContext(authority, TokenCache.DefaultShared);
            var result = await context.AcquireTokenAsync(resource, clientCredential).ConfigureAwait(false);
            return result.AccessToken;
        }
    }
}
