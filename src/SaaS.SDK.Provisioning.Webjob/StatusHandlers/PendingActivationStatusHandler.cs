using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Contracts;
using Microsoft.Marketplace.SaasKit.Models;
using Microsoft.Marketplace.SaaS.SDK.Services.Models;
using Microsoft.Marketplace.SaasKit.Provisioning.Webjob;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.Marketplace.SaaS.SDK.Services.Helpers;

namespace Microsoft.Marketplace.SaasKit.Provisioning.Webjob.StatusHandlers
{

    class PendingActivationStatusHandler : AbstractSubscriptionStatusHandler
    {

        readonly IFulfillmentApiClient fulfillApiclient;
        readonly IApplicationConfigRepository applicationConfigRepository;
        readonly ISubscriptionsRepository subscriptionsRepository;
        readonly ISubscriptionLogRepository subscriptionLogRepository;
        readonly ISubscriptionTemplateParametersRepository subscriptionTemplateParametersRepository;
        public PendingActivationStatusHandler(IFulfillmentApiClient fulfillApiClient, IApplicationConfigRepository applicationConfigRepository, ISubscriptionsRepository subscriptionsRepository, ISubscriptionLogRepository subscriptionLogRepository,
            ISubscriptionTemplateParametersRepository subscriptionTemplateParametersRepository) : base(new SaasKitContext())
        {
            this.fulfillApiclient = fulfillApiClient;
            this.applicationConfigRepository = applicationConfigRepository;
            this.subscriptionsRepository = subscriptionsRepository;
            this.subscriptionLogRepository = subscriptionLogRepository;
            this.subscriptionTemplateParametersRepository = subscriptionTemplateParametersRepository;


        }
        public override void Process(Guid subscriptionID)
        {
            Console.WriteLine("PendingActivationStatusHandler {0}", subscriptionID);
            var subscription = this.GetSubscriptionById(subscriptionID);
            Console.WriteLine("Get User");

            Console.WriteLine("subscription : {0}", JsonConvert.SerializeObject(subscription));

            var userdeatils = this.GetUserById(subscription.UserId);

            if (subscription.SubscriptionStatus == SubscriptionStatusEnumExtension.PendingActivation.ToString() ||
                subscription.SubscriptionStatus == SubscriptionStatusEnumExtension.DeploymentSuccessful.ToString())
            {
                try
                {
                    Console.WriteLine("Get attributelsit");

                    var subscriptionData = this.fulfillApiclient.ActivateSubscriptionAsync(subscriptionID, subscription.AmpplanId).ConfigureAwait(false).GetAwaiter().GetResult();

                    //Console.WriteLine("subscriptionData : {0}", JsonConvert.SerializeObject(subscriptionData));
                    Console.WriteLine("UpdateWebJobSubscriptionStatus");

                    this.subscriptionsRepository.UpdateStatusForSubscription(subscriptionID, SubscriptionStatusEnumExtension.Subscribed.ToString(), true);

                    SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
                    {
                        Attribute = SubscriptionLogAttributes.Status.ToString(),
                        SubscriptionId = subscription.Id,
                        NewValue = SubscriptionStatusEnumExtension.Subscribed.ToString(),
                        OldValue = subscription.SubscriptionStatus,
                        CreateBy = userdeatils.UserId,
                        CreateDate = DateTime.Now
                    };
                    this.subscriptionLogRepository.Add(auditLog);
                }
                catch (Exception ex)
                {
                    string errorDescriptin = string.Format("Exception: {0} :: Innser Exception:{1}", ex.Message, ex.InnerException);
                   this.subscriptionLogRepository.LogStatusDuringProvisioning(subscriptionID, default, DeploymentStatusEnum.ARMTemplateDeploymentSuccess.ToString(), errorDescriptin, SubscriptionStatusEnumExtension.ActivationFailed.ToString());
                    Console.WriteLine(errorDescriptin);

                    this.subscriptionsRepository.UpdateStatusForSubscription(subscriptionID, SubscriptionStatusEnumExtension.ActivationFailed.ToString(), true);

                    // Activation Failure SubscriptionStatusEnumExtension.ActivationFailed

                    SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
                    {
                        Attribute = SubscriptionLogAttributes.Status.ToString(),
                        SubscriptionId = subscription.Id,
                        NewValue = SubscriptionStatusEnumExtension.ActivationFailed.ToString(),
                        OldValue = subscription.SubscriptionStatus,
                        CreateBy = userdeatils.UserId,
                        CreateDate = DateTime.Now
                    };
                    this.subscriptionLogRepository.Add(auditLog);


                    //Call Email helper with ActivationFailed
                }

            }
        }

    }
}
