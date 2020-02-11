namespace Microsoft.Marketplace.SaasKit.WebHook
{
    using System.Threading.Tasks;

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
    }
}
