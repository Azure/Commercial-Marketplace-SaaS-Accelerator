// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Text.Json;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.Exceptions;
using Marketplace.SaaS.Accelerator.Services.Models;
using Microsoft.Extensions.Logging;

namespace Marketplace.SaaS.Accelerator.Services.StatusHandlers;

/// <summary>
/// Status handler to handle the subscriptions that are in PendingActivation status.
/// </summary>
/// <seealso cref="Microsoft.Marketplace.SaasKit.Provisioning.Webjob.StatusHandlers.AbstractSubscriptionStatusHandler" />
public class PendingActivationStatusHandler : AbstractSubscriptionStatusHandler
{
    /// <summary>
    /// The fulfillment apiclient.
    /// </summary>
    private readonly IFulfillmentApiService fulfillmentApiService;

    /// <summary>
    /// The subscription log repository.
    /// </summary>
    private readonly ISubscriptionLogRepository subscriptionLogRepository;

    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger<PendingActivationStatusHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PendingActivationStatusHandler"/> class.
    /// </summary>
    /// <param name="fulfillApiService">The fulfill API client.</param>
    /// <param name="subscriptionsRepository">The subscriptions repository.</param>
    /// <param name="subscriptionLogRepository">The subscription log repository.</param>
    /// <param name="subscriptionTemplateParametersRepository">The subscription template parameters repository.</param>
    /// <param name="plansRepository">The plans repository.</param>
    /// <param name="usersRepository">The users repository.</param>
    /// <param name="logger">The logger.</param>
    public PendingActivationStatusHandler(
        IFulfillmentApiService fulfillApiService,
        ISubscriptionsRepository subscriptionsRepository,
        ISubscriptionLogRepository subscriptionLogRepository,
        IPlansRepository plansRepository,
        IUsersRepository usersRepository,
        ILogger<PendingActivationStatusHandler> logger)
        : base(subscriptionsRepository, plansRepository, usersRepository)
    {
        this.fulfillmentApiService = fulfillApiService;
        this.subscriptionLogRepository = subscriptionLogRepository;
        this.logger = logger;
    }

    /// <summary>
    /// Processes the specified subscription identifier.
    /// </summary>
    /// <param name="subscriptionID">The subscription identifier.</param>
    public override void Process(Guid subscriptionID)
    {
        this.logger?.LogInformation("PendingActivationStatusHandler {0}", subscriptionID);
        var subscription = this.GetSubscriptionById(subscriptionID);
        this.logger?.LogInformation("Result subscription : {0}", JsonSerializer.Serialize(subscription.AmpplanId));
        this.logger?.LogInformation("Get User");
        var userdeatils = this.GetUserById(subscription.UserId);
        string oldstatus = subscription.SubscriptionStatus;

        if (subscription.SubscriptionStatus == SubscriptionStatusEnumExtension.PendingActivation.ToString())
        {
            try
            {
                // Guard against the race where Microsoft auto-activates the subscription asynchronously (e.g. the
                // Subscribe webhook hasn't been processed yet, but the customer already clicked Activate on the
                // landing page). Calling Activate on an already-active subscription errors out on the Marketplace
                // side, so re-check the live status first and skip the call entirely if it's already Subscribed.
                var liveSubscription = this.fulfillmentApiService.GetSubscriptionById(subscriptionID);
                if (liveSubscription != null && liveSubscription.SaasSubscriptionStatus == SubscriptionStatusEnum.Subscribed)
                {
                    this.logger?.LogInformation("Subscription {0} is already Subscribed on the Marketplace side; skipping Activate call.", subscriptionID);
                    this.MarkSubscribed(subscription, userdeatils, oldstatus, "Already active on Marketplace, Activate skipped");
                    return;
                }

                this.logger?.LogInformation("Get attributelsit");

                var subscriptionData = this.fulfillmentApiService.ActivateSubscriptionAsync(subscriptionID, subscription.AmpplanId).ConfigureAwait(false).GetAwaiter().GetResult();

                this.logger?.LogInformation("UpdateWebJobSubscriptionStatus");

                this.MarkSubscribed(subscription, userdeatils, oldstatus, "Activated");
            }
            catch (MarketplaceException mex) when (mex.ErrorCode == SaasApiErrorCode.Conflict)
            {
                // Microsoft's own auto-activation won the race and already activated the subscription between our
                // status check above and the Activate call - treat this as success rather than ActivationFailed.
                this.logger?.LogInformation("Activate returned {0} for {1}; subscription is already active on the Marketplace side, treating as success.", mex.ErrorCode, subscriptionID);
                this.MarkSubscribed(subscription, userdeatils, oldstatus, $"Already active on Marketplace, Activate returned {mex.ErrorCode}");
            }
            catch (Exception ex)
            {
                string errorDescriptin = string.Format("Exception: {0} :: Innser Exception:{1}", ex.Message, ex.InnerException);
                this.subscriptionLogRepository.LogStatusDuringProvisioning(subscriptionID, errorDescriptin, SubscriptionStatusEnumExtension.ActivationFailed.ToString());
                this.logger?.LogInformation(errorDescriptin);

                this.subscriptionsRepository.UpdateStatusForSubscription(subscriptionID, SubscriptionStatusEnumExtension.ActivationFailed.ToString(), false);

                // Set the status as ActivationFailed.
                SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
                {
                    Attribute = SubscriptionLogAttributes.Status.ToString(),
                    SubscriptionId = subscription.Id,
                    NewValue = SubscriptionStatusEnumExtension.ActivationFailed.ToString(),
                    OldValue = subscription.SubscriptionStatus,
                    CreateBy = userdeatils.UserId,
                    CreateDate = DateTime.Now,
                };
                this.subscriptionLogRepository.Save(auditLog);
            }
        }
    }

    /// <summary>
    /// Marks the subscription as Subscribed locally, and records the audit trail / provisioning log entry.
    /// </summary>
    /// <param name="subscription">The local subscription record.</param>
    /// <param name="userdeatils">The user that owns the subscription.</param>
    /// <param name="oldStatus">The status the subscription was in before this update.</param>
    /// <param name="provisioningLogMessage">The message to record in the provisioning log.</param>
    private void MarkSubscribed(Subscriptions subscription, Users userdeatils, string oldStatus, string provisioningLogMessage)
    {
        this.subscriptionsRepository.UpdateStatusForSubscription(subscription.AmpsubscriptionId, SubscriptionStatusEnumExtension.Subscribed.ToString(), true);

        SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
        {
            Attribute = SubscriptionLogAttributes.Status.ToString(),
            SubscriptionId = subscription.Id,
            NewValue = SubscriptionStatusEnumExtension.Subscribed.ToString(),
            OldValue = oldStatus,
            CreateBy = userdeatils.UserId,
            CreateDate = DateTime.Now,
        };
        this.subscriptionLogRepository.Save(auditLog);

        this.subscriptionLogRepository.LogStatusDuringProvisioning(subscription.AmpsubscriptionId, provisioningLogMessage, SubscriptionStatusEnumExtension.Subscribed.ToString());
    }
}