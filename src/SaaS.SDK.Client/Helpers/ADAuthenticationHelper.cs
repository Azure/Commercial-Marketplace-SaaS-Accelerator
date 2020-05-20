// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.
namespace Microsoft.Marketplace.SaasKit.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Marketplace.SaaS.SDK.Client.Models;
    using Microsoft.Marketplace.SaaS.SDK.Client.Network;
    using Microsoft.Marketplace.SaasKit.Configurations;
    using Microsoft.Marketplace.SaasKit.Network;

    /// <summary>
    /// Helper class to complete authentication against Azure Active Directory and get access token detail.
    /// </summary>
    public static class ADAuthenticationHelper
    {
        /// <summary>
        /// Gets the access token.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns>Get Authentication Token.</returns>
        public static async Task<ADAuthenticationResult> GetAccessToken(SaaSApiClientConfiguration settings)
        {
            string authorizeUrl = string.Format($"https://login.microsoftonline.com/{settings.TenantId}/oauth2/token");
            var webRequestHelper = new WebRequestHelper(authorizeUrl, HttpMethods.POST, "application/x-www-form-urlencoded");

            var payload = new Dictionary<string, object>();
            payload.Add("Grant_type", "client_credentials");
            payload.Add("Client_id", settings.ClientId);
            payload.Add("Client_secret", settings.ClientSecret);
            payload.Add("Resource", settings.Resource);

            await webRequestHelper.PrepareDataForRequest(payload).DoRequestAsync().ConfigureAwait(false);
            return await webRequestHelper.BuildResultFromResponse<ADAuthenticationResult>();
        }
    }
}