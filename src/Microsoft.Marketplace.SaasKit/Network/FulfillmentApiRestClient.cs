using Microsoft.Marketplace.SaasKit.Configurations;
using Microsoft.Marketplace.SaasKit.Contracts;
using Microsoft.Marketplace.SaasKit.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.Network
{
    public class FulfillmentApiRestClient<T> : AbstractSaaSApiRestClient<T> where T : SaaSApiResult, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FulfillmentApiRestClient{T}"/> class.
        /// </summary>
        /// <param name="clientConfiguration">The client configuration.</param>
        /// <param name="logger">The logger.</param>
        public FulfillmentApiRestClient(SaaSApiClientConfiguration clientConfiguration, ILogger logger) : base(clientConfiguration, logger)
        {
        }

        /// <summary>
        /// Processes the error response.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        /// <exception cref="FulfillmentException">
        /// Token expired. Please logout and login again.
        /// or NotFound
        /// or Conflict
        /// or BadRequest
        /// or Internal server error.
        /// </exception>
        protected override T ProcessErrorResponse(string url, WebException ex)
        {
            var httpResponse = ex.Response;
            if (httpResponse != null)
            {
                var webresponse = httpResponse as System.Net.HttpWebResponse;
                if (webresponse != null)
                {
                    this.logger?.Info("Error :: " + (new StreamReader(ex.Response.GetResponseStream())).ReadToEnd());
                    if (webresponse.StatusCode == HttpStatusCode.Unauthorized || webresponse.StatusCode == HttpStatusCode.Forbidden)
                    {
                        throw new FulfillmentException("Token expired. Please logout and login again.", SaasApiErrorCode.Unauthorized);
                    }

                    else if (webresponse.StatusCode == HttpStatusCode.NotFound)
                    {
                        this.logger?.Warn("Returning the error as " + JsonConvert.SerializeObject(new { Error = "Not Found" }));
                        throw new FulfillmentException(string.Format("Unable to find the request {0}", url), SaasApiErrorCode.NotFound);
                    }
                    else if (webresponse.StatusCode == HttpStatusCode.Conflict)
                    {
                        this.logger?.Warn("Returning the error as " + JsonConvert.SerializeObject(new { Error = "Conflict" }));
                        throw new FulfillmentException(string.Format("Conflict came for {0}", url), SaasApiErrorCode.Conflict);
                    }
                    else if (webresponse.StatusCode == HttpStatusCode.BadRequest)
                    {
                        this.logger?.Warn("Returning the error as " + JsonConvert.SerializeObject(new { Error = "Bad Request" }));
                        throw new FulfillmentException(string.Format("Unable to process the request {0}, server responding as BadRequest. Please verify the post data. ", url), SaasApiErrorCode.BadRequest);
                    }
                }
            }

            if (httpResponse != null && httpResponse.GetResponseStream() != null)
            {
                using (StreamReader reader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseAsString = reader.ReadToEnd();
                    var errorFromAPI = JsonConvert.DeserializeObject<FulfillmentErrorResult>(responseAsString);
                    
                    this.logger?.Warn("Returning the error as " + JsonConvert.SerializeObject(new { Error = responseAsString }));

                    throw new FulfillmentException(errorFromAPI.Error.Message, SaasApiErrorCode.InternalServerError);
                }
            }

            this.logger?.Error("Error while completing the request as " + JsonConvert.SerializeObject(new
            {
                Error = httpResponse
            }));

            return null;
        }
    }
}
