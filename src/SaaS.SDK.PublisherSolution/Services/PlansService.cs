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

        public IOfferAttributesRepository offerAttributeRepository;

        public IOffersRepository offerRepository;
        public PlansService(IPlansRepository plansRepository, IOfferAttributesRepository offerAttributeRepository, IOffersRepository offerRepository)
        {
            this.offerRepository = offerRepository;
            this.plansRepository = plansRepository;
            this.offerAttributeRepository = offerAttributeRepository;
        }

        public List<PlansModel> GetPlans()
        {
            List<PlansModel> plansList = new List<PlansModel>();
            var allPlansData = this.plansRepository.GetPlansByUser();
            foreach (var item in allPlansData)
            {
                PlansModel Plans = new PlansModel();
                Plans.planId = item.PlanId;
                Plans.DisplayName = item.DisplayName;
                Plans.Description = item.Description;
                Plans.IsmeteringSupported = item.IsmeteringSupported;
                Plans.offerID = item.OfferId;
                Plans.DeployToCustomerSubscription = item.DeployToCustomerSubscription;
                Plans.PlanGUID = item.PlanGuid;
                plansList.Add(Plans);
            }
            return plansList;
        }

        public PlansModel GetPlanDetailByPlanGuId(Guid planGuId)
        {
            var existingPlan = this.plansRepository.GetPlanDetailByPlanGuId(planGuId);
            var planAttributes = this.plansRepository.GetPlanAttributesByPlanGuId(planGuId, existingPlan.OfferId);
            var planEvents = this.plansRepository.GetPlanEventsByPlanGuId(planGuId, existingPlan.OfferId);
            var offerDetails = this.offerRepository.GetOfferDetailByOfferId(existingPlan.OfferId);
            //var offerAttributes = this.offerAttributeRepository.GetOfferAttributeDetailByOfferId();
            //var activeAttribute = offerAttributes.Where(s => s.Isactive == true);

            PlansModel plan = new PlansModel
            {
                Id = existingPlan.Id,
                planId = existingPlan.PlanId,
                IsmeteringSupported = existingPlan.IsmeteringSupported,
                offerID = existingPlan.OfferId,
                DisplayName = existingPlan.DisplayName,
                Description = existingPlan.Description,
                PlanGUID = existingPlan.PlanGuid,
                OfferName = offerDetails.OfferName
            };

            plan.PlanAttributes = new List<PlanAttributesModel>();

            foreach (var attribute in planAttributes)
            {
                PlanAttributesModel planAttributesmodel = new PlanAttributesModel()
                {
                    OfferAttributeId = attribute.OfferAttributeId,
                    PlanAttributeId = attribute.PlanAttributeId,
                    PlanId = existingPlan.PlanGuid,
                    DisplayName = attribute.DisplayName,
                    IsEnabled = attribute.IsEnabled
                };
                plan.PlanAttributes.Add(planAttributesmodel);
            }

            plan.PlanEvents = new List<PlanEventsModel>();

            return plan;
        }



    }
}
