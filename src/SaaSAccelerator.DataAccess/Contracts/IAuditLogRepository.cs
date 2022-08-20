using System;
using Microsoft.Marketplace.SaaSAccelerator.DataAccess.Entities;

namespace Microsoft.Marketplace.SaaSAccelerator.DataAccess.Contracts
{
    /// <summary>
    /// Audit Log Repository.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.IBaseRepository{Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities.SubscriptionAuditLogs}" />
    public interface IAuditLogRepository : IDisposable, IBaseRepository<SubscriptionAuditLogs>
    {
    }
}
