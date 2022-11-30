using System;

namespace Marketplace.SaaS.Accelerator.DataAccess.Entities;

public partial class SubscriptionAttributeValues
{
    public int Id { get; set; }
    public int PlanAttributeId { get; set; }
    public string Value { get; set; }
    public Guid SubscriptionId { get; set; }
    public DateTime? CreateDate { get; set; }
    public int? UserId { get; set; }
    public Guid PlanId { get; set; }
    public Guid OfferId { get; set; }
}