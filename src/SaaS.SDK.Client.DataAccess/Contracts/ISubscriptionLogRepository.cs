namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using System;
    using System.Collections.Generic;
    using System.Text;

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

        /// <summary>
        /// Logs the status during provisioning.
        /// </summary>
        /// <param name="subscriptionID">The subscription identifier.</param>
        /// <param name="ArmtempalteId">The armtempalte identifier.</param>
        /// <param name="deploymentStatus">The deployment status.</param>
        /// <param name="errorDescription">The error description.</param>
        /// <param name="subscriptionStatus">The subscription status.</param>
        void LogStatusDuringProvisioning(Guid subscriptionID, Guid? ArmtempalteId, string deploymentStatus, string errorDescription, string subscriptionStatus);

    }
}
