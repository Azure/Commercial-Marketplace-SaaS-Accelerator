using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.DataModel
{
    public class PlanEventsModel
    {
        public int Id { get; set; }
        public Guid PlanId { get; set; }
        public int EventId { get; set; }
        public string EventsName { get; set; }
        public string SuccessStateEmails { get; set; }
        public string FailureStateEmails { get; set; }
        public bool Isactive { get; set; }
        public int UserId { get; set; }
        public bool CopyToCustomer { get; set; }
    }
}
