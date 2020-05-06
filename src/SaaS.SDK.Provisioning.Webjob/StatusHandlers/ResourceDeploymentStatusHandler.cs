namespace Microsoft.Marketplace.SaasKit.Provisioning.Webjob.StatusHandlers
{
    using Microsoft.Azure.Management.ResourceManager.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Marketplace.SaaS.SDK.Services.Contracts;
    using Microsoft.Marketplace.SaaS.SDK.Services.Helpers;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using Microsoft.Marketplace.SaasKit.Contracts;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Status handler to handle the subscription in PendingDeployment status.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Provisioning.Webjob.StatusHandlers.AbstractSubscriptionStatusHandler" />
    public class ResourceDeploymentStatusHandler : AbstractSubscriptionStatusHandler
    {
        /// <summary>
        /// The fulfillment apiclient
        /// </summary>
        protected readonly IFulfillmentApiClient fulfillmentApiclient;

        /// <summary>
        /// The application configuration repository
        /// </summary>
        protected readonly IApplicationConfigRepository applicationConfigRepository;

        /// <summary>
        /// The subscription log repository
        /// </summary>
        protected readonly ISubscriptionLogRepository subscriptionLogRepository;

        /// <summary>
        /// The azure key vault client
        /// </summary>
        protected readonly IVaultService azureKeyVaultClient;

        /// <summary>
        /// The azure BLOB file client
        /// </summary>
        protected readonly IARMTemplateStorageService azureBlobFileClient;

        /// <summary>
        /// The key vault configuration
        /// </summary>
        protected readonly KeyVaultConfig keyVaultConfig;

        /// <summary>
        /// The logger
        /// </summary>
        protected readonly ILogger<ResourceDeploymentStatusHandler> logger;

        /// <summary>
        /// The arm template deployment manager
        /// </summary>
        protected readonly ARMTemplateDeploymentManager armTemplateDeploymentManager;

        /// <summary>
        /// The offers repository
        /// </summary>
        protected readonly IOffersRepository offersRepository;

        /// <summary>
        /// The arm template repository
        /// </summary>
        protected readonly IArmTemplateRepository armTemplateRepository;

        /// <summary>
        /// The plan events mapping repository
        /// </summary>
        protected readonly IPlanEventsMappingRepository planEventsMappingRepository;

        /// <summary>
        /// The events repository
        /// </summary>
        protected readonly IEventsRepository eventsRepository;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceDeploymentStatusHandler"/> class.
        /// </summary>
        /// <param name="fulfillApiClient">The fulfill API client.</param>
        /// <param name="applicationConfigRepository">The application configuration repository.</param>
        /// <param name="subscriptionLogRepository">The subscription log repository.</param>
        /// <param name="subscriptionsRepository">The subscriptions repository.</param>
        /// <param name="azureKeyVaultClient">The azure key vault client.</param>
        /// <param name="azureBlobFileClient">The azure BLOB file client.</param>
        /// <param name="keyVaultConfig">The key vault configuration.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="armTemplateDeploymentManager">The arm template deployment manager.</param>
        public ResourceDeploymentStatusHandler(IFulfillmentApiClient fulfillApiClient,
                                                IApplicationConfigRepository applicationConfigRepository,
                                                ISubscriptionLogRepository subscriptionLogRepository,
                                                ISubscriptionsRepository subscriptionsRepository,
                                                IVaultService azureKeyVaultClient,
                                                IARMTemplateStorageService azureBlobFileClient,
                                                KeyVaultConfig keyVaultConfig,
                                                IPlansRepository plansRepository,
                                                IUsersRepository usersRepository,
                                                IOffersRepository offersRepository,
                                                IArmTemplateRepository armTemplateRepository,
                                                IPlanEventsMappingRepository planEventsMappingRepository,
                                                IEventsRepository eventsRepository,
                                                ILogger<ResourceDeploymentStatusHandler> logger,
                                                ARMTemplateDeploymentManager armTemplateDeploymentManager) : base(subscriptionsRepository, plansRepository, usersRepository)
        {
            this.fulfillmentApiclient = fulfillApiClient;
            this.applicationConfigRepository = applicationConfigRepository;
            this.subscriptionLogRepository = subscriptionLogRepository;
            this.offersRepository = offersRepository;
            this.armTemplateRepository = armTemplateRepository;
            this.planEventsMappingRepository = planEventsMappingRepository;
            this.eventsRepository = eventsRepository;
            this.azureKeyVaultClient = azureKeyVaultClient;
            this.azureBlobFileClient = azureBlobFileClient;
            this.keyVaultConfig = keyVaultConfig;
            this.logger = logger;
            this.armTemplateDeploymentManager = armTemplateDeploymentManager;
        }

        /// <summary>
        /// Processes the specified subscription identifier.
        /// </summary>
        /// <param name="subscriptionID">The subscription identifier.</param>
        public override void Process(Guid subscriptionID)
        {
            this.logger.LogInformation("ResourceDeploymentStatusHandler Process...");
            this.logger.LogInformation("Get SubscriptionById");
            var subscription = this.GetSubscriptionById(subscriptionID);
            this.logger.LogInformation("Get PlanById");
            var planDetails = this.GetPlanById(subscription.AmpplanId);

            this.logger.LogInformation("Get User");
            var userdeatils = this.GetUserById(subscription.UserId);

            this.logger.LogInformation("Get Offers");
            
            var offer = offersRepository.GetOfferById(planDetails.OfferId);
            this.logger.LogInformation("Get Events");

            var events = this.eventsRepository.GetByName("Activate"); //Context.Events.Where(s => s.EventsName == "Activate").FirstOrDefault();

            this.logger.LogInformation("subscription.SubscriptionStatus: SubscriptionStatus: {0}", subscription.SubscriptionStatus);

            if (SubscriptionStatusEnumExtension.PendingActivation.ToString().Equals(subscription?.SubscriptionStatus, StringComparison.InvariantCultureIgnoreCase))
            {

                // Check if arm template is available for the plan in Plan Event Mapping table with isactive=1
                this.logger.LogInformation("Get PlanEventsMapping");
                var planEvent = planEventsMappingRepository.GetPlanEvent(planDetails.PlanGuid, events.EventsId);
                //Context.PlanEventsMapping.Where(s => s.PlanId == planDetails.PlanGuid && s.EventId == events.EventsId && s.Isactive == true).FirstOrDefault();
                this.logger.LogInformation("Get Armtemplates");
                var armTemplate = armTemplateRepository.GetById(planEvent.ArmtemplateId);
                //Context.Armtemplates.Where(s => s.ArmtempalteId == planEvent.ArmtemplateId).FirstOrDefault();
                this.logger.LogInformation("Get GetTemplateParameters");
                var attributelsit = GetTemplateParameters(subscriptionID, planDetails.PlanGuid, Context, userdeatils);

                try
                {
                    if (armTemplate != null)
                    {
                        if (attributelsit != null)
                        {
                            this.logger.LogInformation("Get attributelsit");
                            var parametersList = attributelsit.Where(s => s.ParameterType.ToLower() == "input" && s.EventsName == "Activate").ToList();

                            this.logger.LogInformation("Attributelsit : {0}", JsonConvert.SerializeObject(parametersList));
                            if (parametersList.Count() > 0)
                            {

                                this.logger.LogInformation("UpdateWebJobSubscriptionStatus");

                                this.subscriptionLogRepository.LogStatusDuringProvisioning(subscriptionID, armTemplate.ArmtempalteId, DeploymentStatusEnum.ARMTemplateDeploymentPending.ToString(), "Start Deployment", subscription.SubscriptionStatus);

                                this.subscriptionsRepository.UpdateStatusForSubscription(subscriptionID, SubscriptionStatusEnumExtension.DeploymentPending.ToString(), true);

                                this.logger.LogInformation("Get SubscriptionKeyValut");
                                string secretKey = "";
                                if (planDetails.DeployToCustomerSubscription != null && planDetails.DeployToCustomerSubscription == true)
                                {
                                    var keyvault = this.subscriptionsRepository.GetDeploymentConfig(subscriptionID); 
                                    // Context.SubscriptionKeyValut.Where(s => s.SubscriptionId == subscriptionID).FirstOrDefault();
                                    secretKey = keyvault.SecureId;
                                }
                                else
                                {
                                    secretKey = string.Format("{0}secrets/HostedsubscriptionCredentials", keyVaultConfig.KeyVaultUrl);
                                }

                                this.logger.LogInformation("Get DoVault");
                                string secretValue = azureKeyVaultClient.GetKeyAsync(secretKey).ConfigureAwait(false).GetAwaiter().GetResult();

                                var credenitals = JsonConvert.DeserializeObject<CredentialsModel>(secretValue);
                                this.logger.LogInformation("SecretValue : {0}", secretValue);


                                this.logger.LogInformation("Start Deployment: DeployARMTemplate");
                                string armTemplateCOntent = azureBlobFileClient.GetARMTemplateContentAsString(armTemplate.ArmtempalteName);

                                var output = this.armTemplateDeploymentManager.DeployARMTemplate(armTemplate, parametersList, credenitals, armTemplateCOntent).ConfigureAwait(false).GetAwaiter().GetResult();

                                string outputstring = JsonConvert.SerializeObject(output.Properties.Outputs);
                                var outPutList = GenerateParmlist(output);
                                if (outPutList != null)
                                {

                                    UpdateSubscriptionTemplateParameters(outPutList.ToList(), subscriptionID, Context);

                                }

                                this.logger.LogInformation(outputstring);


                                this.subscriptionsRepository.UpdateStatusForSubscription(subscriptionID, SubscriptionStatusEnumExtension.DeploymentSuccessful.ToString(), true);

                                SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
                                {
                                    Attribute = SubscriptionLogAttributes.Deployment.ToString(),
                                    SubscriptionId = subscription.Id,
                                    NewValue = SubscriptionStatusEnumExtension.DeploymentSuccessful.ToString(),
                                    OldValue = SubscriptionStatusEnumExtension.DeploymentPending.ToString(),
                                    CreateBy = userdeatils.UserId,
                                    CreateDate = DateTime.Now
                                };
                                this.subscriptionLogRepository.Save(auditLog);

                                this.subscriptionLogRepository.LogStatusDuringProvisioning(subscriptionID, armTemplate.ArmtempalteId, DeploymentStatusEnum.ARMTemplateDeploymentSuccess.ToString(), "Deployment Successful", SubscriptionStatusEnumExtension.DeploymentSuccessful.ToString());
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    //Change status to  ARMTemplateDeploymentFailure
                    string errorDescriptin = string.Format("Exception: {0} :: Innser Exception:{1}", ex.Message, ex.InnerException);
                    this.subscriptionLogRepository.LogStatusDuringProvisioning(subscriptionID, armTemplate.ArmtempalteId, DeploymentStatusEnum.ARMTemplateDeploymentFailure.ToString(), errorDescriptin, subscription.SubscriptionStatus.ToString());
                    this.logger.LogInformation(errorDescriptin);

                    this.subscriptionsRepository.UpdateStatusForSubscription(subscriptionID, SubscriptionStatusEnumExtension.DeploymentFailed.ToString(), true);

                    SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
                    {
                        Attribute = SubscriptionLogAttributes.Deployment.ToString(),
                        SubscriptionId = subscription.Id,
                        NewValue = SubscriptionStatusEnumExtension.DeploymentFailed.ToString(),
                        OldValue = SubscriptionStatusEnumExtension.PendingActivation.ToString(),
                        CreateBy = userdeatils.UserId,
                        CreateDate = DateTime.Now
                    };
                    this.subscriptionLogRepository.Save(auditLog);
                }

            }
        }

        /// <summary>
        /// Gets the template parameters.
        /// </summary>
        /// <param name="subscriptionID">The subscription identifier.</param>
        /// <param name="PlanGuid">The plan unique identifier.</param>
        /// <param name="Context">The context.</param>
        /// <param name="userdeatils">The userdeatils.</param>
        /// <returns></returns>
        public List<SubscriptionTemplateParameters> GetTemplateParameters(Guid subscriptionID, Guid PlanGuid, Users userdeatils)
        {
            List<SubscriptionTemplateParameters> _list = new List<SubscriptionTemplateParameters>();
            var subscriptionAttributes = Context.SubscriptionTemplateParametersOutPut.FromSqlRaw("dbo.spGetSubscriptionTemplateParameters {0},{1}", subscriptionID, PlanGuid);

            var existingdata = this.subscription //Context.SubscriptionTemplateParameters.Where(s => s.AmpsubscriptionId == subscriptionID);
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
                                    CreateDate = DateTime.Now,
                                    UserId = userdeatils.UserId
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
            //KB: Expect subscriptionID to this method
            // Call a stored procedure to return the default set of values as key value pairs
            /* foreach param
             *  Apply nvelocity and take the processed value.
             */
            // Use Nvelocity to do the substitution


            foreach (var parm in parmList)
            {
                parm.Value = parm.Value.Replace("${Subscription}", parm.SubscriptionName.Replace(" ", "-"));
                parm.Value = parm.Value.Replace("${Offer}", parm.OfferName.Replace(" ", "-"));
                parm.Value = parm.Value.Replace("${Plan}", parm.PlanId.Replace(" ", "-"));
                parm.Value = parm.Value.Replace("${Subscription}", parm.SubscriptionName.Replace("_", "-"));
                parm.Value = parm.Value.Replace("${Offer}", parm.OfferName.Replace("_", "-"));
                parm.Value = parm.Value.Replace("${Plan}", parm.PlanId.Replace("_", "-"));
            }

            return parmList;
        }
    }
}