using System;
using System.Collections.Generic;
using System.Linq;
using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.SaaS.Accelerator.DataAccess.Services;

/// <summary>
/// Repository to access Email Templates.
/// </summary>
/// <seealso cref="IEmailTemplateRepository" />
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

    /// <summary>
    /// Gets all email templates
    /// </summary>
    /// <returns>
    /// List of email templates
    /// </returns>
    public IEnumerable<EmailTemplate> GetAll()
    {
        var templates = this.context.EmailTemplate;
        return templates;
    }

    /// <summary>
    /// Saves email configuration field
    /// </summary>
    /// <returns>
    /// True or False
    /// </returns>
    public string SaveEmailTemplateByStatus(EmailTemplate template)
    {
        var emailTemplate = this.context.EmailTemplate.Where(a => a.Status == template.Status).FirstOrDefault();
        if (emailTemplate != null)
        {
            emailTemplate.IsActive = template.IsActive;
            emailTemplate.Subject = template.Subject;
            emailTemplate.Description = template.Description;
            emailTemplate.TemplateBody = template.TemplateBody;
            emailTemplate.ToRecipients = template.ToRecipients;
            emailTemplate.Bcc = template.Bcc;
            emailTemplate.Cc = template.Cc;
            this.context.SaveChanges();
        }
        return template.Status;
    }
}