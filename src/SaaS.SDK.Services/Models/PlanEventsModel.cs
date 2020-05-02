using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Marketplace.SaaS.SDK.Services.Models
{
    public class PlanEventsModel
    {
        public int Id { get; set; }
        public Guid PlanId { get; set; }
        public Guid ArmtemplateId { get; set; }
        public int EventId { get; set; }
        public string EventName { get; set; }
        public bool Isactive { get; set; }
        public string SuccessStateEmails { get; set; }
        public string FailureStateEmails { get; set; }
        public int UserId { get; set; }
        public bool CopyToCustomer { get; set; }
    }
}
