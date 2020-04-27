using Microsoft.EntityFrameworkCore;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using Microsoft.Marketplace.SaasKit.Contracts;
using Microsoft.Marketplace.SaasKit.WebJob;
using Microsoft.Marketplace.SaasKit.WebJob.Helpers;
using Microsoft.Marketplace.SaasKit.WebJob.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.WebJob.StatusHandlers
{

    class ResourceDeploymentStatusHandler : AbstractSubscriptionStatusHandler
    {

        readonly IFulfillmentApiClient fulfillApiclient;
        //private readonly ISubscriptionsRepository subscriptionsRepository;

        public ResourceDeploymentStatusHandler(IFulfillmentApiClient fulfillApiClient) : base(new SaasKitContext())
        {
            this.fulfillApiclient = fulfillApiClient;


        }
        public override void Process(Guid subscriptionID)
        {
            Console.Write("Process...");
            Console.Write("GetSubscriptionById");
            var subscription = this.GetSubscriptionById(subscriptionID);
            Console.Write("GetPlanById");
            var planDetails = this.GetPlanById(subscription.AmpplanId);
            Console.Write("Offers");
            var offer = Context.Offers.Where(s => s.OfferGuid == planDetails.OfferId).FirstOrDefault();
            Console.Write("Events");
            var events = Context.Events.Where(s => s.EventsName == "Activate").FirstOrDefault();
            Console.Write("Events");

            Console.Write("subscription.SubscriptionStatus: SubscriptionStatus: {0}", subscription.SubscriptionStatus);
            if (subscription.SubscriptionStatus == "PendingActivation")
            {
                try
                {
                    // Check if arm template is available fo rthe plan in Plan Event Mapping table with isactive=1

                    var planEvent = Context.PlanEventsMapping.Where(s => s.PlanId == planDetails.PlanGuid && s.EventId == events.EventsId).FirstOrDefault();
                    var armTemplate = Context.Armtemplates.Where(s => s.ArmtempalteId == planEvent.ArmtemplateId).FirstOrDefault();

                    var attributelsit = GetTemplateParameters(subscriptionID, planDetails.PlanGuid, Context);


                    if (armTemplate != null)
                    {
                        //&& subscriptionAttributes != null && subscriptionAttributes.Count() > 0
                        //var credenitals = subscriptionAttributes.Where(s => s.Type.ToLower() == "deployment").ToList();
                        var templateParameters = Context.SubscriptionTemplateParameters.Where(
                           s => s.PlanGuid == planDetails.PlanGuid &&
                           s.ArmtemplateId == armTemplate.ArmtempalteId &&
                           s.AmpsubscriptionId == subscriptionID &&
                           s.OfferGuid == offer.OfferGuid);

                        if (templateParameters != null && templateParameters.Count() > 0)
                        {
                            var parametersList = templateParameters.ToList();
                            WebJobSubscriptionStatus status = new WebJobSubscriptionStatus()
                            {

                                SubscriptionId = subscriptionID,
                                ArmtemplateId = armTemplate.ArmtempalteId,
                                SubscriptionStatus = subscription.SubscriptionStatus,
                                DeploymentStatus = "ARMTemplateDeploymentPending",
                                Description = "Start Deployment",
                                InsertDate = System.DateTime.Now

                            };

                            Context.WebJobSubscriptionStatus.Add(status);
                            Console.Write("Start Deployment");
                            Deploy obj = new Deploy();

                            var keyvaultUrl = Context.SubscriptionKeyValut.Where(s => s.SubscriptionId == subscriptionID).FirstOrDefault();
                            string secretValue = AzureKeyVaultHelper.DoVault(keyvaultUrl.SecuteId);

                            var credenitals = JsonConvert.DeserializeObject<CredentialsModel>(secretValue);
                            var output = obj.DeployARMTemplate(armTemplate, parametersList, credenitals).ConfigureAwait(false).GetAwaiter().GetResult();
                            string k = JsonConvert.SerializeObject(output.Properties.Outputs);
                            Console.WriteLine(k);
                        }

                    }

                    /*
                     {"Tenant ID":"6d7e0652-b03d-4ed2-bf86-f1999cecde17","Subscription ID":"AC57EDA4-EA49-41BD-4452-3AF5F3BBA081","Service Principal ID":"28b1d793-eede-411a-a9fe-ba996808d4ea","Client Secret":"sXJn9bGcp5cmhZ@Ns:?Z77Jb?Zp[?x3."}
                     */

                    //Start ARM template Deployment
                    //Change status to ARMTemplateDeploymentSuccess

                }


                catch (Exception ex)
                {
                    //Change status to ARMTemplateDeploymentFailure

                }

            }
        }

        public static List<SubscriptionTemplateParameters> GetTemplateParameters(Guid subscriptionID, Guid PlanGuid, SaasKitContext Context)
        {
            List<SubscriptionTemplateParameters> _list = new List<SubscriptionTemplateParameters>();
            var subscriptionAttributes = Context.SubscriptionTemplateParametersOutPut.FromSqlRaw("dbo.spGetSubscriptionTemplateParameters {0},{1}", subscriptionID, PlanGuid);

            if (subscriptionAttributes != null)
            {

                var subscriptionAttributesList = subscriptionAttributes.ToList();


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
                        Context.SubscriptionTemplateParameters.Add(parm);
                        _list.Add(parm);
                    }

                }
            }
            return _list;

        }
    }
}

