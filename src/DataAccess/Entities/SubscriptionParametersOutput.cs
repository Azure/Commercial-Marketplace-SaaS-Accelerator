using System;

namespace Marketplace.SaaS.Accelerator.DataAccess.Entities;

public partial class SubscriptionParametersOutput
{
    public int RowNumber { get; set; }
    public int Id { get; set; }
    public int PlanAttributeId { get; set; }
    public int OfferAttributeId { get; set; }
    public string DisplayName { get; set; }
    public string Type { get; set; }
    public string ValueType { get; set; }
    public int DisplaySequence { get; set; }
    public bool IsEnabled { get; set; }
    public bool? IsRequired { get; set; }
    public string Value { get; set; }
    public Guid SubscriptionId { get; set; }
    public Guid OfferId { get; set; }
    public Guid PlanId { get; set; }
    public int? UserId { get; set; }
    public DateTime? CreateDate { get; set; }
    public bool FromList { get; set; }
    public string ValuesList { get; set; }
    public int Max { get; set; }
    public int Min { get; set; }
    public string Htmltype { get; set; }
}