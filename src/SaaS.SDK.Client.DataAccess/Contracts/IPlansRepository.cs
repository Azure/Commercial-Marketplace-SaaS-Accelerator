using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    public interface IPlansRepository : IDisposable, IBaseRepository<Plans>
    {
        Plans GetPlanDetailByPlanId(string planId);
    }
}
