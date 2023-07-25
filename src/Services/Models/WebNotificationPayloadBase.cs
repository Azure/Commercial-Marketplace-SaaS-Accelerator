// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using Marketplace.SaaS.Accelerator.Services.WebHook;
using System;
using System.Text.Json.Serialization;

namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// Web Notification Payload.
/// </summary>
public abstract class WebNotificationPayloadBase
{
    /// <summary>
    /// Gets or sets the Web Notification Custom Info identifier.
    /// </summary>
    /// <value>
    /// The usage event identifier.
    /// </value>
    [JsonPropertyName("webNotificationCustomInfo")]
    public WebNotificationCustomInfo WebNotificationCustomInfo { get; set; }

}