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
    /// Subscription Log Repository.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.ISubscriptionLogRepository" />
    public class SubscriptionLogRepository : ISubscriptionLogRepository
    {
        /// <summary>
        /// The context.
        /// </summary>
        private readonly SaasKitContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionLogRepository"/> class.
        /// </summary>
        /// <param name="context">The this.context.</param>
        public SubscriptionLogRepository(SaasKitContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Adds the specified subscription logs.
        /// </summary>
        /// <param name="subscriptionLogs">The subscription logs.</param>
        /// <returns> log Id.</returns>
        public int Save(SubscriptionAuditLogs subscriptionLogs)
        {
            this.context.SubscriptionAuditLogs.Add(subscriptionLogs);
            this.context.SaveChanges();
            return subscriptionLogs.Id;
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns> List of log.</returns>
        public IEnumerable<SubscriptionAuditLogs> Get()
        {
            return this.context.SubscriptionAuditLogs.Include(s => s.Subscription);
        }

        /// <summary>
        /// Gets the subscription by subscription identifier.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns> Subscription Audit Logs.</returns>
        public IEnumerable<SubscriptionAuditLogs> GetSubscriptionBySubscriptionId(Guid subscriptionId)
        {
            return this.context.SubscriptionAuditLogs.Include(s => s.Subscription).Where(s => s.Subscription.AmpsubscriptionId == subscriptionId);
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns> Subscription Audit Logs.</returns>
        public SubscriptionAuditLogs Get(int id)
        {
            return this.context.SubscriptionAuditLogs.Where(s => s.Id == id).FirstOrDefault();
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
        /// Logs the status during provisioning.
        /// </summary>
        /// <param name="subscriptionID">The subscription identifier.</param>
        /// <param name="armtemplateId">The armtempalte identifier.</param>
        /// <param name="deploymentStatus">The deployment status.</param>
        /// <param name="errorDescription">The error description.</param>
        /// <param name="subscriptionStatus">The subscription status.</param>
        public void LogStatusDuringProvisioning(Guid subscriptionID, Guid? armtemplateId, string deploymentStatus, string errorDescription, string subscriptionStatus)
        {
            var subscription = this.context.Subscriptions.Where(s => s.AmpsubscriptionId == subscriptionID).FirstOrDefault();

            // var existingWebJobStatus = this.context.WebJobSubscriptionStatus.Where(s => s.SubscriptionId == subscriptionID).FirstOrDefault();
            // if (existingWebJobStatus == null)
            // {
            WebJobSubscriptionStatus status = new WebJobSubscriptionStatus()
            {
                SubscriptionId = subscriptionID,
                ArmtemplateId = armtemplateId,
                SubscriptionStatus = subscriptionStatus,
                DeploymentStatus = deploymentStatus,
                Description = errorDescription,
                InsertDate = DateTime.Now,
            };
            this.context.WebJobSubscriptionStatus.Add(status);
            this.context.SaveChanges();

            // }
            // else
            // {
            //    existingWebJobStatus.SubscriptionId = subscriptionID;
            //    if (armtemplateId != default)
            //    {
            //        existingWebJobStatus.ArmtemplateId = armtemplateId;
            //    }
            //    existingWebJobStatus.SubscriptionStatus = subscription.SubscriptionStatus;
            //    existingWebJobStatus.DeploymentStatus = deploymentStatus;
            //    existingWebJobStatus.Description = errorDescription;
            //    this.context.WebJobSubscriptionStatus.Update(existingWebJobStatus);
            //    this.context.SaveChanges();
            // }
        }
    }
}
