namespace Microsoft.Marketplace.SaasKit.Network
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Microsoft.Marketplace.SaasKit.Attributes;
    using Microsoft.Marketplace.SaasKit.Configurations;
    using Microsoft.Marketplace.SaasKit.Contracts;
    using Microsoft.Marketplace.SaasKit.Helpers;
    using Microsoft.Marketplace.SaasKit.Models;
    using System.Text.Json;

    /// <summary>
    /// rest client call implementation.
    /// </summary>
    /// <typeparam name="T">type of entity.</typeparam>
    public abstract class AbstractSaaSApiRestClient<T>
        where T : SaaSApiResult, new()
    {
        /// <summary>
        /// The SDK settings.
        /// </summary>
        protected readonly SaaSApiClientConfiguration clientConfiguration;

        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractSaaSApiRestClient{T}" /> class.
        /// </summary>
        /// <param name="clientConfiguration">The client configuration.</param>
        /// <param name="logger">The logger.</param>
        public AbstractSaaSApiRestClient(SaaSApiClientConfiguration clientConfiguration, ILogger logger)
        {
            this.clientConfiguration = clientConfiguration;
            this.logger = logger;
        }

        /// <summary>
        /// Does the request.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="method">The method.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <returns>
        /// Request result.
        /// </returns>
        /// <exception cref="FulfillmentException">Token expired. Please logout and login again.</exception>
        public async Task<T> DoRequest(string url, string method, Dictionary<string, object> parameters, Dictionary<string, object> headers = null, string contentType = "application/json")
        {
            try
            {
                this.logger?.Info("Call Rest Service : {0}" + JsonSerializer.Serialize(new { url = url, method = method, parameters = parameters, headers = headers, contentType = contentType }));
                var accessTokenResult = await ADAuthenticationHelper.GetAccessToken(this.clientConfiguration).ConfigureAwait(false);

                string formattedParams = string.Empty;
                if ((string.Equals(HttpMethods.GET.ToString(), method) || string.Equals(HttpMethods.DELETE.ToString(), method)) && parameters != null && parameters.Count() > 0)
                {
                    formattedParams = string.Join("&", parameters.Select(x => x.Key + "=" + System.Net.WebUtility.UrlEncode(x.Value.ToString())));
                    url = string.Format("{0}?{1}", url, formattedParams);
                }
                else if ((string.Equals(HttpMethods.POST.ToString(), method) || string.Equals(HttpMethods.PUT.ToString(), method) || string.Equals(HttpMethods.PATCH.ToString(), method)) && parameters != null && parameters.Count() > 0)
                {
                    if (parameters != null && parameters.Count() > 0)
                    {
                        if ("application/json".Equals(contentType))
                        {
                            formattedParams = JsonSerializer.Serialize(parameters);
                        }
                        else
                        {
                            formattedParams = string.Join("&", parameters.Select(x => x.Key + "=" + x.Value));
                        }
                    }
                }

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
                request.Method = method;
                request.Accept = "application/json";

                this.FillHeaders(headers, accessTokenResult, request);
                await this.DoRequest(method, parameters, contentType, formattedParams, request).ConfigureAwait(false);
                return await this.BuildResultFromResponse(request).ConfigureAwait(false);
            }
            catch (WebException ex)
            {
                return this.ProcessErrorResponse(url, ex);
            }
        }

        /// <summary>
        /// Processes the error response.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="ex">The ex.</param>
        /// <returns>Error result built using the data in the response.</returns>
        protected abstract T ProcessErrorResponse(string url, WebException ex);

        /// <summary>
        /// Builds the result from response.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>An instance of type T deserialized from the response.</returns>
        protected virtual async Task<T> BuildResultFromResponse(HttpWebRequest request)
        {
            try
            {
                WebResponse response = await request.GetResponseAsync().ConfigureAwait(false);
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string responseAsString = reader.ReadToEnd();

                    var result = new T();
                    if (!string.IsNullOrWhiteSpace(responseAsString))
                    {
                        result = JsonSerializer.Deserialize<T>(responseAsString);

                        if (result == null)
                        {
                            result = new T();
                        }
                    }

                    // Fill headers.
                    var t = typeof(T);
                    var properties = t.GetProperties();
                    var responseHeaders = response.Headers;
                    foreach (var prop in properties)
                    {
                        var fromHeaderAttribute = prop.GetCustomAttributes(typeof(FromRequestHeaderAttribute), false).FirstOrDefault() as FromRequestHeaderAttribute;
                        if (fromHeaderAttribute != null)
                        {
                            var valFromData = responseHeaders[fromHeaderAttribute?.HeaderKey?.ToString()];
                            if (valFromData != null)
                            {
                                // MP : Null value check added
                                if (result != null && !string.IsNullOrEmpty(valFromData))
                                {
                                    prop.SetValue(result, valFromData, null);
                                }
                            }
                        }
                    }

                    this.logger?.Info("Response : " + responseAsString);
                    return result;
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// Does the request.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="formattedParams">The formatted parameters.</param>
        /// <param name="request">The request.</param>
        /// <returns> Request result.</returns>
        protected virtual async Task DoRequest(string method, Dictionary<string, object> parameters, string contentType, string formattedParams, HttpWebRequest request)
        {
            if (string.Equals(HttpMethods.POST.ToString(), method) || string.Equals(HttpMethods.PUT.ToString(), method) || string.Equals(HttpMethods.PATCH.ToString(), method))
            {
                if (parameters != null && parameters.Count > 0)
                {
                    request.ContentType = contentType;
                    using (Stream stream = await request.GetRequestStreamAsync().ConfigureAwait(false))
                    {
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            writer.Write(formattedParams);
                            writer.Flush();
                        }

                        stream.Close();
                    }
                }
                else
                {
                    request.ContentLength = 1;

                    using (Stream stream = await request.GetRequestStreamAsync().ConfigureAwait(false))
                    {
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            writer.Write(" ");
                            writer.Flush();
                        }

                        stream.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Fills the headers.
        /// </summary>
        /// <param name="headers">The headers.</param>
        /// <param name="accessTokenResult">The access token result.</param>
        /// <param name="request">The request.</param>
        protected virtual void FillHeaders(Dictionary<string, object> headers, AuthenticationResult accessTokenResult, HttpWebRequest request)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            if (headers == null)
            {
                headers = new Dictionary<string, object>();
            }

            if (headers != null)
            {
                // Add bearer token
                headers.Add("Authorization", string.Format($"Bearer {accessTokenResult.AccessToken}"));

                foreach (KeyValuePair<string, object> kvp in headers)
                {
                    request.Headers[kvp.Key] = kvp.Value.ToString();
                }
            }
        }
    }
}