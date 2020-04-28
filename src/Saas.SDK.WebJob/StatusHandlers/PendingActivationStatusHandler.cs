using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Contracts;
using Microsoft.Marketplace.SaasKit.Models;
using Microsoft.Marketplace.SaasKit.WebJob.Models;
using Microsoft.Marketplace.SaasKit.WebJob;
using Microsoft.Marketplace.SaasKit.WebJob.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.Marketplace.SaasKit.WebJob.Helpers;

namespace Microsoft.Marketplace.SaasKit.WebJob.StatusHandlers
{

    class PendingActivationStatusHandler : AbstractSubscriptionStatusHandler
    {

        readonly IFulfillmentApiClient fulfillApiclient;
        readonly IApplicationConfigRepository applicationConfigRepository;
        readonly ISubscriptionsRepository subscriptionsRepository;
        readonly ISubscriptionLogRepository subscriptionLogRepository;

        public PendingActivationStatusHandler(IFulfillmentApiClient fulfillApiClient, IApplicationConfigRepository applicationConfigRepository, ISubscriptionsRepository subscriptionsRepository, ISubscriptionLogRepository subscriptionLogRepository) : base(new SaasKitContext())
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

            if (subscription.SubscriptionStatus == SubscriptionWebJobStatusEnum.PendingActivation.ToString() && deploymentStatus.DeploymentStatus == DeploymentStatusEnum.ARMTemplateDeploymentSuccess.ToString())
            {
                try
                {
                    Console.WriteLine("Get attributelsit");

                    var subscriptionData = this.fulfillApiclient.ActivateSubscriptionAsync(subscriptionID, subscription.AmpplanId).ConfigureAwait(false).GetAwaiter().GetResult();

                    Console.WriteLine("subscriptionData : {0}", JsonConvert.SerializeObject(subscriptionData));
                    Console.WriteLine("UpdateWebJobSubscriptionStatus");
                    this.subscriptionsRepository.UpdateStatusForSubscription(subscriptionID, SubscriptionWebJobStatusEnum.Subscribed.ToString(), true);

                    SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
                    {
                        Attribute = SubscriptionLogAttributes.Status.ToString(),
                        SubscriptionId = subscription.Id,
                        NewValue = SubscriptionWebJobStatusEnum.Subscribed.ToString(),
                        OldValue = subscription.SubscriptionStatus,
                        CreateBy = 0,
                        CreateDate = DateTime.Now
                    };
                    this.subscriptionLogRepository.Add(auditLog);
                }
                catch (Exception ex)
                {
                    string errorDescriptin = string.Format("Exception: {0} :: Innser Exception:{1}", ex.Message, ex.InnerException);
                    StatusUpadeHelpers.UpdateWebJobSubscriptionStatus(subscriptionID, default, DeploymentStatusEnum.ARMTemplateDeploymentSuccess.ToString(), errorDescriptin, Context, SubscriptionWebJobStatusEnum.ActivationFailure.ToString());
                    Console.WriteLine(errorDescriptin);

                    // Activation Failure SubscriptionWebJobStatusEnum.ActivationFailure

                    /*SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
                    {
                        Attribute = SubscriptionLogAttributes.Status.ToString(),
                        SubscriptionId = subscription.Id,
                        NewValue = SubscriptionWebJobStatusEnum.ActivationFailure.ToString(),
                        OldValue = "None",
                        CreateBy = 0,
                        CreateDate = DateTime.Now
                    };
                    this.subscriptionLogRepository.Add(auditLog);*/


                    //Call Email helper with ActivationFailure
                }

            }
        }

    }
}
