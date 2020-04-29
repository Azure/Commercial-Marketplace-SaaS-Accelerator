using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Marketplace.SaasKit.Client.Models
{
    public class SubscriptionProcessQueueModel
    {
        public Guid SubscriptionID { get; set; }
        public string TriggerEvent { get; set; }
        public int UserId { get; set; }
        public string PortalName { get; set; }
    }
}
