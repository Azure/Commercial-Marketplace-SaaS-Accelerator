using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.WebJob.Models
{
    public class SubscriptionProcessQueueModel
    {
        public Guid SubscriptionID { get; set; }
        public string TriggerEvent { get; set; }
    }
}
