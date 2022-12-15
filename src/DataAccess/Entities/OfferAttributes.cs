using System;

namespace Marketplace.SaaS.Accelerator.DataAccess.Entities;

public partial class OfferAttributes
{
    public int Id { get; set; }
    public string ParameterId { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public int? ValueTypeId { get; set; }
    public bool FromList { get; set; }
    public string ValuesList { get; set; }
    public int? Max { get; set; }
    public int? Min { get; set; }
    public string Type { get; set; }
    public int? DisplaySequence { get; set; }
    public bool Isactive { get; set; }
    public DateTime? CreateDate { get; set; }
    public int? UserId { get; set; }
    public Guid OfferId { get; set; }
    public bool? IsDelete { get; set; }
    public bool? IsRequired { get; set; }
}