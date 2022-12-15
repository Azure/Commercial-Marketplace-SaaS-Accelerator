using System;
using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Contracts;

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

    /// <summary>
    /// Gets the email body for subscription.
    /// </summary>
    /// <param name="subscriptionID">The subscription identifier.</param>
    /// <param name="processStatus">The process status.</param>
    /// <returns> Email body.</returns>
    string GetEmailBodyForSubscription(Guid subscriptionID, string processStatus);

    /// <summary>
    /// Gets all editable email templates
    /// </summary>
    /// <returns> A list of EmailTemplates </returns>
    IEnumerable<EmailTemplate> GetAll();

    /// <summary>
    /// Saves modified EmailTemplate
    /// </summary>
    /// <returns> Returns the status of the modified EmailTemplate </returns>
    string SaveEmailTemplateByStatus(EmailTemplate template);
}