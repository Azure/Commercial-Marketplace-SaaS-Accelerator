using System;

namespace Marketplace.SaaS.Accelerator.DataAccess.Entities;

public partial class SubscriptionAuditLogs
{
    public int Id { get; set; }
    public int? SubscriptionId { get; set; }
    public string Attribute { get; set; }
    public string OldValue { get; set; }
    public string NewValue { get; set; }
    public DateTime? CreateDate { get; set; }
    public int? CreateBy { get; set; }

    public virtual Subscriptions Subscription { get; set; }
}