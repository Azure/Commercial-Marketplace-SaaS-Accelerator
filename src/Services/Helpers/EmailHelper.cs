using System;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.Services.Models;

namespace Marketplace.SaaS.Accelerator.Services.Helpers;

/// <summary>
/// Email Helper.
/// </summary>
public class EmailHelper
{
    private readonly IApplicationConfigRepository applicationConfigRepository;
    private readonly ISubscriptionsRepository subscriptionsRepository;
    private readonly IEmailTemplateRepository emailTemplateRepository;
    private readonly IEventsRepository eventsRepository;
    private readonly IPlanEventsMappingRepository planEventsMappingRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailHelper"/> class.
    /// </summary>
    /// <param name="applicationConfigRepository">The application configuration repository.</param>
    /// <param name="subscriptionsRepository">The subscriptions repository.</param>
    /// <param name="emailTemplateRepository">The email template repository.</param>
    /// <param name="planEventsMappingRepository">The plan events mapping repository.</param>
    /// <param name="eventsRepository">The events repository.</param>
    public EmailHelper(
        IApplicationConfigRepository applicationConfigRepository,
        ISubscriptionsRepository subscriptionsRepository,
        IEmailTemplateRepository emailTemplateRepository,
        IPlanEventsMappingRepository planEventsMappingRepository,
        IEventsRepository eventsRepository)
    {
        this.applicationConfigRepository = applicationConfigRepository;
        this.subscriptionsRepository = subscriptionsRepository;
        this.emailTemplateRepository = emailTemplateRepository;
        this.eventsRepository = eventsRepository;
        this.planEventsMappingRepository = planEventsMappingRepository;
    }

    /// <summary>
    /// Prepares the content of the email.
    /// </summary>
    /// <param name="subscriptionID">The subscription identifier.</param>
    /// <param name="planGuId">The plan gu identifier.</param>
    /// <param name="processStatus">The process status.</param>
    /// <param name="planEventName">Name of the plan event.</param>
    /// <param name="subscriptionStatus">The subscription status.</param>
    /// <returns>
    /// Email Content Model.
    /// </returns>
    /// <exception cref="Exception">Error while sending an email, please check the configuration.
    /// or
    /// Error while sending an email, please check the configuration.</exception>
    public EmailContentModel PrepareEmailContent(Guid subscriptionID, Guid planGuId, string processStatus, string planEventName, string subscriptionStatus)
    {
        EmailContentModel emailContent = new EmailContentModel();
        string body = this.emailTemplateRepository.GetEmailBodyForSubscription(subscriptionID, processStatus);
        var subscriptionEvent = this.eventsRepository.GetByName(planEventName);
        var emailTemplateData = this.emailTemplateRepository.GetTemplateForStatus(subscriptionStatus);
        if (processStatus == "failure")
        {
            emailTemplateData = this.emailTemplateRepository.GetTemplateForStatus("Failed");
        }

        string subject = string.Empty;

        bool copyToCustomer = false;
        bool isActive = false;
        string toReceipents = string.Empty;
        string ccReceipents = string.Empty;
        string bccReceipents = string.Empty;

        string fromMail = this.applicationConfigRepository.GetValueByName("SMTPFromEmail");
        string password = this.applicationConfigRepository.GetValueByName("SMTPPassword");
        string username = this.applicationConfigRepository.GetValueByName("SMTPUserName");
        bool smtpSsl = bool.Parse(this.applicationConfigRepository.GetValueByName("SMTPSslEnabled"));
        int port = int.Parse(this.applicationConfigRepository.GetValueByName("SMTPPort"));
        string smtpHost = this.applicationConfigRepository.GetValueByName("SMTPHost");

        var eventData = this.planEventsMappingRepository.GetPlanEvent(planGuId, subscriptionEvent.EventsId);

        if (eventData != null)
        {
            toReceipents = eventData.SuccessStateEmails;
            copyToCustomer = Convert.ToBoolean(eventData.CopyToCustomer);
        }

        if (string.IsNullOrEmpty(toReceipents))
        {
            throw new Exception(" Error while sending an email, please check the configuration. ");
        }

        if (emailTemplateData != null)
        {
            if (!string.IsNullOrEmpty(toReceipents) && !string.IsNullOrEmpty(emailTemplateData.Cc))
            {
                ccReceipents = emailTemplateData.Cc;
            }

            if (!string.IsNullOrEmpty(emailTemplateData.Bcc))
            {
                bccReceipents = emailTemplateData.Bcc;
            }

            subject = emailTemplateData.Subject;
        }

        emailContent.BCCEmails = bccReceipents;
        emailContent.CCEmails = ccReceipents;
        emailContent.ToEmails = toReceipents;
        emailContent.Body = body;
        emailContent.Subject = subject;
        emailContent.CopyToCustomer = copyToCustomer;
        emailContent.IsActive = isActive;
        emailContent.FromEmail = fromMail;
        emailContent.Password = password;
        emailContent.SSL = smtpSsl;
        emailContent.UserName = username;
        emailContent.Port = port;
        emailContent.SMTPHost = smtpHost;

        return emailContent;
    }
}