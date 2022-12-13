// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// Metering Dimension Details.
/// </summary>
/// <seealso cref="Microsoft.Marketplace.SaasKit.Models.SaaSApiResult" />
public class MeteringDimension
{
    [JsonPropertyName("id")]
    [DisplayName("id")]
    /// <summary>
    /// Gets or Sets Id
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    public string Id { get; set; }

    [JsonPropertyName("currency")]
    [DisplayName("currency")]
    /// <summary>
    /// Gets or Sets Currency
    /// </summary>
    /// <value>
    /// Currency value
    /// </value>
    public string Currency { get; set; }

    [JsonPropertyName("pricePerUnit")]
    [DisplayName("pricePerUnit")]
    /// <summary>
    /// Gets or Sets PricePerUnit
    /// </summary>
    /// <value>
    /// price per unit
    /// </value>
    public float? PricePerUnit { get; set; }

    [JsonPropertyName("unitOfMeasure")]
    [DisplayName("unitOfMeasure")]
    /// <summary>
    /// Gets or Sets UnitOfMeasure
    /// </summary>
    /// <value>
    /// unit of measure for meteric
    /// </value>
    public string UnitOfMeasure { get; set; }

    [JsonPropertyName("displayName")]
    [DisplayName("displayName")]
    /// <summary>
    /// Gets or Sets DisplayName
    /// </summary>
    /// <value>
    /// Display name
    /// </value>
    public string DisplayName { get; set; }
}