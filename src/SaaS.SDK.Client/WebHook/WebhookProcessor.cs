namespace Microsoft.Marketplace.SaasKit.WebHook
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Marketplace.SaasKit.Contracts;

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
        private IFulfillmentApiClient apiClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebhookProcessor"/> class.
        /// </summary>
        /// <param name="apiClient">The API client.</param>
        /// <param name="webhookHandler">The webhook handler.</param>
        public WebhookProcessor(IFulfillmentApiClient apiClient, IWebhookHandler webhookHandler)
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
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
