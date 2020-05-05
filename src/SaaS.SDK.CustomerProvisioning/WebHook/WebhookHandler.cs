using Microsoft.Marketplace.SaasKit.Models;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using Microsoft.Marketplace.SaasKit.WebHook;
using Microsoft.Marketplace.SaaS.SDK.Services.Models;
using Microsoft.Marketplace.SaaS.SDK.Services.Services;
using System;
using System.Threading.Tasks;

namespace Microsoft.Marketplace.SaasKit.Client.WebHook
{
    /// <summary>
    /// Handler For the WebHook Actions
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.WebHook.IWebhookHandler" />
    public class WebHookHandler : IWebhookHandler
    {
        /// <summary>
        /// The application log repository
        /// </summary>
        private readonly IApplicationLogRepository ApplicationLogRepository;

        /// <summary>
        /// The subscriptions repository
        /// </summary>
        private readonly ISubscriptionsRepository SubscriptionsRepository;

        /// <summary>
        /// The plan repository
        /// </summary>
        private readonly IPlansRepository PlanRepository;

        /// <summary>
        /// The subscription service
        /// </summary>
        private readonly SubscriptionService subscriptionService;

        /// <summary>
        /// The application log service
        /// </summary>
        private readonly ApplicationLogService applicationLogService;

        /// <summary>
        /// The subscriptions log repository
        /// </summary>
        private readonly ISubscriptionLogRepository SubscriptionsLogRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebHookHandler"/> class.
        /// </summary>
        /// <param name="applicationLogRepository">The application log repository.</param>
        /// <param name="subscriptionsLogRepository">The subscriptions log repository.</param>
        /// <param name="subscriptionsRepository">The subscriptions repository.</param>
        /// <param name="planRepository">The plan repository.</param>
        public WebHookHandler(IApplicationLogRepository applicationLogRepository, ISubscriptionLogRepository subscriptionsLogRepository, ISubscriptionsRepository subscriptionsRepository, IPlansRepository planRepository)
        {
            ApplicationLogRepository = applicationLogRepository;
            SubscriptionsRepository = subscriptionsRepository;
            PlanRepository = planRepository;
            SubscriptionsLogRepository = subscriptionsLogRepository;
            applicationLogService = new ApplicationLogService(ApplicationLogRepository);
            subscriptionService = new SubscriptionService(SubscriptionsRepository, PlanRepository);
        }
        /// <summary>
        /// Changes the plan asynchronous.
        /// </summary>
        /// <param name="payload">The payload.</param>
        public async Task ChangePlanAsync(WebhookPayload payload)
        {
            var oldValue = subscriptionService.GetSubscriptionsBySubscriptionId(payload.SubscriptionId);

            subscriptionService.UpdateSubscriptionPlan(payload.SubscriptionId, payload.PlanId);
            applicationLogService.AddApplicationLog("Plan Successfully Changed.");

            if (oldValue != null)
            {
                SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
                {
                    Attribute = Convert.ToString(SubscriptionLogAttributes.Plan),
                    SubscriptionId = oldValue.SubscribeId,
                    NewValue = payload.PlanId,
                    OldValue = oldValue.PlanId,
                    CreateBy = null,
                    CreateDate = DateTime.Now
                };
                SubscriptionsLogRepository.Add(auditLog);
            }
            await Task.CompletedTask;
        }

        /// <summary>
        /// Changes the quantity asynchronous.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task ChangeQuantityAsync(WebhookPayload payload)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reinstated the asynchronous.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task ReinstatedAsync(WebhookPayload payload)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Suspended the asynchronous.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task SuspendedAsync(WebhookPayload payload)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Unsubscribed the asynchronous.
        /// </summary>
        /// <param name="payload">The payload.</param>
        public async Task UnsubscribedAsync(WebhookPayload payload)
        {
            var oldValue = subscriptionService.GetSubscriptionsBySubscriptionId(payload.SubscriptionId);            
            subscriptionService.UpdateStateOfSubscription(payload.SubscriptionId, SubscriptionStatusEnumExtension.Unsubscribed.ToString(), false);
            applicationLogService.AddApplicationLog("Offer Successfully UnSubscribed.");
            
            if (oldValue != null)
            {
                SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
                {
                    Attribute = Convert.ToString(SubscriptionLogAttributes.Status),
                    SubscriptionId = oldValue.SubscribeId,
                    NewValue = Convert.ToString(SubscriptionStatusEnum.Unsubscribed),
                    OldValue = Convert.ToString(oldValue.SaasSubscriptionStatus),
                    CreateBy = null,
                    CreateDate = DateTime.Now
                };
                SubscriptionsLogRepository.Add(auditLog);
            }

            //KB: Email notification missing.
            //Trigger the webjob so that the notifications can get processed. Set the current status as Unsubscribed.
            await Task.CompletedTask;
        }
    }
}
