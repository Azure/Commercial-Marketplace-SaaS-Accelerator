using System;
using System.Collections.Generic;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities
{
    public partial class Plans
    {
        public Plans()
        {
            MeteredDimensions = new HashSet<MeteredDimensions>();
        }

        public int Id { get; set; }
        public string PlanId { get; set; }
        public string Description { get; set; }
        public string DisplayName { get; set; }
        public bool? IsmeteringSupported { get; set; }
        public bool? IsPerUser { get; set; }

        public virtual ICollection<MeteredDimensions> MeteredDimensions { get; set; }
    }
}
