// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.
namespace Microsoft.Marketplace.SaaS.SDK.Services.Models
{
    using System.ComponentModel;
    using System.Text.Json.Serialization;
    using System.Collections.Generic;
    /// <summary>
    /// Meteringed Quantity Included.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Models.SaaSApiResult" />
    public class MeteringedQuantityIncluded
    {
        [JsonPropertyName("dimensionId")]
        [DisplayName("dimensionId")]
        /// <summary>
        /// Gets or Sets DimensionId
        /// </summary>
        /// <value>
        /// DimensionId
        /// </value>
        public string DimensionId { get; set; }

        [JsonPropertyName("units")]
        [DisplayName("units")]
        /// <summary>
        /// Gets or Sets Unit value
        /// </summary>
        /// <value>
        /// Unit 
        /// </value>
        public string Units { get; set; }
    }
}
