using System;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.Services.Exceptions;
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
        string body = this.emailTemplateRepository.GetEmailBodyForSubscription(subscriptionID, processStatus);
        var subscriptionEvent = this.eventsRepository.GetByName(planEventName);
        var emailTemplateData = this.emailTemplateRepository.GetTemplateForStatus(subscriptionStatus);
        if (processStatus == "failure")
        {
            emailTemplateData = this.emailTemplateRepository.GetTemplateForStatus("Failed");
        }

        string subject = string.Empty;
        bool copyToCustomer = false;
        string toReceipents = string.Empty;
        string ccReceipents = string.Empty;
        string bccReceipents = string.Empty;

        var eventData = this.planEventsMappingRepository.GetPlanEvent(planGuId, subscriptionEvent.EventsId);

        //First add To, Cc, Bcc email addresses from email template
        if (emailTemplateData != null)
        {
            if (!string.IsNullOrEmpty(emailTemplateData.ToRecipients))
            {
                toReceipents = emailTemplateData.ToRecipients;
            }

            if (!string.IsNullOrEmpty(emailTemplateData.Cc))
            {
                ccReceipents = emailTemplateData.Cc;
            }

            if (!string.IsNullOrEmpty(emailTemplateData.Bcc))
            {
                bccReceipents = emailTemplateData.Bcc;
            }

            subject = emailTemplateData.Subject;
        }

        //If the plan event data contains plan specific ToEmailAddress then override the above
        if (eventData != null)
        {
            if (!string.IsNullOrEmpty(eventData.SuccessStateEmails))
            {
                toReceipents = eventData.SuccessStateEmails;
            }

            copyToCustomer = Convert.ToBoolean(eventData.CopyToCustomer);
        }

        if (string.IsNullOrEmpty(toReceipents))
        {
            throw new Exception(" Error while sending an email, please check the configuration. To email empty");
        }

        return FinalizeContentEmail(subject, body, ccReceipents, bccReceipents, toReceipents, copyToCustomer);
        
    }
    /// <summary>
    /// Prepares the content of the scheduler email.
    /// </summary>
    /// <param name="subscriptionName">The subscription Name.</param>
    /// <param name="schedulerTaskName">scheduler Task Name.</param>
    /// <param name="responseJson">response Json.</param>
    /// <param name="subscriptionStatus">The subscription status.</param>
    /// <returns>
    /// Email Content Model.
    /// </returns>
    /// <exception cref="Exception">Error while sending an email, please check the configuration.
    /// or
    /// Error while sending an email, please check the configuration.</exception>
    public EmailContentModel PrepareMeteredEmailContent(string schedulerTaskName, String subscriptionName, string subscriptionStatus, string responseJson)
    {
        var emailTemplateData = this.emailTemplateRepository.GetTemplateForStatus(subscriptionStatus);
        string toReceipents = this.applicationConfigRepository.GetValueByName("SchedulerEmailTo");
        if (string.IsNullOrEmpty(toReceipents))
        {
            throw new Exception(" Error while sending an email, please check the configuration. ");
        }
        var body = emailTemplateData.TemplateBody.Replace("****SubscriptionName****", subscriptionName).Replace("****SchedulerTaskName****", schedulerTaskName).Replace("****ResponseJson****", responseJson); ;
        return FinalizeContentEmail(emailTemplateData.Subject,body, string.Empty, string.Empty, toReceipents, false);
    }
    private EmailContentModel FinalizeContentEmail(string subject, string body, string ccEmails,string bcEmails, string toEmails, bool copyToCustomer)
    {
        EmailContentModel emailContent = new EmailContentModel();
        emailContent.BCCEmails = bcEmails;
        emailContent.CCEmails = ccEmails;
        emailContent.ToEmails = toEmails;
        emailContent.IsActive = false;
        emailContent.Subject = subject;
        emailContent.Body = body;
        emailContent.CopyToCustomer = copyToCustomer;
        emailContent.FromEmail = this.applicationConfigRepository.GetValueByName("SMTPFromEmail");
        emailContent.Password = this.applicationConfigRepository.GetValueByName("SMTPPassword");
        emailContent.SSL = bool.TryParse(this.applicationConfigRepository.GetValueByName("SMTPSslEnabled"), out bool smtpssl) ? smtpssl : throw new MarketplaceException("Invalid settings : SMTP SSL");
        emailContent.UserName = this.applicationConfigRepository.GetValueByName("SMTPUserName");
        emailContent.Port = int.TryParse(this.applicationConfigRepository.GetValueByName("SMTPPort"), out int smtpport) ? smtpport : throw new MarketplaceException("Invalid settings : SMTP Port");
        emailContent.SMTPHost = this.applicationConfigRepository.GetValueByName("SMTPHost");
        return emailContent;
    }


}