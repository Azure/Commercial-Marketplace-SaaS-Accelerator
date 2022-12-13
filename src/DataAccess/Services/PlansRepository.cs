using System;
using System.Collections.Generic;
using System.Linq;
using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.DataModel;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.SaaS.Accelerator.DataAccess.Services;

/// <summary>
/// Plans Repository.
/// </summary>
/// <seealso cref="Microsoft.Marketplace.SaasKit.DAL.Interface.IPlansRepository" />
public class PlansRepository : IPlansRepository
{
    /// <summary>
    /// The context.
    /// </summary>
    private readonly SaasKitContext context;

    /// <summary>
    /// The application configuration repository.
    /// </summary>
    private readonly IApplicationConfigRepository applicationConfigRepository;

    /// <summary>
    /// The disposed.
    /// </summary>
    private bool disposed = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlansRepository" /> class.
    /// </summary>
    /// <param name="context">The this.context.</param>
    /// <param name="applicationConfigRepository">The application configuration repository.</param>
    public PlansRepository(SaasKitContext context, IApplicationConfigRepository applicationConfigRepository)
    {
        this.context = context;
        this.applicationConfigRepository = applicationConfigRepository;
    }

    /// <summary>
    /// Gets all the records.
    /// </summary>
    /// <returns> List of plans.</returns>
    public IEnumerable<Plans> Get()
    {
        return this.context.Plans;
    }

    /// <summary>
    /// Gets the specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns> Plans.</returns>
    public Plans Get(int id)
    {
        return this.context.Plans.Where(s => s.Id == id).FirstOrDefault();
    }

    /// <summary>
    /// Adds/Updates the specified plan details.
    /// </summary>
    /// <param name="planDetails">The plan details.</param>
    /// <returns> Plan Id.</returns>
    public int Save(Plans planDetails)
    {
        if (planDetails != null && !string.IsNullOrEmpty(planDetails.PlanId))
        {
            var existingPlan = this.context.Plans.Include(p => p.MeteredDimensions).Where(s => s.PlanId == planDetails.PlanId).FirstOrDefault();
            if (existingPlan != null)
            {
                //room for improvement as these values dont change we dont make a db trip if something changes?
                existingPlan.PlanId = planDetails.PlanId;
                existingPlan.Description = planDetails.Description;
                existingPlan.DisplayName = planDetails.DisplayName;
                existingPlan.OfferId = planDetails.OfferId;
                existingPlan.IsmeteringSupported = planDetails.IsmeteringSupported;
                this.CheckMeteredDimension(planDetails, existingPlan);
                this.context.Plans.Update(existingPlan);
                this.context.SaveChanges();
                return existingPlan.Id;
            }
            else
            {
                this.context.Plans.Add(planDetails);
                this.context.SaveChanges();
                return planDetails.Id;
            }
        }

        return 0;
    }

    /// <summary>
    /// Adds the specified plan details with information available from GetSubscription API
    /// This is more relevent for an Unsubscribed subscription where the ListAvailablePlans API wont work
    /// </summary>
    /// <param name="planDetails">The plan details.</param>
    /// <returns> Plan Id.</returns>
    public int Add(Plans planDetails)
    {
        if (planDetails != null && !string.IsNullOrEmpty(planDetails.PlanId))
        {
            var existingPlan = this.context.Plans.Include(p => p.MeteredDimensions).Where(s => s.PlanId == planDetails.PlanId).FirstOrDefault();
            if (existingPlan == null)
            {
                this.context.Plans.Add(planDetails);
                this.context.SaveChanges();
            }
            return planDetails.Id;
        }

        return 0;
    }


    /// <summary>
    /// Check if there is Metered Dimensions exists or updated
    /// </summary>
    /// <param name="planDetails">Incoming Plans data from Payload</param>
    /// <param name="existingPlan">Existing Plans data from database</param>
    private void CheckMeteredDimension(Plans planDetails, Plans existingPlan)
    {
        // Check if Metered Dimension exists or new Metered Dimension to add
        foreach (MeteredDimensions metered in planDetails.MeteredDimensions)
        {
            // Assign Plan.Id to metered PlandId
            metered.PlanId = existingPlan.Id;

            // Query DB for metered dimension using PlanID and Dimension ID
            var existingMeteredDimensions = this.context.MeteredDimensions.Where(s => s.PlanId == existingPlan.Id && s.Dimension == metered.Dimension).FirstOrDefault();

            // Check if Metered Dimensions exists
            if (existingMeteredDimensions != null)
            {
                // Metered Dimension exists. No needs to updated Keys. We could update description if it is modified.
                if (existingMeteredDimensions.Description != metered.Description)
                {
                    var existingMetered = existingPlan.MeteredDimensions.Where(s => s.PlanId == existingPlan.Id && s.Dimension == metered.Dimension).FirstOrDefault();
                    existingMetered.Description = metered.Description;
                }

            }
            else
            {
                // Add new metered Dimension
                existingPlan.MeteredDimensions.Add(metered);
            }
        }
    }

    /// <summary>
    /// Gets the plan detail by plan identifier.
    /// </summary>
    /// <param name="planId">The plan identifier.</param>
    /// <returns> Plan.</returns>
    public Plans GetById(string planId)
    {
        return this.context.Plans.Where(s => s.PlanId == planId).FirstOrDefault();
    }

    /// <summary>
    /// Gets the plans by user.
    /// </summary>
    /// <returns> List of Plans.</returns>
    public IEnumerable<Plans> GetPlansByUser()
    {
        var getAllPlans = this.context.Plans;
        return getAllPlans;
    }

    /// <summary>
    /// Gets the plan detail by plan identifier.
    /// </summary>
    /// <param name="planGuId">The plan gu identifier.</param>
    /// <returns>
    /// Plan detail for the internal reference (GUID).
    /// </returns>
    public Plans GetByInternalReference(Guid planGuId)
    {
        return this.context.Plans.Where(s => s.PlanGuid == planGuId).FirstOrDefault();
    }

    /// <summary>
    /// Gets the plan detail by plan identifier.
    /// </summary>
    /// <param name="offerId">The offer identifier.</param>
    /// <returns> List of plans.</returns>
    public List<Plans> GetPlansByOfferId(Guid offerId)
    {
        return this.context.Plans.Where(s => s.OfferId == offerId).ToList();
    }

    /// <summary>
    /// Gets the plan attribute on offer attribute identifier.
    /// </summary>
    /// <param name="offerAttributeId">The offer attribute identifier.</param>
    /// <param name="planGuId">The plan gu identifier.</param>
    /// <returns> Plan attributes.</returns>
    public PlanAttributeMapping GetPlanAttributeOnOfferAttributeId(int offerAttributeId, Guid planGuId)
    {
        var planAttribute = this.context.PlanAttributeMapping.Where(s => s.OfferAttributeId == offerAttributeId && s.PlanId == planGuId).FirstOrDefault();
        return planAttribute;
    }

    /// <summary>
    /// Gets the plan attributes.
    /// </summary>
    /// <param name="planGuId">The plan gu identifier.</param>
    /// <param name="offerId">The offer identifier.</param>
    /// <returns> List type="of Plan attributes.</returns>
    public IEnumerable<PlanAttributesModel> GetPlanAttributes(Guid planGuId, Guid offerId)
    {
        var offerAttributescCall = this.context.PlanAttributeOutput.FromSqlRaw("dbo.spGetOfferParameters {0}", planGuId);
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

    /// <summary>
    /// Gets the events by plan.
    /// </summary>
    /// <param name="planGuId">The plan gu identifier.</param>
    /// <param name="offerId">The offer identifier.</param>
    /// <returns> Plan Events Model.</returns>
    public IEnumerable<PlanEventsModel> GetEventsByPlan(Guid planGuId, Guid offerId)
    {
        var allEvents = this.context.PlanEventsOutPut.FromSqlRaw("dbo.spGetPlanEvents {0}", planGuId).ToList();

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
                else if (!Convert.ToBoolean(this.applicationConfigRepository.GetValueByName("IsAutomaticProvisioningSupported")))
                {
                    eventsList.Add(planEvent);
                }
            }
        }

        return eventsList;
    }

    /// <summary>
    /// Adds the plan attributes.
    /// </summary>
    /// <param name="planAttributes">The plan attributes.</param>
    /// <returns> Plan Attribute Id.</returns>
    public int? SavePlanAttributes(PlanAttributeMapping planAttributes)
    {
        if (planAttributes != null)
        {
            var existingPlanAttribute = this.context.PlanAttributeMapping.Where(s => s.PlanAttributeId ==
                planAttributes.PlanAttributeId).FirstOrDefault();
            if (existingPlanAttribute != null)
            {
                existingPlanAttribute.OfferAttributeId = planAttributes.OfferAttributeId;
                existingPlanAttribute.IsEnabled = planAttributes.IsEnabled;
                existingPlanAttribute.PlanId = planAttributes.PlanId;
                existingPlanAttribute.UserId = planAttributes.UserId;
                existingPlanAttribute.PlanAttributeId = planAttributes.PlanAttributeId;
                existingPlanAttribute.CreateDate = DateTime.Now;

                this.context.PlanAttributeMapping.Update(existingPlanAttribute);
                this.context.SaveChanges();
                return existingPlanAttribute.PlanAttributeId;
            }
            else
            {
                this.context.PlanAttributeMapping.Add(planAttributes);
                this.context.SaveChanges();
                return planAttributes.PlanAttributeId;
            }
        }

        return null;
    }

    /// <summary>
    /// Adds the plan events.
    /// </summary>
    /// <param name="planEvents">The plan events.</param>
    /// <returns> Plan Event Id.</returns>
    public int? AddPlanEvents(PlanEventsMapping planEvents)
    {
        if (planEvents != null)
        {
            var existingPlanEvents = this.context.PlanEventsMapping.Where(s => s.Id ==
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
                this.context.PlanEventsMapping.Update(existingPlanEvents);
                this.context.SaveChanges();
                return existingPlanEvents.Id;
            }
            else
            {
                this.context.PlanEventsMapping.Add(planEvents);
                this.context.SaveChanges();
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
        var existingPlan = this.context.Plans.Where(s => s.Id == planDetails.Id).FirstOrDefault();
        if (existingPlan != null)
        {
            this.context.Plans.Remove(existingPlan);
            this.context.SaveChanges();
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
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
                this.context.Dispose();
            }
        }

        this.disposed = true;
    }
}