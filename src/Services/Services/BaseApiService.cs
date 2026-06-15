// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Net;
using System.Text.Json;
using Azure;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.Exceptions;
using Marketplace.SaaS.Accelerator.Services.Models;

namespace Marketplace.SaaS.Accelerator.Services.Services;

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

    /// <summary>
    /// Process errors when calling the client library.
    /// </summary>
    /// <param name="marketplaceAction">The MarketplaceActionEnum.</param>
    /// <param name="ex">The exception from the client library.</param>
    // Status codes that represent transient Marketplace API failures.
    // These are re-thrown as RequestFailedException so that an outer Polly
    // retry/circuit-breaker policy (e.g. FulfillmentApiServiceWithPolicy) can
    // intercept them. Non-transient codes are converted to MarketplaceException
    // as before so existing callers still see a consistent exception type.
    private static readonly System.Collections.Generic.HashSet<int> TransientStatusCodes =
        new System.Collections.Generic.HashSet<int> { 408, 429, 500, 502, 503, 504 };

    public void ProcessErrorResponse(MarketplaceActionEnum marketplaceAction, Exception ex)
    {
        int statusCode = 0;
        if (ex.InnerException != null && ex.InnerException is Microsoft.Identity.Client.MsalServiceException msalInnerException)
        {
            statusCode = msalInnerException.StatusCode;
        }
        else if (ex is RequestFailedException reqFailedInnerException)
        {
            statusCode = reqFailedInnerException.Status;
        }

        // Re-throw RequestFailedException for transient status codes so that
        // an outer Polly policy can retry them. This is the seam that allows
        // FulfillmentApiServiceWithPolicy to intercept 408/429/5xx errors.
        if (ex is RequestFailedException rfe && TransientStatusCodes.Contains(rfe.Status))
        {
            throw rfe;
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