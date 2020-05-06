namespace Microsoft.Marketplace.SaaS.SDK.Services.Services
{
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Service to manage plans
    /// </summary>
    public class PlanService
    {
        /// <summary>
        /// The plan repository
        /// </summary>
        public IPlansRepository plansRepository;

        /// <summary>
        /// The offer attributes repository
        /// </summary>
        public IOfferAttributesRepository offerAttributesRepository;

        /// <summary>
        /// The offer repository
        /// </summary>
        public IOffersRepository offerRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlanService"/> class.
        /// </summary>
        /// <param name="plansRepository">The plans repository.</param>
        /// <param name="offerAttributesRepository">The offer attributes repository.</param>
        /// <param name="offerRepository">The offer repository.</param>
        public PlanService(IPlansRepository plansRepository, IOfferAttributesRepository offerAttributesRepository, IOffersRepository offerRepository)
        {
            this.plansRepository = plansRepository;
            this.offerAttributesRepository = offerAttributesRepository;
            this.offerRepository = offerRepository;
        }

        /// <summary>
        /// Gets the plans.
        /// </summary>
        /// <returns>List of plans</returns>
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
                Plans.DeployToCustomerSubscription = item.DeployToCustomerSubscription ?? false;
                Plans.PlanGUID = item.PlanGuid;
                plansList.Add(Plans);
            }
            return plansList;
        }

        /// <summary>
        /// Gets the plan detail by plan gu identifier.
        /// </summary>
        /// <param name="planGuId">The plan gu identifier.</param>
        /// <returns></returns>
        public PlansModel GetPlanDetailByPlanGuId(Guid planGuId)
        {
            var existingPlan = this.plansRepository.GetByInternalReference(planGuId);
            var planAttributes = this.plansRepository.GetPlanAttributes(planGuId, existingPlan.OfferId);
            var planEvents = this.plansRepository.GetEventsByPlan(planGuId, existingPlan.OfferId);
            var offerDetails = this.offerRepository.GetOfferById(existingPlan.OfferId);
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
                OfferName = offerDetails.OfferName,
                DeployToCustomerSubscription = existingPlan.DeployToCustomerSubscription ?? false
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
                    IsEnabled = attribute.IsEnabled,
                    Type = attribute.Type
                };
                plan.PlanAttributes.Add(planAttributesmodel);
            }

            plan.PlanEvents = new List<PlanEventsModel>();

            foreach (var events in planEvents)
            {
                PlanEventsModel planEventsModel = new PlanEventsModel()
                {
                    Id = events.Id,
                    PlanId = events.PlanId,
                    Isactive = events.Isactive,
                    SuccessStateEmails = events.SuccessStateEmails,
                    FailureStateEmails = events.FailureStateEmails,
                    EventName = events.EventsName,
                    EventId = events.EventId,
                    CopyToCustomer = events.CopyToCustomer
                };
                plan.PlanEvents.Add(planEventsModel);
            }
            return plan;
        }

        /// <summary>
        /// Updates the deploy to customer subscription flag.
        /// </summary>
        /// <param name="plan">The plan.</param>
        /// <returns></returns>
        public void UpdateDeployToCustomerSubscriptionFlag(PlansModel plan)
        {
            var existingPlan = this.plansRepository.GetByInternalReference(plan.PlanGUID);
            existingPlan.DeployToCustomerSubscription = plan.DeployToCustomerSubscription;
            this.plansRepository.Save(existingPlan);            
        }

        /// <summary>
        /// Saves the plan attributes.
        /// </summary>
        /// <param name="planAttributes">The plan attributes.</param>
        /// <returns></returns>
        public int? SavePlanAttributes(PlanAttributesModel planAttributes)
        {
            if (planAttributes != null)
            {
                PlanAttributeMapping attribute = new PlanAttributeMapping();
                attribute.OfferAttributeId = planAttributes.OfferAttributeId;
                attribute.IsEnabled = planAttributes.IsEnabled;
                attribute.PlanId = planAttributes.PlanId;
                attribute.UserId = planAttributes.UserId;
                attribute.PlanAttributeId = planAttributes.PlanAttributeId;
                attribute.CreateDate = DateTime.Now;

                var planEventsId = this.plansRepository.SavePlanAttributes(attribute);
                return planEventsId;
            }
            return null;
        }
        
        /// <summary>
        /// Saves the plan events.
        /// </summary>
        /// <param name="planEvents">The plan events.</param>
        /// <returns></returns>
        public int? SavePlanEvents(PlanEventsModel planEvents)
        {
            if (planEvents != null)
            {
                PlanEventsMapping events = new PlanEventsMapping();
                events.Id = planEvents.Id;
                events.Isactive = planEvents.Isactive;
                events.PlanId = planEvents.PlanId;
                events.SuccessStateEmails = planEvents.SuccessStateEmails;
                events.FailureStateEmails = planEvents.FailureStateEmails;
                events.EventId = planEvents.EventId;
                events.UserId = planEvents.UserId;
                events.CreateDate = DateTime.Now;
                events.CopyToCustomer = planEvents.CopyToCustomer;
                events.ArmtemplateId = planEvents.ArmtemplateId;
                var planEventsId = this.plansRepository.AddPlanEvents(events);
                return planEventsId;
            }
            return null;
        }

        /// <summary>
        /// Saves the plan deployment attributes.
        /// </summary>
        /// <param name="plan">The plan.</param>
        /// <param name="currentUserId">The current user identifier.</param>
        /// <returns></returns>
        public int? SavePlanDeploymentAttributes(Plans plan, int currentUserId)
        {
            var offerAttributes = this.offerAttributesRepository.GetAllOfferAttributesByOfferId(plan.OfferId);            
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
                var planEventsId = this.plansRepository.SavePlanAttributes(attribute);
            }
            return null;
        }
    }
}