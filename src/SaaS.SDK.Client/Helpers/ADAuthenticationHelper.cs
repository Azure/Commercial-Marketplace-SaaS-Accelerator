﻿namespace Microsoft.Marketplace.SaasKit.Helpers
{
    using Microsoft.Marketplace.SaaS.SDK.Client.Models;
    using Microsoft.Marketplace.SaaS.SDK.Client.Network;
    using Microsoft.Marketplace.SaasKit.Configurations;
    using Microsoft.Marketplace.SaasKit.Network;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;

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
        public static async Task<ADAuthenticationResult> GetAccessToken(SaaSApiClientConfiguration settings)
        {
            //var credential = new ClientCredential(settings.ClientId, settings.ClientSecret);
            //var authContext = new AuthenticationContext($"{settings.AdAuthenticationEndPoint}/{settings.TenantId}", false);
            //var result = await authContext.AcquireTokenAsync(settings.Resource, credential).ConfigureAwait(false);
            //return result;
            string authorizeUrl = string.Format($"https://login.microsoftonline.com/{settings.TenantId}/oauth2/token");
            var webRequestHelper = new WebRequestHelper(authorizeUrl, HttpMethods.POST, "application/x-www-form-urlencoded");
            
            var payload = new Dictionary<string, Object>();
            payload.Add("Grant_type", "client_credentials");
            payload.Add("Client_id", settings.ClientId);
            payload.Add("Client_secret", settings.ClientSecret);
            payload.Add("Resource", settings.Resource);
            
            await webRequestHelper.PrepareDataForRequest(payload).DoRequestAsync().ConfigureAwait(false);
            return await webRequestHelper.BuildResultFromResponse<ADAuthenticationResult>();
        }
    }
}