using System;
using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Contracts;

/// <summary>
/// ISubscriptionLogRepository Interface.
/// </summary>
/// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.IBaseRepository{Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities.SubscriptionAuditLogs}" />
/// <seealso cref="Microsoft.Marketplace.SaasKit.DataAccess.Contracts.IBaseRepository{Microsoft.Marketplace.SaasKit.DataAccess.Entities.SubscriptionAuditLogs}" />
public interface ISubscriptionLogRepository : IBaseRepository<SubscriptionAuditLogs>
{
    /// <summary>
    /// Gets the subscription by subscription identifier.
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <returns> Subscription Audit Logs.</returns>
    IEnumerable<SubscriptionAuditLogs> GetSubscriptionBySubscriptionId(Guid subscriptionId);

    /// <summary>
    /// Logs the status during provisioning.
    /// </summary>
    /// <param name="subscriptionID">The subscription identifier.</param>
    /// <param name="errorDescription">The error description.</param>
    /// <param name="subscriptionStatus">The subscription status.</param>
    void LogStatusDuringProvisioning(Guid subscriptionID, string errorDescription, string subscriptionStatus);
}