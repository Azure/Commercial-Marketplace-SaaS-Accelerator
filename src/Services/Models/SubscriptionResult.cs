// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// Subscription Response.
/// </summary>
/// <seealso cref="Microsoft.Marketplace.SaasKit.Models.SaaSApiResult" />
public class SubscriptionResult : SaaSApiResult
{
    /// <summary>
    /// Gets or sets the action.
    /// </summary>
    /// <value>
    /// The action.
    /// </value>
    public string Action { get; set; }

    /// <summary>
    /// Gets or sets the activity identifier.
    /// </summary>
    /// <value>
    /// The activity identifier.
    /// </value>
    public Guid ActivityId { get; set; }

    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    /// <value>
    /// The error message.
    /// </value>
    public string ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the error status code.
    /// </summary>
    /// <value>
    /// The error status code.
    /// </value>
    public string ErrorStatusCode { get; set; }

    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    [JsonPropertyName("id")]
    [DisplayName("Subscription Id")]
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
    /// Gets or sets the operation request source.
    /// </summary>
    /// <value>
    /// The operation request source.
    /// </value>
    public string OperationRequestSource { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    [JsonPropertyName("name")]
    [DisplayName("Subscription Name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the subscription status.
    /// </summary>
    /// <value>
    /// The subscription status.
    /// </value>
    [JsonPropertyName("saasSubscriptionStatus")]
    [DisplayName("Subscription Status")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SubscriptionStatusEnum SaasSubscriptionStatus { get; set; }

    /// <summary>
    /// Gets or sets the plan identifier.
    /// </summary>
    /// <value>
    /// The plan identifier.
    /// </value>
    [DisplayName("Plan Id")]
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
    /// Gets or sets the resource location.
    /// </summary>
    /// <value>
    /// The resource location.
    /// </value>
    public Uri ResourceLocation { get; set; }

    /// <summary>
    /// Gets or sets the fulfillment identifier.
    /// </summary>
    /// <value>
    /// The fulfillment identifier.
    /// </value>
    public string FulfillmentId { get; set; }

    /// <summary>
    /// Gets or sets the store front.
    /// </summary>
    /// <value>
    /// The store front.
    /// </value>
    public string StoreFront { get; set; }

    /// <summary>
    /// Gets or sets the name of the subscription.
    /// </summary>
    /// <value>
    /// The name of the subscription.
    /// </value>
    [DisplayName("Subscription Name")]
    public string SubscriptionName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is active subscription.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is active subscription; otherwise, <c>false</c>.
    /// </value>
    public bool IsActiveSubscription { get; set; }

    /// <summary>
    /// Gets or sets the time stamp.
    /// </summary>
    /// <value>
    /// The time stamp.
    /// </value>
    public DateTimeOffset TimeStamp { get; set; }

    /// <summary>
    /// Gets or sets the subscribe identifier.
    /// </summary>
    /// <value>
    /// The subscribe identifier.
    /// </value>
    public int SubscribeId { get; set; }

    /// <summary>
    /// Gets or sets the selected plan identifier.
    /// </summary>
    /// <value>
    /// The selected plan identifier.
    /// </value>
    public string SelectedPlanId { get; set; }

    /// <summary>
    /// Gets or sets the plan list.
    /// </summary>
    /// <value>
    /// The plan list.
    /// </value>
    public List<PlanDetailResult> PlanList { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show welcome screen].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [show welcome screen]; otherwise, <c>false</c>.
    /// </value>
    public bool ShowWelcomeScreen { get; set; }

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

    /// <summary>Gets or sets the customer email address.</summary>
    /// <value>The customer email address.</value>
    public string CustomerEmailAddress { get; set; }

    /// <summary>Gets or sets the name of the customer.</summary>
    /// <value>The name of the customer.</value>
    public string CustomerName { get; set; }
}