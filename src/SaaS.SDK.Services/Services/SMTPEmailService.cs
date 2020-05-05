using Microsoft.Marketplace.SaaS.SDK.Services.Contracts;
using Microsoft.Marketplace.SaaS.SDK.Services.Models;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Microsoft.Marketplace.SaaS.SDK.Services.Services
{
    public class SMTPEmailService : IEmailService
    {
        private readonly IApplicationConfigRepository applicationConfigRepository;

        public SMTPEmailService(IApplicationConfigRepository applicationConfigRepository)
        {
            this.applicationConfigRepository = applicationConfigRepository;

        }
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
