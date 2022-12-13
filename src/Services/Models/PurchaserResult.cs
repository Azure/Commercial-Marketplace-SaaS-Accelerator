// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Text.Json.Serialization;

namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// Purchaser Result.
/// </summary>
public class PurchaserResult
{
    /// <summary>
    /// Gets or sets the tenant identifier.
    /// </summary>
    /// <value>
    /// The tenant identifier.
    /// </value>
    [JsonPropertyName("tenantId")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// Gets or sets the email identifier.
    /// </summary>
    /// <value>
    /// The email identifier.
    /// </value>
    [JsonPropertyName("emailId")]
    public string EmailId { get; set; }

    /// <summary>
    /// Gets or sets the object identifier.
    /// </summary>
    /// <value>
    /// The object identifier.
    /// </value>
    [JsonPropertyName("objectId")]
    public Guid ObjectId { get; set; }
}