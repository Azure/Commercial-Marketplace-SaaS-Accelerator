namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

    /// <summary>
    /// Repository to access email templates.
    /// </summary>
    public interface IEmailTemplateRepository
    {
        /// <summary>
        /// Gets the email template for subscription status.
        /// </summary>
        /// <param name="status">The subscription status.</param>
        /// <returns>Email template relevant to the status of the subscription.</returns>
        EmailTemplate GetTemplateForStatus(string status);
    }
}
