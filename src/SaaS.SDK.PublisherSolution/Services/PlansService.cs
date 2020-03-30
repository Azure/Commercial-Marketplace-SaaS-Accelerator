using Microsoft.Marketplace.Saas.Web.Models;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Marketplace.SaasKit.Client.Services
{
    public class PlansService
    {
        public IPlansRepository plansRepository;

        public PlansService(IPlansRepository plansRepository)
        {
            this.plansRepository = plansRepository;
        }

        public List<PlansModel> GetPlans()
        {
            List<PlansModel> plansList = new List<PlansModel>();
            var allPlansData= this.plansRepository.GetPlansByUser();
            foreach (var item in allPlansData)
            {
                PlansModel Plans = new PlansModel();
                Plans.planId = item.PlanId;
                Plans.DisplayName =item.DisplayName;
                Plans.Description = item.Description;
                Plans.IsmeteringSupported = item.IsmeteringSupported;
                Plans.offerID = item.OfferId;
                Plans.DeployToCustomerSubscription = item.DeployToCustomerSubscription;
                plansList.Add(Plans);
            }
            return plansList;
        }

    }
}
