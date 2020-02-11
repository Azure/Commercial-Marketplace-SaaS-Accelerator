namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Subscription Usage Logs Repository Interface
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.IBaseRepository{Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities.MeteredAuditLogs}" />
    public interface ISubscriptionUsageLogsRepository : IDisposable, IBaseRepository<MeteredAuditLogs>
    {
        /// <summary>
        /// Gets the metered audit logs by subscription identifier.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns></returns>
        List<MeteredAuditLogs> GetMeteredAuditLogsBySubscriptionId(int subscriptionId);
    }
}
