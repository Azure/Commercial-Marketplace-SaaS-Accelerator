using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Contracts;
using Microsoft.Marketplace.SaasKit.Provisioning.Webjob.Models;
using Microsoft.Marketplace.SaasKit.Provisioning.Webjob.StatusHandlers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaaS.SDK.Provisioning.Webjob
{
    public class Functions
    {
        protected readonly IFulfillmentApiClient fulfillmentApiClient;
        protected readonly ISubscriptionsRepository subscriptionRepository;

        private readonly IApplicationConfigRepository applicationConfigrepository;
        private readonly ISubscriptionLogRepository subscriptionLogrepository;
        private readonly IEmailTemplateRepository emailTemplaterepository;
        private readonly IPlanEventsMappingRepository planEventsMappingrepository;
        private readonly IOfferAttributesRepository offerAttributesrepository;
        private readonly IEventsRepository eventsrepository;





        public Functions(IFulfillmentApiClient fulfillmentApiClient,
                            ISubscriptionsRepository subscriptionRepository,
                            IApplicationConfigRepository applicationConfigRepository,
                            ISubscriptionLogRepository subscriptionLogRepository, IEmailTemplateRepository emailTemplateRepository,
        IPlanEventsMappingRepository planEventsMappingRepository, IOfferAttributesRepository offerAttributesRepository,
           IEventsRepository eventsRepository)
        {
            this.fulfillmentApiClient = fulfillmentApiClient;
            this.subscriptionRepository = subscriptionRepository;
        }


        protected static List<ISubscriptionStatusHandler> activateStatusHandlers = new List<ISubscriptionStatusHandler>()
        {
            new ResourceDeploymentStatusHandler(fulfillmentApiClient,applicationConfigrepository,subscriptionLogrepository,subscriptionsrepository),
            new PendingActivationStatusHandler(fulfillApiclient,applicationConfigrepository,subscriptionsrepository,subscriptionLogrepository),
            new NotificationStatusHandler(fulfillApiclient,applicationConfigrepository,emailTemplaterepository,planEventsMappingrepository,offerAttributesrepository,eventsrepository,subscriptionsrepository)
        };
        protected static List<ISubscriptionStatusHandler> deactivateStatusHandlers = new List<ISubscriptionStatusHandler>()
        {

            new PendingDeleteStatusHandler(fulfillApiclient,applicationConfigrepository,subscriptionLogrepository,subscriptionsrepository),
            new UnsubscribeStatusHandler(fulfillApiclient,applicationConfigrepository,subscriptionsrepository,subscriptionLogrepository),
            new NotificationStatusHandler(fulfillApiclient,applicationConfigrepository,emailTemplaterepository,planEventsMappingrepository,offerAttributesrepository,eventsrepository,subscriptionsrepository)
        };

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
