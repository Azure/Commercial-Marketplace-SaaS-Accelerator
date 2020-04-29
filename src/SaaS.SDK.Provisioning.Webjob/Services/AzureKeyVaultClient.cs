using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using SaaS.SDK.Provisioning.Webjob.Contracts;
using SaaS.SDK.Provisioning.Webjob.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Marketplace.SaasKit.Provisioning.Webjob.Helpers
{
    public class AzureKeyVaultClient : IAzureKeyVaultClient
    {        
        protected const string CONTENT_TYPE = "AMP-SaaS";        
        private KeyVaultClient client = null;
        private KeyVaultConfig keyVaultConfig = null;

        public AzureKeyVaultClient(KeyVaultConfig keyVaultConfig)
        {
            this.keyVaultConfig = keyVaultConfig;
        }

        public async Task<string> WriteKeyAsync(string key, string val)
        {
            SecretAttributes attribs = new SecretAttributes
            {
                Enabled = true
            };

            IDictionary<string, string> tags = new Dictionary<string, string>();
            tags.Add("subscriptionId", key);

            string Name = key;
            string Value = val; // Json
            string contentType = CONTENT_TYPE;
            try
            {
                client = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessToken));
                SecretBundle bundle = await client.SetSecretAsync(keyVaultConfig.KeyVaultUrl, Name, Value, tags, contentType, attribs);
                return bundle.SecretIdentifier.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public async Task<string> GetKeyAsync(string key)
        {
            client = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessToken));
            SecretBundle secret = await client.GetSecretAsync(key);
            return secret.Value;
        }

        protected async Task<string> GetAccessToken(string authority, string resource, string scope)
        {
            authority = string.Format("https://login.windows.net/{0}", keyVaultConfig.TenantID);
            resource = "https://vault.azure.net";
            ClientCredential clientCredential = new ClientCredential(keyVaultConfig.ClientID, keyVaultConfig.ClientSecret);
            var context = new AuthenticationContext(authority, TokenCache.DefaultShared);
            var result = await context.AcquireTokenAsync(resource, clientCredential).ConfigureAwait(false);
            return result.AccessToken;
        }

        public bool ValidateUserParameters(IDictionary<string, string> dictionary)
        {
            string authority = "";
            string resource = "";
            string clientId = dictionary["Service Principal ID"];
            string clientSecret = dictionary["Client Secret"];
            string tenantId = dictionary["Tenant ID"];
            string subscriptionId = dictionary["Subscription ID"];

            authority = string.Format("https://login.windows.net/{0}", tenantId);
            resource = "https://vault.azure.net";
            KeyVaultClient kvc = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessToken));
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
                return false;
            }
        }
    }
}
