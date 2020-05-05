namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.DataModel;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Plans Repository
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.DAL.Interface.IPlansRepository" />
    public class PlansRepository : IPlansRepository
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly SaasKitContext context;

        /// <summary>
        /// The application configuration repository
        /// </summary>
        private readonly IApplicationConfigRepository applicationConfigRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlansRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public PlansRepository(SaasKitContext context, IApplicationConfigRepository applicationConfigRepository)
        {
            this.context = context;
            this.applicationConfigRepository = applicationConfigRepository;
        }

        /// <summary>
        /// The disposed
        /// </summary>
        private bool disposed = false;


        /// <summary>
        /// Gets all the records
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Plans> Get()
        {
            return context.Plans;
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Plans Get(int id)
        {
            return context.Plans.Where(s => s.Id == id).FirstOrDefault();
        }

        /// <summary>
        /// Adds the specified plan details.
        /// </summary>
        /// <param name="planDetails">The plan details.</param>
        /// <returns></returns>
        public int Add(Plans planDetails)
        {
            if (planDetails != null && !string.IsNullOrEmpty(planDetails.PlanId))
            {
                var existingPlan = context.Plans.Where(s => s.PlanId == planDetails.PlanId).FirstOrDefault();
                if (existingPlan != null)
                {
                    existingPlan.PlanId = planDetails.PlanId;
                    existingPlan.Description = planDetails.Description;
                    existingPlan.DisplayName = planDetails.DisplayName;
                    existingPlan.OfferId = planDetails.OfferId;
                    context.Plans.Update(existingPlan);
                    context.SaveChanges();
                    return existingPlan.Id;
                }
                else
                {
                    context.Plans.Add(planDetails);
                    context.SaveChanges();
                    return planDetails.Id;
                }
            }
            return 0;
        }

        /// <summary>
        /// Gets the plan detail by plan identifier.
        /// </summary>
        /// <param name="planId">The plan identifier.</param>
        /// <returns></returns>
        public Plans GetById(string planId)
        {
            return context.Plans.Where(s => s.PlanId == planId).FirstOrDefault();
        }

        /// <summary>
        /// Gets the plans by user.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Plans> GetPlansByUser()
        {
            var getAllPlans = this.context.Plans;
            return getAllPlans;
        }

        /// <summary>
        /// Gets the plan detail by plan identifier.
        /// </summary>
        /// <param name="planId">The plan identifier.</param>
        /// <returns></returns>
        public Plans GetByInternalReference(Guid planGuId)
        {
            return context.Plans.Where(s => s.PlanGuid == planGuId).FirstOrDefault();
        }

        /// <summary>
        /// Gets the plan detail by plan identifier.
        /// </summary>
        /// <param name="planId">The plan identifier.</param>
        /// <returns></returns>
        public List<Plans> GetPlansByOfferId(Guid offerId)
        {
            return context.Plans.Where(s => s.OfferId == offerId).ToList();
        }

        /// <summary>
        /// Gets the plan attribute on offer attribute identifier.
        /// </summary>
        /// <param name="offerAttributeId">The offer attribute identifier.</param>
        /// <param name="planGuId">The plan gu identifier.</param>
        /// <returns></returns>
        public PlanAttributeMapping GetPlanAttributeOnOfferAttributeId(int offerAttributeId, Guid planGuId)
        {
            var planAttribute = context.PlanAttributeMapping.Where(s => s.OfferAttributeId == offerAttributeId && s.PlanId == planGuId).FirstOrDefault();
            return planAttribute;

        }

        /// <summary>
        /// Gets the plan attributes.
        /// </summary>
        /// <param name="planGuId">The plan gu identifier.</param>
        /// <param name="offerId">The offer identifier.</param>
        /// <returns></returns>
        public IEnumerable<PlanAttributesModel> GetPlanAttributes(Guid planGuId, Guid offerId)
        {
            try
            {
                var offerAttributescCall = context.PlanAttributeOutput.FromSqlRaw("dbo.spGetOfferParameters {0}", planGuId);
                var offerAttributes = offerAttributescCall.ToList();

                List<PlanAttributesModel> attributesList = new List<PlanAttributesModel>();

                if (offerAttributes != null && offerAttributes.Count() > 0)
                {
                    foreach (var offerAttribute in offerAttributes)
                    {
                        PlanAttributesModel planAttributes = new PlanAttributesModel();
                        planAttributes.PlanAttributeId = offerAttribute.PlanAttributeId;
                        planAttributes.PlanId = offerAttribute.PlanId;
                        planAttributes.OfferAttributeId = offerAttribute.OfferAttributeId;
                        planAttributes.IsEnabled = offerAttribute.IsEnabled;
                        planAttributes.DisplayName = offerAttribute.DisplayName;
                        planAttributes.Type = offerAttribute.Type;
                        attributesList.Add(planAttributes);
                    }
                }
                return attributesList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return new List<PlanAttributesModel>();
        }

        /// <summary>
        /// Gets the events by plan.
        /// </summary>
        /// <param name="planGuId">The plan gu identifier.</param>
        /// <param name="offerId">The offer identifier.</param>
        /// <returns></returns>
        public IEnumerable<PlanEventsModel> GetEventsByPlan(Guid planGuId, Guid offerId)
        {            
            try
            {
                var allEvents = context.PlanEventsOutPut.FromSqlRaw("dbo.spGetPlanEvents {0}", planGuId).ToList();

                List<PlanEventsModel> eventsList = new List<PlanEventsModel>();

                if (allEvents != null && allEvents.Count() > 0)
                {
                    foreach (var events in allEvents)
                    {
                        PlanEventsModel planEvent = new PlanEventsModel();
                        planEvent.Id = events.Id;
                        planEvent.PlanId = events.PlanId;
                        planEvent.Isactive = events.Isactive;
                        planEvent.SuccessStateEmails = events.SuccessStateEmails;
                        planEvent.FailureStateEmails = events.FailureStateEmails;
                        planEvent.EventsName = events.EventsName;
                        planEvent.EventId = events.EventId;

                        planEvent.CopyToCustomer = events.CopyToCustomer ?? false;
                        if (planEvent.EventsName != "Pending Activation")
                        {
                            eventsList.Add(planEvent);
                        }
                        else if (Convert.ToBoolean(applicationConfigRepository.GetValueByName("IsAutomaticProvisioningSupported")))
                        {
                            eventsList.Add(planEvent);
                        }
                    }
                }
                return eventsList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return new List<PlanEventsModel>();
        }

        /// <summary>
        /// Adds the plan attributes.
        /// </summary>
        /// <param name="planAttributes">The plan attributes.</param>
        /// <returns></returns>
        public int? AddPlanAttributes(PlanAttributeMapping planAttributes)
        {
            if (planAttributes != null)
            {
                var existingPlanAttribute = context.PlanAttributeMapping.Where(s => s.PlanAttributeId ==
                planAttributes.PlanAttributeId).FirstOrDefault();
                if (existingPlanAttribute != null)
                {
                    existingPlanAttribute.OfferAttributeId = planAttributes.OfferAttributeId;
                    existingPlanAttribute.IsEnabled = planAttributes.IsEnabled;
                    existingPlanAttribute.PlanId = planAttributes.PlanId;
                    existingPlanAttribute.UserId = planAttributes.UserId;
                    existingPlanAttribute.PlanAttributeId = planAttributes.PlanAttributeId;
                    existingPlanAttribute.CreateDate = DateTime.Now;

                    context.PlanAttributeMapping.Update(existingPlanAttribute);
                    context.SaveChanges();
                    return existingPlanAttribute.PlanAttributeId;
                }
                else
                {
                    context.PlanAttributeMapping.Add(planAttributes);
                    context.SaveChanges();
                    return planAttributes.PlanAttributeId;
                }
            }

            return null;
        }

        /// <summary>
        /// Adds the plan events.
        /// </summary>
        /// <param name="planEvents">The plan events.</param>
        /// <returns></returns>
        public int? AddPlanEvents(PlanEventsMapping planEvents)
        {
            if (planEvents != null)
            {
                var existingPlanEvents = context.PlanEventsMapping.Where(s => s.Id ==
                planEvents.Id).FirstOrDefault();
                if (existingPlanEvents != null)
                {
                    existingPlanEvents.Id = planEvents.Id;
                    existingPlanEvents.Isactive = planEvents.Isactive;
                    existingPlanEvents.PlanId = planEvents.PlanId;
                    existingPlanEvents.SuccessStateEmails = planEvents.SuccessStateEmails;
                    existingPlanEvents.FailureStateEmails = planEvents.FailureStateEmails;
                    existingPlanEvents.EventId = planEvents.EventId;
                    existingPlanEvents.UserId = planEvents.UserId;
                    existingPlanEvents.CreateDate = DateTime.Now;
                    existingPlanEvents.CopyToCustomer = planEvents.CopyToCustomer;
                    existingPlanEvents.ArmtemplateId = planEvents.ArmtemplateId;
                    context.PlanEventsMapping.Update(existingPlanEvents);
                    context.SaveChanges();
                    return existingPlanEvents.Id;
                }
                else
                {
                    context.PlanEventsMapping.Add(planEvents);
                    context.SaveChanges();
                    return planEvents.Id;
                }
            }

            return null;
        }


        /// <summary>
        /// Removes the specified plan details.
        /// </summary>
        /// <param name="planDetails">The plan details.</param>
        public void Remove(Plans planDetails)
        {
            var existingPlan = context.Plans.Where(s => s.Id == planDetails.Id).FirstOrDefault();
            if (existingPlan != null)
            {
                context.Plans.Remove(existingPlan);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }
    }
}
