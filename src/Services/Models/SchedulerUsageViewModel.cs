using System;
using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// Subscription Usage ViewModel.
/// </summary>
public class SchedulerUsageViewModel
{
    /// <summary>
    /// Scheduler Task Name
    /// </summary>
    public string SchedulerName { get; set; }


    /// <summary>
    /// Gets or sets the subscription list.
    /// </summary>
    /// <value>
    /// The dimensions list.
    /// </value>
    /// </summary>
    public SelectList SubscriptionList { get; set; }


    /// <summary>
    /// Gets or sets the selected subscription.
    /// </summary>
    /// <value>
    /// The dimensions list.
    /// </value>
    /// </summary>
    public string SelectedSubscription { get; set; }


    /// <summary>
    /// Gets or sets the dimensions list.
    /// </summary>
    /// <value>
    /// The dimensions list.
    /// </value>
    public SelectList DimensionsList { get; set; }


    /// <summary>Gets or sets the selected dimension.</summary>
    /// <value>The selected dimension.</value>
    public string SelectedDimension { get; set; }


    public SelectList SchedulerFrequencyList { get; set; }
    public string SelectedSchedulerFrequency { get; set; }

    /// <summary>
    /// Gets or sets the quantity.
    /// </summary>
    /// <value>
    /// The quantity.
    /// </value>
    public double Quantity { get; set; }

    /// <summary>
    /// Get or set First Run Time
    /// </summary>
    public DateTime FirstRunDate { get; set; }


    /// <summary>
    /// Get or set Next Run Time
    /// </summary>
    public DateTime? NextRunDate { get; set; }

    /// <summary>
    /// Gets or sets the metered audit logs.
    /// </summary>
    /// <value>
    /// The metered audit logs.
    /// </value>
    /// 
    public List<MeteredAuditLogs> MeteredAuditLogs { get; set; }

    /// <summary>
    /// Gets or sets the user's timezone offset compared to UTC
    /// </summary>
    /// <value>
    /// The user's timezone offset.
    /// </value>
    public int TimezoneOffset { get; set; }
}