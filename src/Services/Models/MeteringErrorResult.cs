// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>Metering API Exception Response.</summary>
/// <seealso cref="Microsoft.Marketplace.SaasKit.Models.SaaSApiResult"/>
public class MeteringErrorResult : SaaSApiResult
{
    /// <summary>Gets or sets the error message.</summary>
    /// <value>The error message.</value>
    [JsonPropertyName("message")]
    public string ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the additional information.
    /// </summary>
    /// <value>
    /// The additional information.
    /// </value>
    [JsonPropertyName("additionalInfo")]
    public AdditionalInfo AdditionalInfo { get; set; }

    /// <summary>
    /// Gets or sets the error detail.
    /// </summary>
    /// <value>
    /// The error detail.
    /// </value>
    [JsonPropertyName("details")]
    public List<ErrorDetail> ErrorDetail { get; set; }

    /// <summary>Gets or sets the error code.</summary>
    /// <value>The error code.</value>
    [JsonPropertyName("code")]
    public string ErrorCode { get; set; }
}