// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Threading.Tasks;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.Services.Configurations;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.WebHook;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Marketplace.SaaS.Accelerator.Services.Test;

/// <summary>
/// Example-based tests for Task 8.4 (legacy preservation).
///
/// Validates Property 6: when the <c>ValidateWebhookOperation</c> flag is
/// disabled, the Get Operation API is never called and the processor dispatches
/// to the handler exactly as it did before the validation feature was added
/// (legacy behavior).
///
/// A real <see cref="WebhookOperationValidator"/> is wired into the processor so
/// that the only path to the Get Operation API runs through the same
/// <see cref="IFulfillmentApiService"/> mock the assertions inspect; if the
/// processor were to invoke the validator, Get Operation would be observed.
///
/// Validates: Requirements 3.2 / Property 6.
/// </summary>
[TestClass]
public class WebhookProcessorLegacyPreservationTests
{
    private const string ValidateWebhookOperation = "ValidateWebhookOperation";

    private Mock<IFulfillmentApiService> fulfillApiService;
    private Mock<IWebhookHandler> webhookHandler;
    private Mock<IWebNotificationService> webNotificationService;
    private Mock<IApplicationConfigRepository> applicationConfigRepository;
    private Mock<IApplicationLogRepository> applicationLogRepository;
    private WebhookProcessor processor;

    [TestInitialize]
    public void Initialize()
    {
        this.fulfillApiService = new Mock<IFulfillmentApiService>();
        this.webhookHandler = new Mock<IWebhookHandler>();
        this.webNotificationService = new Mock<IWebNotificationService>();
        this.applicationConfigRepository = new Mock<IApplicationConfigRepository>();
        this.applicationLogRepository = new Mock<IApplicationLogRepository>();

        // Flag explicitly disabled: legacy path. Only the literal "false" disables
        // validation (Requirement 3.2).
        this.applicationConfigRepository
            .Setup(x => x.GetValueByName(ValidateWebhookOperation))
            .Returns("false");

        // Use the real validator so the only route to Get Operation is the same
        // fulfillment mock the assertions inspect.
        var validator = new WebhookOperationValidator(
            this.fulfillApiService.Object,
            this.applicationLogRepository.Object);

        this.processor = new WebhookProcessor(
            this.fulfillApiService.Object,
            this.webhookHandler.Object,
            this.webNotificationService.Object,
            validator,
            this.applicationConfigRepository.Object,
            this.applicationLogRepository.Object);
    }

    [TestMethod]
    public async Task ChangePlan_FlagDisabled_GetOperationNotCalled_DispatchesToHandler()
    {
        var payload = CreatePayload(WebhookAction.ChangePlan);

        await this.processor.ProcessWebhookNotificationAsync(payload, NullConfig());

        this.AssertGetOperationNeverCalled();
        this.webhookHandler.Verify(h => h.ChangePlanAsync(payload), Times.Once);
        this.AssertNoOtherDispatch(except: nameof(IWebhookHandler.ChangePlanAsync), payload);
    }

    [TestMethod]
    public async Task ChangeQuantity_FlagDisabled_GetOperationNotCalled_DispatchesToHandler()
    {
        var payload = CreatePayload(WebhookAction.ChangeQuantity);

        await this.processor.ProcessWebhookNotificationAsync(payload, NullConfig());

        this.AssertGetOperationNeverCalled();
        this.webhookHandler.Verify(h => h.ChangeQuantityAsync(payload), Times.Once);
        this.AssertNoOtherDispatch(except: nameof(IWebhookHandler.ChangeQuantityAsync), payload);
    }

    [TestMethod]
    public async Task Reinstate_FlagDisabled_GetOperationNotCalled_DispatchesToHandler()
    {
        var payload = CreatePayload(WebhookAction.Reinstate);

        await this.processor.ProcessWebhookNotificationAsync(payload, NullConfig());

        this.AssertGetOperationNeverCalled();
        this.webhookHandler.Verify(h => h.ReinstatedAsync(payload), Times.Once);
        this.AssertNoOtherDispatch(except: nameof(IWebhookHandler.ReinstatedAsync), payload);
    }

    [TestMethod]
    public async Task AllAckRequiredActions_FlagDisabled_ValidatorIsNeverConsulted()
    {
        // Sweep every ACK-required action in one pass to assert the Get Operation
        // API is never reached regardless of which action arrives while disabled.
        foreach (var action in new[]
                 {
                     WebhookAction.ChangePlan,
                     WebhookAction.ChangeQuantity,
                     WebhookAction.Reinstate,
                 })
        {
            await this.processor.ProcessWebhookNotificationAsync(CreatePayload(action), NullConfig());
        }

        this.AssertGetOperationNeverCalled();
    }

    private void AssertGetOperationNeverCalled()
    {
        this.fulfillApiService.Verify(
            f => f.GetOperationStatusResultAsync(It.IsAny<Guid>(), It.IsAny<Guid>()),
            Times.Never,
            "Get Operation API must not be called when ValidateWebhookOperation is disabled.");

        // Legacy path also never acknowledges.
        this.fulfillApiService.Verify(
            f => f.PatchOperationStatusResultAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<Microsoft.Marketplace.SaaS.Models.UpdateOperationStatusEnum>()),
            Times.Never);
    }

    private void AssertNoOtherDispatch(string except, WebhookPayload payload)
    {
        if (except != nameof(IWebhookHandler.ChangePlanAsync))
        {
            this.webhookHandler.Verify(h => h.ChangePlanAsync(It.IsAny<WebhookPayload>()), Times.Never);
        }

        if (except != nameof(IWebhookHandler.ChangeQuantityAsync))
        {
            this.webhookHandler.Verify(h => h.ChangeQuantityAsync(It.IsAny<WebhookPayload>()), Times.Never);
        }

        if (except != nameof(IWebhookHandler.ReinstatedAsync))
        {
            this.webhookHandler.Verify(h => h.ReinstatedAsync(It.IsAny<WebhookPayload>()), Times.Never);
        }

        // Notify-only and unknown handlers are never invoked for ACK-required actions.
        this.webhookHandler.Verify(h => h.SuspendedAsync(It.IsAny<WebhookPayload>()), Times.Never);
        this.webhookHandler.Verify(h => h.UnsubscribedAsync(It.IsAny<WebhookPayload>()), Times.Never);
        this.webhookHandler.Verify(h => h.RenewedAsync(), Times.Never);
        this.webhookHandler.Verify(h => h.UnknownActionAsync(It.IsAny<WebhookPayload>()), Times.Never);
    }

    private static WebhookPayload CreatePayload(WebhookAction action)
    {
        return new WebhookPayload
        {
            Action = action,
            SubscriptionId = Guid.NewGuid(),
            OperationId = Guid.NewGuid(),
            PlanId = "plan-1",
            Quantity = 5,
        };
    }

    private static SaaSApiClientConfiguration NullConfig()
    {
        // The processor does not read the configuration argument; a default
        // instance keeps the call site faithful without coupling to its fields.
        return new SaaSApiClientConfiguration();
    }
}
