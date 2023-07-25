// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System.Threading.Tasks;
using Marketplace.SaaS.Accelerator.Services.Configurations;
using Marketplace.SaaS.Accelerator.Services.Contracts;

namespace Marketplace.SaaS.Accelerator.Services.WebHook;

/// <summary>
/// The webhook processor.
/// </summary>
/// <seealso cref="Microsoft.Marketplace.SaasKit.WebHook.IWebhookProcessor" />
public class WebhookProcessor : IWebhookProcessor
{
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
    /// Initializes a new instance of the <see cref="WebhookProcessor"/> class.
    /// </summary>
    /// <param name="apiClient">The API client.</param>
    /// <param name="webhookHandler">The webhook handler.</param>
    public WebhookProcessor(IFulfillmentApiService apiClient, 
                            IWebhookHandler webhookHandler,
                            IWebNotificationService webNotificationService)
    {
        this.apiClient = apiClient;
        this.webhookHandler = webhookHandler;
        this._webNotificationService = webNotificationService;
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
}