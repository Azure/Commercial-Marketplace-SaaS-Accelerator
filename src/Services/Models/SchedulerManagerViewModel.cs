using System;

namespace Marketplace.SaaS.Accelerator.Services.Models;

public partial class SchedulerManagerViewModel
{
    /// <summary>
    /// Gets Scheduler Manager View.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    public int Id { get; set; }
    /// <summary>
    /// Gets or sets the AMP Subscription Id.
    /// </summary>
    /// <value>
    /// Subscription Id.
    /// </value>
    public Guid AMPSubscriptionId { get; set; }
    /// <summary>
    /// Gets or sets Scheduler Name
    /// </summary>
    /// <value>
    /// Scheduler Name
    /// </value>
    public string SchedulerName { get; set; }

    /// Gets or sets Subscription Name
    /// </summary>
    /// <value>
    /// Subscription Name
    /// </value>
    public string SubscriptionName { get; set; }
    /// <summary>
    /// Gets or sets Purchaser Email
    /// </summary>
    /// <value>
    /// Purchaser Email
    /// </value>
    public string PurchaserEmail { get; set; }

    /// <summary>
    /// Gets or sets identifier.
    /// </summary>
    /// <value>
    /// plan identifier.
    /// </value>
    public string PlanId { get; set; }
    /// <summary>
    /// Gets or sets Dimension identifier.
    /// </summary>
    /// <value>
    /// Dimension identifier.
    /// </value>
    public string Dimension { get; set; }
    /// <summary>
    /// Gets or sets Frequency identifier.
    /// </summary>
    /// <value>
    /// Frequency identifier.
    /// </value>
    public string Frequency { get; set; }
    /// <summary>
    /// Gets or sets Quantity.
    /// </summary>
    /// <value>
    /// Quantity.
    /// </value>
    public double Quantity { get; set; }
    /// <summary>
    /// Gets or sets Start Date.
    /// </summary>
    /// <value>
    /// Schedule Start date.
    /// </value>
    public DateTime StartDate { get; set; }
    /// <summary>
    /// Gets or sets schedule next run datatime.
    /// </summary>
    /// <value>
    /// schedule next run.
    /// </value>
    public DateTime? NextRunTime { get; set; }

    /// <summary>
    /// Gets or sets schedule last run datatime.
    /// </summary>
    /// <value>
    /// schedule last run.
    /// </value>
    public DateTime? LastRunTime { get; set; }
}