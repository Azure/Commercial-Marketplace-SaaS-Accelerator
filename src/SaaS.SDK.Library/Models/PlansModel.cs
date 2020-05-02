using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Marketplace.SaaS.SDK.Library.Models
{
    public class PlansModel
    {
        public int Id { get; set; }

        public string planId { get; set; }

        public bool? IsmeteringSupported { get; set; }

        public Guid? offerID { get; set; }

        public string OfferName { get; set; }
        public bool DeployToCustomerSubscription { get; set; }

        public SelectList PlansList { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public Guid PlanGUID { get; set; }

        public List<PlanAttributesModel> PlanAttributes { get; set; }
        public List<PlanEventsModel> PlanEvents { get; set; }
    }
}
