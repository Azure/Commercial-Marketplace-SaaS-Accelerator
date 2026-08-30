// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;

namespace Marketplace.SaaS.Accelerator.Services.Exceptions;

/// <summary>
/// Raised when an inbound webhook notification fails Get Operation validation.
/// </summary>
/// <remarks>
/// The <see cref="Retryable"/> flag tells the controller which HTTP status to
/// return so the marketplace can decide whether to retry:
/// <list type="bullet">
/// <item>
/// <description>
/// <see cref="Retryable"/> == <c>false</c> maps to a 4xx response (for example a
/// subscription mismatch). The notification is permanently rejected and the
/// marketplace should not retry.
/// </description>
/// </item>
/// <item>
/// <description>
/// <see cref="Retryable"/> == <c>true</c> maps to a 5xx response (for example a
/// transient Get Operation failure). The marketplace should retry the delivery.
/// </description>
/// </item>
/// </list>
/// </remarks>
/// <seealso cref="System.Exception" />
public class WebhookValidationException : Exception
{
    /// <summary>
    /// Gets a value indicating whether the failure is retryable.
    /// </summary>
    /// <value>
    /// <c>false</c> maps to a 4xx response (non-retryable, e.g. subscription
    /// mismatch); <c>true</c> maps to a 5xx response (retryable, e.g. a
    /// transient Get Operation failure).
    /// </value>
    public bool Retryable { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebhookValidationException"/> class.
    /// </summary>
    /// <param name="message">A message that describes the validation failure.</param>
    /// <param name="retryable">
    /// <c>true</c> if the marketplace should retry (maps to 5xx); <c>false</c> if
    /// the notification is permanently rejected (maps to 4xx).
    /// </param>
    public WebhookValidationException(string message, bool retryable)
        : base(message)
    {
        this.Retryable = retryable;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebhookValidationException"/> class.
    /// </summary>
    /// <param name="message">A message that describes the validation failure.</param>
    /// <param name="retryable">
    /// <c>true</c> if the marketplace should retry (maps to 5xx); <c>false</c> if
    /// the notification is permanently rejected (maps to 4xx).
    /// </param>
    /// <param name="inner">The inner exception.</param>
    public WebhookValidationException(string message, bool retryable, Exception inner)
        : base(message, inner)
    {
        this.Retryable = retryable;
    }
}
