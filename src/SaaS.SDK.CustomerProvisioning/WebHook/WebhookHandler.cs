namespace Microsoft.Marketplace.SaasKit.Client.WebHook
{
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using Microsoft.Marketplace.SaaS.SDK.Services.Services;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using Microsoft.Marketplace.SaasKit.Models;
    using Microsoft.Marketplace.SaasKit.WebHook;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Queue;

    /// <summary>
    /// Handler For the WebHook Actions.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.WebHook.IWebhookHandler" />
    public class WebHookHandler : IWebhookHandler
    {
        /// <summary>
        /// The application log repository.
        /// </summary>
        private readonly IApplicationLogRepository applicationLogRepository;

        /// <summary>
        /// The subscriptions repository.
        /// </summary>
        private readonly ISubscriptionsRepository subscriptionsRepository;

        /// <summary>
        /// The plan repository.
        /// </summary>
        private readonly IPlansRepository planRepository;

        /// <summary>
        /// The subscription service.
        /// </summary>
        private readonly SubscriptionService subscriptionService;

        /// <summary>
        /// The application log service.
        /// </summary>
        private readonly ApplicationLogService applicationLogService;

        /// <summary>
        /// The subscriptions log repository.
        /// </summary>
        private readonly ISubscriptionLogRepository subscriptionsLogRepository;

        private readonly CloudStorageConfigs cloudConfigs;

        private string azureWebJobsStorage;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebHookHandler" /> class.
        /// </summary>
        /// <param name="applicationLogRepository">The application log repository.</param>
        /// <param name="subscriptionsLogRepository">The subscriptions log repository.</param>
        /// <param name="subscriptionsRepository">The subscriptions repository.</param>
        /// <param name="planRepository">The plan repository.</param>
        /// <param name="cloudConfigs">The cloud configs.</param>
        public WebHookHandler(IApplicationLogRepository applicationLogRepository, ISubscriptionLogRepository subscriptionsLogRepository, ISubscriptionsRepository subscriptionsRepository, IPlansRepository planRepository, CloudStorageConfigs cloudConfigs)
        {
            this.applicationLogRepository = applicationLogRepository;
            this.subscriptionsRepository = subscriptionsRepository;
            this.planRepository = planRepository;
            this.subscriptionsLogRepository = subscriptionsLogRepository;
            this.applicationLogService = new ApplicationLogService(this.applicationLogRepository);
            this.subscriptionService = new SubscriptionService(this.subscriptionsRepository, this.planRepository);
            this.cloudConfigs = cloudConfigs;
            this.azureWebJobsStorage = cloudConfigs.AzureWebJobsStorage;
        }

        /// <summary>
        /// Changes the plan asynchronous.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task ChangePlanAsync(WebhookPayload payload)
        {
            var oldValue = this.subscriptionService.GetSubscriptionsBySubscriptionId(payload.SubscriptionId);

            this.subscriptionService.UpdateSubscriptionPlan(payload.SubscriptionId, payload.PlanId);
            this.applicationLogService.AddApplicationLog("Plan Successfully Changed.");

            if (oldValue != null)
            {
                SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
                {
                    Attribute = Convert.ToString(SubscriptionLogAttributes.Plan),
                    SubscriptionId = oldValue.SubscribeId,
                    NewValue = payload.PlanId,
                    OldValue = oldValue.PlanId,
                    CreateBy = null,
                    CreateDate = DateTime.Now,
                };
                this.subscriptionsLogRepository.Save(auditLog);
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// Changes the quantity asynchronous.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <returns>
        /// Change QuantityAsync.
        /// </returns>
        /// <exception cref="NotImplementedException"> Exception.</exception>
        public Task ChangeQuantityAsync(WebhookPayload payload)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reinstated the asynchronous.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <returns> Exception.</returns>
        /// <exception cref="NotImplementedException"> Not Implemented Exception. </exception>
        public Task ReinstatedAsync(WebhookPayload payload)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Suspended the asynchronous.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <returns> Exception.</returns>
        /// <exception cref="NotImplementedException"> Implemented Exception.</exception>
        public Task SuspendedAsync(WebhookPayload payload)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Unsubscribed the asynchronous.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task UnsubscribedAsync(WebhookPayload payload)
        {
            var oldValue = this.subscriptionService.GetSubscriptionsBySubscriptionId(payload.SubscriptionId);
            this.subscriptionService.UpdateStateOfSubscription(payload.SubscriptionId, SubscriptionStatusEnumExtension.Unsubscribed.ToString(), false);
            this.applicationLogService.AddApplicationLog("Offer Successfully UnSubscribed.");

            if (oldValue != null)
            {
                SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
                {
                    Attribute = Convert.ToString(SubscriptionLogAttributes.Status),
                    SubscriptionId = oldValue.SubscribeId,
                    NewValue = Convert.ToString(SubscriptionStatusEnum.Unsubscribed),
                    OldValue = Convert.ToString(oldValue.SaasSubscriptionStatus),
                    CreateBy = null,
                    CreateDate = DateTime.Now,
                };
                this.subscriptionsLogRepository.Save(auditLog);
            }

            SubscriptionProcessQueueModel queueObject = new SubscriptionProcessQueueModel();

            queueObject.SubscriptionID = payload.SubscriptionId;
            queueObject.TriggerEvent = "Unsubscribe";
            queueObject.UserId = 0;
            queueObject.PortalName = "Admin";
            await Task.CompletedTask;

            string queueMessage = JsonSerializer.Serialize(queueObject);
            string storageConnectionString = this.cloudConfigs.AzureWebJobsStorage ?? this.azureWebJobsStorage;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            //// Create the queue client.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("saas-provisioning-queue");

            ////Create the queue if it doesn't already exist
            queue.CreateIfNotExistsAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            //// Create a message and add it to the queue.
            CloudQueueMessage message = new CloudQueueMessage(queueMessage);
            queue.AddMessageAsync(message).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
