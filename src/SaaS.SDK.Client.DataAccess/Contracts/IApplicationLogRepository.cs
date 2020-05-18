namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    using System;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

    /// <summary>
    /// Repository to access application log entries.
    /// </summary>
    public interface IApplicationLogRepository
    {
        /// <summary>
        /// Adds the log.
        /// </summary>
        /// <param name="logDetail">The log detail.</param>
        void AddLog(ApplicationLog logDetail);
    }
}
