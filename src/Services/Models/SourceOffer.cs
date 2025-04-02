// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// Private offers Id.
/// </summary>
/// <seealso cref="Microsoft.Marketplace.SaasKit.Models.SaaSApiResult" />
public class SourceOffer
{

    [JsonPropertyName("externalId")]
    [DisplayName("externalId")]
    /// <summary>
    /// Gets or Sets externalId
    /// </summary>
    public Guid? externalId { get; set; }
        
}