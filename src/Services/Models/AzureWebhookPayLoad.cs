using System;
using System.Text.Json.Serialization;

namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// Wbe hook pary load.
/// </summary>
public class AzureWebHookPayLoad
{
    /// <summary>
    /// Gets or sets the action.
    /// </summary>
    /// <value>
    /// The action.
    /// </value>
    [JsonPropertyName("action")]
    public string Action { get; set; }

    /// <summary>
    /// Gets or sets the activity identifier.
    /// </summary>
    /// <value>
    /// The activity identifier.
    /// </value>
    [JsonPropertyName("activityId")]
    public string ActivityId { get; set; }

    /// <summary>
    /// Gets or sets the offer identifier.
    /// </summary>
    /// <value>
    /// The offer identifier.
    /// </value>
    [JsonPropertyName("offerId")]
    public string OfferId { get; set; }

    // Operation Id is presented as Id property on the json payload.

    /// <summary>
    /// Gets or sets the operation identifier.
    /// </summary>
    /// <value>
    /// The operation identifier.
    /// </value>
    [JsonPropertyName("id")]
    public string OperationId { get; set; }

    /// <summary>
    /// Gets or sets the plan identifier.
    /// </summary>
    /// <value>
    /// The plan identifier.
    /// </value>
    public string PlanId { get; set; }

    /// <summary>
    /// Gets or sets the publisher identifier.
    /// </summary>
    /// <value>
    /// The publisher identifier.
    /// </value>
    public string PublisherId { get; set; }

    /// <summary>
    /// Gets or sets the quantity.
    /// </summary>
    /// <value>
    /// The quantity.
    /// </value>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    /// <value>
    /// The status.
    /// </value>
    public string Status { get; set; }

    /// <summary>
    /// Gets or sets the subscription identifier.
    /// </summary>
    /// <value>
    /// The subscription identifier.
    /// </value>
    [JsonPropertyName("subscriptionId")]
    public string SubscriptionId { get; set; }

    /// <summary>
    /// Gets or sets the time stamp.
    /// </summary>
    /// <value>
    /// The time stamp.
    /// </value>
    [JsonPropertyName("timeStamp")]
    public DateTimeOffset TimeStamp { get; set; }
}