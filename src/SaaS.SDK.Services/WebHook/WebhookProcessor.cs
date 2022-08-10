// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.
namespace Microsoft.Marketplace.SaaS.SDK.Services.WebHook
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Marketplace.SaaS.SDK.Services.Contracts;

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
        /// Initializes a new instance of the <see cref="WebhookProcessor"/> class.
        /// </summary>
        /// <param name="apiClient">The API client.</param>
        /// <param name="webhookHandler">The webhook handler.</param>
        public WebhookProcessor(IFulfillmentApiService apiClient, IWebhookHandler webhookHandler)
        {
            this.apiClient = apiClient;
            this.webhookHandler = webhookHandler;
        }

        /// <summary>
        /// Processes the webhook notification asynchronous.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <returns> Notification.</returns>
        public async Task ProcessWebhookNotificationAsync(WebhookPayload payload)
        {
            var webhookOperation = await apiClient.GetOperationStatusResultAsync(
                payload.SubscriptionId, payload.OperationId);

            if (webhookOperation != null && // Is the Marketplace aware of this webhook invocation?                       
                webhookOperation.ID == payload.OperationId.ToString() && // Does the operation ID match? Are we talking about the same invocation?
                webhookOperation.SubscriptionId == payload.SubscriptionId.ToString() && // Does it apply to this subscription?
                webhookOperation.ActionType == payload.Action.ToString()) // Does the action type match?
            {
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

                    default:
                        await this.webhookHandler.UnknownActionAsync(payload).ConfigureAwait(false);
                        break;
                }
            }
            else
            {
                // We couldn't verify this webhook notification so something is definitely wrong.
                // Throw an exception and let the upstream web app handle it...

                case WebhookAction.Renew:
                    await this.webhookHandler.RenewedAsync().ConfigureAwait(false);
                    break;
                
                default:
                    await this.webhookHandler.UnknownActionAsync(payload).ConfigureAwait(false);
                    break;
            }
        }
    }
}
