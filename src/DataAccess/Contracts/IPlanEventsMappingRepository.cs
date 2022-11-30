using System;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Contracts;

/// <summary>
/// Repository to access events related to plan.
/// </summary>
public interface IPlanEventsMappingRepository
{
    /// <summary>
    /// Gets the plan event.
    /// </summary>
    /// <param name="planID">The plan identifier.</param>
    /// <param name="eventID">The event identifier.</param>
    /// <returns>
    /// Event detail by plan and event id.
    /// </returns>
    PlanEventsMapping GetPlanEvent(Guid planID, int eventID);
}