using Microsoft.Marketplace.SaasKit.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Marketplace.SaasKit.Client.Services;
using Microsoft.Marketplace.SaasKit.Models;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Services;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using log4net;
using Microsoft.Extensions.Logging;
//using SendGrid;
//using SendGrid.Helpers.Mail;
using Newtonsoft.Json;
namespace Microsoft.Marketplace.SaasKit.Client.Helpers
{
    public class EmailHelper
    {

        public static void SendEmail(SubscriptionResultExtension Subscription, IApplicationConfigRepository applicationConfigRepository, IEmailTemplateRepository emailTemplateRepository)
        {
            MailMessage mail = new MailMessage();
            string FromMail = applicationConfigRepository.GetValuefromApplicationConfig("SMTPFromEmail");
            string password = applicationConfigRepository.GetValuefromApplicationConfig("SMTPPassword");
            string username = applicationConfigRepository.GetValuefromApplicationConfig("SMTPUserName");
            string Subject = emailTemplateRepository.GetSubject(Subscription.SaasSubscriptionStatus.ToString());
            bool smtpSsl = bool.Parse(applicationConfigRepository.GetValuefromApplicationConfig("SMTPSslEnabled"));
            mail.From = new MailAddress(FromMail);

            mail.Subject = Subject;

            string body = TemplateService.ProcessTemplate(Subscription, emailTemplateRepository, applicationConfigRepository);
            mail.Body = body;
            mail.IsBodyHtml = true;
            if (!string.IsNullOrEmpty(emailTemplateRepository.GetToRecipients(Subscription.SaasSubscriptionStatus.ToString())))
            {
                string[] ToEmails = (emailTemplateRepository.GetToRecipients(Subscription.SaasSubscriptionStatus.ToString())).Split(';');
                foreach (string Multimailid in ToEmails)
                {
                    mail.To.Add(new MailAddress(Multimailid));
                }
            }


            if (!string.IsNullOrEmpty(emailTemplateRepository.GetCCRecipients(Subscription.SaasSubscriptionStatus.ToString())))
            {
                string[] CcEmails = (emailTemplateRepository.GetCCRecipients(Subscription.SaasSubscriptionStatus.ToString())).Split(';');
                foreach (string Multimailid in CcEmails)
                {
                    mail.CC.Add(new MailAddress(Multimailid));
                }
            }

            if (!string.IsNullOrEmpty(emailTemplateRepository.GetBccRecipients(Subscription.SaasSubscriptionStatus.ToString())))
            {
                string[] BccEmails = (emailTemplateRepository.GetBccRecipients(Subscription.SaasSubscriptionStatus.ToString())).Split(';');
                foreach (string Multimailid in BccEmails)
                {
                    mail.Bcc.Add(new MailAddress(Multimailid));
                }
            }
            SmtpClient smtp = new SmtpClient();
            smtp.Host = applicationConfigRepository.GetValuefromApplicationConfig("SMTPHost");
            smtp.Port = int.Parse(applicationConfigRepository.GetValuefromApplicationConfig("SMTPPort"));
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(
                username, password);
            smtp.EnableSsl = smtpSsl;
            smtp.Send(mail);
        }
    }
}
