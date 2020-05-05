namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using System;

    /// <summary>
    /// Repository to access events related to plan
    /// </summary>
    public interface IPlanEventsMappingRepository
    {
        /// <summary>
        /// Gets the plan event.
        /// </summary>
        /// <param name="PlanID">The plan identifier.</param>
        /// <param name="eventID">The event identifier.</param>
        /// <returns>Event detail by plan and event id</returns>
        PlanEventsMapping GetPlanEvent(Guid PlanID, int eventID);
    }
}
