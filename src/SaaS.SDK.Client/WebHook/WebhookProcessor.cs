using Microsoft.Marketplace.SaasKit.Contracts;
using Microsoft.Marketplace.SaasKit.WebHook;
using System;
using System.Threading.Tasks;

namespace Microsoft.Marketplace.SaasKit.WebHook
{
    public class WebhookProcessor : IWebhookProcessor
    {
        /// <summary>
        /// Defines the _apiClient
        /// </summary>
        public IFulfillmentApiClient ApiClient;

        /// <summary>
        /// The webhook handler
        /// </summary>
        private readonly IWebhookHandler WebhookHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebhookProcessor"/> class.
        /// </summary>
        /// <param name="apiClient">The API client.</param>
        /// <param name="webhookHandler">The webhook handler.</param>
        public WebhookProcessor(IFulfillmentApiClient apiClient, IWebhookHandler webhookHandler)
        {
            ApiClient = apiClient;
            this.WebhookHandler = webhookHandler;
        }

        /// <summary>
        /// Processes the webhook notification asynchronous.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public async Task ProcessWebhookNotificationAsync(WebhookPayload payload)
        {
            switch (payload.Action)
            {
                case WebhookAction.Unsubscribe:
                    await this.WebhookHandler.UnsubscribedAsync(payload).ConfigureAwait(false);
                    break;

                case WebhookAction.ChangePlan:
                    await this.WebhookHandler.ChangePlanAsync(payload).ConfigureAwait(false);
                    break;

                case WebhookAction.ChangeQuantity:
                    await this.WebhookHandler.ChangeQuantityAsync(payload).ConfigureAwait(false);
                    break;

                case WebhookAction.Suspend:
                    await this.WebhookHandler.SuspendedAsync(payload).ConfigureAwait(false);
                    break;

                case WebhookAction.Reinstate:
                    await this.WebhookHandler.ReinstatedAsync(payload).ConfigureAwait(false);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
