// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace Marketplace.SaaS.Accelerator.Services.Models;

public class WebNotificationSubscription
{

    /// <summary>
    /// Gets or sets the LandingPageCustomFields.
    /// </summary>
    /// <value>
    /// The LandingPageCustomFields.
    /// </value>
    [JsonPropertyName("landingpageSubscriptionParams")]
    public List<WebNotificationLandingPageParam> LandingPageCustomFields { get; set; }

    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the publisher identifier.
    /// </summary>
    /// <value>
    /// The publisher identifier.
    /// </value>
    [JsonPropertyName("publisherId")]
    public string PublisherId { get; set; }

    /// <summary>
    /// Gets or sets the offer identifier.
    /// </summary>
    /// <value>
    /// The offer identifier.
    /// </value>
    [JsonPropertyName("offerId")]
    public string OfferId { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the subscription status.
    /// </summary>
    /// <value>
    /// The subscription status.
    /// </value>
    [JsonPropertyName("saasSubscriptionStatus")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SubscriptionStatusEnum SaasSubscriptionStatus { get; set; }

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
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the purchaser.
    /// </summary>
    /// <value>
    /// The purchaser.
    /// </value>
    [JsonPropertyName("purchaser")]
    public PurchaserResult Purchaser { get; set; }

    /// <summary>
    /// Gets or sets the beneficiary.
    /// </summary>
    /// <value>
    /// The beneficiary.
    /// </value>
    [JsonPropertyName("beneficiary")]
    public BeneficiaryResult Beneficiary { get; set; }

    /// <summary>
    /// Gets or sets the term.
    /// </summary>
    /// <value>
    /// The term.
    /// </value>
    [JsonPropertyName("term")]
    public TermResult Term { get; set; }

}