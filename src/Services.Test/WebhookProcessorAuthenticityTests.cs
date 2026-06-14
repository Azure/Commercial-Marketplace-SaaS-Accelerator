// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Threading.Tasks;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.Services.Configurations;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.Exceptions;
using Marketplace.SaaS.Accelerator.Services.Models;
using Marketplace.SaaS.Accelerator.Services.WebHook;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

// WebhookAction is declared in both the Models and WebHook namespaces; the
// processor dispatches on the WebHook one, so bind the name to it here.
using WebhookAction = Marketplace.SaaS.Accelerator.Services.WebHook.WebhookAction;

namespace Marketplace.SaaS.Accelerator.Services.Test;

/// <summary>
/// Example-based tests for Task 8.5 (authenticity failures and response mapping).
///
/// With the <c>ValidateWebhookOperation</c> flag enabled, ACK-required
/// notifications are validated against the Get Operation API before any local
/// state mutation. These tests assert the three authenticity/idempotency
/// outcomes and the HTTP status code each maps to:
///
/// <list type="bullet">
///   <item><description>
///     A <c>null</c> Get Operation result yields <c>CouldNotValidate</c>, which
///     the processor surfaces as a retryable <see cref="WebhookValidationException"/>
///     (maps to HTTP 500). No mutation, no PATCH.
///   </description></item>
///   <item><description>
///     A subscription-id mismatch yields <c>SubscriptionMismatch</c>, surfaced as
///     a non-retryable <see cref="WebhookValidationException"/> (maps to HTTP 400).
///     No mutation, no PATCH.
///   </description></item>
///   <item><description>
///     A terminal operation status yields <c>AlreadyResolved</c>: the processor
///     skips the dispatch and returns normally (maps to HTTP 200). No mutation,
///     no PATCH.
///   </description></item>
/// </list>
///
/// A real <see cref="WebhookOperationValidator"/> is wired into the processor so
/// the only route to the Get Operation API runs through the same
/// <see cref="IFulfillmentApiService"/> mock the assertions inspect. The
/// retryable-to-status-code mapping mirrors
/// <c>AzureWebhookController.Post</c> (which lives in the CustomerSite project
/// and is therefore not referenced from Services.Test).
///
/// Validates: Requirements 1.2, 1.4, 4.3, 4.4, 4.5 / Properties 5, 7.
/// </summary>
[TestClass]
public class WebhookProcessorAuthenticityTests
{
    private const string ValidateWebhookOperation = "ValidateWebhookOperation";

    private static readonly Guid PayloadSubscriptionId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid OtherSubscriptionId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid OperationId = Guid.Parse("33333333-3333-3333-3333-333333333333");

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

        // Validation enabled (default true): the processor consults the validator
        // for ACK-required actions.
        this.applicationConfigRepository
            .Setup(x => x.GetValueByName(ValidateWebhookOperation))
            .Returns("true");

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

    [DataTestMethod]
    [DataRow(WebhookAction.ChangePlan)]
    [DataRow(WebhookAction.ChangeQuantity)]
    [DataRow(WebhookAction.Reinstate)]
    public async Task NullOperation_CouldNotValidate_MapsTo500_NoMutation_NoPatch(WebhookAction action)
    {
        // Get Operation returns null (not found or transient failure).
        this.fulfillApiService
            .Setup(f => f.GetOperationStatusResultAsync(PayloadSubscriptionId, OperationId))
            .ReturnsAsync((OperationResult)null);

        var payload = CreatePayload(action);

        var ex = await Assert.ThrowsExceptionAsync<WebhookValidationException>(
            () => this.processor.ProcessWebhookNotificationAsync(payload, NullConfig()));

        // CouldNotValidate is retryable so the marketplace retries: maps to 500.
        Assert.IsTrue(ex.Retryable, "CouldNotValidate must be retryable.");
        Assert.AreEqual(500, MapValidationExceptionToStatusCode(ex));

        this.AssertGetOperationCalledOnce();
        this.AssertNoMutation();
        this.AssertNoPatch();
    }

    [DataTestMethod]
    [DataRow(WebhookAction.ChangePlan)]
    [DataRow(WebhookAction.ChangeQuantity)]
    [DataRow(WebhookAction.Reinstate)]
    public async Task SubscriptionMismatch_MapsTo400_NoMutation_NoPatch(WebhookAction action)
    {
        // Get Operation returns an operation that belongs to a different subscription.
        this.fulfillApiService
            .Setup(f => f.GetOperationStatusResultAsync(PayloadSubscriptionId, OperationId))
            .ReturnsAsync(new OperationResult
            {
                ID = OperationId.ToString(),
                SubscriptionId = OtherSubscriptionId.ToString(),
                Status = OperationStatusEnum.InProgress,
            });

        var payload = CreatePayload(action);

        var ex = await Assert.ThrowsExceptionAsync<WebhookValidationException>(
            () => this.processor.ProcessWebhookNotificationAsync(payload, NullConfig()));

        // SubscriptionMismatch is a permanent authenticity failure: maps to 400.
        Assert.IsFalse(ex.Retryable, "SubscriptionMismatch must not be retryable.");
        Assert.AreEqual(400, MapValidationExceptionToStatusCode(ex));

        this.AssertGetOperationCalledOnce();
        this.AssertNoMutation();
        this.AssertNoPatch();
    }

    [DataTestMethod]
    [DataRow(WebhookAction.ChangePlan, OperationStatusEnum.Succeeded)]
    [DataRow(WebhookAction.ChangeQuantity, OperationStatusEnum.Failed)]
    [DataRow(WebhookAction.Reinstate, OperationStatusEnum.Conflict)]
    public async Task AlreadyResolved_TerminalStatus_MapsTo200_Skips_NoMutation_NoPatch(
        WebhookAction action,
        OperationStatusEnum terminalStatus)
    {
        // Authentic operation whose status is already terminal (idempotent replay).
        this.fulfillApiService
            .Setup(f => f.GetOperationStatusResultAsync(PayloadSubscriptionId, OperationId))
            .ReturnsAsync(new OperationResult
            {
                ID = OperationId.ToString(),
                SubscriptionId = PayloadSubscriptionId.ToString(),
                Status = terminalStatus,
            });

        var payload = CreatePayload(action);

        // No exception is thrown: the controller would return 200 (success path).
        await this.processor.ProcessWebhookNotificationAsync(payload, NullConfig());

        this.AssertGetOperationCalledOnce();
        this.AssertNoMutation();
        this.AssertNoPatch();
    }

    /// <summary>
    /// Mirrors the mapping in <c>AzureWebhookController.Post</c>:
    /// a retryable validation failure returns 500, otherwise 400.
    /// </summary>
    private static int MapValidationExceptionToStatusCode(WebhookValidationException ex)
    {
        return ex.Retryable ? 500 : 400;
    }

    private void AssertGetOperationCalledOnce()
    {
        this.fulfillApiService.Verify(
            f => f.GetOperationStatusResultAsync(PayloadSubscriptionId, OperationId),
            Times.Once,
            "Get Operation API must be called once to validate the ACK-required notification.");
    }

    private void AssertNoMutation()
    {
        // No handler dispatch occurs, so no local subscription state is mutated.
        this.webhookHandler.Verify(h => h.ChangePlanAsync(It.IsAny<WebhookPayload>()), Times.Never);
        this.webhookHandler.Verify(h => h.ChangeQuantityAsync(It.IsAny<WebhookPayload>()), Times.Never);
        this.webhookHandler.Verify(h => h.ReinstatedAsync(It.IsAny<WebhookPayload>()), Times.Never);
        this.webhookHandler.Verify(h => h.SuspendedAsync(It.IsAny<WebhookPayload>()), Times.Never);
        this.webhookHandler.Verify(h => h.UnsubscribedAsync(It.IsAny<WebhookPayload>()), Times.Never);
        this.webhookHandler.Verify(h => h.RenewedAsync(), Times.Never);
        this.webhookHandler.Verify(h => h.UnknownActionAsync(It.IsAny<WebhookPayload>()), Times.Never);
    }

    private void AssertNoPatch()
    {
        this.fulfillApiService.Verify(
            f => f.PatchOperationStatusResultAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<Microsoft.Marketplace.SaaS.Models.UpdateOperationStatusEnum>()),
            Times.Never,
            "No PATCH acknowledgment must be issued for an unauthenticated or already-resolved operation.");
    }

    private static WebhookPayload CreatePayload(WebhookAction action)
    {
        return new WebhookPayload
        {
            Action = action,
            SubscriptionId = PayloadSubscriptionId,
            OperationId = OperationId,
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
