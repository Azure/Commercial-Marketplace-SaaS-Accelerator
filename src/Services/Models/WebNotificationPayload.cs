// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using Marketplace.SaaS.Accelerator.Services.WebHook;
using System.Text.Json.Serialization;

namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// WebNotificationPayload.
/// </summary>
public class WebNotificationPayload
{
    /// <summary>
    /// Gets or sets the ApplicationName identifier.
    /// </summary>
    /// <value>
    /// The Application Name.
    /// </value>
    [JsonPropertyName("applicationName")]
    public string ApplicationName { get; set; }

    /// <summary>
    /// Gets or sets the EventType.
    /// </summary>
    /// <value>
    /// The EventType.
    /// </value>
    [JsonPropertyName("eventType")]
    public WebNotificationEventTypeEnum EventType { get; set; }

    /// <summary>
    /// Gets or sets the PayloadFromLandingpage.
    /// </summary>
    /// <value>
    /// The PayloadFromLandingpage.
    /// </value>
    [JsonPropertyName("payloadFromLandingpage")]
    [JsonConverter(typeof(NullToEmptyObjectConverter<WebNotificationSubscription>))]
    public WebNotificationSubscription PayloadFromLandingpage { get; set; }

    /// <summary>
    /// Gets or sets the PayloadFromWebhook.
    /// </summary>
    /// <value>
    /// The PayloadFromWebhook.
    /// </value>
    [JsonPropertyName("payloadFromWebhook")]
    [JsonConverter(typeof(NullToEmptyObjectConverter<WebhookPayload>))]
    public WebhookPayload PayloadFromWebhook { get; set; }

}