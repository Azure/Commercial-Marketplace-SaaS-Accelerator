// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.
namespace Microsoft.Marketplace.SaasKit.Client.Controllers.WebHook
{
    using System.Text.Json;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Marketplace.SaaS.SDK.Services.Services;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaaS.SDK.Services.WebHook;

    /// <summary>
    /// Azure Web hook.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("api/[controller]")]
    [ApiController]
    public class AzureWebhookController : ControllerBase
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
        /// The subscriptions log repository.
        /// </summary>
        private readonly ISubscriptionLogRepository subscriptionsLogRepository;

        /// <summary>
        /// The web hook processor.
        /// </summary>
        private readonly IWebhookProcessor webhookProcessor;

        /// <summary>
        /// The application log service.
        /// </summary>
        private readonly ApplicationLogService applicationLogService;

        /// <summary>
        /// The subscription service.
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
            await this.applicationLogService.AddApplicationLog("The azure Webhook Triggered.").ConfigureAwait(false);

            if (request != null)
            {
                var json = JsonSerializer.Serialize(request);
                await this.applicationLogService.AddApplicationLog("Webhook Serialize Object " + json).ConfigureAwait(false);
                await this.webhookProcessor.ProcessWebhookNotificationAsync(request).ConfigureAwait(false);
            }
        }
    }
}