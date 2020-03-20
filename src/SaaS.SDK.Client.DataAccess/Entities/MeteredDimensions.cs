using System;
using System.Collections.Generic;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities
{
    public partial class MeteredDimensions
    {
        public int Id { get; set; }
        public string Dimension { get; set; }
        public int? PlanId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Description { get; set; }

        public virtual Plans Plan { get; set; }
    }
}
