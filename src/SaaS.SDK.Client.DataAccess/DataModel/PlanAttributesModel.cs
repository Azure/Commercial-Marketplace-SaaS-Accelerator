using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.DataModel
{
   public class PlanAttributesModel
    {
        public int PlanAttributeId { get; set; }
        public Guid PlanId { get; set; }
        public int OfferAttributeId { get; set; }
        public string DisplayName { get; set; }
        public bool IsEnabled { get; set; }
    }
}
