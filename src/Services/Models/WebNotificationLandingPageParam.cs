// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// WebNotificationLandingPageParam.
/// </summary>
public class WebNotificationLandingPageParam
{
    public WebNotificationLandingPageParam(string key, string value)
    {
        this.Key = key;
        Value = value;
    }

    /// <summary>
    /// Gets or sets the Key.
    /// </summary>
    /// <value>
    /// The EventType.
    /// </value>
    [JsonPropertyName("key")]
    public string Key { get; set; }

    /// <summary>
    /// Gets or sets the Value.
    /// </summary>
    /// <value>
    /// The EventType.
    /// </value>
    [JsonPropertyName("value")]
    public string Value { get; set; }

}