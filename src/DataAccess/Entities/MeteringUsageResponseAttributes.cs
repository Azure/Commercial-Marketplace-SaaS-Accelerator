// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Text.Json.Serialization;

namespace Marketplace.SaaS.Accelerator.DataAccess.Entities;

public partial class MeteringUsageResponseAttributes
{
    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("effectiveStartTime")]
    public DateTime? UsagePostedDate { get; set; }

    [JsonPropertyName("usageEventId")]
    public Guid UsageEventId { get; set; }

    [JsonPropertyName("messageTime")]
    public DateTime MessageTime { get; set; }

    [JsonPropertyName("resourceId")]
    public Guid ResourceId { get; set; }

    [JsonPropertyName("quantity")]
    public double Quantity { get; set; }

    [JsonPropertyName("dimension")]
    public string Dimension { get; set; }

    [JsonPropertyName("planId")]
    public string PlanId { get; set; }
}