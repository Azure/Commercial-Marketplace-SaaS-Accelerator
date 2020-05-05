namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class AuditLogRepository : IAuditLogRepository
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly SaasKitContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditLogRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public AuditLogRepository(SaasKitContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// The disposed
        /// </summary>
        private bool disposed = false;

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
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>List of subscription audit logs</returns>        
        public IEnumerable<SubscriptionAuditLogs> Get()
        {
            return context.SubscriptionAuditLogs;
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Subscription Audit log by id</returns>        
        public SubscriptionAuditLogs Get(int id)
        {
            return context.SubscriptionAuditLogs.Where(s => s.Id == id).FirstOrDefault();
        }

        /// <summary>
        /// Adds the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public int Add(SubscriptionAuditLogs entity)
        {
            context.SubscriptionAuditLogs.Add(entity);
            context.SaveChanges();
            return entity.Id;
        }

        /// <summary>
        /// Removes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void Remove(SubscriptionAuditLogs entity)
        {
            context.SubscriptionAuditLogs.Remove(entity);
            context.SaveChanges();
        }
    }
}
