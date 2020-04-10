using Microsoft.Marketplace.SaasKit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Marketplace.SaasKit.Client.Models
{
    public class SubscriptionResultExtension : SubscriptionResult
    {
        public bool IsPerUserPlan { get; set; }

    }
}
