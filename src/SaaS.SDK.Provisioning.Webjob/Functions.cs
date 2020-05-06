namespace SaaS.SDK.Provisioning.Webjob
{
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using Microsoft.Marketplace.SaaS.SDK.Services.Contracts;
    using Microsoft.Marketplace.SaaS.SDK.Services.Helpers;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Contracts;
    using Microsoft.Marketplace.SaasKit.Provisioning.Webjob.StatusHandlers;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Processor for the trigger from storage queue. Move the subscription through statuses.
    /// </summary>
    public class Functions
    {
        /// <summary>
        /// The fulfillment API client
        /// </summary>
        protected readonly IFulfillmentApiClient fulfillmentApiClient;

        /// <summary>
        /// The subscription repository
        /// </summary>
        protected readonly ISubscriptionsRepository subscriptionRepository;

        /// <summary>
        /// The application configrepository
        /// </summary>
        protected readonly IApplicationConfigRepository applicationConfigrepository;

        /// <summary>
        /// The subscription log repository
        /// </summary>
        protected readonly ISubscriptionLogRepository subscriptionLogRepository;

        /// <summary>
        /// The email templaterepository
        /// </summary>
        protected readonly IEmailTemplateRepository emailTemplaterepository;

        /// <summary>
        /// The plan events mapping repository
        /// </summary>
        protected readonly IPlanEventsMappingRepository planEventsMappingRepository;

        /// <summary>
        /// The offer attributes repository
        /// </summary>
        protected readonly IOfferAttributesRepository offerAttributesRepository;

        /// <summary>
        /// The events repository
        /// </summary>
        protected readonly IEventsRepository eventsRepository;

        /// <summary>
        /// The azure key vault client
        /// </summary>
        protected readonly IVaultService azureKeyVaultClient;

        /// <summary>
        /// The azure BLOB file client
        /// </summary>
        protected readonly IARMTemplateStorageService azureBlobFileClient;

        /// <summary>
        /// The plan repository
        /// </summary>
        protected readonly IPlansRepository planRepository;

        /// <summary>
        /// The offers repository
        /// </summary>
        protected readonly IOffersRepository offersRepository;

        /// <summary>
        /// The users repository
        /// </summary>
        protected readonly IUsersRepository usersRepository;

        /// <summary>
        /// The subscription template parameters repository
        /// </summary>
        protected readonly ISubscriptionTemplateParametersRepository subscriptionTemplateParametersRepository;

        /// <summary>
        /// The emial service
        /// </summary>
        protected readonly IEmailService emialService;

        /// <summary>
        /// The activate status handlers
        /// </summary>
        private readonly List<ISubscriptionStatusHandler> activateStatusHandlers;

        /// <summary>
        /// The deactivate status handlers
        /// </summary>
        private readonly List<ISubscriptionStatusHandler> deactivateStatusHandlers;

        /// <summary>
        /// The key vault configuration
        /// </summary>
        protected readonly KeyVaultConfig keyVaultConfig;

        /// <summary>
        /// The email helper
        /// </summary>
        protected readonly EmailHelper emailHelper;

        /// <summary>
        /// The arm template deployment manager
        /// </summary>
        protected readonly ARMTemplateDeploymentManager armTemplateDeploymentManager;

        /// <summary>
        /// The arm template repository
        /// </summary>
        protected readonly IArmTemplateRepository armTemplateRepository;
        
        /// <summary>
        /// The logger factory
        /// </summary>
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
                            IUsersRepository usersRepository,
                            IArmTemplateRepository armTemplateRepository,
                            IARMTemplateStorageService azureBlobFileClient,
                            ISubscriptionTemplateParametersRepository subscriptionTemplateParametersRepository,
                            KeyVaultConfig keyVaultConfig,
                            IEmailService emailService,
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
            this.usersRepository = usersRepository;
            this.subscriptionTemplateParametersRepository = subscriptionTemplateParametersRepository;
            this.keyVaultConfig = keyVaultConfig;
            this.emialService = emailService;
            this.emailHelper = emailHelper;
            this.loggerFactory = loggerFactory;
            this.armTemplateRepository = armTemplateRepository;

            this.activateStatusHandlers = new List<ISubscriptionStatusHandler>();
            this.deactivateStatusHandlers = new List<ISubscriptionStatusHandler>();

            var armTemplateDeploymentManager = new ARMTemplateDeploymentManager(this.loggerFactory.CreateLogger<ARMTemplateDeploymentManager>());

            activateStatusHandlers.Add(new ResourceDeploymentStatusHandler(
                                                                            fulfillmentApiClient, 
                                                                            applicationConfigrepository, 
                                                                            subscriptionLogRepository, 
                                                                            subscriptionRepository, 
                                                                            azureKeyVaultClient, 
                                                                            azureBlobFileClient, 
                                                                            keyVaultConfig, 
                                                                            planRepository,
                                                                            usersRepository,
                                                                            this.offersRepository,
                                                                            this.armTemplateRepository,
                                                                            this.planEventsMappingRepository,
                                                                            this.eventsRepository,
                                                                            this.loggerFactory.CreateLogger<ResourceDeploymentStatusHandler>(), 
                                                                            armTemplateDeploymentManager));
            
            activateStatusHandlers.Add(new PendingActivationStatusHandler(
                                                                            fulfillmentApiClient, 
                                                                            applicationConfigrepository, 
                                                                            subscriptionRepository, 
                                                                            subscriptionLogRepository, 
                                                                            subscriptionTemplateParametersRepository,
                                                                            planRepository,
                                                                            usersRepository,
                                                                            this.loggerFactory.CreateLogger<PendingActivationStatusHandler>()));

            activateStatusHandlers.Add(new PendingFulfillmentStatusHandler(
                                                                            fulfillmentApiClient, 
                                                                            applicationConfigrepository, 
                                                                            subscriptionRepository, 
                                                                            subscriptionLogRepository,
                                                                            planRepository,
                                                                            usersRepository,
                                                                            this.loggerFactory.CreateLogger<PendingFulfillmentStatusHandler>()));
            
            activateStatusHandlers.Add(new NotificationStatusHandler(
                                                                        fulfillmentApiClient, 
                                                                        planRepository, 
                                                                        applicationConfigrepository, 
                                                                        emailTemplaterepository, 
                                                                        planEventsMappingRepository, 
                                                                        offerAttributesRepository, 
                                                                        eventsRepository, 
                                                                        subscriptionRepository,
                                                                        usersRepository,
                                                                        offersRepository,   
                                                                        subscriptionTemplateParametersRepository, 
                                                                        emailService, 
                                                                        emailHelper,
                                                                        this.loggerFactory.CreateLogger<NotificationStatusHandler>()));
                        
            deactivateStatusHandlers.Add(new PendingDeleteStatusHandler(
                                                                            fulfillmentApiClient, 
                                                                            applicationConfigrepository, 
                                                                            subscriptionLogRepository, 
                                                                            subscriptionRepository, 
                                                                            azureKeyVaultClient, 
                                                                            keyVaultConfig, 
                                                                            subscriptionTemplateParametersRepository,
                                                                            planRepository,
                                                                            usersRepository,
                                                                            this.loggerFactory.CreateLogger<PendingDeleteStatusHandler>(), 
                                                                            armTemplateDeploymentManager));

            deactivateStatusHandlers.Add(new UnsubscribeStatusHandler(
                                                                        fulfillmentApiClient, 
                                                                        applicationConfigrepository, 
                                                                        subscriptionRepository, 
                                                                        subscriptionLogRepository,
                                                                        planRepository,
                                                                        usersRepository,
                                                                        this.loggerFactory.CreateLogger<UnsubscribeStatusHandler>()
                                                                        ));

            deactivateStatusHandlers.Add(new NotificationStatusHandler(
                                                                        fulfillmentApiClient, 
                                                                        planRepository, 
                                                                        applicationConfigrepository, 
                                                                        emailTemplaterepository, 
                                                                        planEventsMappingRepository, 
                                                                        offerAttributesRepository, 
                                                                        eventsRepository, 
                                                                        subscriptionRepository, 
                                                                        usersRepository,
                                                                        offersRepository, 
                                                                        subscriptionTemplateParametersRepository, 
                                                                        emailService, 
                                                                        emailHelper,
                                                                        this.loggerFactory.CreateLogger<NotificationStatusHandler>()));
        }

        /// <summary>
        /// Processes the queue message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="logger">The logger.</param>
        public void ProcessQueueMessage([QueueTrigger("saas-provisioning-queue")] string message,
                                                                               Microsoft.Extensions.Logging.ILogger logger)
        {
            try
            {
                logger.LogInformation($"Payload received for the webjob as {message}");

                var model = JsonConvert.DeserializeObject<SubscriptionProcessQueueModel>(message);

                if ("Activate".Equals(model.TriggerEvent, StringComparison.InvariantCultureIgnoreCase))
                {
                    foreach (var subscriptionStatusHandler in activateStatusHandlers)
                    {
                        subscriptionStatusHandler.Process(model.SubscriptionID);
                    }
                }
                if ("Unsubscribe".Equals(model.TriggerEvent, StringComparison.InvariantCultureIgnoreCase))
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
