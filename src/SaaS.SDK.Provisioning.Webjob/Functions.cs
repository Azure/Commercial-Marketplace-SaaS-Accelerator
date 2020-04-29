using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaaS.SDK.Provisioning.Webjob
{
    public class Functions
    {
        protected readonly IFulfillmentApiClient fulfillmentApiClient;
        protected readonly ISubscriptionsRepository subscriptionRepository;

        public Functions(IFulfillmentApiClient fulfillmentApiClient,
                            ISubscriptionsRepository subscriptionRepository)
        {
            this.fulfillmentApiClient = fulfillmentApiClient;
            this.subscriptionRepository = subscriptionRepository;
        }

        public void ProcessQueueMessage([QueueTrigger("saas-provisioning-queue")] string message,
                                                                                           Microsoft.Extensions.Logging.ILogger logger)
        {
            try
            {
                logger.LogInformation($"{message} and FulfillmentClient is null : ${fulfillmentApiClient == null}");
                Guid subscriptionid = Guid.Parse("c07e41d0-67ac-73a4-4eef-c6f18dee7000");
                var subscriptionData = this.fulfillmentApiClient.ActivateSubscriptionAsync(subscriptionid, "tiered-plan-with-onetime-fee").ConfigureAwait(false).GetAwaiter().GetResult();
                // Deserialize the message as object
                // Do process
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
