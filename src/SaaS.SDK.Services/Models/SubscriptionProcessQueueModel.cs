using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Marketplace.SaaS.SDK.Services.Models
{
    public class SubscriptionProcessQueueModel
    {
        public Guid SubscriptionID { get; set; }
        public string TriggerEvent { get; set; }
        public int UserId { get; set; }
        public string PortalName { get; set; }
    }
}
