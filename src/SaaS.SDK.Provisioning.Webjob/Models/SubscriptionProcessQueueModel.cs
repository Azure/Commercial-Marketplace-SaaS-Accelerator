using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.Provisioning.Webjob.Models
{
    public class SubscriptionProcessQueueModel
    {
        public Guid SubscriptionID { get; set; }
        public string TriggerEvent { get; set; }
    }
}
