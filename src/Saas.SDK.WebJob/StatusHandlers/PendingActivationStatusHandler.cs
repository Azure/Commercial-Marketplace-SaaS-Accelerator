using Microsoft.Marketplace.SaasKit.Contracts;
using Microsoft.Marketplace.SaasKit.WebJob;
using System;
using System.Collections.Generic;
using System.Text;

namespace Saas.SDK.WebJob.StatusHandlers
{


    class PendingActivationStatusHandler : AbstractSubscriptionStatusHandler
    {

        readonly IFulfillmentApiClient fulfillApiClient;

        public PendingActivationStatusHandler(IFulfillmentApiClient fulfillApiClient)
        {
            this.fulfillApiClient = fulfillApiClient;

        }
      public override void Process(Guid subscriptionID)
        {
            var subscription = this.GetSubscriptionById(subscriptionID);

            if (subscription.SubscriptionStatus == "PendingActivation")
            {
                var subscriptionData = this.fulfillApiClient.GetSubscriptionByIdAsync(subscriptionID).ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }

    }
}
