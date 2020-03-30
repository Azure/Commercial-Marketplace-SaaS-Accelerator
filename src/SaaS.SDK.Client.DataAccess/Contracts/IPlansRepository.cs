using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System;
using System.Collections.Generic;


namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    public interface IPlansRepository : IDisposable, IBaseRepository<Plans>
    {
        Plans GetPlanDetailByPlanId(string planId);

        IEnumerable<Plans> GetPlansByUser();
    }
}
