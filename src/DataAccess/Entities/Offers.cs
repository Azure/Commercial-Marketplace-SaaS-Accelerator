using System;

namespace Marketplace.SaaS.Accelerator.DataAccess.Entities;

public partial class Offers
{
    public int Id { get; set; }
    public string OfferId { get; set; }
    public string OfferName { get; set; }
    public DateTime? CreateDate { get; set; }
    public int? UserId { get; set; }
    public Guid OfferGuid { get; set; }
}