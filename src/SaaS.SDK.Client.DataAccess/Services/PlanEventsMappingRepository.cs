using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    public class PlanEventsMappingRepository : IPlanEventsMappingRepository
    {
        private readonly SaasKitContext Context;

        public PlanEventsMappingRepository(SaasKitContext context)
        {
            Context = context;
        }

        public PlanEventsMapping GetPlanEventsMappingEmails(Guid PlanID, int eventID)
        {
            var results = Context.PlanEventsMapping.Where(s => s.PlanId == PlanID && s.EventId == eventID);
            if (results.Count() == 0)
                return null;
            else
                return Context.PlanEventsMapping.Where(s => s.PlanId == PlanID && s.EventId == eventID).FirstOrDefault();
        }
    }
}
