using Microsoft.Marketplace.SaasKit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Saas.SDK.WebJob.Models
{
    public class SubscriptionResultExtension : SubscriptionResult
    {
        public bool IsMeteringSupported { get; set; }

        public bool IsPerUserPlan { get; set; }

        public Guid GuidPlanId { get; set; }
        public List<SubscriptionParametersModel> SubscriptionParameters { get; set; }

        public List<SubscriptionTemplateParametersModel> ARMTemplateParameters { get; set; }

    }
}
