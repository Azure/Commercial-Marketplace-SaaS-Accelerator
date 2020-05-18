﻿namespace Microsoft.Marketplace.SaasKit.Configurations
{
    /// <summary>
    /// Fulfillment Client Configuration.
    /// </summary>
    public class SaaSApiClientConfiguration
    {
        /// <summary>
        /// Gets or sets the type of the grant.
        /// </summary>
        /// <value>
        /// The type of the grant.
        /// </value>
        public string GrantType { get; set; }

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        /// <value>
        /// The client secret.
        /// </value>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the resource.
        /// </summary>
        /// <value>
        /// The resource.
        /// </value>
        public string Resource { get; set; }

        /// <summary>
        /// Gets or sets the base URL.
        /// </summary>
        /// <value>
        /// The base URL.
        /// </value>
        public string FulFillmentAPIBaseURL { get; set; }

        /// <summary>
        /// Gets or sets the signed out redirect URI.
        /// </summary>
        /// <value>
        /// The signed out redirect URI.
        /// </value>
        public string SignedOutRedirectUri { get; set; }

        /// <summary>
        /// Gets or sets the tenant identifier.
        /// </summary>
        /// <value>
        /// The tenant identifier.
        /// </value>
        public string TenantId { get; set; }

        /// <summary>
        /// Gets or sets the API version.
        /// </summary>
        /// <value>
        /// The API version.
        /// </value>
        public string FulFillmentAPIVersion { get; set; }

        /// <summary>
        /// Gets or sets the Authentication end point.
        /// </summary>
        /// <value>
        /// The Authentication end point.
        /// </value>
        public string AdAuthenticationEndPoint { get; set; }

        /// <summary>
        /// Gets or sets the saa s application URL.
        /// </summary>
        /// <value>
        /// The saas application URL.
        /// </value>
        public string SaaSAppUrl { get; set; }
    }
}