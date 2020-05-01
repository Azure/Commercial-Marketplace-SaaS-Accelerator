using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using Microsoft.Marketplace.SaasKit.Contracts;
using Microsoft.Marketplace.SaasKit.WebJob;
using Microsoft.Marketplace.SaasKit.WebJob.Helpers;
using Microsoft.Marketplace.SaasKit.WebJob.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.WebJob.StatusHandlers
{

    class ResourceDeploymentStatusHandler : AbstractSubscriptionStatusHandler
    {

        readonly IFulfillmentApiClient fulfillApiclient;
        private readonly ISubscriptionsRepository subscriptionsRepository;
        readonly IApplicationConfigRepository applicationConfigRepository;
        readonly ISubscriptionLogRepository subscriptionLogRepository;
        public ResourceDeploymentStatusHandler(IFulfillmentApiClient fulfillApiClient, IApplicationConfigRepository applicationConfigRepository, ISubscriptionLogRepository subscriptionLogRepository, ISubscriptionsRepository subscriptionsRepository) : base(new SaasKitContext())
        {
            this.fulfillApiclient = fulfillApiClient;
            this.applicationConfigRepository = applicationConfigRepository;
            this.subscriptionLogRepository = subscriptionLogRepository;
            this.subscriptionsRepository = subscriptionsRepository;

        }
        public override void Process(Guid subscriptionID)
        {
            Console.WriteLine("ResourceDeploymentStatusHandler Process...");
            Console.WriteLine("Get SubscriptionById");
            var subscription = this.GetSubscriptionById(subscriptionID);
            Console.WriteLine("Get PlanById");
            var planDetails = this.GetPlanById(subscription.AmpplanId);
            Console.WriteLine("Get Offers");
            var offer = Context.Offers.Where(s => s.OfferGuid == planDetails.OfferId).FirstOrDefault();
            Console.WriteLine("Get Events");
            var events = Context.Events.Where(s => s.EventsName == "Activate").FirstOrDefault();

            Console.WriteLine("subscription.SubscriptionStatus: SubscriptionStatus: {0}", subscription.SubscriptionStatus);
            if (subscription.SubscriptionStatus == SubscriptionWebJobStatusEnum.PendingActivation.ToString())
            {

                // Check if arm template is available for the plan in Plan Event Mapping table with isactive=1
                Console.WriteLine("Get PlanEventsMapping");
                var planEvent = Context.PlanEventsMapping.Where(s => s.PlanId == planDetails.PlanGuid && s.EventId == events.EventsId && s.Isactive == true).FirstOrDefault();
                Console.WriteLine("Get Armtemplates");
                var armTemplate = Context.Armtemplates.Where(s => s.ArmtempalteId == planEvent.ArmtemplateId).FirstOrDefault();
                Console.WriteLine("Get GetTemplateParameters");
                var attributelsit = GetTemplateParameters(subscriptionID, planDetails.PlanGuid, Context);


                try
                {
                    if (armTemplate != null)
                    {
                        if (attributelsit != null)
                        {
                            Console.WriteLine("Get attributelsit");
                            var parametersList = attributelsit.Where(s => s.ParameterType.ToLower() == "input" && s.EventsName == "Activate").ToList();

                            Console.WriteLine("Attributelsit : {0}", JsonConvert.SerializeObject(parametersList));
                            if (parametersList.Count() > 0)
                            {

                                Console.WriteLine("UpdateWebJobSubscriptionStatus");

                                StatusUpadeHelpers.UpdateWebJobSubscriptionStatus(subscriptionID, armTemplate.ArmtempalteId, DeploymentStatusEnum.ARMTemplateDeploymentPending.ToString(), "Start Deployment", Context, subscription.SubscriptionStatus);

                                this.subscriptionsRepository.UpdateStatusForSubscription(subscriptionID, SubscriptionWebJobStatusEnum.DeploymentPending.ToString(), true);

                                Console.WriteLine("Get SubscriptionKeyValut");
                                string keyvaultUrl = "";
                                if (planDetails.DeployToCustomerSubscription != null && planDetails.DeployToCustomerSubscription == true)
                                {
                                    var keyvault = Context.SubscriptionKeyValut.Where(s => s.SubscriptionId == subscriptionID).FirstOrDefault();
                                    keyvaultUrl = keyvault.SecureId;
                                }
                                else
                                {
                                    keyvaultUrl = applicationConfigRepository.GetValuefromApplicationConfig("LocalkeyvaultUrl");
                                }


                                Console.WriteLine("Get DoVault");
                                string secretValue = AzureKeyVaultHelper.DoVault(keyvaultUrl);

                                var credenitals = JsonConvert.DeserializeObject<CredentialsModel>(secretValue);
                                Console.WriteLine("SecretValue : {0}", secretValue);

                                Deploy deploy = new Deploy();
                                Console.WriteLine("Start Deployment: DeployARMTemplate");
                                var output = deploy.DeployARMTemplate(armTemplate, parametersList, credenitals).ConfigureAwait(false).GetAwaiter().GetResult();

                                string outputstring = JsonConvert.SerializeObject(output.Properties.Outputs);
                                var outPutList = GenerateParmlist(output);
                                if (outPutList != null)
                                {

                                    UpdateSubscriptionTemplateParameters(outPutList.ToList(), subscriptionID, Context);


                                }

                                Console.WriteLine(outputstring);


                                this.subscriptionsRepository.UpdateStatusForSubscription(subscriptionID, SubscriptionWebJobStatusEnum.DeploymentSuccessful.ToString(), true);

                                SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
                                {
                                    Attribute = SubscriptionLogAttributes.Deployment.ToString(),
                                    SubscriptionId = subscription.Id,
                                    NewValue = SubscriptionWebJobStatusEnum.DeploymentSuccessful.ToString(),
                                    OldValue = SubscriptionWebJobStatusEnum.DeploymentPending.ToString(),
                                    CreateBy = 0,
                                    CreateDate = DateTime.Now
                                };
                                this.subscriptionLogRepository.Add(auditLog);

                                StatusUpadeHelpers.UpdateWebJobSubscriptionStatus(subscriptionID, armTemplate.ArmtempalteId, DeploymentStatusEnum.ARMTemplateDeploymentSuccess.ToString(), "Deployment Successful", Context, SubscriptionWebJobStatusEnum.DeploymentSuccessful.ToString());
                            }
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
                    //Change status to  ARMTemplateDeploymentFailure
                    string errorDescriptin = string.Format("Exception: {0} :: Innser Exception:{1}", ex.Message, ex.InnerException);
                    StatusUpadeHelpers.UpdateWebJobSubscriptionStatus(subscriptionID, armTemplate.ArmtempalteId, DeploymentStatusEnum.ARMTemplateDeploymentFailure.ToString(), errorDescriptin, Context, subscription.SubscriptionStatus.ToString());
                    Console.WriteLine(errorDescriptin);

                    SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
                    {
                        Attribute = SubscriptionLogAttributes.Deployment.ToString(),
                        SubscriptionId = subscription.Id,
                        NewValue = DeploymentStatusEnum.ARMTemplateDeploymentFailure.ToString(),
                        OldValue = DeploymentStatusEnum.ARMTemplateDeploymentPending.ToString(),
                        CreateBy = 0,
                        CreateDate = DateTime.Now
                    };
                    this.subscriptionLogRepository.Add(auditLog);
                }

            }
        }

        public static List<SubscriptionTemplateParameters> GetTemplateParameters(Guid subscriptionID, Guid PlanGuid, SaasKitContext Context)
        {
            List<SubscriptionTemplateParameters> _list = new List<SubscriptionTemplateParameters>();
            var subscriptionAttributes = Context.SubscriptionTemplateParametersOutPut.FromSqlRaw("dbo.spGetSubscriptionTemplateParameters {0},{1}", subscriptionID, PlanGuid);

            var existingdata = Context.SubscriptionTemplateParameters.Where(s => s.AmpsubscriptionId == subscriptionID);
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

                        var subscriptionAttributesList = ReplaceDeploymentparms(beforeReplaceList);

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
                                Context.SaveChanges();
                                _list.Add(parm);
                            }

                        }


                    }

                }


            }
            return _list;
        }


        public static List<ChildParameterViewModel> GenerateParmlist(DeploymentExtended outputstring)
        {
            List<ChildParameterViewModel> childlist = new List<ChildParameterViewModel>();

            JObject templateOutputs = (JObject)outputstring.Properties.Outputs;


            foreach (JToken child in templateOutputs.Children())
            {
                ChildParameterViewModel childparms = new ChildParameterViewModel();
                childparms = new ChildParameterViewModel();
                childparms.ParameterType = "output";
                var paramName = (child as JProperty).Name;
                childparms.ParameterName = paramName;
                object paramValue = string.Empty;

                foreach (JToken grandChild in child)
                {
                    foreach (JToken grandGrandChild in grandChild)
                    {
                        var property = grandGrandChild as JProperty;

                        if (property != null && property.Name == "value")
                        {
                            var type = property.Value.GetType();

                            if (type == typeof(JValue) || type == typeof(JArray) ||
                            property.Value.Type == JTokenType.Object ||
                            property.Value.Type == JTokenType.Date)
                            {
                                paramValue = property.Value;
                                if (paramValue != null)
                                {
                                    childparms.ParameterValue = paramValue.ToString();
                                }
                            }

                        }
                    }
                }
                childlist.Add(childparms);
            }
            return childlist;
        }

        public static void UpdateSubscriptionTemplateParameters(List<ChildParameterViewModel> parms, Guid subscriptionID, SaasKitContext Context)
        {
            foreach (var parm in parms)
            {

                var outputparm = Context.SubscriptionTemplateParameters.Where(s => s.AmpsubscriptionId == subscriptionID && s.Parameter == parm.ParameterName && s.ParameterType.ToLower() == "output").FirstOrDefault();

                if (outputparm != null)
                {
                    outputparm.Value = parm.ParameterValue;
                    Context.SubscriptionTemplateParameters.Update(outputparm);
                    Context.SaveChanges();
                }


            }


        }


        public static List<SubscriptionTemplateParametersOutPut> ReplaceDeploymentparms(List<SubscriptionTemplateParametersOutPut> parmList)
        {
            foreach (var parm in parmList)
            {

                parm.Value = parm.Value.Replace("${Subscription}", parm.SubscriptionName.Replace(" ", "-"));
                parm.Value = parm.Value.Replace("${Offer}", parm.OfferName.Replace(" ", "-"));
                parm.Value = parm.Value.Replace("${Plan}", parm.PlanId.Replace(" ", "-"));
            }

            return parmList;
        }
    }
}

