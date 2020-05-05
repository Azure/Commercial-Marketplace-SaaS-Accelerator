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
        public int Add(Subscriptions subscriptionDetails)
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
            if (subscriptionId == default)
            {
                if (!isIncludeDeactvated)
                    return context.Subscriptions.Include(s => s.User).Where(s => s.User != null && s.User.EmailAddress == partnerEmailAddress && s.IsActive == true);
                else
                    return context.Subscriptions.Include(s => s.User).Where(s => s.User != null && s.User.EmailAddress == partnerEmailAddress);
            }
            else
            {
                if (!isIncludeDeactvated)
                    return context.Subscriptions.Include(s => s.User).Where(s => s.User != null && s.User.EmailAddress == partnerEmailAddress && s.AmpsubscriptionId == subscriptionId && s.IsActive == true);
                else
                    return context.Subscriptions.Include(s => s.User).Where(s => s.User != null && s.User.EmailAddress == partnerEmailAddress && s.AmpsubscriptionId == subscriptionId);
            }
        }

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
                    return context.Subscriptions.Include(s => s.User).Where(s => s.AmpsubscriptionId == subscriptionId && s.IsActive == true).FirstOrDefault();
                else
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
            var existingKey = context.SubscriptionKeyValut.Where(s => s.SubscriptionId == subscriptionId).FirstOrDefault();
            if (existingKey != null)
            {
                existingKey.SecureId = keyVaultSecret;
                existingKey.SubscriptionId = subscriptionId;
                context.SubscriptionKeyValut.Update(existingKey);
            }
            else
            {
                SubscriptionKeyValut subscriptionKeyValut = new SubscriptionKeyValut()
                {
                    SubscriptionId = subscriptionId,
                    SecureId = keyVaultSecret,
                    CreateDate = DateTime.Now,
                    UserId = userId
                };

                context.SubscriptionKeyValut.Add(subscriptionKeyValut);
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
        /// Gets the subscription template parameter.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="planId">The plan identifier.</param>
        /// <returns></returns>
        public IEnumerable<SubscriptionTemplateParametersModel> GetSubscriptionTemplateParameter(Guid subscriptionId, Guid planId)
        {
            var subscriptionTemplateParams = context.SubscriptionTemplateParametersOutPut.FromSqlRaw("dbo.spGetSubscriptionTemplateParameters {0},{1}", subscriptionId, planId);
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

        /// <summary>
        /// Gets the subscription template parameter by identifier.
        /// </summary>
        /// <param name="subscriptionID">The subscription identifier.</param>
        /// <param name="PlanGuid">The plan unique identifier.</param>
        /// <returns></returns>
        public List<SubscriptionTemplateParameters> GetSubscriptionTemplateParameterById(Guid subscriptionID, Guid PlanGuid)
        {
            List<SubscriptionTemplateParameters> _list = new List<SubscriptionTemplateParameters>();
            var subscriptionAttributes = context.SubscriptionTemplateParametersOutPut.FromSqlRaw("dbo.spGetSubscriptionTemplateParameters {0},{1}", subscriptionID, PlanGuid);

            var existingdata = context.SubscriptionTemplateParameters.Where(s => s.AmpsubscriptionId == subscriptionID);
            var subscriptionKeyValues = context.SubscriptionKeyValueOutPut.FromSqlRaw("dbo.spGetSubscriptionKeyValue {0}", subscriptionID);
            if (existingdata != null)
            {
                var existingdatalist = existingdata.ToList();
                if (existingdatalist.Count() > 0)
                {
                    return existingdatalist;
                }
                else
                {
                    if (subscriptionAttributes != null)
                    {
                        var beforeReplaceList = subscriptionAttributes.ToList();
                        var subscriptionKeyValuesList = subscriptionKeyValues.ToList();
                        var subscriptionAttributesList = ReplaceDeploymentparms(beforeReplaceList, subscriptionKeyValuesList);

                        if (subscriptionAttributesList.Count() > 0)
                        {
                            foreach (var attr in subscriptionAttributesList)
                            {
                                SubscriptionTemplateParameters parm = new SubscriptionTemplateParameters()
                                {
                                    OfferName = attr.OfferName,
                                    OfferGuid = attr.OfferGuid,
                                    PlanGuid = attr.PlanGuid,
                                    PlanId = attr.PlanId,
                                    ArmtemplateId = attr.ArmtemplateId,
                                    Parameter = attr.Parameter,
                                    ParameterDataType = attr.ParameterDataType,
                                    Value = attr.Value,
                                    ParameterType = attr.ParameterType,
                                    EventId = attr.EventId,
                                    EventsName = attr.EventsName,
                                    AmpsubscriptionId = attr.AmpsubscriptionId,
                                    SubscriptionStatus = attr.SubscriptionStatus,
                                    SubscriptionName = attr.SubscriptionName,
                                    CreateDate = DateTime.Now
                                };
                                context.SubscriptionTemplateParameters.Add(parm);
                                context.SaveChanges();
                                _list.Add(parm);
                            }

                        }
                    }
                }
            }
            return _list;
        }

        /// <summary>
        /// Replaces the deploymentparms.
        /// </summary>
        /// <param name="parmList">The parm list.</param>
        /// <param name="subscriptionKeyValuesList">The subscription key values list.</param>
        /// <returns></returns>
        public static List<SubscriptionTemplateParametersOutPut> ReplaceDeploymentparms(List<SubscriptionTemplateParametersOutPut> parmList, List<SubscriptionKeyValueOutPut> subscriptionKeyValuesList)
        {
            Hashtable hashTable = new Hashtable();
            foreach (var keys in subscriptionKeyValuesList)
            {
                hashTable.Add(keys.Key, keys.Value);
            }

            ExtendedProperties properties = new ExtendedProperties();

            VelocityEngine engine = new VelocityEngine();
            engine.Init(properties);

            VelocityContext context = new VelocityContext(hashTable);
            StringWriter writer = new StringWriter();
            engine.Evaluate(context, writer, string.Empty, JsonConvert.SerializeObject(parmList));

            parmList = JsonConvert.DeserializeObject<List<SubscriptionTemplateParametersOutPut>>(writer.ToString());
            return parmList;
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
