using Microsoft.EntityFrameworkCore;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    public class SubscriptionLogRepository : ISubscriptionLogRepository
    {
        /// <summary>
        /// The context
        /// </summary>
        public SaasKitContext Context = new SaasKitContext();

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionLogRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public SubscriptionLogRepository(SaasKitContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Adds the specified subscription logs.
        /// </summary>
        /// <param name="subscriptionLogs">The subscription logs.</param>
        /// <returns></returns>
        public int Add(SubscriptionAuditLogs subscriptionLogs)
        {
            try
            {
                Context.SubscriptionAuditLogs.Add(subscriptionLogs);
                Context.SaveChanges();
                return subscriptionLogs.Id;
            }
            catch (Exception) { }
            return 0;
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SubscriptionAuditLogs> Get()
        {
            return Context.SubscriptionAuditLogs.Include(s => s.Subscription);
        }

        /// <summary>
        /// Gets the subscription by subscription identifier.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns></returns>
        public IEnumerable<SubscriptionAuditLogs> GetSubscriptionBySubscriptionId(Guid subscriptionId)
        {
            return Context.SubscriptionAuditLogs.Include(s => s.Subscription).Where(s => s.Subscription.AmpsubscriptionId == subscriptionId);
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public SubscriptionAuditLogs Get(int id)
        {
            return Context.SubscriptionAuditLogs.Where(s => s.Id == id).FirstOrDefault();
        }

        /// <summary>
        /// Removes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void Remove(SubscriptionAuditLogs entity)
        {
            Context.SubscriptionAuditLogs.Remove(entity);
            Context.SaveChanges();
        }


        public void AddWebJobSubscriptionStatus(Guid subscriptionID, Guid? ArmtempalteId, string deploymentStatus, string errorDescription, string subscriptionStatus)
        {
            var subscription = Context.Subscriptions.Where(s => s.AmpsubscriptionId == subscriptionID).FirstOrDefault();
            var existingWebJobStatus = Context.WebJobSubscriptionStatus.Where(s => s.SubscriptionId == subscriptionID).FirstOrDefault();
            if (existingWebJobStatus == null)
            {
                WebJobSubscriptionStatus status = new WebJobSubscriptionStatus()
                {
                    SubscriptionId = subscriptionID,
                    ArmtemplateId = ArmtempalteId,
                    SubscriptionStatus = subscriptionStatus,
                    DeploymentStatus = deploymentStatus,
                    Description = errorDescription,
                    InsertDate = DateTime.Now
                };
                Context.WebJobSubscriptionStatus.Add(status);
                Context.SaveChanges();
            }
            else
            {
                existingWebJobStatus.SubscriptionId = subscriptionID;
                if (ArmtempalteId != default)
                {
                    existingWebJobStatus.ArmtemplateId = ArmtempalteId;
                }
                existingWebJobStatus.SubscriptionStatus = subscription.SubscriptionStatus;
                existingWebJobStatus.DeploymentStatus = deploymentStatus;
                existingWebJobStatus.Description = errorDescription;
                Context.WebJobSubscriptionStatus.Update(existingWebJobStatus);
                Context.SaveChanges();

            }

        }

    }
}
