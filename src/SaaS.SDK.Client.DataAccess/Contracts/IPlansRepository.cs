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

        IEnumerable<PlanAttributesModel> GetPlanAttributesByPlanGuId(Guid planGuId, Guid OfferId);

        IEnumerable<PlanEventsModel> GetPlanEventsByPlanGuId(Guid planGuId, Guid OfferId);
        IEnumerable<Plans> GetPlansByUser();

        int? AddPlanAttributes(PlanAttributeMapping attributes);
        int? AddPlanEvents(PlanEventsMapping events);
        List<Plans> GetPlanDetailByOfferId(Guid offerId);

        //int? AddAllPlanAttributesOfOffer(Plans plan, List<DeploymentAttributes> deploymentAttributes);

        PlanAttributeMapping GetPlanAttributeOnOfferAttributeId(int offerAttributeId, Guid planGuId);

    }
}
