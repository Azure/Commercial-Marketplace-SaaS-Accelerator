// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Text.Json.Serialization;

namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// Get TermResult.
/// </summary>
public class TermResult
{
    /// <summary>
    /// Gets or sets the end date.
    /// </summary>
    /// <value>
    /// The end date.
    /// </value>
    [JsonPropertyName("endDate")]
    public DateTimeOffset EndDate { get; set; }

    /// <summary>
    /// Gets or sets the start date.
    /// </summary>
    /// <value>
    /// The start date.
    /// </value>
    [JsonPropertyName("startDate")]
    public DateTimeOffset StartDate { get; set; }

    /// <summary>
    /// Gets or sets the term unit.
    /// </summary>
    /// <value>
    /// The term unit.
    /// </value>
    [JsonPropertyName("termUnit")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TermUnitEnum TermUnit { get; set; }
}