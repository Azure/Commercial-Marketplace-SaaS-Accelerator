using System;
using System.Collections.Generic;

namespace Marketplace.SaaS.Accelerator.AdminSite.Models.Offer;

public record OffersListViewModel
{
    public IList<OfferListItem> LineItems { get; set; }

    public class OfferListItem
    {
        public Guid OfferGuid { get; set; }
        
        public string OfferName { get; set; }
        
        public string OfferId { get; set; }
    }
}
