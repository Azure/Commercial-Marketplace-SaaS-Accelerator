using System;
using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.DataAccess.DataModel;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Contracts;

/// <summary>
/// Repository to access plans.
/// </summary>
/// <seealso cref="System.IDisposable" />
/// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.IBaseRepository{Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities.Plans}" />
public interface IPlansRepository : IDisposable, IBaseRepository<Plans>
{
    /// <summary>
    /// Gets the by identifier.
    /// </summary>
    /// <param name="planId">The plan identifier.</param>
    /// <returns>Plan detail for the friendly identifier.</returns>
    Plans GetById(string planId);

    /// <summary>
    /// Gets the by internal reference.
    /// </summary>
    /// <param name="planGuId">The plan gu identifier.</param>
    /// <returns>Plan detail for the internal reference (GUID). </returns>
    Plans GetByInternalReference(Guid planGuId);

    /// <summary>
    /// Gets the plan attributes.
    /// </summary>
    /// <param name="planGuId">The plan gu identifier.</param>
    /// <param name="offerId">The offer identifier.</param>
    /// <returns> Plan Attributes Model.</returns>
    IEnumerable<PlanAttributesModel> GetPlanAttributes(Guid planGuId, Guid offerId);

    /// <summary>
    /// Gets the events by plan.
    /// </summary>
    /// <param name="planGuId">The plan gu identifier.</param>
    /// <param name="offerId">The offer identifier.</param>
    /// <returns>List of event detail by plan in an offer.</returns>
    IEnumerable<PlanEventsModel> GetEventsByPlan(Guid planGuId, Guid offerId);

    /// <summary>
    /// Gets the plans by user.
    /// </summary>
    /// <returns> Plans.</returns>
    IEnumerable<Plans> GetPlansByUser();

    /// <summary>
    /// Adds the plan attributes.
    /// </summary>
    /// <param name="attributes">The attributes.</param>
    /// <returns>ID of the newly created attribute.</returns>
    int? SavePlanAttributes(PlanAttributeMapping attributes);

    /// <summary>
    /// Add the plan events.
    /// </summary>
    /// <param name="events">The events.</param>
    /// <returns>ID of the newly created event under the plan.</returns>
    int? AddPlanEvents(PlanEventsMapping events);

    /// <summary>
    /// Gets the plans by offer identifier.
    /// </summary>
    /// <param name="offerId">The offer identifier.</param>
    /// <returns> Plans.</returns>
    List<Plans> GetPlansByOfferId(Guid offerId);

    /// <summary>
    /// Gets the plan attribute on offer attribute identifier.
    /// </summary>
    /// <param name="offerAttributeId">The offer attribute identifier.</param>
    /// <param name="planGuId">The plan gu identifier.</param>
    /// <returns> Plan Attribute Mapping.</returns>
    PlanAttributeMapping GetPlanAttributeOnOfferAttributeId(int offerAttributeId, Guid planGuId);

    /// <summary>
    /// Adds the specified plan details with information available from GetSubscription API
    /// This is more relevent for an Unsubscribed subscription where the ListAvailablePlans API wont work
    /// </summary>
    /// <param name="planDetails">The plan details.</param>
    /// <returns> Plan Id.</returns>
    int Add(Plans planDetails);
}