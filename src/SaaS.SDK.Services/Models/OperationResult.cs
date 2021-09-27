// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.
namespace Microsoft.Marketplace.SaaS.SDK.Services.Models
{
    using System;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Subscription Operation Response.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Models.SaaSApiResult" />
    public class OperationResult : SaaSApiResult
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [JsonPropertyName("id")]
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        [JsonPropertyName("status")]
        public OperationStatusEnum Status { get; set; }

        /// <summary>
        /// Gets or sets the resource location.
        /// </summary>
        /// <value>
        /// The resource location.
        /// </value>
        [JsonPropertyName("resourceLocation")]
        public string ResourceLocation { get; set; }

        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>
        /// The created.
        /// </value>
        [JsonPropertyName("created")]
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the last modified.
        /// </summary>
        /// <value>
        /// The last modified.
        /// </value>
        [JsonPropertyName("lastModified")]
        public DateTime? LastModified { get; set; }
    }
}
