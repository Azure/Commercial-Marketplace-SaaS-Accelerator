using System;

namespace Marketplace.SaaS.Accelerator.DataAccess.Entities;

public partial class MeteredPlanSchedulerManagement
{
    public int Id { get; set; }
    public string SchedulerName { get; set; }
    public int? SubscriptionId { get; set; }
    public int? PlanId { get; set; }
    public int? DimensionId { get; set; }
    public int? FrequencyId { get; set; }
    public double? Quantity { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? NextRunTime { get; set; }

    public virtual Plans Plan { get; set; }
    public virtual Subscriptions Subscriptions { get; set; }
    public virtual MeteredDimensions MeteredDimensions { get; set; }
    public virtual SchedulerFrequency SchedulerFrequency { get; set; }

}