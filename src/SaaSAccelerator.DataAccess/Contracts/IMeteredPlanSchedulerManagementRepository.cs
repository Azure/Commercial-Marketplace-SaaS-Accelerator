using System;
using Microsoft.Marketplace.SaaSAccelerator.DataAccess.Entities;

namespace Microsoft.Marketplace.SaaSAccelerator.DataAccess.Contracts
{
    /// <summary>
    /// Scheduler Manager for Metered Plan.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.IBaseRepository{Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities.MeteredPlanSchedulerManagement}" />
    public interface IMeteredPlanSchedulerManagementRepository : IDisposable, IBaseRepository<MeteredPlanSchedulerManagement>
    {

    }
}
