namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

    /// <summary>
    /// Subscription Usage Log.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.ISubscriptionUsageLogsRepository" />
    public class SubscriptionUsageLogsRepository : ISubscriptionUsageLogsRepository
    {
        /// <summary>
        /// The this.context.
        /// </summary>
        private readonly SaasKitContext context;

        /// <summary>
        /// The disposed.
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionUsageLogsRepository"/> class.
        /// </summary>
        /// <param name="context">The this.context.</param>
        public SubscriptionUsageLogsRepository(SaasKitContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Adds the specified metered audit logs.
        /// </summary>
        /// <param name="meteredAuditLogs">The metered audit logs.</param>
        /// <returns> Audit log id.</returns>
        public int Save(MeteredAuditLogs meteredAuditLogs)
        {
            this.context.MeteredAuditLogs.Add(meteredAuditLogs);
            this.context.SaveChanges();
            return meteredAuditLogs.Id;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets all the records.
        /// </summary>
        /// <returns> Metered Audit Logs.</returns>
        public IEnumerable<MeteredAuditLogs> Get()
        {
            return this.context.MeteredAuditLogs.Include(s => s.Subscription);
        }

        /// <summary>
        /// Gets the record for the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// Entity for the given identifier.
        /// </returns>
        public MeteredAuditLogs Get(int id)
        {
            return this.context.MeteredAuditLogs.Include(s => s.Subscription).Where(s => s.Id == id).FirstOrDefault();
        }

        /// <summary>
        /// Gets the metered audit logs by subscription identifier.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns> Metered Audit Logs.</returns>
        public List<MeteredAuditLogs> GetMeteredAuditLogsBySubscriptionId(int subscriptionId)
        {
            return this.context.MeteredAuditLogs.Include(s => s.Subscription).Where(s => s.Subscription.Id == subscriptionId).OrderByDescending(s => s.CreatedDate).ToList();
        }

        /// <summary>
        /// Removes the specified metered audit logs.
        /// </summary>
        /// <param name="meteredAuditLogs">The metered audit logs.</param>
        public void Remove(MeteredAuditLogs meteredAuditLogs)
        {
            this.context.MeteredAuditLogs.Remove(meteredAuditLogs);
            this.context.SaveChanges();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.context.Dispose();
                }
            }

            this.disposed = true;
        }
    }
}