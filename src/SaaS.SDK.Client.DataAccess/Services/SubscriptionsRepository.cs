using Commons.Collections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.DataModel;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using Newtonsoft.Json;
using NVelocity;
using NVelocity.App;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    public class SubscriptionsRepository : ISubscriptionsRepository
    {
        /// <summary>
        /// The disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// The context
        /// </summary>
        private readonly SaasKitContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionsRepository" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public SubscriptionsRepository(SaasKitContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Adds the specified subscription details.
        /// </summary>
        /// <param name="subscriptionDetails">The subscription details.</param>
        /// <returns></returns>
        public int Save(Subscriptions subscriptionDetails)
        {

            var existingSubscriptions = context.Subscriptions.Where(s => s.AmpsubscriptionId == subscriptionDetails.AmpsubscriptionId).FirstOrDefault();
            if (existingSubscriptions != null)
            {
                existingSubscriptions.SubscriptionStatus = subscriptionDetails.SubscriptionStatus;
                existingSubscriptions.AmpplanId = subscriptionDetails.AmpplanId;
                existingSubscriptions.Ampquantity = subscriptionDetails.Ampquantity;
                context.Subscriptions.Update(existingSubscriptions);
                context.SaveChanges();
                return existingSubscriptions.Id;
            }
            else
            {
                context.Subscriptions.Add(subscriptionDetails);
            }
            context.SaveChanges();

            return subscriptionDetails.Id;
        }

        /// <summary>
        /// Adds the specified subscription details.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="subscriptionStatus">The subscription status.</param>
        /// <param name="isActive">if set to <c>true</c> [is active].</param>
        public void UpdateStatusForSubscription(Guid subscriptionId, string subscriptionStatus, bool isActive)
        {
            var existingSubscription = context.Subscriptions.Where(s => s.AmpsubscriptionId == subscriptionId).FirstOrDefault();
            if (existingSubscription != null)
            {
                existingSubscription.IsActive = isActive;
                existingSubscription.SubscriptionStatus = subscriptionStatus;
                context.Subscriptions.Update(existingSubscription);
            }
            context.SaveChanges();
        }

        /// <summary>
        /// Updates the plan for subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="planId">The plan identifier.</param>
        public void UpdatePlanForSubscription(Guid subscriptionId, string planId)
        {
            var existingSubscription = context.Subscriptions.Where(s => s.AmpsubscriptionId == subscriptionId).FirstOrDefault();
            if (existingSubscription != null)
            {
                existingSubscription.AmpplanId = planId;
                context.Subscriptions.Update(existingSubscription);
            }
            context.SaveChanges();
        }

        /// <summary>
        /// Updates the Quantity for subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="quantity">The Quantity.</param>
        public void UpdateQuantityForSubscription(Guid subscriptionId, int quantity)
        {
            var existingSubscription = context.Subscriptions.Where(s => s.AmpsubscriptionId == subscriptionId).FirstOrDefault();
            if (existingSubscription != null)
            {
                existingSubscription.Ampquantity = quantity;
                context.Subscriptions.Update(existingSubscription);
            }
            context.SaveChanges();
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Subscriptions> Get()
        {
            return context.Subscriptions.Include(s => s.User).OrderByDescending(s => s.CreateDate);
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Subscriptions Get(int id)
        {
            return context.Subscriptions.Where(s => s.Id == id).FirstOrDefault();
        }

        /// <summary>
        /// Gets the subscriptions by email address.
        /// </summary>
        /// <param name="partnerEmailAddress">The partner email address.</param>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="isIncludeDeactvated">if set to <c>true</c> [is include deactvated].</param>
        /// <returns></returns>
        public IEnumerable<Subscriptions> GetSubscriptionsByEmailAddress(string partnerEmailAddress, Guid subscriptionId, bool isIncludeDeactvated = false)
        {
            if (subscriptionId != default)
            {
                return context.Subscriptions.Include(s => s.User).Where(s => s.User != null && s.User.EmailAddress == partnerEmailAddress && s.AmpsubscriptionId == subscriptionId);
            }

            else
            {
                return context.Subscriptions.Include(s => s.User).Where(s => s.User != null && s.User.EmailAddress == partnerEmailAddress);
            }
        }

        /// <summary>
        /// Gets the subscriptions by ScheduleId
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="isIncludeDeactvated">if set to <c>true</c> [is include deactvated].</param>
        /// <returns></returns>
        public Subscriptions GetById(Guid subscriptionId, bool isIncludeDeactvated = false)
        {
            if (subscriptionId != default)
            {
                //if (!isIncludeDeactvated)
                //    return context.Subscriptions.Include(s => s.User).Where(s => s.AmpsubscriptionId == subscriptionId && s.IsActive == true).FirstOrDefault();
                //else
                    return context.Subscriptions.Include(s => s.User).Where(s => s.AmpsubscriptionId == subscriptionId).FirstOrDefault();
            }
            return null;
        }

        /// <summary>
        /// Gets the subscriptions by ScheduleId
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="isIncludeDeactvated">if set to <c>true</c> [is include deactvated].</param>
        /// <returns></returns>
        public List<SubscriptionParametersOutput> GetSubscriptionsParametersById(Guid subscriptionId, Guid planId)
        {
            if (subscriptionId != default)
            {
                var subscriptionParameters = context.SubscriptionParametersOutput.FromSqlRaw("dbo.spGetSubscriptionParameters {0},{1}", subscriptionId, planId).ToList();
                return subscriptionParameters.ToList();
            }
            return new List<SubscriptionParametersOutput>();
        }

        /// <summary>
        /// Saves the deployment credentials.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="keyVaultSecret">The key vault secret.</param>
        /// <param name="userId">The user identifier.</param>
        public void SaveDeploymentCredentials(Guid subscriptionId, string keyVaultSecret, int userId)
        {
            var existingKey = context.SubscriptionKeyVault.Where(s => s.SubscriptionId == subscriptionId).FirstOrDefault();
            if (existingKey != null)
            {
                existingKey.SecureId = keyVaultSecret;
                existingKey.SubscriptionId = subscriptionId;
                context.SubscriptionKeyVault.Update(existingKey);
            }
            else
            {
                SubscriptionKeyVault SubscriptionKeyVault = new SubscriptionKeyVault()
                {
                    SubscriptionId = subscriptionId,
                    SecureId = keyVaultSecret,
                    CreateDate = DateTime.Now,
                    UserId = userId
                };

                context.SubscriptionKeyVault.Add(SubscriptionKeyVault);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Updates the plan for subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="planId">The plan identifier.</param>
        public void AddSubscriptionParameters(SubscriptionParametersOutput subscriptionParametersOutput)
        {

            var existingSubscriptionparameter = context.SubscriptionAttributeValues.Where(s => s.Id == subscriptionParametersOutput.Id).FirstOrDefault();
            if (existingSubscriptionparameter != null)
            {
                existingSubscriptionparameter.OfferId = subscriptionParametersOutput.OfferId;
                context.SubscriptionAttributeValues.Update(existingSubscriptionparameter);
                context.SaveChanges();
            }
            else
            {
                SubscriptionAttributeValues newAttributeValue = new SubscriptionAttributeValues();
                newAttributeValue.OfferId = subscriptionParametersOutput.OfferId;
                newAttributeValue.PlanAttributeId = subscriptionParametersOutput.PlanAttributeId;
                newAttributeValue.Value = subscriptionParametersOutput.Value;
                newAttributeValue.SubscriptionId = subscriptionParametersOutput.SubscriptionId;
                newAttributeValue.CreateDate = subscriptionParametersOutput.CreateDate;
                newAttributeValue.UserId = subscriptionParametersOutput.UserId;
                newAttributeValue.PlanId = subscriptionParametersOutput.PlanId;
                context.SubscriptionAttributeValues.Add(newAttributeValue);
                context.SaveChanges();
            }
        }

       
        /// <summary>
        /// Removes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void Remove(Subscriptions entity)
        {
            context.Subscriptions.Remove(entity);
            context.SaveChanges();
        }

        /// <summary>
        /// Gets the deployment configuration.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns></returns>
        public SubscriptionKeyVault GetDeploymentConfig(Guid subscriptionId)
        {
            return context.SubscriptionKeyVault.Where(s => s.SubscriptionId == subscriptionId).FirstOrDefault();
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
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }
    }
}
