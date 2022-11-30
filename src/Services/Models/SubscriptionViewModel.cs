using System.Collections.Generic;

namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// Subscription ViewModel.
/// </summary>
public class SubscriptionViewModel
{
    /// <summary>
    /// Gets or sets the subscriptions.
    /// </summary>
    /// <value>
    /// The subscriptions.
    /// </value>
    public List<SubscriptionResultExtension> Subscriptions { get; set; } = new List<SubscriptionResultExtension>();

    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    /// <value>
    /// The error message.
    /// </value>
    public string ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is success.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is success; otherwise, <c>false</c>.
    /// </value>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Gets or sets the saa s application URL.
    /// </summary>
    /// <value>
    /// The saa s application URL.
    /// </value>
    public string SaaSAppUrl { get; set; }
}