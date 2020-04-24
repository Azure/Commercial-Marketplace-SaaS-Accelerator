using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Contracts;
using Microsoft.Marketplace.SaasKit.WebJob;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.WebJob.StatusHandlers
{

    class NotificationStatusHandler : AbstractSubscriptionStatusHandler
    {

        readonly IFulfillmentApiClient fulfillApiclient;

        public NotificationStatusHandler(IFulfillmentApiClient fulfillApiClient) : base(new SaasKitContext())
        {
            this.fulfillApiclient = fulfillApiClient;

        }
        public override void Process(Guid subscriptionID)
        {
            var subscription = this.GetSubscriptionById(subscriptionID);

            if (subscription.SubscriptionStatus == "Subscribed")
            {
                var subscriptionData = this.fulfillApiclient.GetSubscriptionByIdAsync(subscriptionID).ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }

    }
}
