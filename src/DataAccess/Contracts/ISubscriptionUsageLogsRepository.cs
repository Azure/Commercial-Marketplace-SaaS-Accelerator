using System;
using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Contracts;

/// <summary>
/// Subscription Usage Logs Repository Interface.
/// </summary>
/// <seealso cref="System.IDisposable" />
/// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.IBaseRepository{Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities.MeteredAuditLogs}" />
public interface ISubscriptionUsageLogsRepository : IDisposable, IBaseRepository<MeteredAuditLogs>
{
    /// <summary>
    /// Gets the metered audit logs by subscription identifier.
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <param name="format">Specify to format the result properly.</param>
    /// <returns> Metered Audit Logs.</returns>
    List<MeteredAuditLogs> GetMeteredAuditLogsBySubscriptionId(int subscriptionId, bool format = false);
}