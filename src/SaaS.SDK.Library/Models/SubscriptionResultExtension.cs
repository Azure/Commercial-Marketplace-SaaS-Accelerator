using Microsoft.Marketplace.SaasKit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Marketplace.SaaS.SDK.Library.Models
{
    public class SubscriptionResultExtension : SubscriptionResult
    {
        public bool IsMeteringSupported { get; set; }

        public bool IsPerUserPlan { get; set; }

        public Guid GuidPlanId { get; set; }
        public List<SubscriptionParametersModel> SubscriptionParameters { get; set; }

        public List<SubscriptionTemplateParametersModel> ARMTemplateParameters { get; set; }
        public string EventName { get; set; }

        public SubscriptionStatusEnumExtension SubscriptionStatus { get; set; }

        public bool DeployToCustomerSubscription { get; set; }


    }
}
