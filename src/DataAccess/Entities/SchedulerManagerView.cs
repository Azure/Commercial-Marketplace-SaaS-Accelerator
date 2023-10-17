using System;

namespace Marketplace.SaaS.Accelerator.DataAccess.Entities;

public partial class SchedulerManagerView
{
    public int Id { get; set; }
    public string SchedulerName { get; set; }
    public string SubscriptionName { get; set; }
    public string PurchaserEmail { get; set; }
    public Guid AMPSubscriptionId { get; set; }
    public string PlanId { get; set; }
    public string Dimension { get; set; }
    public string Frequency { get; set; }
    public double Quantity { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? NextRunTime { get; set; }
}

public class SchedulerManagerViewExt
{
    private readonly SchedulerManagerView v;

    public SchedulerManagerViewExt(SchedulerManagerView schedulerManagerView)
    {
        this.v = schedulerManagerView;
    }

    public int? SubscriptionId { get; set; }

    public int Id => v.Id;
    public string SchedulerName => v.SchedulerName;
    public string SubscriptionName => v.SubscriptionName;
    public string PurchaserEmail => v.PurchaserEmail;
    public Guid AMPSubscriptionId => v.AMPSubscriptionId;
    public string PlanId => v.PlanId;
    public string Dimension => v.Dimension;
    public string Frequency => v.Frequency;
    public double Quantity => v.Quantity;
    public DateTime StartDate => v.StartDate;
    public DateTime? NextRunTime => v.NextRunTime;

}