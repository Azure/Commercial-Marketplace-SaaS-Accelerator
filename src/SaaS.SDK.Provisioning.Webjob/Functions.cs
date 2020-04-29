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
        protected readonly ISubscriptionLogRepository subscriptionLogrepository;
        protected readonly IEmailTemplateRepository emailTemplaterepository;
        protected readonly IPlanEventsMappingRepository planEventsMappingrepository;
        protected readonly IOfferAttributesRepository offerAttributesrepository;
        protected readonly IEventsRepository eventsrepository;
        protected readonly IAzureKeyVaultClient azureKeyVaultClient;


        private readonly List<ISubscriptionStatusHandler> activateStatusHandlers;
        private readonly List<ISubscriptionStatusHandler> deactivateStatusHandlers;

        public Functions(IFulfillmentApiClient fulfillmentApiClient,
                            ISubscriptionsRepository subscriptionRepository,
                            IApplicationConfigRepository applicationConfigRepository,
                            ISubscriptionLogRepository subscriptionLogRepository, 
                            IEmailTemplateRepository emailTemplateRepository,
                            IPlanEventsMappingRepository planEventsMappingRepository, 
                            IOfferAttributesRepository offerAttributesRepository,
                            IEventsRepository eventsRepository,
                            IAzureKeyVaultClient azureKeyVaultClient)
        {
            this.fulfillmentApiClient = fulfillmentApiClient;
            this.subscriptionRepository = subscriptionRepository;
            this.azureKeyVaultClient = azureKeyVaultClient;

            this.activateStatusHandlers = new List<ISubscriptionStatusHandler>();
            this.deactivateStatusHandlers = new List<ISubscriptionStatusHandler>();

            // Add status handlers
            activateStatusHandlers.Add(new ResourceDeploymentStatusHandler(fulfillmentApiClient, applicationConfigrepository, subscriptionLogrepository, subscriptionRepository, azureKeyVaultClient));
            activateStatusHandlers.Add(new PendingActivationStatusHandler(fulfillmentApiClient, applicationConfigrepository, subscriptionRepository, subscriptionLogrepository));
            activateStatusHandlers.Add(new NotificationStatusHandler(fulfillmentApiClient, applicationConfigrepository, emailTemplaterepository, planEventsMappingrepository, offerAttributesrepository, eventsrepository, subscriptionRepository));

            deactivateStatusHandlers.Add(new PendingDeleteStatusHandler(fulfillmentApiClient, applicationConfigrepository, subscriptionLogrepository, subscriptionRepository, azureKeyVaultClient));
            deactivateStatusHandlers.Add(new UnsubscribeStatusHandler(fulfillmentApiClient, applicationConfigrepository, subscriptionRepository, subscriptionLogrepository));
            deactivateStatusHandlers.Add(new NotificationStatusHandler(fulfillmentApiClient, applicationConfigrepository, emailTemplaterepository, planEventsMappingrepository, offerAttributesrepository, eventsrepository, subscriptionRepository));
        }

        public void ProcessQueueMessage([QueueTrigger("saas-provisioning-queue")] string message,
                                                                               Microsoft.Extensions.Logging.ILogger logger)
        {
            try
            {
                logger.LogInformation($"{message} and FulfillmentClient is null : ${fulfillmentApiClient == null}");
                //Guid subscriptionid = Guid.Parse("c07e41d0-67ac-73a4-4eef-c6f18dee7000");
                //var subscriptionData = this.fulfillmentApiClient.ActivateSubscriptionAsync(subscriptionid, "tiered-plan-with-onetime-fee").ConfigureAwait(false).GetAwaiter().GetResult();
                // Deserialize the message as object
                // Do process
                SubscriptionProcessQueueModel model = new SubscriptionProcessQueueModel();
                model = JsonConvert.DeserializeObject<SubscriptionProcessQueueModel>(message);

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
