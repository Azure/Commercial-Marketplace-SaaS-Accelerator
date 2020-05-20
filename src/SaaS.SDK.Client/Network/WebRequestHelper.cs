// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.
namespace Microsoft.Marketplace.SaaS.SDK.Client.Network
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.Marketplace.SaasKit.Attributes;
    using Microsoft.Marketplace.SaasKit.Network;

    /// <summary>
    /// Helper class to prepare and make web requests and build response from the request.
    /// </summary>
    public class WebRequestHelper
    {
        /// <summary>
        /// The URL.
        /// </summary>
        private string webURL;

        /// <summary>
        /// The method.
        /// </summary>
        private string method;

        /// <summary>
        /// The content type.
        /// </summary>
        private string contentType;

        /// <summary>
        /// The request.
        /// </summary>
        private HttpWebRequest request;

        /// <summary>
        /// The payload.
        /// </summary>
        private string payload;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebRequestHelper"/> class.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="method">The method.</param>
        /// <param name="contentType">Type of the content.</param>
        public WebRequestHelper(string url, string method, string contentType)
        {
            this.webURL = url;
            this.method = method;
            this.contentType = contentType;

            this.request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            this.request.Method = method;
            this.request.ContentType = this.contentType;
            this.request.Accept = "application/json";
        }

        /// <summary>
        /// Prepares the data for request.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Current instance.</returns>
        public WebRequestHelper PrepareDataForRequest(Dictionary<string, object> parameters)
        {
            if ((string.Equals(HttpMethods.GET.ToString(), this.method) || string.Equals(HttpMethods.DELETE.ToString(), this.method)) && parameters != null && parameters.Count() > 0)
            {
                this.payload = string.Join("&", parameters.Select(x => x.Key + "=" + System.Net.WebUtility.UrlEncode(x.Value.ToString())));
                this.webURL = string.Format("{0}?{1}", this.webURL, this.payload);
            }
            else if ((string.Equals(HttpMethods.POST.ToString(), this.method) || string.Equals(HttpMethods.PUT.ToString(), this.method) || string.Equals(HttpMethods.PATCH.ToString(), this.method)) && parameters != null && parameters.Count() > 0)
            {
                if (parameters != null && parameters.Count() > 0)
                {
                    if ("application/json".Equals(this.contentType))
                    {
                        this.payload = JsonSerializer.Serialize(parameters);
                    }
                    else
                    {
                        this.payload = string.Join("&", parameters.Select(x => x.Key + "=" + x.Value));
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Does the request.
        /// </summary>
        /// <returns>Current instance.</returns>
        public virtual async Task<WebRequestHelper> DoRequestAsync()
        {
            if (string.Equals(HttpMethods.POST.ToString(), this.method) || string.Equals(HttpMethods.PUT.ToString(), this.method) || string.Equals(HttpMethods.PATCH.ToString(), this.method))
            {
                if (!string.IsNullOrWhiteSpace(this.payload))
                {
                    using (Stream stream = await this.request.GetRequestStreamAsync().ConfigureAwait(false))
                    {
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            writer.Write(this.payload);
                            writer.Flush();
                        }

                        stream.Close();
                    }
                }
                else
                {
                    this.request.ContentLength = 1;

                    using (Stream stream = await this.request.GetRequestStreamAsync().ConfigureAwait(false))
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

            return this;
        }

        /// <summary>
        /// Fills the headers.
        /// </summary>
        /// <param name="headers">The headers.</param>
        /// <returns>Current instance.</returns>
        public virtual WebRequestHelper FillHeaders(Dictionary<string, object> headers)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            if (headers != null)
            {
                foreach (KeyValuePair<string, object> kvp in headers)
                {
                    this.request.Headers[kvp.Key] = kvp.Value.ToString();
                }
            }

            return this;
        }

        /// <summary>
        /// Builds the result from response.
        /// </summary>
        /// <typeparam name="T">T can be any class.</typeparam>
        /// <returns>
        /// An instance of type T deserialized from the response.
        /// </returns>
        public virtual async Task<T> BuildResultFromResponse<T>()
                                                                 where T : new()
        {
            WebResponse response = await this.request.GetResponseAsync().ConfigureAwait(false);
            var result = new T();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string responseAsString = reader.ReadToEnd();
                if (!string.IsNullOrWhiteSpace(responseAsString))
                {
                    result = JsonSerializer.Deserialize<T>(responseAsString);
                    if (result == null)
                    {
                        result = new T();
                    }
                }

                // Fill headers
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
                            if (result != null && !string.IsNullOrEmpty(valFromData))
                            {
                                prop.SetValue(result, valFromData, null);
                            }
                        }
                    }
                }

                return result;
            }
        }
    }
}