using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Contracts;
using Microsoft.Marketplace.SaasKit.Provisioning.Webjob.Models;
using Microsoft.Marketplace.SaasKit.Provisioning.Webjob.StatusHandlers;
using Newtonsoft.Json;
using SaaS.SDK.Provisioning.Webjob.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

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
        protected readonly IAzureKeyVaultClient azureKeyVaultClient;


        private readonly List<ISubscriptionStatusHandler> activateStatusHandlers;
        private readonly List<ISubscriptionStatusHandler> deactivateStatusHandlers;

        public Functions(IFulfillmentApiClient fulfillmentApiClient,
                            ISubscriptionsRepository subscriptionRepository,
                            IApplicationConfigRepository applicationConfigRepository,
                            ISubscriptionLogRepository subscriptionLogRepository,
                            IEmailTemplateRepository emailTemplaterepository,
                            IPlanEventsMappingRepository planEventsMappingRepository,
                            IOfferAttributesRepository offerAttributesRepository,
                            IEventsRepository eventsRepository,
                            IAzureKeyVaultClient azureKeyVaultClient)
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


            this.activateStatusHandlers = new List<ISubscriptionStatusHandler>();
            this.deactivateStatusHandlers = new List<ISubscriptionStatusHandler>();

            //activateStatusHandlers.Add(new ResourceDeploymentStatusHandler(fulfillmentApiClient, applicationConfigrepository, subscriptionLogRepository, subscriptionRepository, azureKeyVaultClient));
            //activateStatusHandlers.Add(new PendingActivationStatusHandler(fulfillmentApiClient, applicationConfigrepository, subscriptionRepository, subscriptionLogRepository));
            //activateStatusHandlers.Add(new PendingFulfillmentStatusHandler(fulfillmentApiClient, applicationConfigrepository, subscriptionRepository, subscriptionLogRepository));
            activateStatusHandlers.Add(new NotificationStatusHandler(fulfillmentApiClient, applicationConfigrepository, emailTemplaterepository, planEventsMappingRepository, offerAttributesRepository, eventsRepository, subscriptionRepository));


            //deactivateStatusHandlers.Add(new PendingDeleteStatusHandler(fulfillmentApiClient, applicationConfigrepository, subscriptionLogRepository, subscriptionRepository, azureKeyVaultClient));
            //deactivateStatusHandlers.Add(new UnsubscribeStatusHandler(fulfillmentApiClient, applicationConfigrepository, subscriptionRepository, subscriptionLogRepository));
            deactivateStatusHandlers.Add(new NotificationStatusHandler(fulfillmentApiClient, applicationConfigrepository, emailTemplaterepository, planEventsMappingRepository, offerAttributesRepository, eventsRepository, subscriptionRepository));
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
