// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.
using System.Text.Json.Serialization;

namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// WebNotificationEventType Enum
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WebNotificationEventTypeEnum
{
    /// <summary>
    /// The LandingPage.
    /// </summary>
    LandingPage,

    /// <summary>
    /// The Webhook.
    /// </summary>
    Webhook,
}