using System;
using System.Collections.Generic;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities
{
    public partial class PlanAttributeOutput
    {
        public int RowNumber { get; set; }
        public int PlanAttributeId { get; set; }
        public Guid PlanId { get; set; }
        public int OfferAttributeId { get; set; }
        public string DisplayName { get; set; }
        public bool IsEnabled { get; set; }
    }
}
