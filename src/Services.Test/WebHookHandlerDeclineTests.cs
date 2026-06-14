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
using Microsoft.Marketplace.SaaS.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Marketplace.SaaS.Accelerator.Services.Test;

// Disambiguate WebhookAction (also defined under Services.Models) to the type used by WebhookPayload.Action.
using WebhookAction = Marketplace.SaaS.Accelerator.Services.WebHook.WebhookAction;

/// <summary>
/// Example-based tests for the ACK-required decline paths in <see cref="WebHookHandler"/>.
/// Covers Requirement 2.4 / Property 4: an authenticated ACK-required action declined by a
/// business rule (the <c>AcceptSubscriptionUpdates</c> gate is off, or the subscription is not
/// in the local database) issues exactly one <c>Failure</c> PATCH and performs no mutation.
/// </summary>
[TestClass]
public class WebHookHandlerDeclineTests
{
    private const string AcceptSubscriptionUpdates = "AcceptSubscriptionUpdates";

    private Mock<IApplicationLogRepository> applicationLogRepository;
    private Mock<ISubscriptionLogRepository> subscriptionLogRepository;
    private Mock<ISubscriptionsRepository> subscriptionsRepository;
    private Mock<IPlansRepository> plansRepository;
    private Mock<IOfferAttributesRepository> offerAttributesRepository;
    private Mock<IOffersRepository> offersRepository;
    private Mock<IFulfillmentApiService> fulfillmentApiService;
    private Mock<IUsersRepository> usersRepository;
    private Mock<ILoggerFactory> loggerFactory;
    private Mock<IEmailService> emailService;
    private Mock<IEventsRepository> eventsRepository;
    private Mock<IApplicationConfigRepository> applicationConfigRepository;
    private Mock<IEmailTemplateRepository> emailTemplateRepository;
    private Mock<IPlanEventsMappingRepository> planEventsMappingRepository;

    [TestInitialize]
    public void Initialize()
    {
        this.applicationLogRepository = new Mock<IApplicationLogRepository>();
        this.subscriptionLogRepository = new Mock<ISubscriptionLogRepository>();
        this.subscriptionsRepository = new Mock<ISubscriptionsRepository>();
        this.plansRepository = new Mock<IPlansRepository>();
        this.offerAttributesRepository = new Mock<IOfferAttributesRepository>();
        this.offersRepository = new Mock<IOffersRepository>();
        this.fulfillmentApiService = new Mock<IFulfillmentApiService>();
        this.usersRepository = new Mock<IUsersRepository>();
        this.loggerFactory = new Mock<ILoggerFactory>();
        this.emailService = new Mock<IEmailService>();
        this.eventsRepository = new Mock<IEventsRepository>();
        this.applicationConfigRepository = new Mock<IApplicationConfigRepository>();
        this.emailTemplateRepository = new Mock<IEmailTemplateRepository>();
        this.planEventsMappingRepository = new Mock<IPlanEventsMappingRepository>();

        // NotificationStatusHandler (built inside the handler) requests a logger.
        this.loggerFactory
            .Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<Microsoft.Extensions.Logging.ILogger>());

        // PATCH acknowledgments return a successful (200) response so the handler does not
        // log a PATCH failure; the decline tests only assert that the PATCH was issued.
        var okResponse = new Mock<Response>();
        okResponse.SetupGet(r => r.Status).Returns(200);
        this.fulfillmentApiService
            .Setup(x => x.PatchOperationStatusResultAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<UpdateOperationStatusEnum>()))
            .ReturnsAsync(okResponse.Object);
    }

    private WebHookHandler CreateHandler()
    {
        return new WebHookHandler(
            this.applicationLogRepository.Object,
            this.subscriptionLogRepository.Object,
            this.subscriptionsRepository.Object,
            this.plansRepository.Object,
            this.offerAttributesRepository.Object,
            this.offersRepository.Object,
            this.fulfillmentApiService.Object,
            this.usersRepository.Object,
            this.loggerFactory.Object,
            this.emailService.Object,
            this.eventsRepository.Object,
            this.applicationConfigRepository.Object,
            this.emailTemplateRepository.Object,
            this.planEventsMappingRepository.Object);
    }

    private void SetAcceptSubscriptionUpdates(bool value)
    {
        this.applicationConfigRepository
            .Setup(x => x.GetValueByName(AcceptSubscriptionUpdates))
            .Returns(value.ToString());
    }

    private void SetupExistingSubscription(Guid subscriptionId, string planId, int quantity)
    {
        this.subscriptionsRepository
            .Setup(x => x.GetById(subscriptionId, It.IsAny<bool>()))
            .Returns(new Subscriptions
            {
                Id = 1,
                AmpsubscriptionId = subscriptionId,
                AmpplanId = planId,
                Ampquantity = quantity,
                SubscriptionStatus = "Subscribed",
                IsActive = true,
            });
    }

    private void SetupMissingSubscription(Guid subscriptionId)
    {
        this.subscriptionsRepository
            .Setup(x => x.GetById(subscriptionId, It.IsAny<bool>()))
            .Returns((Subscriptions)null);
    }

    private void VerifyExactlyOneFailurePatch(Guid subscriptionId, Guid operationId)
    {
        this.fulfillmentApiService.Verify(
            x => x.PatchOperationStatusResultAsync(
                subscriptionId, operationId, UpdateOperationStatusEnum.Failure),
            Times.Once);

        // No Success ACK should ever be issued on a declined operation.
        this.fulfillmentApiService.Verify(
            x => x.PatchOperationStatusResultAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), UpdateOperationStatusEnum.Success),
            Times.Never);

        // Exactly one PATCH total.
        this.fulfillmentApiService.Verify(
            x => x.PatchOperationStatusResultAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<UpdateOperationStatusEnum>()),
            Times.Once);
    }

    private void VerifyNoPlanMutation()
    {
        this.subscriptionsRepository.Verify(
            x => x.UpdatePlanForSubscription(It.IsAny<Guid>(), It.IsAny<string>()),
            Times.Never);
    }

    private void VerifyNoQuantityMutation()
    {
        this.subscriptionsRepository.Verify(
            x => x.UpdateQuantityForSubscription(It.IsAny<Guid>(), It.IsAny<int>()),
            Times.Never);
    }

    [TestMethod]
    public async Task ChangePlan_GateOff_IssuesSingleFailurePatch_NoMutation()
    {
        var subscriptionId = Guid.NewGuid();
        var operationId = Guid.NewGuid();

        // Gate is off and the requested plan differs from the current plan -> decline.
        this.SetAcceptSubscriptionUpdates(false);
        this.SetupExistingSubscription(subscriptionId, planId: "OldPlan", quantity: 5);

        var payload = new WebhookPayload
        {
            Action = WebhookAction.ChangePlan,
            SubscriptionId = subscriptionId,
            OperationId = operationId,
            PlanId = "NewPlan",
            Subscription = new SubscriptionWebhookResult { PlanId = "OldPlan" },
        };

        await this.CreateHandler().ChangePlanAsync(payload);

        this.VerifyExactlyOneFailurePatch(subscriptionId, operationId);
        this.VerifyNoPlanMutation();
    }

    [TestMethod]
    public async Task ChangePlan_SubscriptionMissing_IssuesSingleFailurePatch_NoMutation()
    {
        var subscriptionId = Guid.NewGuid();
        var operationId = Guid.NewGuid();

        // Gate is on, but the subscription is not in the local database -> decline.
        this.SetAcceptSubscriptionUpdates(true);
        this.SetupMissingSubscription(subscriptionId);

        var payload = new WebhookPayload
        {
            Action = WebhookAction.ChangePlan,
            SubscriptionId = subscriptionId,
            OperationId = operationId,
            PlanId = "NewPlan",
            Subscription = new SubscriptionWebhookResult { PlanId = "OldPlan" },
        };

        await this.CreateHandler().ChangePlanAsync(payload);

        this.VerifyExactlyOneFailurePatch(subscriptionId, operationId);
        this.VerifyNoPlanMutation();
    }

    [TestMethod]
    public async Task ChangeQuantity_GateOff_IssuesSingleFailurePatch_NoMutation()
    {
        var subscriptionId = Guid.NewGuid();
        var operationId = Guid.NewGuid();

        // Gate is off and the requested quantity differs from the current quantity -> decline.
        this.SetAcceptSubscriptionUpdates(false);
        this.SetupExistingSubscription(subscriptionId, planId: "OldPlan", quantity: 5);

        var payload = new WebhookPayload
        {
            Action = WebhookAction.ChangeQuantity,
            SubscriptionId = subscriptionId,
            OperationId = operationId,
            Quantity = 10,
            Subscription = new SubscriptionWebhookResult { Quantity = 5 },
        };

        await this.CreateHandler().ChangeQuantityAsync(payload);

        this.VerifyExactlyOneFailurePatch(subscriptionId, operationId);
        this.VerifyNoQuantityMutation();
    }

    [TestMethod]
    public async Task ChangeQuantity_SubscriptionMissing_IssuesSingleFailurePatch_NoMutation()
    {
        var subscriptionId = Guid.NewGuid();
        var operationId = Guid.NewGuid();

        // Gate is on, but the subscription is not in the local database -> decline.
        this.SetAcceptSubscriptionUpdates(true);
        this.SetupMissingSubscription(subscriptionId);

        var payload = new WebhookPayload
        {
            Action = WebhookAction.ChangeQuantity,
            SubscriptionId = subscriptionId,
            OperationId = operationId,
            Quantity = 10,
            Subscription = new SubscriptionWebhookResult { Quantity = 5 },
        };

        await this.CreateHandler().ChangeQuantityAsync(payload);

        this.VerifyExactlyOneFailurePatch(subscriptionId, operationId);
        this.VerifyNoQuantityMutation();
    }
}
