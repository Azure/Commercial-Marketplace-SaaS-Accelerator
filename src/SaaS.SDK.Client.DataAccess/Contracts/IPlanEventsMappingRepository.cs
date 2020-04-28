using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    public interface IPlanEventsMappingRepository
    {
        PlanEventsMapping GetPlanEventsMappingEmails(Guid PlanID, int eventID);
    }
}
