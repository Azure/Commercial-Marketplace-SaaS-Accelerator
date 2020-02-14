namespace Microsoft.Marketplace.SaasKit.WebHook
{
    using System.Threading.Tasks;

    /// <summary>
    /// Web hook Processor Interface
    /// </summary>
    public interface IWebhookProcessor
    {
        /// <summary>
        /// Processes the Web hook notification asynchronous.
        /// </summary>
        /// <param name="details">The details.</param>
        /// <returns>Processes the Web hook notification</returns>
        Task ProcessWebhookNotificationAsync(WebhookPayload details);
    }
}
