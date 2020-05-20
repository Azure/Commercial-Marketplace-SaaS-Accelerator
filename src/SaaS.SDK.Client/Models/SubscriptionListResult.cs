// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.
namespace Microsoft.Marketplace.SaasKit.Models
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text.Json.Serialization;

    /// <summary>
    /// the subscription list result.
    /// </summary>
    public class SubscriptionListResult : SaaSApiResult
    {
        /// <summary>
        /// Gets or sets the subscriptions result.
        /// </summary>
        /// <value>
        /// The subscriptions result.
        /// </value>
        [JsonPropertyName("Subscriptions")]
        [DisplayName("Subscriptions")]
        public List<SubscriptionResult> SubscriptionsResult { get; set; }

        /// <summary>
        /// Gets or sets indicates the URL for retrieving more subscriptions.
        /// </summary>
        /// <value>
        /// This value will be null when no further subscriptions can be retrieved.
        /// This value will contain a URL for subsequent queries when most subscriptions
        /// exist to be retrieved.
        /// </value>
        [JsonPropertyName("@nextLink")]
        [DisplayName("NextLink")]
        public string NextLink { get; set; }
    }
}
