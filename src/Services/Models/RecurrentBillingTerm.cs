// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// Recurrent Billing Term Detail.
/// </summary>
/// <seealso cref="Microsoft.Marketplace.SaasKit.Models.SaaSApiResult" />
public class RecurrentBillingTerm
{
    [JsonPropertyName("currency")]
    [DisplayName("currency")]
    /// <summary>
    /// Gets or Sets Billing Term Currency
    /// </summary>
    /// <value>
    /// Currency value
    /// </value>
    public string Currency { get; set; }

    [JsonPropertyName("price")]
    [DisplayName("price")]
    /// <summary>
    /// Gets or Sets Billing Term Price
    /// </summary>
    /// <value>
    /// price value
    /// </value>
    public float? Price { get; set; }


    [JsonPropertyName("termDescription")]
    [DisplayName("termDescription")]
    /// <summary>
    /// Gets or Sets Billing Term Description
    /// </summary>
    /// <value>
    /// Billing Term Description
    /// </value>
    public string TermDescription { get; set; }

    /// <summary>
    /// Gets or Sets Term Unit
    /// </summary>
    /// <value>
    /// Term Unit
    /// </value>
    public string TermUnit { get; set; }

    [JsonPropertyName("meteredQuantityIncluded")]
    [DisplayName("meteredQuantityIncluded")]
    /// <summary>
    /// Gets or Sets Billing Term Metered Quantity
    /// </summary>
    /// <value>
    /// List of Metered Quantity
    /// </value>
    public List<MeteringedQuantityIncluded> MeteredQuantityIncluded { get; set; }
}