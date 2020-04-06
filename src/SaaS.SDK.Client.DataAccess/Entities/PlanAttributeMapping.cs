using System;
using System.Collections.Generic;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities
{
    public partial class PlanAttributeMapping
    {
        public int PlanAttributeId { get; set; }
        public Guid? PlanId { get; set; }
        public int? OfferAttributeId { get; set; }
        public int? IsEnabled { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? UserId { get; set; }
    }
}
