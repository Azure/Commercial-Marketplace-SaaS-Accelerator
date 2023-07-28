// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using Marketplace.SaaS.Accelerator.Services.WebHook;
using System;
using System.Text.Json.Serialization;

namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// Web Notification Payload.
/// </summary>
public class WebNotificationPayloadWebhook
{
    /// <summary>
    /// Gets or sets the WebNotificationWebhookCustomInfo.
    /// </summary>
    /// <value>
    /// The WebNotificationWebhookCustomInfo.
    /// </value>
    [JsonPropertyName("webNotificationCustomInfo")]
    public WebNotificationCustomInfo WebNotificationWebhookCustomInfo { get; set; }


    /// <summary>
    /// Gets or sets the PayloadFromMarketplace.
    /// </summary>
    /// <value>
    /// The PayloadFromMarketplace.
    /// </value>
    [JsonPropertyName("payloadfrommarketplace")]
    public WebhookPayload PayloadFromMarketplace { get; set; }

}