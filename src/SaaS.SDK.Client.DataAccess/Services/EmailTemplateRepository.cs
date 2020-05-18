namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    using System;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

    /// <summary>
    /// Repository to access Email Templates.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.IEmailTemplateRepository" />
    public class EmailTemplateRepository : IEmailTemplateRepository
    {
        /// <summary>
        /// The context.
        /// </summary>
        private readonly SaasKitContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailTemplateRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public EmailTemplateRepository(SaasKitContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Gets the email template for subscription status.
        /// </summary>
        /// <param name="status">The subscription status.</param>
        /// <returns>
        /// Email template relevant to the status of the subscription.
        /// </returns>
        public EmailTemplate GetTemplateForStatus(string status)
        {
            var template = this.context.EmailTemplate.Where(s => s.Status == status).FirstOrDefault();
            if (template != null)
            {
                return template;
            }

            return null;
        }

        /// <summary>
        /// Gets the email template for subscription status.
        /// </summary>
        /// <param name="subscriptionID">The subscription identifier.</param>
        /// <param name="processStatus">The process status.</param>
        /// <returns>
        /// Email template relevant to the status of the subscription.
        /// </returns>
        public string GetEmailBodyForSubscription(Guid subscriptionID, string processStatus)
        {
            var emialResult = this.context.SubscriptionEmailOutput.FromSqlRaw("dbo.spGetFormattedEmailBody {0},{1}", subscriptionID, processStatus).ToList();
            var emailRecord = emialResult.FirstOrDefault();
            if (emailRecord != null)
            {
                return emailRecord.Value;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
