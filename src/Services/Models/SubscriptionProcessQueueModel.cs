using System;

namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// Subscription Process Queue Model.
/// </summary>
public class SubscriptionProcessQueueModel
{
    /// <summary>
    /// Gets or sets the subscription identifier.
    /// </summary>
    /// <value>
    /// The subscription identifier.
    /// </value>
    public Guid SubscriptionID { get; set; }

    /// <summary>
    /// Gets or sets the trigger event.
    /// </summary>
    /// <value>
    /// The trigger event.
    /// </value>
    public string TriggerEvent { get; set; }

    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    /// <value>
    /// The user identifier.
    /// </value>
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the name of the portal.
    /// </summary>
    /// <value>
    /// The name of the portal.
    /// </value>
    public string PortalName { get; set; }
}