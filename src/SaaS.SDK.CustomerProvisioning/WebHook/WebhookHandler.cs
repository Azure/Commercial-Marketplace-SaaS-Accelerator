// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.
namespace Microsoft.Marketplace.SaaS.SDK.Services.WebHook
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Marketplace.SaaS.SDK.Services.Contracts;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using Microsoft.Marketplace.SaaS.SDK.Services.Services;
    using Microsoft.Marketplace.SaaS.SDK.Services.StatusHandlers;
    using Microsoft.Marketplace.SaaS.SDK.Services.WebHook;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

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
        /// The application configuration repository.
        /// </summary>
        private readonly IApplicationConfigRepository applicationConfigRepository;

        /// <summary>
        /// The email template repository.
        /// </summary>
        private readonly IEmailTemplateRepository emailTemplateRepository;

        /// <summary>
        /// The plan events mapping repository.
        /// </summary>
        private readonly IPlanEventsMappingRepository planEventsMappingRepository;

        /// <summary>
        /// The events repository.
        /// </summary>
        private readonly IEventsRepository eventsRepository;

        /// <summary>
        /// The fulfill API client.
        /// </summary>
        private readonly IFulfillmentApiService fulfillApiService;

        /// <summary>
        /// The users repository.
        /// </summary>
        private readonly IUsersRepository usersRepository;

        /// <summary>
        /// The subscriptions log repository.
        /// </summary>
        private readonly ISubscriptionLogRepository subscriptionsLogRepository;

        private readonly ISubscriptionStatusHandler notificationStatusHandlers;

        private readonly ILoggerFactory loggerFactory;

        private readonly IEmailService emailService;

        private readonly IOffersRepository offersRepository;

        private readonly IOfferAttributesRepository offersAttributeRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebHookHandler" /> class.
        /// </summary>
        /// <param name="applicationLogRepository">The application log repository.</param>
        /// <param name="subscriptionsLogRepository">The subscriptions log repository.</param>
        /// <param name="subscriptionsRepository">The subscriptions repository.</param>
        /// <param name="planRepository">The plan repository.</param>
        /// <param name="offersAttributeRepository">The offers attribute repository.</param>
        /// <param name="offersRepository">The offers repository.</param>
        /// <param name="fulfillApiClient">The fulfill API client.</param>
        /// <param name="usersRepository">The users repository.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="emailService">The email service.</param>
        /// <param name="eventsRepository">The events repository.</param>
        /// <param name="applicationConfigRepository">The application configuration repository.</param>
        /// <param name="emailTemplateRepository">The email template repository.</param>
        /// <param name="planEventsMappingRepository">The plan events mapping repository.</param>
        public WebHookHandler(IApplicationLogRepository applicationLogRepository, ISubscriptionLogRepository subscriptionsLogRepository, ISubscriptionsRepository subscriptionsRepository, IPlansRepository planRepository, IOfferAttributesRepository offersAttributeRepository, IOffersRepository offersRepository, IFulfillmentApiService fulfillApiService, IUsersRepository usersRepository, ILoggerFactory loggerFactory, IEmailService emailService, IEventsRepository eventsRepository, IApplicationConfigRepository applicationConfigRepository, IEmailTemplateRepository emailTemplateRepository, IPlanEventsMappingRepository planEventsMappingRepository)
        {
            this.applicationLogRepository = applicationLogRepository;
            this.subscriptionsRepository = subscriptionsRepository;
            this.planRepository = planRepository;
            this.subscriptionsLogRepository = subscriptionsLogRepository;
            this.applicationLogService = new ApplicationLogService(this.applicationLogRepository);
            this.subscriptionService = new SubscriptionService(this.subscriptionsRepository, this.planRepository);
            this.emailService = emailService;
            this.loggerFactory = loggerFactory;
            this.usersRepository = usersRepository;
            this.eventsRepository = eventsRepository;
            this.offersAttributeRepository = offersAttributeRepository;
            this.fulfillApiService = fulfillApiService;
            this.applicationConfigRepository = applicationConfigRepository;
            this.emailTemplateRepository = emailTemplateRepository;
            this.planEventsMappingRepository = planEventsMappingRepository;
            this.offersRepository = offersRepository;
            this.notificationStatusHandlers = new NotificationStatusHandler(
                                                                        fulfillApiService,
                                                                        planRepository,
                                                                        applicationConfigRepository,
                                                                        emailTemplateRepository,
                                                                        planEventsMappingRepository,
                                                                        offersAttributeRepository,
                                                                        eventsRepository,
                                                                        subscriptionsRepository,
                                                                        usersRepository,
                                                                        offersRepository,
                                                                        emailService,
                                                                        this.loggerFactory.CreateLogger<NotificationStatusHandler>());
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

            this.notificationStatusHandlers.Process(payload.SubscriptionId);

            await Task.CompletedTask;
        }

        /// <summary>
        /// Report unknow action from the webhook the asynchronous.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task UnknownActionAsync(WebhookPayload payload)
        {
            this.applicationLogService.AddApplicationLog("Offer Received an unknow action: " + payload.Action);

            await Task.CompletedTask;
        }
    }
}
