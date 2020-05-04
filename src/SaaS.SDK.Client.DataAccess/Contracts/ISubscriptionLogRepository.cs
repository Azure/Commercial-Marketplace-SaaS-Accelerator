using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    /// <summary>
    /// ISubscriptionLogRepository Interface
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.DataAccess.Contracts.IBaseRepository{Microsoft.Marketplace.SaasKit.DataAccess.Entities.SubscriptionAuditLogs}" />
    public interface ISubscriptionLogRepository : IBaseRepository<SubscriptionAuditLogs>
    {
        /// <summary>
        /// Gets the subscription by subscription identifier.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns></returns>
        IEnumerable<SubscriptionAuditLogs> GetSubscriptionBySubscriptionId(Guid subscriptionId);
        void AddWebJobSubscriptionStatus(Guid subscriptionID, Guid? ArmtempalteId, string deploymentStatus, string errorDescription, string subscriptionStatus);
    }
}
