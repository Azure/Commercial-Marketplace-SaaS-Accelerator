// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using Marketplace.SaaS.Accelerator.CustomerSite.WebHook;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.Services.Configurations;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.Exceptions;
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
/// Property-based tests for the webhook pipeline invariants (Task 9).
///
/// No property-based testing framework is present in the repository, so these
/// tests use a deterministic seeded generator loop: each property draws many
/// random combinations of (action, operation status / authenticity scenario,
/// configuration flag state) and asserts the invariant holds across the whole
/// sampled input space. A fixed seed keeps any failure reproducible, and the
/// failing combination is included in the assertion message so it can be
/// replayed as a concrete example.
///
/// The tests wire the real <see cref="WebhookProcessor"/>, the real
/// <see cref="WebhookOperationValidator"/>, and the real
/// <see cref="WebHookHandler"/> together, swapping only the repositories and the
/// <see cref="IFulfillmentApiService"/> for Moq doubles so no live marketplace
/// calls are made (Requirement 5.6).
///
/// Properties asserted:
/// <list type="bullet">
///   <item><description>
///     Property 1 (validation precedes mutation): for any ACK-required payload
///     with validation enabled, no local subscription mutation occurs unless the
///     validator outcome was <see cref="WebhookValidationOutcome.Valid"/>.
///   </description></item>
///   <item><description>
///     Property 2 (notify-only purity): for any <c>Renew</c>/<c>Suspend</c>/
///     <c>Unsubscribe</c> payload, the Get Operation API and the PATCH Operation
///     API are never called.
///   </description></item>
///   <item><description>
///     Property 5 (idempotency): for a payload whose operation is already
///     terminal, repeated deliveries produce no mutation, no PATCH, and no
///     thrown validation exception (a 200 response).
///   </description></item>
/// </list>
///
/// Validates: Requirements 1.1, 1.4, 2.6 / Properties 1, 2, 5.
/// </summary>
[TestClass]
public class WebhookInvariantPropertyTests
{
    private const string AcceptSubscriptionUpdates = "AcceptSubscriptionUpdates";
    private const string ValidateWebhookOperation = "ValidateWebhookOperation";

    /// <summary>Number of random cases sampled per property.</summary>
    private const int Cases = 600;

    /// <summary>Fixed seed so any counterexample is reproducible.</summary>
    private const int Seed = 20240611;

    private const string OldPlanId = "old-plan";
    private const string NewPlanId = "new-plan";

    private static readonly Guid PayloadSubscriptionId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid OtherSubscriptionId = Guid.Parse("99999999-9999-9999-9999-999999999999");
    private static readonly Guid OperationId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    private static readonly WebhookAction[] AckRequiredActions =
    {
        WebhookAction.ChangePlan,
        WebhookAction.ChangeQuantity,
        WebhookAction.Reinstate,
    };

    private static readonly WebhookAction[] NotifyOnlyActions =
    {
        WebhookAction.Renew,
        WebhookAction.Suspend,
        WebhookAction.Unsubscribe,
    };

    private static readonly OperationStatusEnum[] TerminalStatuses =
    {
        OperationStatusEnum.Succeeded,
        OperationStatusEnum.Failed,
        OperationStatusEnum.Conflict,
    };

    private static readonly OperationStatusEnum[] NonTerminalStatuses =
    {
        OperationStatusEnum.NotStarted,
        OperationStatusEnum.InProgress,
    };

    /// <summary>
    /// The authenticity / status scenario the Get Operation API returns for a
    /// generated case, together with the validator outcome it implies.
    /// </summary>
    private enum OperationScenario
    {
        /// <summary>Get Operation returns null -> CouldNotValidate.</summary>
        Null,

        /// <summary>Operation belongs to a different subscription -> SubscriptionMismatch.</summary>
        Mismatch,

        /// <summary>Operation status is terminal -> AlreadyResolved.</summary>
        Terminal,

        /// <summary>Operation status is non-terminal and matches -> Valid.</summary>
        Actionable,
    }

    /// <summary>
    /// Property 1: validation precedes mutation. For every ACK-required action,
    /// over a random sweep of operation scenarios and the
    /// <c>AcceptSubscriptionUpdates</c> gate (with validation enabled), no local
    /// subscription mutation is ever observed unless the validator outcome was
    /// <see cref="WebhookValidationOutcome.Valid"/>. When the outcome is not
    /// <c>Valid</c> the processor must also issue no PATCH.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [TestMethod]
    public async Task Property1_NoMutationUnlessValid()
    {
        var rng = new Random(Seed);

        for (var i = 0; i < Cases; i++)
        {
            var action = AckRequiredActions[rng.Next(AckRequiredActions.Length)];
            var scenario = (OperationScenario)rng.Next(4);
            var acceptUpdates = rng.Next(2) == 0;
            var status = ChooseStatus(scenario, rng);

            var ctx = new PipelineContext(validationEnabled: true, acceptUpdates: acceptUpdates);
            ctx.SetupGetOperation(scenario, status);

            var payload = CreateAckPayload(action);

            // CouldNotValidate and SubscriptionMismatch surface as exceptions; the
            // controller maps them to 5xx/4xx. AlreadyResolved and Valid return
            // normally. Either way the invariant below must hold.
            try
            {
                await ctx.Processor.ProcessWebhookNotificationAsync(payload, NullConfig());
            }
            catch (WebhookValidationException)
            {
                // Expected for Null (retryable) and Mismatch (non-retryable).
            }

            var expectedValid = scenario == OperationScenario.Actionable;
            var detail = $"case#{i} seed={Seed} action={action} scenario={scenario} " +
                         $"status={status} acceptUpdates={acceptUpdates}";

            if (!expectedValid)
            {
                ctx.VerifyNoMutation(detail);
                ctx.VerifyNoPatch(detail);
            }
            else
            {
                // When the outcome is Valid the handler is dispatched; mutation is
                // permitted (it still depends on the accept gate) but the
                // one-directional invariant "mutation => Valid" is satisfied by
                // construction here. The dispatch must have consulted Get Operation.
                ctx.VerifyGetOperationCalledOnce(detail);
            }
        }
    }

    /// <summary>
    /// Property 2: notify-only purity. For every notify-only action, over a
    /// random sweep of the validation flag, the accept gate, and the (ignored)
    /// operation scenario, neither the Get Operation API nor the PATCH Operation
    /// API is ever called.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [TestMethod]
    public async Task Property2_NotifyOnlyPurity()
    {
        var rng = new Random(Seed);

        for (var i = 0; i < Cases; i++)
        {
            var action = NotifyOnlyActions[rng.Next(NotifyOnlyActions.Length)];
            var scenario = (OperationScenario)rng.Next(4);
            var validationEnabled = rng.Next(2) == 0;
            var acceptUpdates = rng.Next(2) == 0;
            var status = ChooseStatus(scenario, rng);

            var ctx = new PipelineContext(validationEnabled, acceptUpdates);
            ctx.SetupGetOperation(scenario, status);

            var payload = CreateNotifyOnlyPayload(action);

            await ctx.Processor.ProcessWebhookNotificationAsync(payload, NullConfig());

            var detail = $"case#{i} seed={Seed} action={action} validationEnabled={validationEnabled} " +
                         $"acceptUpdates={acceptUpdates}";

            ctx.VerifyGetOperationNeverCalled(detail);
            ctx.VerifyNoPatch(detail);
        }
    }

    /// <summary>
    /// Property 5: idempotency for terminal operations. For every ACK-required
    /// action, when validation is enabled and the (matching) operation is already
    /// terminal, repeated deliveries produce no mutation, no PATCH, and no thrown
    /// validation exception (the controller would return 200).
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [TestMethod]
    public async Task Property5_TerminalOperationsAreIdempotent()
    {
        var rng = new Random(Seed);

        for (var i = 0; i < Cases; i++)
        {
            var action = AckRequiredActions[rng.Next(AckRequiredActions.Length)];
            var status = TerminalStatuses[rng.Next(TerminalStatuses.Length)];
            var acceptUpdates = rng.Next(2) == 0;
            var deliveries = 1 + rng.Next(3); // 1..3 repeated deliveries

            var ctx = new PipelineContext(validationEnabled: true, acceptUpdates: acceptUpdates);
            ctx.SetupGetOperation(OperationScenario.Terminal, status);

            var payload = CreateAckPayload(action);
            var detail = $"case#{i} seed={Seed} action={action} status={status} " +
                         $"acceptUpdates={acceptUpdates} deliveries={deliveries}";

            for (var d = 0; d < deliveries; d++)
            {
                // Already-resolved operations must never throw: a duplicate
                // delivery is skipped and acknowledged with 200.
                await ctx.Processor.ProcessWebhookNotificationAsync(payload, NullConfig());
            }

            ctx.VerifyNoMutation(detail);
            ctx.VerifyNoPatch(detail);
        }
    }

    private static OperationStatusEnum ChooseStatus(OperationScenario scenario, Random rng)
    {
        return scenario switch
        {
            OperationScenario.Terminal => TerminalStatuses[rng.Next(TerminalStatuses.Length)],
            _ => NonTerminalStatuses[rng.Next(NonTerminalStatuses.Length)],
        };
    }

    private static WebhookPayload CreateAckPayload(WebhookAction action)
    {
        return new WebhookPayload
        {
            Action = action,
            SubscriptionId = PayloadSubscriptionId,
            OperationId = OperationId,
            PlanId = NewPlanId,
            Quantity = 5,
            Subscription = new SubscriptionWebhookResult { PlanId = OldPlanId, Quantity = 1 },
        };
    }

    private static WebhookPayload CreateNotifyOnlyPayload(WebhookAction action)
    {
        return new WebhookPayload
        {
            Action = action,
            SubscriptionId = PayloadSubscriptionId,
            OperationId = OperationId,
            PlanId = OldPlanId,
            Quantity = 1,
            Subscription = new SubscriptionWebhookResult { PlanId = OldPlanId, Quantity = 1 },
        };
    }

    private static SaaSApiClientConfiguration NullConfig()
    {
        return new SaaSApiClientConfiguration();
    }

    /// <summary>
    /// Builds and holds the real processor pipeline (real validator + real
    /// handler) wired onto fresh Moq doubles for a single generated case, and
    /// exposes the assertions used by the properties.
    /// </summary>
    private sealed class PipelineContext
    {
        private readonly Mock<IFulfillmentApiService> fulfillApiService;
        private readonly Mock<ISubscriptionsRepository> subscriptionsRepository;
        private readonly Mock<ISubscriptionLogRepository> subscriptionLogRepository;
        private readonly Mock<IApplicationConfigRepository> applicationConfigRepository;
        private readonly Mock<IApplicationLogRepository> applicationLogRepository;
        private readonly Mock<IWebNotificationService> webNotificationService;

        public PipelineContext(bool validationEnabled, bool acceptUpdates)
        {
            this.fulfillApiService = new Mock<IFulfillmentApiService>();
            this.subscriptionsRepository = new Mock<ISubscriptionsRepository>();
            this.subscriptionLogRepository = new Mock<ISubscriptionLogRepository>();
            this.applicationConfigRepository = new Mock<IApplicationConfigRepository>();
            this.applicationLogRepository = new Mock<IApplicationLogRepository>();
            this.webNotificationService = new Mock<IWebNotificationService>();

            this.applicationConfigRepository
                .Setup(x => x.GetValueByName(ValidateWebhookOperation))
                .Returns(validationEnabled ? "true" : "false");
            this.applicationConfigRepository
                .Setup(x => x.GetValueByName(AcceptSubscriptionUpdates))
                .Returns(acceptUpdates ? "true" : "false");

            // The subscription exists locally (SubscribeId > 0) so that a Valid +
            // accepted apply is able to mutate state; this keeps the Property 1
            // "mutation => Valid" invariant meaningful rather than vacuous.
            this.subscriptionsRepository
                .Setup(x => x.GetById(It.IsAny<Guid>(), It.IsAny<bool>()))
                .Returns(new Subscriptions
                {
                    Id = 1,
                    AmpsubscriptionId = PayloadSubscriptionId,
                    AmpplanId = OldPlanId,
                    AmpOfferId = "offer-1",
                    Ampquantity = 1,
                    SubscriptionStatus = "Suspended",
                    Name = "test-subscription",
                    UserId = 1,
                });

            // PATCH (if ever issued) returns a 200 response.
            this.fulfillApiService
                .Setup(x => x.PatchOperationStatusResultAsync(
                    It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<SdkModels.UpdateOperationStatusEnum>()))
                .ReturnsAsync(Mock.Of<Response>(r => r.Status == 200));

            this.webNotificationService
                .Setup(x => x.PushExternalWebNotificationAsync(It.IsAny<WebhookPayload>()))
                .Returns(Task.CompletedTask);

            this.Processor = this.BuildProcessor();
        }

        public WebhookProcessor Processor { get; }

        public void SetupGetOperation(OperationScenario scenario, OperationStatusEnum status)
        {
            switch (scenario)
            {
                case OperationScenario.Null:
                    this.fulfillApiService
                        .Setup(x => x.GetOperationStatusResultAsync(PayloadSubscriptionId, OperationId))
                        .ReturnsAsync((OperationResult)null);
                    break;

                case OperationScenario.Mismatch:
                    this.fulfillApiService
                        .Setup(x => x.GetOperationStatusResultAsync(PayloadSubscriptionId, OperationId))
                        .ReturnsAsync(new OperationResult
                        {
                            ID = OperationId.ToString(),
                            SubscriptionId = OtherSubscriptionId.ToString(),
                            Status = status,
                        });
                    break;

                default:
                    this.fulfillApiService
                        .Setup(x => x.GetOperationStatusResultAsync(PayloadSubscriptionId, OperationId))
                        .ReturnsAsync(new OperationResult
                        {
                            ID = OperationId.ToString(),
                            SubscriptionId = PayloadSubscriptionId.ToString(),
                            Status = status,
                        });
                    break;
            }
        }

        public void VerifyNoMutation(string detail)
        {
            this.subscriptionsRepository.Verify(
                x => x.UpdatePlanForSubscription(It.IsAny<Guid>(), It.IsAny<string>()),
                Times.Never,
                $"Plan must not be mutated. {detail}");
            this.subscriptionsRepository.Verify(
                x => x.UpdateQuantityForSubscription(It.IsAny<Guid>(), It.IsAny<int>()),
                Times.Never,
                $"Quantity must not be mutated. {detail}");
            this.subscriptionsRepository.Verify(
                x => x.UpdateStatusForSubscription(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<bool>()),
                Times.Never,
                $"Status must not be mutated. {detail}");
        }

        public void VerifyNoPatch(string detail)
        {
            this.fulfillApiService.Verify(
                x => x.PatchOperationStatusResultAsync(
                    It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<SdkModels.UpdateOperationStatusEnum>()),
                Times.Never,
                $"No PATCH acknowledgment must be issued. {detail}");
        }

        public void VerifyGetOperationNeverCalled(string detail)
        {
            this.fulfillApiService.Verify(
                x => x.GetOperationStatusResultAsync(It.IsAny<Guid>(), It.IsAny<Guid>()),
                Times.Never,
                $"Get Operation API must not be called. {detail}");
        }

        public void VerifyGetOperationCalledOnce(string detail)
        {
            this.fulfillApiService.Verify(
                x => x.GetOperationStatusResultAsync(PayloadSubscriptionId, OperationId),
                Times.Once,
                $"Get Operation API must be called once for an ACK-required action. {detail}");
        }

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
}
