// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System.Text.Json.Serialization;

namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// Additional  Info.
/// </summary>
public class AdditionalInfo
{
    /// <summary>
    /// Gets or sets the accepted message.
    /// </summary>
    /// <value>
    /// The accepted message.
    /// </value>
    [JsonPropertyName("acceptedMessage")]
    public AcceptedMessage AcceptedMessage { get; set; }
}