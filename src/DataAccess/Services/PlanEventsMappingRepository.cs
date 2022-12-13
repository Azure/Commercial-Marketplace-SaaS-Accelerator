using System;
using System.Linq;
using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Services;

/// <summary>
/// Repository to access Plan events.
/// </summary>
/// <seealso cref="IPlanEventsMappingRepository" />
public class PlanEventsMappingRepository : IPlanEventsMappingRepository
{
    /// <summary>
    /// The this.context.
    /// </summary>
    private readonly SaasKitContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlanEventsMappingRepository"/> class.
    /// </summary>
    /// <param name="context">The this.context.</param>
    public PlanEventsMappingRepository(SaasKitContext context)
    {
        this.context = context;
    }

    /// <summary>
    /// Gets the plan event.
    /// </summary>
    /// <param name="planID">The plan identifier.</param>
    /// <param name="eventID">The event identifier.</param>
    /// <returns>
    /// Event detail by plan and event id.
    /// </returns>
    public PlanEventsMapping GetPlanEvent(Guid planID, int eventID)
    {
        var results = this.context.PlanEventsMapping.Where(s => s.PlanId == planID && s.EventId == eventID);
        if (results == null || results.ToList().Count() == 0)
        {
            return null;
        }
        else
        {
            return this.context.PlanEventsMapping.Where(s => s.PlanId == planID && s.EventId == eventID).FirstOrDefault();
        }
    }
}