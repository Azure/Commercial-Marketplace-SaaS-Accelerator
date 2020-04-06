using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Marketplace.Saas.Web.Models
{
    public class PlansModel
    {
        public int Id { get; set; }

        public string planId { get; set; }

        public bool? IsmeteringSupported { get; set; }

        public Guid? offerID { get; set; }

        public bool? DeployToCustomerSubscription { get; set; }

        public SelectList PlansList { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }
    }
}
