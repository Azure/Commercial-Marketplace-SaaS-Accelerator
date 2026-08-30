// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Threading.Tasks;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.Models;
using Marketplace.SaaS.Accelerator.Services.Services;

namespace Marketplace.SaaS.Accelerator.Services.WebHook;

/// <summary>
/// Validates inbound webhook notifications against the marketplace Get Operation
/// API before any local subscription state is mutated.
/// </summary>
/// <remarks>
/// This validator performs only the read-side authorization check (Get Operation).
/// Authorization is based on operation existence and subscription match only; plan
/// and quantity values are intentionally not reconciled (Requirement 1.6). The
/// <c>ValidateWebhookOperation</c> configuration flag is honored by the processor,
/// which skips this validator entirely when validation is disabled (legacy path).
/// </remarks>
/// <seealso cref="IWebhookOperationValidator" />
public class WebhookOperationValidator : IWebhookOperationValidator
{
    /// <summary>
    /// The fulfillment API service used to call the Get Operation API.
    /// </summary>
    private readonly IFulfillmentApiService fulfillApiService;

    /// <summary>
    /// The application log service used to record each validation outcome.
    /// </summary>
    private readonly ApplicationLogService applicationLogService;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebhookOperationValidator"/> class.
    /// </summary>
    /// <param name="fulfillApiService">The fulfillment API service.</param>
    /// <param name="applicationLogRepository">The application log repository.</param>
    public WebhookOperationValidator(
        IFulfillmentApiService fulfillApiService,
        IApplicationLogRepository applicationLogRepository)
    {
        this.fulfillApiService = fulfillApiService;
        this.applicationLogService = new ApplicationLogService(applicationLogRepository);
    }

    /// <summary>
    /// Validates the webhook payload by confirming the operation exists and
    /// belongs to the payload's subscription, and reports whether it is actionable.
    /// </summary>
    /// <param name="payload">The inbound webhook payload.</param>
    /// <returns>The <see cref="WebhookValidationOutcome"/> describing the result.</returns>
    public async Task<WebhookValidationOutcome> ValidateAsync(WebhookPayload payload)
    {
        // Call the Get Operation API with the payload's subscription and operation ids.
        var operation = await this.fulfillApiService
            .GetOperationStatusResultAsync(payload.SubscriptionId, payload.OperationId)
            .ConfigureAwait(false);

        // A null result covers not-found and transient/non-success responses (the
        // fulfillment service swallows the status code and returns null). Treat as
        // a condition that could not be validated and should be retried (5xx).
        if (operation == null)
        {
            await this.LogOutcomeAsync(WebhookValidationOutcome.CouldNotValidate, payload).ConfigureAwait(false);
            return WebhookValidationOutcome.CouldNotValidate;
        }

        // The operation must belong to the payload's subscription. Compare as GUIDs
        // when possible (case-insensitive); fall back to a case-insensitive string
        // compare if the operation's subscription id is not a parseable GUID.
        if (!SubscriptionMatches(payload.SubscriptionId, operation.SubscriptionId))
        {
            await this.LogOutcomeAsync(WebhookValidationOutcome.SubscriptionMismatch, payload).ConfigureAwait(false);
            return WebhookValidationOutcome.SubscriptionMismatch;
        }

        // A terminal status indicates the operation has already been resolved; skip
        // mutation for idempotency on duplicate deliveries.
        if (IsTerminal(operation.Status))
        {
            await this.LogOutcomeAsync(WebhookValidationOutcome.AlreadyResolved, payload).ConfigureAwait(false);
            return WebhookValidationOutcome.AlreadyResolved;
        }

        // NotStarted or InProgress: the operation is authentic and actionable.
        await this.LogOutcomeAsync(WebhookValidationOutcome.Valid, payload).ConfigureAwait(false);
        return WebhookValidationOutcome.Valid;
    }

    /// <summary>
    /// Determines whether the operation's subscription id matches the payload's
    /// subscription id using a case-insensitive GUID comparison.
    /// </summary>
    /// <param name="payloadSubscriptionId">The payload subscription id.</param>
    /// <param name="operationSubscriptionId">The operation subscription id (string).</param>
    /// <returns><c>true</c> when the subscriptions match; otherwise <c>false</c>.</returns>
    private static bool SubscriptionMatches(Guid payloadSubscriptionId, string operationSubscriptionId)
    {
        if (string.IsNullOrWhiteSpace(operationSubscriptionId))
        {
            return false;
        }

        if (Guid.TryParse(operationSubscriptionId, out var operationGuid))
        {
            return payloadSubscriptionId == operationGuid;
        }

        return string.Equals(
            payloadSubscriptionId.ToString(),
            operationSubscriptionId,
            StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines whether the operation status is terminal (already resolved).
    /// </summary>
    /// <param name="status">The operation status.</param>
    /// <returns><c>true</c> when the status is terminal; otherwise <c>false</c>.</returns>
    private static bool IsTerminal(OperationStatusEnum status)
    {
        return status == OperationStatusEnum.Succeeded
            || status == OperationStatusEnum.Failed
            || status == OperationStatusEnum.Conflict;
    }

    /// <summary>
    /// Records a validation outcome via the application log, including the action
    /// type, subscription id, and operation id. Tokens are never logged.
    /// </summary>
    /// <param name="outcome">The validation outcome.</param>
    /// <param name="payload">The inbound webhook payload.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private Task LogOutcomeAsync(WebhookValidationOutcome outcome, WebhookPayload payload)
    {
        return this.applicationLogService.AddApplicationLog(
            $"Webhook operation validation outcome: {outcome}. " +
            $"Action:{payload.Action} Sub:{payload.SubscriptionId} Op:{payload.OperationId}");
    }
}
