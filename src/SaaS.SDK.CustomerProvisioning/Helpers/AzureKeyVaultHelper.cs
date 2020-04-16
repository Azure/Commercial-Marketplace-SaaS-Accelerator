using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Microsoft.Marketplace.SaaS.SDK.CustomerProvisioning.Helpers
{
    public class AzureKeyVaultHelper
    {
        const string CLIENTSECRET = "sXJn9bGcp5cmhZ@Ns:?Z77Jb?Zp[?x3.";
        const string CLIENTID = "28b1d793-eede-411a-a9fe-ba996808d4ea";
        // available from the Key Vault resource page
        const string BASESECRETURI = "https://amp-saas-keyvault-test.vault.azure.net/";
        // const string SECRETNAME = "AC57EDA4-EA49-41BD-4452-3AF5F3BBA081";
        const string CONTENTTYPE = "AMP-SaaS";
        static KeyVaultClient client = null;

        /*Creates the secret values and Tag values*/
        //public static string writeKeyVault();
        private static async Task<string> writeKeyVault(string subscriptionId, string deploymentParameters)
        {

            SecretAttributes attribs = new SecretAttributes
            {
                Enabled = true
                //,
                //Expires = DateTime.UtcNow.AddYears(2), // if you want to expire the info
                //NotBefore = DateTime.UtcNow.AddDays(1) // if you want the info to 
                // start being available later
            };

            IDictionary<string, string> tags = new Dictionary<string, string>();
            tags.Add("OfferId", "56E5E465-1657-45C0-BE9D-C1F011D0E77D");

            string Name = subscriptionId;
            string Value = deploymentParameters; // Json
            string contentType = CONTENTTYPE;
            try
            {
                client = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessToken));
                SecretBundle bundle = await client.SetSecretAsync(BASESECRETURI, Name, Value, tags, contentType, attribs);
                return bundle.SecretIdentifier.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public static void DoVault()
        {
            client = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessToken));
            //Secret Identifier URL: that can be found under 

            SecretBundle secret = Task.Run(() => client.GetSecretAsync("https://amp-saas-keyvault-test.vault.azure.net/secrets/Amp-testkey/262f295ab9b74d5cbaf8c4310b325a5b")).ConfigureAwait(false).GetAwaiter().GetResult();
            Console.WriteLine(secret.Tags["OfferId"].ToString());
            Console.WriteLine(secret.Tags["PlanId"].ToString());
            Console.WriteLine(secret.Tags["SubscriptionId"].ToString());
            Console.WriteLine(secret.ContentType);
            Console.WriteLine(secret.Value);
            Console.ReadLine();

        }
        public static async Task<string> GetAccessToken(string authority, string resource, string scope)
        {
            string clientId = "";
            string clientSecret = "";

            ClientCredential clientCredential = new ClientCredential(clientId, clientSecret);
            var context = new AuthenticationContext(authority, TokenCache.DefaultShared);
            var result = await context.AcquireTokenAsync(resource, clientCredential);
            return result.AccessToken;
        }

        public string ValidateUserParameters(string authority, string resource, string clientId, string clientSecret)
        {
            //string authority = "https://login.windows.net/6d7e0652-b03d-4ed2-bf86-f1999cecde17";
            //string resource = "https://vault.azure.net";
            var authContext = new AuthenticationContext(authority);
            ClientCredential clientCred = new ClientCredential(clientId, clientSecret);
            try
            {
                AuthenticationResult result = authContext.AcquireTokenAsync(resource, clientCred).ConfigureAwait(false).GetAwaiter().GetResult();

                if (result == null)
                {
                    return "Failed to obtain the JWT token";
                }
                //throw new InvalidOperationException("Failed to obtain the JWT token");
                else
                {
                    return result.AccessToken;
                }
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
        }
    }
}
