using System;

namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// the SubscriptionLogsViewModel.
/// </summary>
public class SubscriptionLogsViewModel
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the subscription identifier.
    /// </summary>
    /// <value>
    /// The subscription identifier.
    /// </value>
    public int? SubscriptionId { get; set; }

    /// <summary>
    /// Gets or sets the attribute.
    /// </summary>
    /// <value>
    /// The attribute.
    /// </value>
    public string Attribute { get; set; }

    /// <summary>
    /// Gets or sets the old value.
    /// </summary>
    /// <value>
    /// The old value.
    /// </value>
    public string OldValue { get; set; }

    /// <summary>
    /// Gets or sets the new value.
    /// </summary>
    /// <value>
    /// The new value.
    /// </value>
    public string NewValue { get; set; }

    /// <summary>
    /// Gets or sets the create date.
    /// </summary>
    /// <value>
    /// The create date.
    /// </value>
    public DateTime? CreateDate { get; set; }

    /// <summary>
    /// Gets or sets the create by.
    /// </summary>
    /// <value>
    /// The create by.
    /// </value>
    public int? CreateBy { get; set; }
}