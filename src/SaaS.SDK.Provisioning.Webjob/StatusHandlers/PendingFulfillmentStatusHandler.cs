using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Contracts;
using Microsoft.Marketplace.SaasKit.Provisioning.Webjob;
using Microsoft.Marketplace.SaasKit.Provisioning.Webjob.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using Microsoft.Marketplace.SaasKit.Provisioning.Webjob.Helpers;

namespace Microsoft.Marketplace.SaasKit.Provisioning.Webjob.StatusHandlers
{

    class PendingFulfillmentStatusHandler : AbstractSubscriptionStatusHandler
    {

        private readonly IFulfillmentApiClient fulfillApiclient;
        private readonly IApplicationConfigRepository applicationConfigRepository;
        private readonly ISubscriptionsRepository subscriptionsRepository;
        private readonly ISubscriptionLogRepository subscriptionLogRepository;

        public PendingFulfillmentStatusHandler(IFulfillmentApiClient fulfillApiClient, IApplicationConfigRepository applicationConfigRepository, ISubscriptionsRepository subscriptionsRepository, ISubscriptionLogRepository subscriptionLogRepository) : base(new SaasKitContext())
        {
            this.fulfillApiclient = fulfillApiClient;
            this.applicationConfigRepository = applicationConfigRepository;
            this.subscriptionsRepository = subscriptionsRepository;
            this.subscriptionLogRepository = subscriptionLogRepository;
        }
        public override void Process(Guid subscriptionID)
        {
            Console.WriteLine("PendingActivationStatusHandler {0}", subscriptionID);
            var subscription = this.GetSubscriptionById(subscriptionID);
            Console.WriteLine("subscription : {0}", JsonConvert.SerializeObject(subscription));
            var deploymentStatus = Context.WebJobSubscriptionStatus.Where(s => s.SubscriptionId == subscriptionID).FirstOrDefault();


            if (subscription.SubscriptionStatus == SubscriptionWebJobStatusEnum.PendingFulfillmentStart.ToString())
            {
                try
                {

                    this.subscriptionsRepository.UpdateStatusForSubscription(subscriptionID, SubscriptionWebJobStatusEnum.PendingActivation.ToString(), true);

                    SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
                    {
                        Attribute = SubscriptionLogAttributes.Status.ToString(),
                        SubscriptionId = subscription.Id,
                        NewValue = SubscriptionWebJobStatusEnum.PendingActivation.ToString(),
                        OldValue = subscription.SubscriptionStatus,
                        CreateBy = 0,
                        CreateDate = DateTime.Now
                    };
                    this.subscriptionLogRepository.Add(auditLog);
                }
                catch (Exception ex)
                {
                    string errorDescriptin = string.Format("Exception: {0} :: Innser Exception:{1}", ex.Message, ex.InnerException);

                    Console.WriteLine(errorDescriptin);


                    this.subscriptionsRepository.UpdateStatusForSubscription(subscriptionID, SubscriptionWebJobStatusEnum.PendingActivation.ToString(), true);


                    SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
                    {
                        Attribute = SubscriptionLogAttributes.Status.ToString(),
                        SubscriptionId = subscription.Id,
                        NewValue = SubscriptionWebJobStatusEnum.PendingActivation.ToString(),
                        OldValue = subscription.SubscriptionStatus,
                        CreateBy = 0,
                        CreateDate = DateTime.Now
                    };
                    this.subscriptionLogRepository.Add(auditLog);
                }
            }
        }
    }

}

