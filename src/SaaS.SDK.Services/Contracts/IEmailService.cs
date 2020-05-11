namespace Microsoft.Marketplace.SaaS.SDK.Services.Contracts
{
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;

    /// <summary>
    /// Contract for Emails service smtp /send grid.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="emailContent">Content of the email.</param>
        void SendEmail(EmailContentModel emailContent);
    }
}
