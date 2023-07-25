// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// WebNotificationCustomInfo.
/// </summary>
public class WebNotificationCustomInfo
{
    /// <summary>
    /// Gets or sets the usage event identifier.
    /// </summary>
    /// <value>
    /// The Application Name.
    /// </value>
    [JsonPropertyName("applicationName")]
    public string ApplicationName { get; set; }

    /// <summary>
    /// Gets or sets the eventtype.
    /// </summary>
    /// <value>
    /// The status.
    /// </value>
    [JsonPropertyName("eventType")]
    public string EventType { get; set; }

    /// <summary>
    /// Gets or sets the eventtype.
    /// </summary>
    /// <value>
    /// The status.
    /// </value>
    [JsonPropertyName("landingpageSubscriptionParams")]
    public List<KeyValuePair<string, string>> LandingPageCustomFields { get; set; }

}