namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using System;
    using System.Linq;

    /// <summary>
    /// Repository to access Plan events.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.IPlanEventsMappingRepository" />
    public class PlanEventsMappingRepository : IPlanEventsMappingRepository
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly SaasKitContext context;

        public PlanEventsMappingRepository(SaasKitContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Gets the plan event.
        /// </summary>
        /// <param name="PlanID">The plan identifier.</param>
        /// <param name="eventID">The event identifier.</param>
        /// <returns>
        /// Event detail by plan and event id
        /// </returns>
        public PlanEventsMapping GetPlanEvent(Guid PlanID, int eventID)
        {
            var results = context.PlanEventsMapping.Where(s => s.PlanId == PlanID && s.EventId == eventID);
            if (results.Count() == 0)
                return null;
            else
                return context.PlanEventsMapping.Where(s => s.PlanId == PlanID && s.EventId == eventID).FirstOrDefault();
        }
    }
}
