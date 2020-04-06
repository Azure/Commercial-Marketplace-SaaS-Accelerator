using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Marketplace.Saas.Web.Models
{
    public class OffersViewModel
    {

        public int Id { get; set; }
        public string OfferID { get; set; }
        public string OfferName { get; set; }

        public Guid? OfferGuid { get; set; }

        public List<OfferAttributesModel> OfferAttributes { get; set; }
    }
}
