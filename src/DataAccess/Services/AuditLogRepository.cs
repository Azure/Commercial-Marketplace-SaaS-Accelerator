namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

    /// <summary>
    /// Audit Log Repository.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.IAuditLogRepository" />
    public class AuditLogRepository : IAuditLogRepository
    {
        /// <summary>
        /// The context.
        /// </summary>
        private readonly SaasKitContext context;

        /// <summary>
        /// The disposed.
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditLogRepository"/> class.
        /// </summary>
        /// <param name="context">The this.context.</param>
        public AuditLogRepository(SaasKitContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// Subscription Audit log by id.
        /// </returns>
        public SubscriptionAuditLogs Get(int id)
        {
            return this.context.SubscriptionAuditLogs.Where(s => s.Id == id).FirstOrDefault();
        }

        /// <summary>
        /// Adds the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns> entity id.</returns>
        public int Save(SubscriptionAuditLogs entity)
        {
            this.context.SubscriptionAuditLogs.Add(entity);
            this.context.SaveChanges();
            return entity.Id;
        }

        /// <summary>
        /// Removes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void Remove(SubscriptionAuditLogs entity)
        {
            this.context.SubscriptionAuditLogs.Remove(entity);
            this.context.SaveChanges();
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
        /// Gets this instance.
        /// </summary>
        /// <returns> List of subscription audit logs.</returns>
        public IEnumerable<SubscriptionAuditLogs> Get()
        {
            return this.context.SubscriptionAuditLogs;
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
