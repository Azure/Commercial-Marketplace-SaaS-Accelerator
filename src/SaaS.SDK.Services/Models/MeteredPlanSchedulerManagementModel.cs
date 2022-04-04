using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities
{
    public partial class MeteredPlanSchedulerManagementModel
    {
        public int Id { get; set; }
        public int? SubscriptionId { get; set; }
        public int? PlanId { get; set; }
        public int? DimensionId { get; set; }
        public int? FrequencyId { get; set; }
        public double? Quantity { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? NextRunTime { get; set; }

    }
}
