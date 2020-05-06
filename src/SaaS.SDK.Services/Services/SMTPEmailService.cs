namespace Microsoft.Marketplace.SaaS.SDK.Services.Services
{
    using Microsoft.Marketplace.SaaS.SDK.Services.Contracts;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using System.Net;
    using System.Net.Mail;

    /// <summary>
    /// Service to send emails using SMTP settings
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaaS.SDK.Services.Contracts.IEmailService" />
    public class SMTPEmailService : IEmailService
    {
        /// <summary>
        /// The application configuration repository
        /// </summary>
        private readonly IApplicationConfigRepository applicationConfigRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SMTPEmailService"/> class.
        /// </summary>
        /// <param name="applicationConfigRepository">The application configuration repository.</param>
        public SMTPEmailService(IApplicationConfigRepository applicationConfigRepository)
        {
            this.applicationConfigRepository = applicationConfigRepository;
        }

        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="emailContent">Content of the email.</param>
        public void SendEmail(EmailContentModel emailContent)
        {
            MailMessage mail = new MailMessage();
            if (!string.IsNullOrEmpty(emailContent.ToEmails) || !string.IsNullOrEmpty(emailContent.BCCEmails))
            {

                mail.From = new MailAddress(emailContent.FromEmail);
                mail.IsBodyHtml = true;

                string[] toEmails = (emailContent.ToEmails).Split(';');
                foreach (string Multimailid in toEmails)
                {
                    mail.To.Add(new MailAddress(Multimailid));
                }

                if (!string.IsNullOrEmpty(emailContent.BCCEmails))
                {
                    string[] bccEmails = (emailContent.BCCEmails).Split(';');
                    foreach (string Multimailid in toEmails)
                    {
                        mail.Bcc.Add(new MailAddress(Multimailid));
                    }
                }
                SmtpClient smtp = new SmtpClient();
                smtp.Host = emailContent.SMTPHost;
                smtp.Port = emailContent.Port;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(
                    emailContent.UserName, emailContent.Password);
                smtp.EnableSsl = emailContent.SSL;
                smtp.Send(mail);
            }
        }
    }
}