// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
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
        string oldStatus = subscription.SubscriptionStatus;
        string oldTermStartDate = subscription.StartDate.ToString();
        string oldTermEndDate = subscription.EndDate.ToString();

        if (subscription.SubscriptionStatus == SubscriptionStatusEnumExtension.PendingActivation.ToString())
        {
            try
            {
                this.logger?.LogInformation("Get attributelsit");

                //1. Activate the subscription
                var subscriptionActivate = this.fulfillmentApiService.ActivateSubscriptionAsync(subscriptionID, subscription.AmpplanId).ConfigureAwait(false).GetAwaiter().GetResult();

                //2. if the status is not 200 or 201 then throw an exception.
                if(subscriptionActivate.Status != 200 && subscriptionActivate.Status != 201)
                {
                    throw new MarketplaceException($"Error while activating the subscription, Status code:{subscriptionActivate.Status}");
                }

                //3. Wait 5 secs and check the status of the subscription if its changed to Subscribed.
                Task.Delay(5000).Wait();
                var subscriptionData = this.fulfillmentApiService.GetSubscriptionByIdAsync(subscriptionID).ConfigureAwait(false).GetAwaiter().GetResult();
                SubscriptionStatusEnum subscriptionStatus = subscriptionData.SaasSubscriptionStatus;
                this.logger?.LogInformation($"Called the GET call after activation, current status {subscriptionStatus}");

                var counter = 0;
                //4. if its not subscribed then wait for 5 more seconds and check the status again.
                while (subscriptionStatus != SubscriptionStatusEnum.Subscribed || counter > 10)
                {
                    Task.Delay(5000).Wait();
                    var loopedsubscriptionData = this.fulfillmentApiService.GetSubscriptionByIdAsync(subscriptionID).ConfigureAwait(false).GetAwaiter().GetResult();
                    subscriptionStatus = subscriptionData.SaasSubscriptionStatus;
                    this.logger?.LogInformation($"Check {counter}: Current status {subscriptionStatus}");
                    if (counter > 10)
                    {
                        this.logger?.LogInformation($"Current status {subscriptionStatus}, checked last times 10times but the subscription status didnt changed to Subscribed. Proceeding normally as Activate call returned 200 and there could be a delay in subscription state change from Pending to Subscribed.");
                        break;
                    }
                }

                this.logger?.LogInformation("UpdateWebJobSubscriptionStatus");

                this.subscriptionsRepository.UpdateStatusAndTermDatesForSubscription(
                    subscriptionID, 
                    SubscriptionStatusEnumExtension.Subscribed.ToString(), 
                    true, 
                    subscriptionData.Term.StartDate, 
                    subscriptionData.Term.EndDate);

                List<SubscriptionAuditLogs> auditLogs = new List<SubscriptionAuditLogs>();
                auditLogs.AddRange(new List<SubscriptionAuditLogs>() 
                { 
                    new SubscriptionAuditLogs()
                    {
                        Attribute = SubscriptionLogAttributes.Status.ToString(),
                        SubscriptionId = subscription.Id,
                        NewValue = SubscriptionStatusEnumExtension.Subscribed.ToString(),
                        OldValue = oldStatus,
                        CreateBy = userdeatils.UserId,
                        CreateDate = DateTime.Now,
                    },
                    new SubscriptionAuditLogs()
                    {
                        Attribute = SubscriptionLogAttributes.TermStartDate.ToString(),
                        SubscriptionId = subscription.Id,
                        NewValue = subscriptionData.Term.StartDate.ToString(),
                        OldValue = oldTermStartDate,
                        CreateBy = userdeatils.UserId,
                        CreateDate = DateTime.Now,
                    },
                    new SubscriptionAuditLogs()
                    {
                        Attribute = SubscriptionLogAttributes.TermEndDate.ToString(),
                        SubscriptionId = subscription.Id,
                        NewValue = subscriptionData.Term.EndDate.ToString(),
                        OldValue = oldTermEndDate,
                        CreateBy = userdeatils.UserId,
                        CreateDate = DateTime.Now,
                    }
                });

                this.subscriptionLogRepository.SaveAll(auditLogs);

                this.subscriptionLogRepository.LogStatusDuringProvisioning(subscriptionID, "Activated", SubscriptionStatusEnumExtension.Subscribed.ToString());
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
}