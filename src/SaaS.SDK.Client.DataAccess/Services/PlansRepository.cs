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
        private readonly SaasKitContext Context;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlansRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public PlansRepository(SaasKitContext context)
        {
            Context = context;
        }

        /// <summary>
        /// The disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Plans> Get()
        {
            return Context.Plans;
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Plans Get(int id)
        {
            return Context.Plans.Where(s => s.Id == id).FirstOrDefault();
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
                var existingPlan = Context.Plans.Where(s => s.PlanId == planDetails.PlanId).FirstOrDefault();
                if (existingPlan != null)
                {
                    existingPlan.PlanId = planDetails.PlanId;
                    existingPlan.Description = planDetails.Description;
                    existingPlan.DisplayName = planDetails.DisplayName;

                    Context.Plans.Update(existingPlan);
                    Context.SaveChanges();
                    return existingPlan.Id;
                }
                else
                {
                    Context.Plans.Add(planDetails);
                    Context.SaveChanges();
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
        public Plans GetPlanDetailByPlanId(string planId)
        {
            return Context.Plans.Where(s => s.PlanId == planId).FirstOrDefault();
        }

        public IEnumerable<Plans> GetPlansByUser()
        {
            var getAllPlans = this.Context.Plans;
            return getAllPlans;
        }

        /// <summary>
        /// Gets the plan detail by plan identifier.
        /// </summary>
        /// <param name="planId">The plan identifier.</param>
        /// <returns></returns>
        public Plans GetPlanDetailByPlanGuId(Guid planGuId)
        {
            return Context.Plans.Where(s => s.PlanGuid == planGuId).FirstOrDefault();
        }


        public IEnumerable<PlanAttributesModel> GetPlanAttributesByPlanGuId(Guid planGuId, Guid offerId)
        {
            try
            {
                var offerAttributescCall = Context.PlanAttributeOutput.FromSqlRaw("dbo.spGetOfferParameters {0}", planGuId);
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
                        attributesList.Add(planAttributes);
                    }
                }
                return attributesList;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
            return new List<PlanAttributesModel>();
        }

        public IEnumerable<PlanEventsModel> GetPlanEventsByPlanGuId(Guid planGuId, Guid offerId)
        {
            //return Context.PlanEventsMapping.Where(s => s.PlanId == planGuId);


            try
            {

                var allEvents = Context.PlanEventsOutPut.FromSqlRaw("dbo.spGetPlanEvents {0}", planGuId).ToList();

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
                        eventsList.Add(planEvent);
                    }
                }
                return eventsList;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
            return new List<PlanEventsModel>();
        }


        public int? AddPlanAttributes(PlanAttributeMapping planAttributes)
        {
            if (planAttributes != null)
            {
                var existingPlanAttribute = Context.PlanAttributeMapping.Where(s => s.PlanAttributeId ==
                planAttributes.PlanAttributeId).FirstOrDefault();
                if (existingPlanAttribute != null)
                {
                    existingPlanAttribute.OfferAttributeId = planAttributes.OfferAttributeId;
                    existingPlanAttribute.IsEnabled = planAttributes.IsEnabled;
                    existingPlanAttribute.PlanId = planAttributes.PlanId;
                    existingPlanAttribute.UserId = planAttributes.UserId;
                    existingPlanAttribute.PlanAttributeId = planAttributes.PlanAttributeId;
                    existingPlanAttribute.CreateDate = DateTime.Now;

                    Context.PlanAttributeMapping.Update(existingPlanAttribute);
                    Context.SaveChanges();
                    return existingPlanAttribute.PlanAttributeId;
                }
                else
                {
                    Context.PlanAttributeMapping.Add(planAttributes);
                    Context.SaveChanges();
                    return planAttributes.PlanAttributeId;
                }
            }

            return null;
        }

        public int? AddPlanEvents(PlanEventsMapping planEvents)
        {
            if (planEvents != null)
            {
                var existingPlanEvents = Context.PlanEventsMapping.Where(s => s.Id ==
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

                    Context.PlanEventsMapping.Update(existingPlanEvents);
                    Context.SaveChanges();
                    return existingPlanEvents.Id;
                }
                else
                {
                    Context.PlanEventsMapping.Add(planEvents);
                    Context.SaveChanges();
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
            var existingPlan = Context.Plans.Where(s => s.Id == planDetails.Id).FirstOrDefault();
            if (existingPlan != null)
            {
                Context.Plans.Remove(existingPlan);
                Context.SaveChanges();
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
                    Context.Dispose();
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
