namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Commons.Collections;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.DataModel;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using Newtonsoft.Json;
    using NVelocity;
    using NVelocity.App;

    /// <summary>
    /// Subscription Template Parameters.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.ISubscriptionTemplateParametersRepository" />
    public class SubscriptionTemplateParametersRepository : ISubscriptionTemplateParametersRepository
    {
        /// <summary>
        /// The context.
        /// </summary>
        private readonly SaasKitContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionTemplateParametersRepository"/> class.
        /// </summary>
        /// <param name="context">The this.context.</param>
        public SubscriptionTemplateParametersRepository(SaasKitContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Gets the template parameters by subscription identifier.
        /// </summary>
        /// <param name="subscriptionID">The subscription identifier.</param>
        /// <returns>
        /// List of ARM template parameters associated with the SaaS subscription.
        /// </returns>
        public IEnumerable<SubscriptionTemplateParameters> GetTemplateParametersBySubscriptionId(Guid subscriptionID)
        {
            var results = this.context.SubscriptionTemplateParameters.Where(s => s.AmpsubscriptionId == subscriptionID);
            if (results.Count() == 0)
            {
                return null;
            }
            else
            {
                return this.context.SubscriptionTemplateParameters.Where(s => s.AmpsubscriptionId == subscriptionID);
            }
        }

        /// <summary>
        /// Saves the specified subscription template parameters.
        /// </summary>
        /// <param name="subscriptionTemplateParameters">The subscription template parameters.</param>
        /// <returns> Template Id.</returns>
        public int Save(SubscriptionTemplateParameters subscriptionTemplateParameters)
        {
            var existingRecord = this.context.SubscriptionTemplateParameters.Where(x => x.Id == subscriptionTemplateParameters.Id).FirstOrDefault();
            if (existingRecord == null)
            {
                this.context.SubscriptionTemplateParameters.Add(subscriptionTemplateParameters);
            }
            else
            {
                existingRecord.Parameter = subscriptionTemplateParameters.Parameter;
                existingRecord.ParameterDataType = subscriptionTemplateParameters.ParameterDataType;
                existingRecord.Value = subscriptionTemplateParameters.Value;
                existingRecord.ParameterType = subscriptionTemplateParameters.ParameterType;
            }

            this.context.SaveChanges();

            return subscriptionTemplateParameters.Id;
        }

        /// <summary>
        /// Update subscription template parameter by identifier.
        /// </summary>
        /// <param name="parms"> List of parameters.</param>
        /// <param name="subscriptionID"> Subscription ID.</param>
        public void Update(List<SubscriptionTemplateParameters> parms, Guid subscriptionID)
        {
            foreach (var parm in parms)
            {
                var outputparm = this.context.SubscriptionTemplateParameters.Where(s => s.AmpsubscriptionId == subscriptionID && s.Parameter == parm.Parameter && s.ParameterType.ToLower() == "output").FirstOrDefault();

                if (outputparm != null)
                {
                    outputparm.Value = parm.Value;
                    this.context.SubscriptionTemplateParameters.Update(outputparm);
                    this.context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Gets the subscription template parameter.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="planId">The plan identifier.</param>
        /// <returns> Subscription Template Parameters Model.</returns>
        public IEnumerable<SubscriptionTemplateParametersModel> GetSubscriptionTemplateParameter(Guid subscriptionId, Guid planId)
        {
            var subscriptionTemplateParams = this.context.SubscriptionTemplateParametersOutPut.FromSqlRaw("dbo.spGetSubscriptionTemplateParameters {0},{1}", subscriptionId, planId);
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
        /// <param name="planGuid">The plan unique identifier.</param>
        /// <returns>
        /// List of Subscription Template Parameters.
        /// </returns>
        public List<SubscriptionTemplateParameters> GetById(Guid subscriptionID, Guid planGuid)
        {
            List<SubscriptionTemplateParameters> list = new List<SubscriptionTemplateParameters>();
            var subscriptionAttributes = this.context.SubscriptionTemplateParametersOutPut.FromSqlRaw("dbo.spGetSubscriptionTemplateParameters {0},{1}", subscriptionID, planGuid);

            var existingdata = this.context.SubscriptionTemplateParameters.Where(s => s.AmpsubscriptionId == subscriptionID);
            var subscriptionKeyValues = this.context.SubscriptionKeyValueOutPut.FromSqlRaw("dbo.spGetSubscriptionKeyValue {0}", subscriptionID);
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
                        var subscriptionAttributesList = this.ReplaceDeploymentparms(beforeReplaceList, subscriptionKeyValuesList);

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
                                    CreateDate = DateTime.Now,
                                };
                                this.context.SubscriptionTemplateParameters.Add(parm);
                                this.context.SaveChanges();
                                list.Add(parm);
                            }
                        }
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Replaces the deploymentparms.
        /// </summary>
        /// <param name="parmList">The parm list.</param>
        /// <param name="subscriptionKeyValuesList">The subscription key values list.</param>
        /// <returns> Template Paramemters.</returns>
        public List<SubscriptionTemplateParametersOutPut> ReplaceDeploymentparms(List<SubscriptionTemplateParametersOutPut> parmList, List<SubscriptionKeyValueOutPut> subscriptionKeyValuesList)
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
    }
}