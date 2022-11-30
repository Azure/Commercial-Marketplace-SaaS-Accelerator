// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System.Threading.Tasks;

namespace Marketplace.SaaS.Accelerator.Services.WebHook;

/// <summary>
/// Web Hook Handler Interface
/// </summary>
public interface IWebhookHandler
{
    /// <summary>
    /// Changes the plan asynchronous.
    /// </summary>
    /// <param name="payload">The payload.</param>
    /// <returns>Change PlanAsync</returns>
    Task ChangePlanAsync(WebhookPayload payload);

    /// <summary>
    /// Changes the quantity asynchronous.
    /// </summary>
    /// <param name="payload">The payload.</param>
    /// <returns>Change QuantityAsync</returns>
    Task ChangeQuantityAsync(WebhookPayload payload);

    /// <summary>
    /// Reinitiated the asynchronous.
    /// </summary>
    /// <param name="payload">The payload.</param>
    /// <returns>Reinstated Async Async</returns>
    Task ReinstatedAsync(WebhookPayload payload);

    /// <summary>
    /// Renewed subscription state
    /// </summary>
    /// <param name="payload">The payload.</param>
    /// <returns>Renewed Async</returns>
    Task RenewedAsync();

    /// <summary>
    /// Suspended the asynchronous.
    /// </summary>
    /// <param name="payload">The payload.</param>
    /// <returns>Suspended Async</returns>
    Task SuspendedAsync(WebhookPayload payload);

    /// <summary>
    /// Unsubscribed the asynchronous.
    /// </summary>
    /// <param name="payload">The payload.</param>
    /// <returns>Unsubscribed Async</returns>
    Task UnsubscribedAsync(WebhookPayload payload);

    /// <summary>
    /// Unknowstate the asynchronous.
    /// </summary>
    /// <param name="payload">The payload.</param>
    /// <returns>Unsubscribed Async</returns>
    Task UnknownActionAsync(WebhookPayload payload);

}