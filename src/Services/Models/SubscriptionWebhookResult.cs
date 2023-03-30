// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Text.Json.Serialization;

namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// Subscription object.
/// </summary>
public class SubscriptionWebhookResult
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the offer identifier.
    /// </summary>
    /// <value>
    /// The offer identifier.
    /// </value>
    [JsonPropertyName("offerId")]
    public string OfferId { get; set; }

    /// <summary>
    /// Gets or sets the plan identifier.
    /// </summary>
    /// <value>
    /// The plan identifier.
    /// </value>
    [JsonPropertyName("planId")]
    public string PlanId { get; set; }

    /// <summary>
    /// Gets or sets the quantity.
    /// </summary>
    /// <value>
    /// The quantity.
    /// </value>
    [JsonPropertyName("quantity")]
    public int? Quantity { get; set; }

    /// <summary>
    /// Gets or sets the subscription status.
    /// </summary>
    /// <value>
    /// The subscription status.
    /// </value>
    [JsonPropertyName("saasSubscriptionStatus")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SubscriptionStatusEnum SaasSubscriptionStatus { get; set; }

}