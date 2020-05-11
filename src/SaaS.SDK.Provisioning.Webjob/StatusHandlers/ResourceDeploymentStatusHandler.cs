namespace Microsoft.Marketplace.SaasKit.Provisioning.Webjob.StatusHandlers
{
    using System;
    using System.Linq;
    using Microsoft.Extensions.Logging;
    using Microsoft.Marketplace.SaaS.SDK.Services.Contracts;
    using Microsoft.Marketplace.SaaS.SDK.Services.Helpers;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using Microsoft.Marketplace.SaaS.SDK.Services.Services;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using Microsoft.Marketplace.SaasKit.Contracts;
    using Newtonsoft.Json;

    /// <summary>
    /// Status handler to handle the subscription in PendingDeployment status.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Provisioning.Webjob.StatusHandlers.AbstractSubscriptionStatusHandler" />
    public class ResourceDeploymentStatusHandler : AbstractSubscriptionStatusHandler
    {
        /// <summary>
        /// The fulfillment apiclient.
        /// </summary>
        private readonly IFulfillmentApiClient fulfillmentApiclient;

        /// <summary>
        /// The application configuration repository.
        /// </summary>
        private readonly IApplicationConfigRepository applicationConfigRepository;

        /// <summary>
        /// The subscription log repository.
        /// </summary>
        private readonly ISubscriptionLogRepository subscriptionLogRepository;

        /// <summary>
        /// The azure key vault client.
        /// </summary>
        private readonly IVaultService azureKeyVaultClient;

        /// <summary>
        /// The azure BLOB file client.
        /// </summary>
        private readonly IARMTemplateStorageService azureBlobFileClient;

        /// <summary>
        /// The key vault configuration.
        /// </summary>
        private readonly KeyVaultConfig keyVaultConfig;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<ResourceDeploymentStatusHandler> logger;

        /// <summary>
        /// The arm template deployment manager.
        /// </summary>
        private readonly ARMTemplateDeploymentManager armTemplateDeploymentManager;

        /// <summary>
        /// The offers repository.
        /// </summary>
        private readonly IOffersRepository offersRepository;

        /// <summary>
        /// The arm template repository.
        /// </summary>
        private readonly IArmTemplateRepository armTemplateRepository;

        /// <summary>
        /// The plan events mapping repository.
        /// </summary>
        private readonly IPlanEventsMappingRepository planEventsMappingRepository;

        /// <summary>
        /// The events repository.
        /// </summary>
        private readonly IEventsRepository eventsRepository;

        /// <summary>
        /// The events repository.
        /// </summary>
        private readonly ISubscriptionTemplateParametersRepository subscriptionTemplateParametersRepository;

        /// <summary>
        /// The subscription service.
        /// </summary>
        private readonly SubscriptionService subscriptionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceDeploymentStatusHandler" /> class.
        /// </summary>
        /// <param name="fulfillApiClient">The fulfill API client.</param>
        /// <param name="applicationConfigRepository">The application configuration repository.</param>
        /// <param name="subscriptionLogRepository">The subscription log repository.</param>
        /// <param name="subscriptionsRepository">The subscriptions repository.</param>
        /// <param name="azureKeyVaultClient">The azure key vault client.</param>
        /// <param name="azureBlobFileClient">The azure BLOB file client.</param>
        /// <param name="keyVaultConfig">The key vault configuration.</param>
        /// <param name="plansRepository">The plans repository.</param>
        /// <param name="usersRepository">The users repository.</param>
        /// <param name="offersRepository">The offers repository.</param>
        /// <param name="armTemplateRepository">The arm template repository.</param>
        /// <param name="planEventsMappingRepository">The plan events mapping repository.</param>
        /// <param name="eventsRepository">The events repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="armTemplateDeploymentManager">The arm template deployment manager.</param>
        /// <param name="subscriptionTemplateParametersRepository">The subscription template parameters repository.</param>
        public ResourceDeploymentStatusHandler(
                                                IFulfillmentApiClient fulfillApiClient,
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
                                                ARMTemplateDeploymentManager armTemplateDeploymentManager,
                                                ISubscriptionTemplateParametersRepository subscriptionTemplateParametersRepository)
                                                : base(subscriptionsRepository, plansRepository, usersRepository)
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
            this.subscriptionTemplateParametersRepository = subscriptionTemplateParametersRepository;
            this.subscriptionService = new SubscriptionService(subscriptionsRepository, plansRepository);
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
            this.logger?.LogInformation("Result subscription : {0}", JsonConvert.SerializeObject(subscription.AmpplanId));
            this.logger.LogInformation("Get PlanById");
            var planDetails = this.GetPlanById(subscription.AmpplanId);
            this.logger.LogInformation("Get User");
            var userdeatils = this.GetUserById(subscription.UserId);
            this.logger.LogInformation("Get Offers");
            var offer = this.offersRepository.GetOfferById(planDetails.OfferId);
            this.logger.LogInformation("Get Events");

            var events = this.eventsRepository.GetByName("Activate");

            this.logger.LogInformation("subscription.SubscriptionStatus: SubscriptionStatus: {0}", subscription.SubscriptionStatus);

            if (SubscriptionStatusEnumExtension.PendingActivation.ToString().Equals(subscription?.SubscriptionStatus, StringComparison.InvariantCultureIgnoreCase) && planDetails.DeployToCustomerSubscription == true)
            {
                // Check if arm template is available for the plan in Plan Event Mapping table with isactive=1
                this.logger.LogInformation("Get PlanEventsMapping");
                var planEvent = this.planEventsMappingRepository.GetPlanEvent(planDetails.PlanGuid, events.EventsId);
                this.logger.LogInformation("Get Armtemplates");
                if (planEvent != null && planEvent.Isactive == true)
                {
                    var armTemplate = this.armTemplateRepository.GetById(planEvent.ArmtemplateId);
                    this.logger.LogInformation("Get GetTemplateParameters");
                    var attributelsit = this.subscriptionTemplateParametersRepository.GetById(subscriptionID, planDetails.PlanGuid);

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
                                    this.logger.LogInformation("Get SubscriptionKeyVault");
                                    string secretKey = string.Empty;
                                    if (planDetails.DeployToCustomerSubscription != null && planDetails.DeployToCustomerSubscription == true)
                                    {
                                        var keyvault = this.subscriptionsRepository.GetDeploymentConfig(subscriptionID);

                                        secretKey = keyvault.SecureId;
                                    }
                                    else
                                    {
                                        secretKey = string.Format("{0}secrets/HostedsubscriptionCredentials", this.keyVaultConfig.KeyVaultUrl);
                                    }

                                    this.logger.LogInformation("Get DoVault");
                                    string secretValue = this.azureKeyVaultClient.GetKeyAsync(secretKey).ConfigureAwait(false).GetAwaiter().GetResult();

                                    var credenitals = JsonConvert.DeserializeObject<CredentialsModel>(secretValue);
                                    this.logger.LogInformation("SecretValue : {0}", secretValue);
                                    this.logger.LogInformation("Start Deployment: DeployARMTemplate");
                                    string armTemplateCOntent = this.azureBlobFileClient.GetARMTemplateContentAsString(armTemplate.ArmtempalteName);

                                    var output = this.armTemplateDeploymentManager.DeployARMTemplate(armTemplate, parametersList, credenitals, armTemplateCOntent).ConfigureAwait(false).GetAwaiter().GetResult();

                                    string outputstring = JsonConvert.SerializeObject(output.Properties.Outputs);
                                    var outPutList = this.subscriptionService.GenerateParmlistFromResponse(output);
                                    if (outPutList != null)
                                    {
                                        this.subscriptionTemplateParametersRepository.Update(outPutList.ToList(), subscriptionID);
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
                                        CreateDate = DateTime.Now,
                                    };
                                    this.subscriptionLogRepository.Save(auditLog);
                                    this.subscriptionLogRepository.LogStatusDuringProvisioning(subscriptionID, armTemplate.ArmtempalteId, DeploymentStatusEnum.ARMTemplateDeploymentSuccess.ToString(), "Deployment Successful", SubscriptionStatusEnumExtension.DeploymentSuccessful.ToString());
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Change status to  ARMTemplateDeploymentFailure
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
                            CreateDate = DateTime.Now,
                        };
                        this.subscriptionLogRepository.Save(auditLog);
                    }
                }
            }
        }
    }
}