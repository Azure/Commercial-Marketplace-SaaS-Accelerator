// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using Marketplace.SaaS.Accelerator.Services.WebHook;
using System;
using System.Text.Json.Serialization;

namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// Web Notification Payload.
/// </summary>
public class WebNotificationPayloadLandingPage
{
    /// <summary>
    /// Gets or sets the WebNotificationLandingPageCustomInfo.
    /// </summary>
    /// <value>
    /// The WebNotificationCustomInfo.
    /// </value>
    [JsonPropertyName("webNotificationCustomInfo")]
    public WebNotificationCustomInfoLandingPage WebNotificationLandingPageCustomInfo { get; set; }

    /// <summary>
    /// Gets or sets the PayloadFromMarketplace.
    /// </summary>
    /// <value>
    /// The PayloadFromMarketplace.
    /// </value>
    [JsonPropertyName("payloadFromMarketplace")]
    public WebNotificationSubscription PayloadFromMarketplace { get; set; }

}