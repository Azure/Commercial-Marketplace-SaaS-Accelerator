using System;
using System.Collections.Generic;

namespace Marketplace.SaaS.Accelerator.DataAccess.Entities;

public partial class Subscriptions
{
    public Subscriptions()
    {
        MeteredAuditLogs = new HashSet<MeteredAuditLogs>();
        SubscriptionAuditLogs = new HashSet<SubscriptionAuditLogs>();
        MeteredPlanSchedulerManagements = new HashSet<MeteredPlanSchedulerManagement>();
    }

    public int Id { get; set; }
    public Guid AmpsubscriptionId { get; set; }
    public string SubscriptionStatus { get; set; }
    public string AmpplanId { get; set; }
    public string AmpOfferId { get; set; }
    public bool? IsActive { get; set; }
    public int? CreateBy { get; set; }
    public DateTime? CreateDate { get; set; }
    public DateTime? ModifyDate { get; set; }
    public int? UserId { get; set; }
    public string Name { get; set; }
    public int Ampquantity { get; set; }
    public string PurchaserEmail { get; set; }
    public Guid? PurchaserTenantId { get; set; }

    public string Term { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }


    public virtual Users User { get; set; }
    public virtual ICollection<MeteredAuditLogs> MeteredAuditLogs { get; set; }
    public virtual ICollection<SubscriptionAuditLogs> SubscriptionAuditLogs { get; set; }
    public virtual ICollection<MeteredPlanSchedulerManagement> MeteredPlanSchedulerManagements { get; set; }
}