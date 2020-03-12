namespace Microsoft.Marketplace.SaasKit.Helpers
{
    using System.Threading.Tasks;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Microsoft.Marketplace.SaasKit.Configurations;

    /// <summary>
    /// Helper class to complete authentication against Azure Active Directory and get access token detail.
    /// </summary>
    public static class ADAuthenticationHelper
    {
        /// <summary>
        /// Gets the access token.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns>Get Authentication Token</returns>
        public static async Task<AuthenticationResult> GetAccessToken(SaaSApiClientConfiguration settings)
        {
            var credential = new ClientCredential(settings.ClientId, settings.ClientSecret);
            var authContext = new AuthenticationContext($"{settings.AdAuthenticationEndPoint}/{settings.TenantId}", false);
            var result = await authContext.AcquireTokenAsync(settings.Resource, credential).ConfigureAwait(false);
            return result;
        }
    }
}