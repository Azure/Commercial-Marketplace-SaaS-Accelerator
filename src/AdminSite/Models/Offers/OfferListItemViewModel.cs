using System;

namespace Marketplace.SaaS.Accelerator.AdminSite.Models.Offers;

public record OfferListItemViewModel
{
    public string OfferId { get; init; }
    public Guid? OfferGuid { get; init; }
}
