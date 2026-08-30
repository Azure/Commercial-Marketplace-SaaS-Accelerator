// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.Services.Configurations;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.Exceptions;
using Marketplace.SaaS.Accelerator.Services.Services;

namespace Marketplace.SaaS.Accelerator.Services.WebHook;

/// <summary>
/// The webhook processor.
/// </summary>
/// <seealso cref="Microsoft.Marketplace.SaasKit.WebHook.IWebhookProcessor" />
public class WebhookProcessor : IWebhookProcessor
{
    /// <summary>
    /// Application configuration key that enables Get Operation API validation of
    /// inbound webhook notifications. Defaults to enabled; only the literal
    /// "false" disables validation (legacy path).
    /// </summary>
    private const string ValidateWebhookOperation = "ValidateWebhookOperation";

    /// <summary>
    /// The set of actions that require validation and acknowledgment (ACK-required).
    /// Notify-only actions (<see cref="WebhookAction.Renew"/>,
    /// <see cref="WebhookAction.Suspend"/>, <see cref="WebhookAction.Unsubscribe"/>)
    /// are intentionally excluded.
    /// </summary>
    private static readonly HashSet<WebhookAction> AckRequired = new()
    {
        WebhookAction.ChangePlan,
        WebhookAction.ChangeQuantity,
        WebhookAction.Reinstate,
    };

    /// <summary>
    /// The webhook handler.
    /// </summary>
    private readonly IWebhookHandler webhookHandler;

    /// <summary>
    /// Defines the _apiClient.
    /// </summary>
    private IFulfillmentApiService apiClient;

    /// <summary>
    /// Defines the webNotificationService.
    /// </summary>
    private readonly IWebNotificationService _webNotificationService;

    /// <summary>
    /// Validates ACK-required notifications against the marketplace Get Operation API.
    /// </summary>
    private readonly IWebhookOperationValidator _operationValidator;

    /// <summary>
    /// The application configuration repository, used to read the
    /// <c>ValidateWebhookOperation</c> flag.
    /// </summary>
    private readonly IApplicationConfigRepository _applicationConfigRepository;

    /// <summary>
    /// The application log service used to record skipped (already-resolved) outcomes.
    /// </summary>
    private readonly ApplicationLogService _applicationLogService;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebhookProcessor"/> class.
    /// </summary>
    /// <param name="apiClient">The API client.</param>
    /// <param name="webhookHandler">The webhook handler.</param>
    /// <param name="webNotificationService">The web notification service.</param>
    /// <param name="operationValidator">The webhook operation validator.</param>
    /// <param name="applicationConfigRepository">The application configuration repository.</param>
    /// <param name="applicationLogRepository">The application log repository.</param>
    public WebhookProcessor(IFulfillmentApiService apiClient, 
                            IWebhookHandler webhookHandler,
                            IWebNotificationService webNotificationService,
                            IWebhookOperationValidator operationValidator,
                            IApplicationConfigRepository applicationConfigRepository,
                            IApplicationLogRepository applicationLogRepository)
    {
        this.apiClient = apiClient;
        this.webhookHandler = webhookHandler;
        this._webNotificationService = webNotificationService;
        this._operationValidator = operationValidator;
        this._applicationConfigRepository = applicationConfigRepository;
        this._applicationLogService = new ApplicationLogService(applicationLogRepository);
    }

    /// <summary>
    /// Processes the webhook notification asynchronous.
    /// </summary>
    /// <param name="payload">The payload.</param>
    /// <param name="config">Current environmental configuration</param>
    /// <returns> Notification.</returns>
    public async Task ProcessWebhookNotificationAsync(WebhookPayload payload, SaaSApiClientConfiguration config)
    {
        await _webNotificationService.PushExternalWebNotificationAsync(payload);

        // ACK-required actions are validated against the Get Operation API before
        // any local state mutation, but only when validation is enabled. Notify-only
        // actions and the legacy path skip the validator entirely.
        if (AckRequired.Contains(payload.Action) && this.IsValidateWebhookOperationEnabled())
        {
            var outcome = await this._operationValidator.ValidateAsync(payload).ConfigureAwait(false);
            switch (outcome)
            {
                case WebhookValidationOutcome.AlreadyResolved:
                    // Idempotent skip for duplicate deliveries: no mutation, no dispatch.
                    // The controller returns 200.
                    await this._applicationLogService.AddApplicationLog(
                        $"Operation already resolved, skipping. Sub:{payload.SubscriptionId} Op:{payload.OperationId}")
                        .ConfigureAwait(false);
                    return;

                case WebhookValidationOutcome.SubscriptionMismatch:
                    // Authenticity failure: permanently rejected (maps to 4xx).
                    throw new WebhookValidationException("Operation does not match subscription.", retryable: false);

                case WebhookValidationOutcome.CouldNotValidate:
                    // Transient/not-found: the marketplace should retry (maps to 5xx).
                    throw new WebhookValidationException("Get Operation validation failed.", retryable: true);

                case WebhookValidationOutcome.Valid:
                    // Authentic and actionable: fall through to dispatch.
                    break;
            }
        }

        switch (payload.Action)
        {
            case WebhookAction.Unsubscribe:
                await this.webhookHandler.UnsubscribedAsync(payload).ConfigureAwait(false);
                break;

            case WebhookAction.ChangePlan:
                await this.webhookHandler.ChangePlanAsync(payload).ConfigureAwait(false);
                break;

            case WebhookAction.ChangeQuantity:
                await this.webhookHandler.ChangeQuantityAsync(payload).ConfigureAwait(false);
                break;

            case WebhookAction.Suspend:
                await this.webhookHandler.SuspendedAsync(payload).ConfigureAwait(false);
                break;

            case WebhookAction.Reinstate:
                await this.webhookHandler.ReinstatedAsync(payload).ConfigureAwait(false);
                break;

            case WebhookAction.Renew:
                await this.webhookHandler.RenewedAsync().ConfigureAwait(false);
                break;

            default:
                await this.webhookHandler.UnknownActionAsync(payload).ConfigureAwait(false);
                break;
        }
    }

    /// <summary>
    /// Reads the <c>ValidateWebhookOperation</c> flag. An absent or unparseable
    /// value is treated as enabled (default true); only the literal "false"
    /// disables validation so existing deployments can restore legacy behavior.
    /// </summary>
    /// <returns><c>true</c> when Get Operation validation is enabled.</returns>
    private bool IsValidateWebhookOperationEnabled()
    {
        var value = this._applicationConfigRepository.GetValueByName(ValidateWebhookOperation);

        // Only the literal "false" disables validation; absent or unparseable
        // values default to enabled.
        return !(bool.TryParse(value, out var parsed) && parsed == false);
    }
}
