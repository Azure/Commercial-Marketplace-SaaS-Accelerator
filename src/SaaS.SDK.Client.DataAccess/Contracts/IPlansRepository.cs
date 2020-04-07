using Microsoft.Marketplace.SaasKit.Client.DataAccess.DataModel;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System;
using System.Collections.Generic;


namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    public interface IPlansRepository : IDisposable, IBaseRepository<Plans>
    {
        Plans GetPlanDetailByPlanId(string planId);

        Plans GetPlanDetailByPlanGuId(Guid planGuId);

        List<PlanAttributesModel> GetPlanAttributesByPlanGuId(Guid planGuId, Guid OfferId);

        IEnumerable<PlanEventsMapping> GetPlanEventsByPlanGuId(Guid planGuId, Guid OfferId);
        IEnumerable<Plans> GetPlansByUser();
    }
}
