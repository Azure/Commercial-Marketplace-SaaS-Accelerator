using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Marketplace.SaasKit.Models;

namespace Microsoft.Marketplace.SaaS.SDK.CustomerProvisioning.Models
{
    public class PlanDetailResultExtension : PlanDetailResult
    {
        public int OfferId { get; set; }
    }
}
