using System;

namespace Marketplace.SaaS.Accelerator.DataAccess.Entities;

public partial class PlanAttributeMapping
{
    public int PlanAttributeId { get; set; }
    public Guid PlanId { get; set; }
    public int OfferAttributeId { get; set; }
    public bool IsEnabled { get; set; }
    public DateTime? CreateDate { get; set; }
    public int? UserId { get; set; }
}