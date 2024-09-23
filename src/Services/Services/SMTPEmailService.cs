using System;
using System.Net;
using System.Net.Mail;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.Models;
using Marketplace.SaaS.Accelerator.Services.Utilities;

namespace Marketplace.SaaS.Accelerator.Services.Services;

/// <summary>
/// Service to send emails using SMTP settings.
/// </summary>
/// <seealso cref="IEmailService" />
public class SMTPEmailService : IEmailService
{
    /// <summary>
    /// The application configuration repository.
    /// </summary>
    private readonly IApplicationConfigRepository applicationConfigRepository;

    /// <summary>
    /// The application log repository.
    /// </summary>
    private readonly IApplicationLogRepository applicationLogRepository;

    /// <summary>
    /// The application log service.
    /// </summary>
    private readonly ApplicationLogService applicationLogService;

    /// <summary>
    /// Initializes a new instance of the <see cref="SMTPEmailService"/> class.
    /// </summary>
    /// <param name="applicationConfigRepository">The application configuration repository.</param>
    public SMTPEmailService(IApplicationConfigRepository applicationConfigRepository,
                            IApplicationLogRepository applicationLogRepository)
    {
        this.applicationConfigRepository = applicationConfigRepository;
        this.applicationLogRepository = applicationLogRepository;
        this.applicationLogService = new ApplicationLogService(this.applicationLogRepository);
    }

    /// <summary>
    /// Sends the email.
    /// </summary>
    /// <param name="emailContent">Content of the email.</param>
    public void SendEmail(EmailContentModel emailContent)
    {
        if (!string.IsNullOrEmpty(emailContent.ToEmails) || !string.IsNullOrEmpty(emailContent.BCCEmails))
        {
            try
            {
                using (SmtpClient smtp = new SmtpClient())
                {
                    //set smtp settings
                    smtp.Host = emailContent.SMTPHost;
                    smtp.Port = emailContent.Port;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(
                        emailContent.UserName, emailContent.Password);
                    smtp.EnableSsl = emailContent.SSL;

                    //set message from, body, to and bcc
                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(emailContent.FromEmail);
                    mail.IsBodyHtml = true;
                    mail.Subject = emailContent.Subject;
                    mail.Body = emailContent.Body;
                    string[] toEmails = emailContent.ToEmails.Split(';');
                    foreach (string multimailid in toEmails)
                    {
                        mail.To.Add(new MailAddress(multimailid));
                    }

                    if (!string.IsNullOrEmpty(emailContent.BCCEmails))
                    {
                        foreach (string multimailid1 in toEmails)
                        {
                            mail.Bcc.Add(new MailAddress(multimailid1));
                        }
                    }

                    //send email
                    smtp.Send(mail);
                    this.applicationLogService.AddApplicationLog($"{emailContent?.Subject}: Email sent succesfully!").ConfigureAwait(false);
                }
            }
            catch (SmtpException smtpEx)
            {
                // Log SMTP specific exceptions here
                applicationLogService.AddApplicationLog($"{emailContent?.Subject}: SMTP exception {smtpEx.Message}.").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Log other general exceptions here
                applicationLogService.AddApplicationLog($"{emailContent?.Subject}: General exception {ex.Message}.").ConfigureAwait(false);
            }
        }
        else
        {
            applicationLogService.AddApplicationLog($"{emailContent?.Subject}: Email is Not sent because the To email address is empty. Update at the Email Template or Plan details page.").ConfigureAwait(false);
        }
    }
}