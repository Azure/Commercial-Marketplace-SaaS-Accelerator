// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Threading.Tasks;
using Azure;
using Marketplace.SaaS.Accelerator.CustomerSite.WebHook;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.Models;
using Marketplace.SaaS.Accelerator.Services.WebHook;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SdkModels = Microsoft.Marketplace.SaaS.Models;
using WebhookAction = Marketplace.SaaS.Accelerator.Services.WebHook.WebhookAction;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Marketplace.SaaS.Accelerator.Services.Test;

/// <summary>
/// Example-based tests for the validate -> apply -> acknowledge success path
/// (Task 8.1). For each ACK-required action (<c>ChangePlan</c>,
/// <c>ChangeQuantity</c>, <c>Reinstate</c>) the validator returns
/// <see cref="WebhookValidationOutcome.Valid"/>, the apply mutates local state,
/// and exactly one <c>Success</c> PATCH is issued.
/// </summary>
/// <remarks>
/// Covers Requirements 2.1, 2.2, 2.3 and design Properties 1 (validation
/// precedes mutation) and 3 (ACK completeness). The test wires the real
/// <see cref="WebhookProcessor"/>, the real <see cref="WebhookOperationValidator"/>,
/// and the real <see cref="WebHookHandler"/> together, swapping only the
/// repositories and the <see cref="IFulfillmentApiService"/> for Moq doubles so
/// no live marketplace calls are made.
/// </remarks>
[TestClass]
public class WebhookValidateApplyAcknowledgeTests
{
    private const string AcceptSubscriptionUpdates = "AcceptSubscriptionUpdates";
    private const string ValidateWebhookOperation = "ValidateWebhookOperation";
    private const string OldPlanId = "old-plan";
    private const string NewPlanId = "new-plan";

    private static readonly Guid SubscriptionId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid OperationId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    private Mock<IFulfillmentApiService> fulfillApiService;
    private Mock<ISubscriptionsRepository> subscriptionsRepository;
    private Mock<ISubscriptionLogRepository> subscriptionLogRepository;
    private Mock<IApplicationConfigRepository> applicationConfigRepository;
    private Mock<IApplicationLogRepository> applicationLogRepository;
    private Mock<IWebNotificationService> webNotificationService;

    [TestInitialize]
    public void Initialize()
    {
        this.fulfillApiService = new Mock<IFulfillmentApiService>();
        this.subscriptionsRepository = new Mock<ISubscriptionsRepository>();
        this.subscriptionLogRepository = new Mock<ISubscriptionLogRepository>();
        this.applicationConfigRepository = new Mock<IApplicationConfigRepository>();
        this.applicationLogRepository = new Mock<IApplicationLogRepository>();
        this.webNotificationService = new Mock<IWebNotificationService>();

        // Both gates enabled: Get Operation validation on, subscription updates accepted.
        this.applicationConfigRepository
            .Setup(x => x.GetValueByName(ValidateWebhookOperation))
            .Returns("true");
        this.applicationConfigRepository
            .Setup(x => x.GetValueByName(AcceptSubscriptionUpdates))
            .Returns("true");

        // The subscription exists locally so the apply is accepted.
        this.subscriptionsRepository
            .Setup(x => x.GetById(SubscriptionId, It.IsAny<bool>()))
            .Returns(new Subscriptions
            {
                Id = 1,
                AmpsubscriptionId = SubscriptionId,
                AmpplanId = OldPlanId,
                AmpOfferId = "offer-1",
                Ampquantity = 1,
                SubscriptionStatus = "Suspended",
                Name = "test-subscription",
            });

        // Get Operation returns an authentic, actionable operation for this subscription.
        this.fulfillApiService
            .Setup(x => x.GetOperationStatusResultAsync(SubscriptionId, OperationId))
            .ReturnsAsync(new OperationResult
            {
                ID = OperationId.ToString(),
                SubscriptionId = SubscriptionId.ToString(),
                Status = OperationStatusEnum.InProgress,
            });

        // Both Success and Failure PATCH calls return a 200 response by default.
        this.fulfillApiService
            .Setup(x => x.PatchOperationStatusResultAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<SdkModels.UpdateOperationStatusEnum>()))
            .ReturnsAsync(Mock.Of<Response>(r => r.Status == 200));
    }

    [TestMethod]
    public async Task ChangePlan_WhenValid_AppliesAndAcknowledgesSuccess()
    {
        var payload = new WebhookPayload
        {
            Action = WebhookAction.ChangePlan,
            SubscriptionId = SubscriptionId,
            OperationId = OperationId,
            PlanId = NewPlanId,
            Subscription = new SubscriptionWebhookResult { PlanId = OldPlanId },
        };

        await this.BuildProcessor().ProcessWebhookNotificationAsync(payload, null);

        // Validation precedes mutation (Property 1).
        this.fulfillApiService.Verify(
            x => x.GetOperationStatusResultAsync(SubscriptionId, OperationId), Times.Once);

        // State is mutated.
        this.subscriptionsRepository.Verify(
            x => x.UpdatePlanForSubscription(SubscriptionId, NewPlanId), Times.Once);

        // Exactly one Success PATCH, no Failure PATCH (Property 3).
        this.VerifyExactlyOneSuccessAck();
    }

    [TestMethod]
    public async Task ChangeQuantity_WhenValid_AppliesAndAcknowledgesSuccess()
    {
        var payload = new WebhookPayload
        {
            Action = WebhookAction.ChangeQuantity,
            SubscriptionId = SubscriptionId,
            OperationId = OperationId,
            Quantity = 5,
            Subscription = new SubscriptionWebhookResult { Quantity = 1 },
        };

        await this.BuildProcessor().ProcessWebhookNotificationAsync(payload, null);

        this.fulfillApiService.Verify(
            x => x.GetOperationStatusResultAsync(SubscriptionId, OperationId), Times.Once);

        this.subscriptionsRepository.Verify(
            x => x.UpdateQuantityForSubscription(SubscriptionId, 5), Times.Once);

        this.VerifyExactlyOneSuccessAck();
    }

    [TestMethod]
    public async Task Reinstate_WhenValid_AppliesAndAcknowledgesSuccess()
    {
        var payload = new WebhookPayload
        {
            Action = WebhookAction.Reinstate,
            SubscriptionId = SubscriptionId,
            OperationId = OperationId,
        };

        await this.BuildProcessor().ProcessWebhookNotificationAsync(payload, null);

        this.fulfillApiService.Verify(
            x => x.GetOperationStatusResultAsync(SubscriptionId, OperationId), Times.Once);

        // Reinstate sets the subscription back to Subscribed.
        this.subscriptionsRepository.Verify(
            x => x.UpdateStatusForSubscription(SubscriptionId, "Subscribed", false), Times.Once);

        this.VerifyExactlyOneSuccessAck();
    }

    /// <summary>
    /// Asserts exactly one PATCH to <c>Success</c> for the test operation and no
    /// PATCH to <c>Failure</c>.
    /// </summary>
    private void VerifyExactlyOneSuccessAck()
    {
        this.fulfillApiService.Verify(
            x => x.PatchOperationStatusResultAsync(
                SubscriptionId, OperationId, SdkModels.UpdateOperationStatusEnum.Success),
            Times.Once);

        this.fulfillApiService.Verify(
            x => x.PatchOperationStatusResultAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), SdkModels.UpdateOperationStatusEnum.Failure),
            Times.Never);
    }

    /// <summary>
    /// Builds the full processor pipeline (real validator + real handler) with the
    /// configured Moq doubles.
    /// </summary>
    /// <returns>The wired <see cref="WebhookProcessor"/>.</returns>
    private WebhookProcessor BuildProcessor()
    {
        var validator = new WebhookOperationValidator(
            this.fulfillApiService.Object,
            this.applicationLogRepository.Object);

        var handler = this.BuildHandler();

        return new WebhookProcessor(
            this.fulfillApiService.Object,
            handler,
            this.webNotificationService.Object,
            validator,
            this.applicationConfigRepository.Object,
            this.applicationLogRepository.Object);
    }

    /// <summary>
    /// Builds a real <see cref="WebHookHandler"/> with Moq doubles for all
    /// repository and service dependencies.
    /// </summary>
    /// <returns>The constructed <see cref="WebHookHandler"/>.</returns>
    private WebHookHandler BuildHandler()
    {
        var loggerFactory = new Mock<ILoggerFactory>();
        loggerFactory
            .Setup(f => f.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());

        return new WebHookHandler(
            this.applicationLogRepository.Object,
            this.subscriptionLogRepository.Object,
            this.subscriptionsRepository.Object,
            new Mock<IPlansRepository>().Object,
            new Mock<IOfferAttributesRepository>().Object,
            new Mock<IOffersRepository>().Object,
            this.fulfillApiService.Object,
            new Mock<IUsersRepository>().Object,
            loggerFactory.Object,
            new Mock<IEmailService>().Object,
            new Mock<IEventsRepository>().Object,
            this.applicationConfigRepository.Object,
            new Mock<IEmailTemplateRepository>().Object,
            new Mock<IPlanEventsMappingRepository>().Object);
    }
}
