using System;

namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// Get or Set Metered Plan Scheduler Mananger
/// </summary>
public partial class MeteredPlanSchedulerManagementModel
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    public int Id { get; set; }


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
    /// Gets or sets subscription identifier.
    /// </summary>
    /// <value>
    /// Subscription identifier
    /// </value>
    public int? SubscriptionId { get; set; }
    /// <summary>
    /// Gets or sets plan identifier.
    /// </summary>
    /// <value>
    /// plan identifier.
    /// </value>
    public int? PlanId { get; set; }
    /// <summary>
    /// Gets or sets plan dimension identifier.
    /// </summary>
    /// <value>
    /// plan dimension identifier.
    /// </value>
    public int? DimensionId { get; set; }
    /// <summary>
    /// Gets or sets frequency identifier.
    /// </summary>
    /// <value>
    /// frequency identifier.
    /// </value>
    public int? FrequencyId { get; set; }
    /// <summary>
    /// Gets or sets the Metered Quantity.
    /// </summary>
    /// <value>
    /// a double number as quantity.
    /// </value>
    public double? Quantity { get; set; }
    /// <summary>
    /// Gets or sets the Start Date for schedule.
    /// </summary>
    /// <value>
    /// date to start the trigger.
    /// </value>
    public DateTime? StartDate { get; set; }
    /// <summary>
    /// Gets or sets the next date.
    /// </summary>
    /// <value>
    /// Next Run time.
    /// </value>
    public DateTime? NextRunTime { get; set; }

}