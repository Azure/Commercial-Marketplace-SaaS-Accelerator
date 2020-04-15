using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Marketplace.SaaS.SDK.CustomerProvisioning.Services
{
    public class PlanService
    {
        /// <summary>
        /// The plan repository
        /// </summary>
        public IPlansRepository plansRepository;

        public IOfferAttributesRepository offerAttributesRepository;

        public PlanService(IPlansRepository plansRepository, IOfferAttributesRepository offerAttributesRepository)
        {
            this.plansRepository = plansRepository;
            this.offerAttributesRepository = offerAttributesRepository;
        }

        public int? SavePlanAttributes(Plans plan, int currentUserId)
        {
            var offerAttributes = this.offerAttributesRepository.GetAllOfferAttributeDetailByOfferId(plan.OfferId);
            var deploymentAttributes = offerAttributes.ToList().Where(s => s.Type.ToLower() == "deployment").ToList();
            foreach (var offerAttribute in deploymentAttributes)
            {
                PlanAttributeMapping attribute = new PlanAttributeMapping();
                var existingPlanAttribute = this.plansRepository.GetPlanAttributeOnOfferAttributeId(offerAttribute.Id, plan.PlanGuid);
                if (existingPlanAttribute != null)
                {
                    attribute.OfferAttributeId = existingPlanAttribute.OfferAttributeId;
                    attribute.IsEnabled = existingPlanAttribute.IsEnabled;
                    attribute.PlanId = plan.PlanGuid;
                    attribute.UserId = currentUserId;
                    attribute.PlanAttributeId = existingPlanAttribute.PlanAttributeId;
                    attribute.CreateDate = DateTime.Now;
                }
                else
                {
                    attribute.OfferAttributeId = offerAttribute.Id;
                    attribute.IsEnabled = true;
                    attribute.PlanId = plan.PlanGuid;
                    attribute.UserId = currentUserId;
                    attribute.PlanAttributeId = 0;
                    attribute.CreateDate = DateTime.Now;

                }
                var planEventsId = this.plansRepository.AddPlanAttributes(attribute);
            }
            return null;
        }
    }
}
