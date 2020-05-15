namespace SaaS.SDK.Provisioning.Webjob
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using Microsoft.Marketplace.SaaS.SDK.Services.Contracts;
    using Microsoft.Marketplace.SaaS.SDK.Services.Helpers;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Contracts;
    using Microsoft.Marketplace.SaasKit.Provisioning.Webjob.StatusHandlers;
    using Newtonsoft.Json;

    /// <summary>
    /// Processor for the trigger from storage queue. Move the subscription through statuses.
    /// </summary>
    public class Functions
    {
        /// <summary>
        /// The fulfillment API client.
        /// </summary>
        private readonly IFulfillmentApiClient fulfillmentApiClient;

        /// <summary>
        /// The subscription repository.
        /// </summary>
        private readonly ISubscriptionsRepository subscriptionRepository;

        /// <summary>
        /// The application configrepository.
        /// </summary>
        private readonly IApplicationConfigRepository applicationConfigrepository;

        /// <summary>
        /// The subscription log repository.
        /// </summary>
        private readonly ISubscriptionLogRepository subscriptionLogRepository;

        /// <summary>
        /// The email templaterepository.
        /// </summary>
        private readonly IEmailTemplateRepository emailTemplaterepository;

        /// <summary>
        /// The plan events mapping repository.
        /// </summary>
        private readonly IPlanEventsMappingRepository planEventsMappingRepository;

        /// <summary>
        /// The offer attributes repository.
        /// </summary>
        private readonly IOfferAttributesRepository offerAttributesRepository;

        /// <summary>
        /// The events repository.
        /// </summary>
        private readonly IEventsRepository eventsRepository;

        /// <summary>
        /// The azure key vault client.
        /// </summary>
        private readonly IVaultService azureKeyVaultClient;

        ///--Prasad--
        ///// <summary>
        ///// The azure BLOB file client.
        ///// </summary>
        //private readonly IARMTemplateStorageService azureBlobFileClient;

        /// <summary>
        /// The plan repository.
        /// </summary>
        private readonly IPlansRepository planRepository;

        /// <summary>
        /// The offers repository.
        /// </summary>
        private readonly IOffersRepository offersRepository;

        /// <summary>
        /// The users repository.
        /// </summary>
        private readonly IUsersRepository usersRepository;


        /// <summary>
        /// The emial service.
        /// </summary>
        private readonly IEmailService emialService;

        /// <summary>
        /// The activate status handlers.
        /// </summary>
        private readonly List<ISubscriptionStatusHandler> activateStatusHandlers;

        /// <summary>
        /// The deactivate status handlers.
        /// </summary>
        private readonly List<ISubscriptionStatusHandler> deactivateStatusHandlers;

        /// <summary>
        /// The key vault configuration.
        /// </summary>
        private readonly KeyVaultConfig keyVaultConfig;

        /// <summary>
        /// The email helper.
        /// </summary>
        private readonly EmailHelper emailHelper;

        ///--Prasad--
        /// <summary>
        /// The arm template repository.
        /// </summary>
        //private readonly IArmTemplateRepository armTemplateRepository;

        /// <summary>
        /// The logger factory.
        /// </summary>
        private readonly ILoggerFactory loggerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="Functions"/> class.
        /// </summary>
        /// <param name="fulfillmentApiClient">The fulfillment API client.</param>
        /// <param name="subscriptionRepository">The subscription repository.</param>
        /// <param name="applicationConfigRepository">The application configuration repository.</param>
        /// <param name="subscriptionLogRepository">The subscription log repository.</param>
        /// <param name="emailTemplaterepository">The email templaterepository.</param>
        /// <param name="planEventsMappingRepository">The plan events mapping repository.</param>
        /// <param name="offerAttributesRepository">The offer attributes repository.</param>
        /// <param name="eventsRepository">The events repository.</param>
        /// <param name="azureKeyVaultClient">The azure key vault client.</param>
        /// <param name="planRepository">The plan repository.</param>
        /// <param name="offersRepository">The offers repository.</param>
        /// <param name="usersRepository">The users repository.</param>
        /// <param name="armTemplateRepository">The arm template repository.</param>
        /// <param name="azureBlobFileClient">The azure BLOB file client.</param>
        /// <param name="subscriptionTemplateParametersRepository">The subscription template parameters repository.</param>
        /// <param name="keyVaultConfig">The key vault configuration.</param>
        /// <param name="emailService">The email service.</param>
        /// <param name="emailHelper">The email helper.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public Functions(
                            IFulfillmentApiClient fulfillmentApiClient,
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
                            KeyVaultConfig keyVaultConfig,
                            IEmailService emailService,
                            EmailHelper emailHelper,
                            ILoggerFactory loggerFactory)
        {
            this.fulfillmentApiClient = fulfillmentApiClient;
            this.subscriptionRepository = subscriptionRepository;
            this.azureKeyVaultClient = azureKeyVaultClient;
            this.applicationConfigrepository = applicationConfigRepository;
            this.emailTemplaterepository = emailTemplaterepository;
            this.planEventsMappingRepository = planEventsMappingRepository;
            this.offerAttributesRepository = offerAttributesRepository;
            this.eventsRepository = eventsRepository;
            this.subscriptionLogRepository = subscriptionLogRepository;
            this.planRepository = planRepository;
            this.offersRepository = offersRepository;
            this.usersRepository = usersRepository;
            this.keyVaultConfig = keyVaultConfig;
            this.emialService = emailService;
            this.emailHelper = emailHelper;
            this.loggerFactory = loggerFactory;

            this.activateStatusHandlers = new List<ISubscriptionStatusHandler>();
            this.deactivateStatusHandlers = new List<ISubscriptionStatusHandler>();


            this.activateStatusHandlers.Add(new PendingActivationStatusHandler(
                                                                            fulfillmentApiClient,
                                                                            subscriptionRepository,
                                                                            subscriptionLogRepository,
                                                                            planRepository,
                                                                            usersRepository,
                                                                            this.loggerFactory.CreateLogger<PendingActivationStatusHandler>()));

            this.activateStatusHandlers.Add(new PendingFulfillmentStatusHandler(
                                                                            fulfillmentApiClient,
                                                                            this.applicationConfigrepository,
                                                                            subscriptionRepository,
                                                                            subscriptionLogRepository,
                                                                            planRepository,
                                                                            usersRepository,
                                                                            this.loggerFactory.CreateLogger<PendingFulfillmentStatusHandler>()));

            this.activateStatusHandlers.Add(new NotificationStatusHandler(
                                                                        fulfillmentApiClient,
                                                                        planRepository,
                                                                        this.applicationConfigrepository,
                                                                        emailTemplaterepository,
                                                                        planEventsMappingRepository,
                                                                        offerAttributesRepository,
                                                                        eventsRepository,
                                                                        subscriptionRepository,
                                                                        usersRepository,
                                                                        offersRepository,
                                                                        emailService,
                                                                        this.loggerFactory.CreateLogger<NotificationStatusHandler>()));

            this.deactivateStatusHandlers.Add(new PendingDeleteStatusHandler(
                                                                            fulfillmentApiClient,
                                                                            this.applicationConfigrepository,
                                                                            subscriptionLogRepository,
                                                                            subscriptionRepository,
                                                                            azureKeyVaultClient,
                                                                            keyVaultConfig,
                                                                            planRepository,
                                                                            usersRepository,
                                                                            this.loggerFactory.CreateLogger<PendingDeleteStatusHandler>()
                                                                            ));

            this.deactivateStatusHandlers.Add(new UnsubscribeStatusHandler(
                                                                        fulfillmentApiClient,
                                                                        subscriptionRepository,
                                                                        subscriptionLogRepository,
                                                                        planRepository,
                                                                        usersRepository,
                                                                        this.loggerFactory.CreateLogger<UnsubscribeStatusHandler>()));

            this.deactivateStatusHandlers.Add(new NotificationStatusHandler(
                                                                        fulfillmentApiClient,
                                                                        planRepository,
                                                                        this.applicationConfigrepository,
                                                                        emailTemplaterepository,
                                                                        planEventsMappingRepository,
                                                                        offerAttributesRepository,
                                                                        eventsRepository,
                                                                        subscriptionRepository,
                                                                        usersRepository,
                                                                        offersRepository,
                                                                        emailService,
                                                                        this.loggerFactory.CreateLogger<NotificationStatusHandler>()));
        }

        /// <summary>
        /// Processes the queue message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="logger">The logger.</param>
        public void ProcessQueueMessage([QueueTrigger("saas-provisioning-queue")] string message, Microsoft.Extensions.Logging.ILogger logger)
        {
            try
            {
                logger.LogInformation($"Payload received for the webjob as {message}");

                var model = JsonConvert.DeserializeObject<SubscriptionProcessQueueModel>(message);

                if ("Activate".Equals(model.TriggerEvent, StringComparison.InvariantCultureIgnoreCase))
                {
                    foreach (var subscriptionStatusHandler in this.activateStatusHandlers)
                    {
                        subscriptionStatusHandler.Process(model.SubscriptionID);
                    }
                }

                if ("Unsubscribe".Equals(model.TriggerEvent, StringComparison.InvariantCultureIgnoreCase))
                {
                    foreach (var subscriptionStatusHandler in this.deactivateStatusHandlers)
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
