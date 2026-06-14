// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System.Threading.Tasks;

namespace Marketplace.SaaS.Accelerator.Services.WebHook;

/// <summary>
/// Outcome of validating a webhook notification against the marketplace Get Operation API.
/// </summary>
public enum WebhookValidationOutcome
{
    /// <summary>
    /// The operation is authentic and actionable; processing should proceed.
    /// </summary>
    Valid,

    /// <summary>
    /// The operation is authentic but its status is terminal; the notification
    /// should be skipped (idempotent duplicate delivery) and acknowledged with 200.
    /// </summary>
    AlreadyResolved,

    /// <summary>
    /// The operation belongs to a different subscription than the payload;
    /// the notification is unauthenticated and should be rejected (4xx).
    /// </summary>
    SubscriptionMismatch,

    /// <summary>
    /// The Get Operation API returned no operation (not found or transient
    /// failure); the notification could not be validated and should be retried (5xx).
    /// </summary>
    CouldNotValidate
}

/// <summary>
/// Validates inbound webhook notifications against the marketplace Get Operation
/// API before any local subscription state is mutated.
/// </summary>
public interface IWebhookOperationValidator
{
    /// <summary>
    /// Validates the webhook payload by confirming the operation exists and
    /// belongs to the payload's subscription, and reports whether it is actionable.
    /// </summary>
    /// <param name="payload">The inbound webhook payload.</param>
    /// <returns>The <see cref="WebhookValidationOutcome"/> describing the result.</returns>
    Task<WebhookValidationOutcome> ValidateAsync(WebhookPayload payload);
}
