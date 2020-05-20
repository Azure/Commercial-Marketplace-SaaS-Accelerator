// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.
namespace Microsoft.Marketplace.SaasKit.Models
{
    using System;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Beneficiary Result.
    /// </summary>
    public class BeneficiaryResult
    {
        /// <summary>
        /// Gets or sets the tenant identifier.
        /// </summary>
        /// <value>
        /// The tenant identifier.
        /// </value>
        [JsonPropertyName("tenantId")]
        public Guid TenantId { get; set; }
    }
}