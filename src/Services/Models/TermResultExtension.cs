// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Text.Json.Serialization;

namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// Get TermResult used for Webhook.
/// </summary>
public class TermResultExtension : TermResult
{
    /// <summary>
    /// Gets or sets the end date.
    /// </summary>
    /// <value>
    /// The end date.
    /// </value>
    [JsonPropertyName("chargeDuration")]
    public string ChargeDuration { get; set; }
}