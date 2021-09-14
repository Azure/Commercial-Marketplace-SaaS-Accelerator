// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.
namespace Microsoft.Marketplace.SaaS.SDK.Services.Services
{
    using System;
    using System.Net;
    using System.Text.Json;
    using global::Azure;
    using Microsoft.Marketplace.SaaS.SDK.Services.Contracts;
    using Microsoft.Marketplace.SaaS.SDK.Services.Exceptions;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;

    /// <summary>
    /// Base API Service.
    /// </summary>
    public class BaseApiService
    {
        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        protected ILogger Logger { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseApiService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public BaseApiService(ILogger logger)
        {   
            this.Logger = logger;
        }

        public void ProcessErrorResponse(MarketplaceActionEnum marketplaceAction, Exception ex)
        {
            int statusCode = 0;
            if (ex.InnerException != null && ex.InnerException is Identity.Client.MsalServiceException msalInnerException)
            {
                statusCode = msalInnerException.StatusCode;
            }
            else if (ex is RequestFailedException reqFailedInnerException)
            {
                statusCode = reqFailedInnerException.Status;
            }

            if (statusCode != 0)
            {
                Enum.TryParse<HttpStatusCode>(statusCode.ToString(), out HttpStatusCode httpStatusCode);

                this.Logger?.Error("Error while completing the request as " + JsonSerializer.Serialize(new { Error = ex.Message, }));

                if (httpStatusCode == HttpStatusCode.Unauthorized || httpStatusCode == HttpStatusCode.Forbidden)
                {
                    throw new MarketplaceException("Token invalid or expired. Please try again.", SaasApiErrorCode.Unauthorized);
                }
                else if (httpStatusCode == HttpStatusCode.NotFound)
                {
                    this.Logger?.Warn("Returning the error as " + JsonSerializer.Serialize(new { Error = "Not Found" }));
                    throw new MarketplaceException(string.Format("Unable to find the request {0}", marketplaceAction), SaasApiErrorCode.NotFound);
                }
                else if (httpStatusCode == HttpStatusCode.Conflict)
                {
                    this.Logger?.Warn("Returning the error as " + JsonSerializer.Serialize(new { Error = "Conflict" }));
                    throw new MarketplaceException(string.Format("Conflict came for {0}", marketplaceAction), SaasApiErrorCode.Conflict);
                }
                else if (httpStatusCode == HttpStatusCode.BadRequest)
                {
                    this.Logger?.Warn("Returning the error as " + JsonSerializer.Serialize(new { Error = "Bad Request" }));
                    throw new MarketplaceException(string.Format("Unable to process the request {0}, server responding as BadRequest. Please verify the post data. ", marketplaceAction), SaasApiErrorCode.BadRequest);
                }
                else
                {
                    this.Logger?.Warn("Returning the error as " + JsonSerializer.Serialize(new { Error = "Unknown Error" }));
                    throw new MarketplaceException(string.Format("Unable to process the request {0}, server responding as BadRequest. Please verify the post data. ", marketplaceAction), httpStatusCode.ToString());
                }
            }
            else
            {
                this.Logger?.Error("Error while completing the request as " + JsonSerializer.Serialize(new { Error = ex.Message, }));
                throw new MarketplaceException("Something went wrong, please check logs!");
            }
        }
    }
}