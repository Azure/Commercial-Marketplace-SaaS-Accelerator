using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Contracts;
using Microsoft.Marketplace.SaaS.SDK.Services.Models;
using Microsoft.Marketplace.SaasKit.Provisioning.Webjob.StatusHandlers;
using Newtonsoft.Json;
using Microsoft.Marketplace.SaaS.SDK.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Marketplace.SaaS.SDK.Services.Helpers;

namespace SaaS.SDK.Provisioning.Webjob
{
    public class Functions
    {
        protected readonly IFulfillmentApiClient fulfillmentApiClient;
        protected readonly ISubscriptionsRepository subscriptionRepository;

        protected readonly IApplicationConfigRepository applicationConfigrepository;
        protected readonly ISubscriptionLogRepository subscriptionLogRepository;
        protected readonly IEmailTemplateRepository emailTemplaterepository;
        protected readonly IPlanEventsMappingRepository planEventsMappingRepository;
        protected readonly IOfferAttributesRepository offerAttributesRepository;
        protected readonly IEventsRepository eventsRepository;
        protected readonly IVaultService azureKeyVaultClient;
        protected readonly IARMTemplateStorageService azureBlobFileClient;
        protected readonly IPlansRepository planRepository;
        protected readonly IOffersRepository offersRepository;
        protected readonly ISubscriptionTemplateParametersRepository subscriptionTemplateParametersRepository;
        protected readonly IEmailService emialService;
        private readonly List<ISubscriptionStatusHandler> activateStatusHandlers;
        private readonly List<ISubscriptionStatusHandler> deactivateStatusHandlers;
        protected readonly KeyVaultConfig keyVaultConfig;
        protected readonly EmailHelper emailHelper;
        protected readonly ILoggerFactory loggerFactory;

        public Functions(IFulfillmentApiClient fulfillmentApiClient,
                            ISubscriptionsRepository subscriptionRepository,
                            IApplicationConfigRepository applicationConfigRepository,
                            ISubscriptionLogRepository subscriptionLogRepository,
                            IEmailTemplateRepository emailTemplaterepository,
                            IPlanEventsMappingRepository planEventsMappingRepository,
                            IOfferAttributesRepository offerAttributesRepository,
                            IEventsRepository eventsRepository,
                            IVaultService azureKeyVaultClient,
                            IPlansRepository planRepository,
                            IOffersRepository offersRepository,
                            IARMTemplateStorageService azureBlobFileClient,
                            ISubscriptionTemplateParametersRepository subscriptionTemplateParametersRepository,
                            KeyVaultConfig keyVaultConfig,
                            IEmailService emialService,
                            EmailHelper emailHelper,
                            ILoggerFactory loggerFactory)
        {
            this.fulfillmentApiClient = fulfillmentApiClient;
            this.subscriptionRepository = subscriptionRepository;
            this.azureKeyVaultClient = azureKeyVaultClient;
            this.azureBlobFileClient = azureBlobFileClient;
            this.applicationConfigrepository = applicationConfigRepository;
            this.emailTemplaterepository = emailTemplaterepository;
            this.planEventsMappingRepository = planEventsMappingRepository;
            this.offerAttributesRepository = offerAttributesRepository;
            this.eventsRepository = eventsRepository;
            this.subscriptionLogRepository = subscriptionLogRepository;
            this.planRepository = planRepository;
            this.offersRepository = offersRepository;
            this.subscriptionTemplateParametersRepository = subscriptionTemplateParametersRepository;
            this.keyVaultConfig = keyVaultConfig;
            this.emialService = emialService;
            this.emailHelper = emailHelper;
            this.loggerFactory = loggerFactory;

            this.activateStatusHandlers = new List<ISubscriptionStatusHandler>();
            this.deactivateStatusHandlers = new List<ISubscriptionStatusHandler>();

            var armTemplateDeploymentManager = new ARMTemplateDeploymentManager(this.loggerFactory.CreateLogger<ARMTemplateDeploymentManager>());

            activateStatusHandlers.Add(new ResourceDeploymentStatusHandler(fulfillmentApiClient, applicationConfigrepository, subscriptionLogRepository, subscriptionRepository, azureKeyVaultClient, azureBlobFileClient, keyVaultConfig, this.loggerFactory.CreateLogger<ResourceDeploymentStatusHandler>(), armTemplateDeploymentManager));
            activateStatusHandlers.Add(new PendingActivationStatusHandler(fulfillmentApiClient, applicationConfigrepository, subscriptionRepository, subscriptionLogRepository, subscriptionTemplateParametersRepository));
            activateStatusHandlers.Add(new PendingFulfillmentStatusHandler(fulfillmentApiClient, applicationConfigrepository, subscriptionRepository, subscriptionLogRepository));
            activateStatusHandlers.Add(new NotificationStatusHandler(fulfillmentApiClient, planRepository, applicationConfigrepository, emailTemplaterepository, planEventsMappingRepository, offerAttributesRepository, eventsRepository, subscriptionRepository, offersRepository, subscriptionTemplateParametersRepository, emialService, emailHelper));

            
            deactivateStatusHandlers.Add(new PendingDeleteStatusHandler(fulfillmentApiClient, applicationConfigrepository, subscriptionLogRepository, subscriptionRepository, azureKeyVaultClient, keyVaultConfig, subscriptionTemplateParametersRepository,this.loggerFactory.CreateLogger< PendingDeleteStatusHandler>(), armTemplateDeploymentManager));
            deactivateStatusHandlers.Add(new UnsubscribeStatusHandler(fulfillmentApiClient, applicationConfigrepository, subscriptionRepository, subscriptionLogRepository));
            deactivateStatusHandlers.Add(new NotificationStatusHandler(fulfillmentApiClient, planRepository, applicationConfigrepository, emailTemplaterepository, planEventsMappingRepository, offerAttributesRepository, eventsRepository, subscriptionRepository, offersRepository, subscriptionTemplateParametersRepository, emialService, emailHelper));
        }

        public void ProcessQueueMessage([QueueTrigger("saas-provisioning-queue")] string message,
                                                                               Microsoft.Extensions.Logging.ILogger logger)
        {
            try
            {
                logger.LogInformation($"{message} and FulfillmentClient is null : ${fulfillmentApiClient == null}");

                SubscriptionProcessQueueModel delete = new SubscriptionProcessQueueModel()
                {
                    SubscriptionID = Guid.Parse("25A8379E-E87E-DDDF-C337-259F4FADB09D"),
                    TriggerEvent = "Activate"
                };
                message = JsonConvert.SerializeObject(delete);

                // Do process
                var model = JsonConvert.DeserializeObject<SubscriptionProcessQueueModel>(message);

                if (model.TriggerEvent == "Activate")
                {
                    foreach (var subscriptionStatusHandler in activateStatusHandlers)
                    {
                        subscriptionStatusHandler.Process(model.SubscriptionID);
                    }
                }
                if (model.TriggerEvent == "Unsubscribe")
                {
                    foreach (var subscriptionStatusHandler in deactivateStatusHandlers)
                    {
                        subscriptionStatusHandler.Process(model.SubscriptionID);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
