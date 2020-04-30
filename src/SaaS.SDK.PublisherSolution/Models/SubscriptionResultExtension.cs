using Microsoft.Marketplace.Saas.Web.Models;
using Microsoft.Marketplace.SaasKit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Marketplace.SaasKit.Client.Models
{
    public class SubscriptionResultExtension : SubscriptionResult
    {
        public bool IsMeteringSupported { get; set; }
        public bool IsPerUserPlan { get; set; }
        public Guid GuidPlanId { get; set; }
        public string EventName { get; set; }
        public List<SubscriptionParametersModel> SubscriptionParameters { get; set; }

        public new  SubscriptionStatusEnumExtension SaasSubscriptionStatus { get; set; }



    }
}
