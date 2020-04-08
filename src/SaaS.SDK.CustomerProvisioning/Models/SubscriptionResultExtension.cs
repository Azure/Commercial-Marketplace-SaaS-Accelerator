using Microsoft.Marketplace.SaasKit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Marketplace.SaaS.SDK.CustomerProvisioning.Models
{
    public class SubscriptionResultExtension : SubscriptionResult
    {
        public List<SubscriptionParametersModel> SubscriptionParameters { get; set; }

    }
}
