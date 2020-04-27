using Microsoft.EntityFrameworkCore;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.DataModel;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System;
using System.Collections.Generic;
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
        private readonly SaasKitContext Context;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionsRepository" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public SubscriptionsRepository(SaasKitContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Adds the specified subscription details.
        /// </summary>
        /// <param name="subscriptionDetails">The subscription details.</param>
        /// <returns></returns>
        public int Add(Subscriptions subscriptionDetails)
        {
            try
            {
                var existingSubscriptions = Context.Subscriptions.Where(s => s.AmpsubscriptionId == subscriptionDetails.AmpsubscriptionId).FirstOrDefault();
                if (existingSubscriptions != null)
                {
                    existingSubscriptions.SubscriptionStatus = subscriptionDetails.SubscriptionStatus;
                    existingSubscriptions.AmpplanId = subscriptionDetails.AmpplanId;
                    existingSubscriptions.Ampquantity = subscriptionDetails.Ampquantity;
                    Context.Subscriptions.Update(existingSubscriptions);
                    Context.SaveChanges();
                    return existingSubscriptions.Id;
                }
                else
                    Context.Subscriptions.Add(subscriptionDetails);
                Context.SaveChanges();
            }
            catch (Exception) { }
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
            try
            {
                var existingSubscription = Context.Subscriptions.Where(s => s.AmpsubscriptionId == subscriptionId).FirstOrDefault();
                if (existingSubscription != null)
                {
                    existingSubscription.IsActive = isActive;
                    existingSubscription.SubscriptionStatus = subscriptionStatus;
                    Context.Subscriptions.Update(existingSubscription);
                }
                Context.SaveChanges();
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Updates the plan for subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="planId">The plan identifier.</param>
        public void UpdatePlanForSubscription(Guid subscriptionId, string planId)
        {
            try
            {
                var existingSubscription = Context.Subscriptions.Where(s => s.AmpsubscriptionId == subscriptionId).FirstOrDefault();
                if (existingSubscription != null)
                {
                    existingSubscription.AmpplanId = planId;
                    Context.Subscriptions.Update(existingSubscription);
                }
                Context.SaveChanges();
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Updates the Quantity for subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="quantity">The Quantity.</param>
        public void UpdateQuantityForSubscription(Guid subscriptionId, int quantity)
        {
            try
            {
                var existingSubscription = Context.Subscriptions.Where(s => s.AmpsubscriptionId == subscriptionId).FirstOrDefault();
                if (existingSubscription != null)
                {
                    existingSubscription.Ampquantity = quantity;
                    Context.Subscriptions.Update(existingSubscription);
                }
                Context.SaveChanges();
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Subscriptions> Get()
        {
            return Context.Subscriptions.Include(s => s.User).OrderByDescending(s => s.CreateDate);
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Subscriptions Get(int id)
        {
            return Context.Subscriptions.Where(s => s.Id == id).FirstOrDefault();
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
            if (subscriptionId == default)
            {
                if (!isIncludeDeactvated)
                    return Context.Subscriptions.Include(s => s.User).Where(s => s.User != null && s.User.EmailAddress == partnerEmailAddress && s.IsActive == true);
                else
                    return Context.Subscriptions.Include(s => s.User).Where(s => s.User != null && s.User.EmailAddress == partnerEmailAddress);
            }
            else
            {
                if (!isIncludeDeactvated)
                    return Context.Subscriptions.Include(s => s.User).Where(s => s.User != null && s.User.EmailAddress == partnerEmailAddress && s.AmpsubscriptionId == subscriptionId && s.IsActive == true);
                else
                    return Context.Subscriptions.Include(s => s.User).Where(s => s.User != null && s.User.EmailAddress == partnerEmailAddress && s.AmpsubscriptionId == subscriptionId);
            }
        }

        //public Subscriptions GetSubscriptionsBySubscriptionId(Guid subscriptionId, bool isIncludeDeactvated = false)
        //{
        //    if (subscriptionId != default)
        //    {
        //        if (!isIncludeDeactvated)
        //            return Context.Subscriptions.Include(s => s.User).Where(s => s.AmpsubscriptionId == subscriptionId && s.IsActive == true).FirstOrDefault();
        //        else
        //            return Context.Subscriptions.Include(s => s.User).Where(s => s.AmpsubscriptionId == subscriptionId).FirstOrDefault();
        //    }
        //    return new Subscriptions();
        //}

        /// <summary>
        /// Gets the subscriptions by ScheduleId
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="isIncludeDeactvated">if set to <c>true</c> [is include deactvated].</param>
        /// <returns></returns>
        public Subscriptions GetSubscriptionsByScheduleId(Guid subscriptionId, bool isIncludeDeactvated = false)
        {
            if (subscriptionId != default)
            {
                if (!isIncludeDeactvated)
                    return Context.Subscriptions.Include(s => s.User).Where(s => s.AmpsubscriptionId == subscriptionId && s.IsActive == true).FirstOrDefault();
                else
                    return Context.Subscriptions.Include(s => s.User).Where(s => s.AmpsubscriptionId == subscriptionId).FirstOrDefault();
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
                var subscriptionParameters = Context.SubscriptionParametersOutput.FromSqlRaw("dbo.spGetSubscriptionParameters {0},{1}", subscriptionId, planId).ToList();
                return subscriptionParameters.ToList();
            }
            return new List<SubscriptionParametersOutput>();
        }

        public void AddSubscriptionKeyValutSecret(Guid subscriptionId, string keyVaultSecret, int userId)
        {
            var existingKey = Context.SubscriptionKeyValut.Where(s => s.SubscriptionId == subscriptionId).FirstOrDefault();
            if (existingKey != null)
            {
                existingKey.SecuteId = keyVaultSecret;
                existingKey.SubscriptionId = subscriptionId;
                Context.SubscriptionKeyValut.Update(existingKey);
            }
            else
            {
                SubscriptionKeyValut subscriptionKeyValut = new SubscriptionKeyValut()
                {
                    SubscriptionId = subscriptionId,
                    SecuteId = keyVaultSecret,
                    CreateDate = DateTime.Now,
                    UserId = userId
                };

                Context.SubscriptionKeyValut.Add(subscriptionKeyValut);


            }


        }
        /// <summary>
        /// Updates the plan for subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="planId">The plan identifier.</param>
        public void AddSubscriptionParameters(SubscriptionParametersOutput subscriptionParametersOutput)
        {
            try
            {
                var existingSubscriptionparameter = Context.SubscriptionAttributeValues.Where(s => s.Id == subscriptionParametersOutput.Id).FirstOrDefault();
                if (existingSubscriptionparameter != null)
                {
                    existingSubscriptionparameter.OfferId = subscriptionParametersOutput.OfferId;
                    Context.SubscriptionAttributeValues.Update(existingSubscriptionparameter);
                    Context.SaveChanges();
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
                    Context.SubscriptionAttributeValues.Add(newAttributeValue);
                    Context.SaveChanges();
                }
            }
            catch (Exception) { }
        }

        public IEnumerable<SubscriptionTemplateParametersModel> GetSubscriptionTemplateParameterById(Guid subscriptionId, Guid planId)
        {
            try
            {
                var subscriptionTemplateParams = Context.SubscriptionTemplateParametersOutPut.FromSqlRaw("dbo.spGetSubscriptionTemplateParameters {0},{1}", subscriptionId, planId);
                var subscriptionTemplateParameters = subscriptionTemplateParams.ToList();

                List<SubscriptionTemplateParametersModel> attributesList = new List<SubscriptionTemplateParametersModel>();

                if (subscriptionTemplateParameters != null && subscriptionTemplateParameters.Count() > 0)
                {
                    foreach (var parms in subscriptionTemplateParameters)
                    {
                        SubscriptionTemplateParametersModel subscriptionparms = new SubscriptionTemplateParametersModel();
                        subscriptionparms.Id = parms.Id ?? 0;
                        subscriptionparms.PlanId = parms.OfferName;
                        subscriptionparms.OfferGuid = parms.OfferGuid;
                        subscriptionparms.PlanGuid = parms.PlanGuid;
                        subscriptionparms.PlanId = parms.PlanId;
                        subscriptionparms.ArmtemplateId = parms.ArmtemplateId;
                        subscriptionparms.Parameter = parms.Parameter;
                        subscriptionparms.ParameterDataType = parms.ParameterDataType;
                        subscriptionparms.Value = parms.Value;
                        subscriptionparms.ParameterType = parms.ParameterType;
                        subscriptionparms.EventId = parms.EventId;
                        subscriptionparms.EventsName = parms.EventsName;
                        subscriptionparms.AmpsubscriptionId = parms.AmpsubscriptionId;
                        subscriptionparms.SubscriptionStatus = parms.SubscriptionStatus;
                        subscriptionparms.SubscriptionName = parms.SubscriptionName;

                        attributesList.Add(subscriptionparms);
                    }
                }
                return attributesList;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
            return null;
        }

        /// <summary>
        /// Removes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void Remove(Subscriptions entity)
        {
            Context.Subscriptions.Remove(entity);
            Context.SaveChanges();
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
                    Context.Dispose();
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
