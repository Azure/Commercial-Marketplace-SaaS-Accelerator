// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Text.Json.Serialization;

namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// Accepted Message.
/// </summary>
public class AcceptedMessage
{
    /// <summary>
    /// Gets or sets the usage event identifier.
    /// </summary>
    /// <value>
    /// The usage event identifier.
    /// </value>
    [JsonPropertyName("usageEventId")]
    public string UsageEventId { get; set; }

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    /// <value>
    /// The status.
    /// </value>
    [JsonPropertyName("status")]
    public string Status { get; set; }

    /// <summary>
    /// Gets or sets the message time.
    /// </summary>
    /// <value>
    /// The message time.
    /// </value>
    [JsonPropertyName("messageTime")]
    public string MessageTime { get; set; }

    /// <summary>
    /// Gets or sets the resource identifier.
    /// </summary>
    /// <value>
    /// The resource identifier.
    /// </value>
    [JsonPropertyName("resourceId")]
    public Guid ResourceId { get; set; }

    /// <summary>
    /// Gets or sets the quantity.
    /// </summary>
    /// <value>
    /// The quantity.
    /// </value>
    [JsonPropertyName("quantity")]
    public string Quantity { get; set; }

    /// <summary>
    /// Gets or sets the dimension.
    /// </summary>
    /// <value>
    /// The dimension.
    /// </value>
    [JsonPropertyName("dimension")]
    public string Dimension { get; set; }

    /// <summary>
    /// Gets or sets the effective start time.
    /// </summary>
    /// <value>
    /// The effective start time.
    /// </value>
    [JsonPropertyName("effectiveStartTime")]
    public string EffectiveStartTime { get; set; }

    /// <summary>
    /// Gets or sets the plan identifier.
    /// </summary>
    /// <value>
    /// The plan identifier.
    /// </value>
    [JsonPropertyName("planId")]
    public string PlanId { get; set; }
}