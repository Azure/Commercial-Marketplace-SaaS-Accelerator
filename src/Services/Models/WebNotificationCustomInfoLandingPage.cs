// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// WebNotificationCustomInfo.
/// </summary>
public class WebNotificationCustomInfoLandingPage : WebNotificationCustomInfo
{
    /// <summary>
    /// Gets or sets the LandingPageCustomFields.
    /// </summary>
    /// <value>
    /// The LandingPageCustomFields.
    /// </value>
    [JsonPropertyName("landingpageSubscriptionParams")]
    public List<WebNotificationLandingPageParam> LandingPageCustomFields { get; set; }

}