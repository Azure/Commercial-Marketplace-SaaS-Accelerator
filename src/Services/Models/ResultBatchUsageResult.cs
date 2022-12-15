// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System.Text.Json.Serialization;

namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// BatchUsage Result.
/// </summary>
/// <seealso cref="Microsoft.Marketplace.SaasKit.Models.MeteringUsageResult" />
public class ResultBatchUsageResult : MeteringUsageResult
{
    /// <summary>
    /// Gets or sets the error.
    /// </summary>
    /// <value>
    /// The Error.
    /// </value>
    [JsonPropertyName("error")]
    public object Error { get; set; }
}