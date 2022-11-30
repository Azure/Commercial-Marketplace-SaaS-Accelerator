// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// Subscription Batch Usage Result.
/// </summary>
/// <seealso cref="Microsoft.Marketplace.SaasKit.Models.SaaSApiResult" />
public class MeteringBatchUsageResult : SaaSApiResult
{
    /// <summary>
    /// Gets or sets the count.
    /// </summary>
    /// <value>
    /// The count.
    /// </value>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>
    /// Gets or sets the count.
    /// </summary>
    /// <value>
    /// The result.
    /// </value>
    [JsonPropertyName("result")]
    public IEnumerable<ResultBatchUsageResult> Result { get; set; }
}