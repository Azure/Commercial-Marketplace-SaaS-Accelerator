// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Threading.Tasks;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.Exceptions;
using Marketplace.SaaS.Accelerator.Services.Models;
using Marketplace.SaaS.Accelerator.Services.Services;
using Marketplace.SaaS.Accelerator.Services.StatusHandlers;
using Marketplace.SaaS.Accelerator.Services.WebHook;
using Microsoft.Extensions.Logging;

namespace Marketplace.SaaS.Accelerator.CustomerSite.WebHook;

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

    private const string AcceptSubscriptionUpdates = "AcceptSubscriptionUpdates";

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
    public WebHookHandler(IApplicationLogRepository applicationLogRepository, 
                          ISubscriptionLogRepository subscriptionsLogRepository, 
                          ISubscriptionsRepository subscriptionsRepository, 
                          IPlansRepository planRepository, 
                          IOfferAttributesRepository offersAttributeRepository, 
                          IOffersRepository offersRepository, 
                          IFulfillmentApiService fulfillApiService, 
                          IUsersRepository usersRepository, 
                          ILoggerFactory loggerFactory, 
                          IEmailService emailService, 
                          IEventsRepository eventsRepository, 
                          IApplicationConfigRepository applicationConfigRepository, 
                          IEmailTemplateRepository emailTemplateRepository, 
                          IPlanEventsMappingRepository planEventsMappingRepository)
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
        SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
        {
            Attribute = Convert.ToString(SubscriptionLogAttributes.Plan),
            SubscriptionId = oldValue?.SubscribeId,
            OldValue = oldValue?.PlanId,
            CreateBy = null,
            CreateDate = DateTime.Now,
        };

        // we reject if the config value is set to false and the old plan is not the same as the new plan.
        // if the old plan is the same as new plan then its a REVERT webhook scenario where we have to accept the change.
        // we also reject if the subscription is not in the DB
        var _acceptSubscriptionUpdates = Convert.ToBoolean(this.applicationConfigRepository.GetValueByName(AcceptSubscriptionUpdates));
        if ((!_acceptSubscriptionUpdates && payload.PlanId != payload.Subscription.PlanId) || oldValue == null)
        {
            auditLog.NewValue = oldValue?.PlanId;
            this.subscriptionsLogRepository.Save(auditLog);
            throw new MarketplaceException("Plan Change rejected due to Config settings or Subscription not in database");
        }

        this.subscriptionService.UpdateSubscriptionPlan(payload.SubscriptionId, payload.PlanId);
        await this.applicationLogService.AddApplicationLog("Plan Successfully Changed.").ConfigureAwait(false);
        auditLog.NewValue = payload.PlanId;
        this.subscriptionsLogRepository.Save(auditLog);
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
    public async Task ChangeQuantityAsync(WebhookPayload payload)
    {
        var oldValue = this.subscriptionService.GetSubscriptionsBySubscriptionId(payload.SubscriptionId);
        SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
        {
            Attribute = Convert.ToString(SubscriptionLogAttributes.Quantity),
            SubscriptionId = oldValue?.SubscribeId,
            OldValue = oldValue?.Quantity.ToString(),
            CreateBy = null,
            CreateDate = DateTime.Now,
        };

        // we reject if the config value is set to false and the old quantity is not the same as the new quantity.
        // if the old quantity is the same as new quantity then its a REVERT webhook scenario where we have to accept the change.
        // we also reject if the subscription is not in the DB
        var _acceptSubscriptionUpdates = Convert.ToBoolean(this.applicationConfigRepository.GetValueByName(AcceptSubscriptionUpdates));
        if ((!_acceptSubscriptionUpdates && payload.Quantity != payload.Subscription.Quantity) || oldValue == null)
        {
            auditLog.NewValue = oldValue?.Quantity.ToString();
            this.subscriptionsLogRepository.Save(auditLog);
            throw new MarketplaceException("Quantity Change Request reject due to Config settings or Subscription not in database");
        }

        this.subscriptionService.UpdateSubscriptionQuantity(payload.SubscriptionId, payload.Quantity);
        await this.applicationLogService.AddApplicationLog("Quantity Successfully Changed.").ConfigureAwait(false);
        auditLog.NewValue = payload.Quantity.ToString();
        this.subscriptionsLogRepository.Save(auditLog);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Reinstated is followed by Suspend.
    /// This is called when customer fixed their billing issues and partner can choose to reinstate the suspened subscription to subscribed.
    /// And resume the software access to the customer.
    /// </summary>
    /// <param name="payload">The payload.</param>
    /// <returns> Exception.</returns>
    /// <exception cref="NotImplementedException"> Not Implemented Exception. </exception>
    public async Task ReinstatedAsync(WebhookPayload payload)
    {
        var oldValue = this.subscriptionService.GetSubscriptionsBySubscriptionId(payload.SubscriptionId);
        SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
        {
            Attribute = Convert.ToString(SubscriptionLogAttributes.Status),
            SubscriptionId = oldValue?.SubscribeId,
            OldValue = Convert.ToString(oldValue?.SubscriptionStatus),
            CreateBy = null,
            CreateDate = DateTime.Now,
        };

        //gets the user setting from appconfig, if key doesnt exist, add to control the behavior.
        //_acceptSubscriptionUpdates should be true and subscription should be in db to accept subscription updates
        var _acceptSubscriptionUpdates = Convert.ToBoolean(this.applicationConfigRepository.GetValueByName(AcceptSubscriptionUpdates));
        if (_acceptSubscriptionUpdates && oldValue != null)
        {
            this.subscriptionService.UpdateStateOfSubscription(payload.SubscriptionId, SubscriptionStatusEnumExtension.Subscribed.ToString(), false);
            await this.applicationLogService.AddApplicationLog("Reinstated Successfully.").ConfigureAwait(false);
            auditLog.NewValue = Convert.ToString(SubscriptionStatusEnum.Subscribed);
                
        }
        else
        {
            var patchOperation = await fulfillApiService.PatchOperationStatusResultAsync(payload.SubscriptionId, payload.OperationId, Microsoft.Marketplace.SaaS.Models.UpdateOperationStatusEnum.Failure);
            if (patchOperation != null && patchOperation.Status != 200)
            {
                await this.applicationLogService.AddApplicationLog($"Reinstate operation PATCH failed with status statuscode {patchOperation.Status} {patchOperation.ReasonPhrase}.").ConfigureAwait(false);
                //partner trying to fail update operation from customer but PATCH on operation didnt succeced, hence throwing an error
                throw new Exception(patchOperation.ReasonPhrase);
            }

            await this.applicationLogService.AddApplicationLog("Reinstate Change Request Rejected Successfully.").ConfigureAwait(false);
            auditLog.NewValue = Convert.ToString(oldValue?.SubscriptionStatus);
        }

        this.subscriptionsLogRepository.Save(auditLog); 

        await Task.CompletedTask;
    }

    /// <summary>
    /// Renewed the subscription.
    /// </summary>
    /// <param name="payload">The payload.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task RenewedAsync()
    {
        await this.applicationLogService.AddApplicationLog("Offer Successfully Renewed.").ConfigureAwait(false);

        await Task.CompletedTask;
    }

    /// <summary>
    /// Suspended the asynchronous.
    /// </summary>
    /// <param name="payload">The payload.</param>
    /// <returns> Exception.</returns>
    /// <exception cref="NotImplementedException"> Implemented Exception.</exception>
    public async Task SuspendedAsync(WebhookPayload payload)
    {
        var oldValue = this.subscriptionService.GetSubscriptionsBySubscriptionId(payload.SubscriptionId);
        this.subscriptionService.UpdateStateOfSubscription(payload.SubscriptionId, SubscriptionStatusEnumExtension.Suspend.ToString(), false);
        await this.applicationLogService.AddApplicationLog("Offer Successfully Suspended.").ConfigureAwait(false);

        if (oldValue != null)
        {
            SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
            {
                Attribute = Convert.ToString(SubscriptionLogAttributes.Status),
                SubscriptionId = oldValue.SubscribeId,
                NewValue = Convert.ToString(SubscriptionStatusEnum.Suspended),
                OldValue = Convert.ToString(oldValue.SubscriptionStatus),
                CreateBy = null,
                CreateDate = DateTime.Now,
            };
            this.subscriptionsLogRepository.Save(auditLog);
        }

        await Task.CompletedTask;
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
        await this.applicationLogService.AddApplicationLog("Offer Successfully UnSubscribed.").ConfigureAwait(false);

        if (oldValue != null)
        {
            SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
            {
                Attribute = Convert.ToString(SubscriptionLogAttributes.Status),
                SubscriptionId = oldValue.SubscribeId,
                NewValue = Convert.ToString(SubscriptionStatusEnum.Unsubscribed),
                OldValue = Convert.ToString(oldValue.SubscriptionStatus),
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
        await this.applicationLogService.AddApplicationLog("Offer Received an unknown action: " + payload.Action).ConfigureAwait(false);

        await Task.CompletedTask;
    }
}