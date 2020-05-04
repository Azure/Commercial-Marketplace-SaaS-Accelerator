namespace Microsoft.Marketplace.SaasKit.Client.Controllers.WebHook
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaaS.SDK.Services.Services;
    using Microsoft.Marketplace.SaasKit.WebHook;
    using Newtonsoft.Json;

    /// <summary>
    /// Azure Web hook
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("api/[controller]")]
    [ApiController]
    public class AzureWebhookController : ControllerBase
    {
        /// <summary>
        /// The application log repository
        /// </summary>
        private readonly IApplicationLogRepository applicationLogRepository;

        /// <summary>
        /// The subscriptions repository
        /// </summary>
        private readonly ISubscriptionsRepository subscriptionsRepository;

        /// <summary>
        /// The plan repository
        /// </summary>
        private readonly IPlansRepository planRepository;

        /// <summary>
        /// The subscriptions log repository
        /// </summary>
        private readonly ISubscriptionLogRepository subscriptionsLogRepository;

        /// <summary>
        /// The web hook processor
        /// </summary>
        private readonly IWebhookProcessor webhookProcessor;

        /// <summary>
        /// The application log service
        /// </summary>
        private readonly ApplicationLogService applicationLogService;

        /// <summary>
        /// The subscription service
        /// </summary>
        private readonly SubscriptionService subscriptionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureWebhookController"/> class.
        /// </summary>
        /// <param name="applicationLogRepository">The application log repository.</param>
        /// <param name="webhookProcessor">The Web hook log repository.</param>
        /// <param name="subscriptionsLogRepository">The subscriptions log repository.</param>
        /// <param name="planRepository">The plan repository.</param>
        /// <param name="subscriptionsRepository">The subscriptions repository.</param>
        public AzureWebhookController(IApplicationLogRepository applicationLogRepository, IWebhookProcessor webhookProcessor, ISubscriptionLogRepository subscriptionsLogRepository, IPlansRepository planRepository, ISubscriptionsRepository subscriptionsRepository)
        {
            this.applicationLogRepository = applicationLogRepository;
            this.subscriptionsRepository = subscriptionsRepository;
            this.planRepository = planRepository;
            this.subscriptionsLogRepository = subscriptionsLogRepository;
            this.webhookProcessor = webhookProcessor;
            this.applicationLogService = new ApplicationLogService(this.applicationLogRepository);
            this.subscriptionService = new SubscriptionService(this.subscriptionsRepository, this.planRepository);
        }

        /// <summary>
        /// Posts the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        public async void Post(WebhookPayload request)
        {
            this.applicationLogService.AddApplicationLog("The azure Webhook Triggered.");

            if (request != null)
            {
                var json = JsonConvert.SerializeObject(request);
                this.applicationLogService.AddApplicationLog("Webhook Serialize Object " + json);
                await this.webhookProcessor.ProcessWebhookNotificationAsync(request).ConfigureAwait(false);
            }
        }
    }
}