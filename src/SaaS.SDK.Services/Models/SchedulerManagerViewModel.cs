using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Marketplace.SaaS.SDK.Services.Models
{
    public partial class SchedulerManagerViewModel
    {
        public int Id { get; set; }
        public Guid AMPSubscriptionId { get; set; }
        public string PlanId { get; set; }
        public string Dimension { get; set; }
        public string Frequency { get; set; }
        public double Quantity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? NextRunTime { get; set; }
    }
}

