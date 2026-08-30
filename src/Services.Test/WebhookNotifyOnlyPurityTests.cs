// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Threading.Tasks;
using Marketplace.SaaS.Accelerator.CustomerSite.WebHook;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.Services.Configurations;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.WebHook;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Marketplace.SaaS.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Marketplace.SaaS.Accelerator.Services.Test;

/// <summary>
/// Example-based tests for notify-only purity (Task 8.3 / Property 2).
///
/// For any <see cref="WebhookAction.Renew"/>, <see cref="WebhookAction.Suspend"/>,
/// or <see cref="WebhookAction.Unsubscribe"/> notification, neither the Get
/// Operation API (<see cref="IFulfillmentApiService.GetOperationStatusResultAsync"/>)
/// nor the PATCH Operation API
/// (<see cref="IFulfillmentApiService.PatchOperationStatusResultAsync"/>) is ever
/// called.
///
/// The tests wire the real <see cref="WebhookProcessor"/>, the real
/// <see cref="WebhookOperationValidator"/>, and the real <see cref="WebHookHandler"/>
/// onto a single <see cref="Mock{IFulfillmentApiService}"/> so the assertion
/// covers the full pipeline: the processor never validates notify-only events
/// (so Get Operation is never reached) and the handler never acknowledges them
/// (so PATCH is never issued).
///
/// Validates: Requirements 2.6.
/// </summary>
[TestClass]
public class WebhookNotifyOnlyPurityTests
{
    private static readonly Guid SubscriptionId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid OperationId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    private Mock<IFulfillmentApiService> fulfillApiService;
    private Mock<IApplicationConfigRepository> applicationConfigRepository;
    private Mock<ISubscriptionsRepository> subscriptionsRepository;
    private WebhookProcessor processor;

    [TestInitialize]
    public void Initialize()
    {
        this.fulfillApiService = new Mock<IFulfillmentApiService>(MockBehavior.Strict);

        // Validation is enabled: this is the strict case. Even with Get Operation
        // validation turned on, notify-only events must never reach the validator.
        this.applicationConfigRepository = new Mock<IApplicationConfigRepository>();
        this.applicationConfigRepository
            .Setup(x => x.GetValueByName(It.IsAny<string>()))
            .Returns<string>(name => name == "ValidateWebhookOperation" ? "true" : null);

        // A non-null subscription is required so the Unsubscribe path's
        // NotificationStatusHandler.Process can run without a null reference.
        this.subscriptionsRepository = new Mock<ISubscriptionsRepository>();
        this.subscriptionsRepository
            .Setup(x => x.GetById(It.IsAny<Guid>(), It.IsAny<bool>()))
            .Returns(new Subscriptions
            {
                Id = 1,
                AmpsubscriptionId = SubscriptionId,
                AmpplanId = "test-plan",
                SubscriptionStatus = "Unsubscribed",
                UserId = 1,
            });

        var webNotificationService = new Mock<IWebNotificationService>();
        webNotificationService
            .Setup(x => x.PushExternalWebNotificationAsync(It.IsAny<WebhookPayload>()))
            .Returns(Task.CompletedTask);

        var applicationLogRepository = new Mock<IApplicationLogRepository>();

        var handler = new WebHookHandler(
            applicationLogRepository.Object,
            new Mock<ISubscriptionLogRepository>().Object,
            this.subscriptionsRepository.Object,
            new Mock<IPlansRepository>().Object,
            new Mock<IOfferAttributesRepository>().Object,
            new Mock<IOffersRepository>().Object,
            this.fulfillApiService.Object,
            new Mock<IUsersRepository>().Object,
            NullLoggerFactory.Instance,
            new Mock<IEmailService>().Object,
            new Mock<IEventsRepository>().Object,
            this.applicationConfigRepository.Object,
            new Mock<IEmailTemplateRepository>().Object,
            new Mock<IPlanEventsMappingRepository>().Object);

        var validator = new WebhookOperationValidator(
            this.fulfillApiService.Object,
            applicationLogRepository.Object);

        this.processor = new WebhookProcessor(
            this.fulfillApiService.Object,
            handler,
            webNotificationService.Object,
            validator,
            this.applicationConfigRepository.Object,
            applicationLogRepository.Object);
    }

    [DataTestMethod]
    [DataRow(WebhookAction.Renew)]
    [DataRow(WebhookAction.Suspend)]
    [DataRow(WebhookAction.Unsubscribe)]
    public async Task NotifyOnlyAction_NeverCallsGetOperationOrPatch(WebhookAction action)
    {
        var payload = new WebhookPayload
        {
            Action = action,
            SubscriptionId = SubscriptionId,
            OperationId = OperationId,
            PlanId = "test-plan",
        };

        await this.processor.ProcessWebhookNotificationAsync(payload, new SaaSApiClientConfiguration());

        // The Get Operation API must never be called for notify-only events.
        this.fulfillApiService.Verify(
            x => x.GetOperationStatusResultAsync(It.IsAny<Guid>(), It.IsAny<Guid>()),
            Times.Never,
            $"Get Operation API must not be called for notify-only action {action}.");

        // The PATCH Operation API must never be called for notify-only events.
        this.fulfillApiService.Verify(
            x => x.PatchOperationStatusResultAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<UpdateOperationStatusEnum>()),
            Times.Never,
            $"PATCH Operation API must not be called for notify-only action {action}.");
    }
}
