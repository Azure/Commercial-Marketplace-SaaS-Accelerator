using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Contracts;
using Microsoft.Marketplace.SaasKit.WebJob;
using Saas.SDK.WebJob.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.WebJob.StatusHandlers
{

    class PendingActivationStatusHandler : AbstractSubscriptionStatusHandler
    {

        private readonly IFulfillmentApiClient fulfillApiclient;
        private readonly IApplicationConfigRepository applicationConfigRepository;
        private readonly ISubscriptionsRepository subscriptionsRepository;

        public PendingActivationStatusHandler(IFulfillmentApiClient fulfillApiClient, IApplicationConfigRepository applicationConfigRepository, ISubscriptionsRepository subscriptionsRepository) : base(new SaasKitContext())
        {
            this.fulfillApiclient = fulfillApiClient;
            this.applicationConfigRepository = applicationConfigRepository;
            this.subscriptionsRepository = subscriptionsRepository;


        }
        public override void Process(Guid subscriptionID)
        {
            var subscription = this.GetSubscriptionById(subscriptionID);

            if (subscription.SubscriptionStatus == "PendingActivation")
            {
                bool isDeploymentRequired = false;
                if (!isDeploymentRequired)
                {
                    try
                    {
                        var subscriptionData = this.fulfillApiclient.ActivateSubscriptionAsync(subscriptionID, subscription.AmpplanId).ConfigureAwait(false).GetAwaiter().GetResult();
                        this.subscriptionsRepository.UpdateStatusForSubscription(subscriptionID, SubscriptionWebJobStatusEnum.PendingActivation.ToString(), true);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                else
                {


                    // code to get arm template and start deployment
                }

            }
        }

    }
}
