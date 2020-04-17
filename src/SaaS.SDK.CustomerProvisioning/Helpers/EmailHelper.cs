using Microsoft.Marketplace.SaaS.SDK.CustomerProvisioning.Models;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.Services;
using Microsoft.Marketplace.SaasKit.Models;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Services;
using System.Net;
using System.Net.Mail;
//using SendGrid;
//using SendGrid.Helpers.Mail;
namespace Microsoft.Marketplace.SaasKit.Client.Helpers
{
    public class EmailHelper
    {

        public static void SendEmail(SubscriptionResultExtension Subscription, IApplicationConfigRepository applicationConfigRepository, IEmailTemplateRepository emailTemplateRepository, IPlanEventsMappingRepository planEventsMappingRepository, string planEvent = "success", SubscriptionStatusEnum oldValue = SubscriptionStatusEnum.PendingFulfillmentStart, string newValue = null)
        {
            MailMessage mail = new MailMessage();
            string FromMail = applicationConfigRepository.GetValuefromApplicationConfig("SMTPFromEmail");
            string password = applicationConfigRepository.GetValuefromApplicationConfig("SMTPPassword");
            string username = applicationConfigRepository.GetValuefromApplicationConfig("SMTPUserName");
            string Subject = string.Empty;
            bool smtpSsl = bool.Parse(applicationConfigRepository.GetValuefromApplicationConfig("SMTPSslEnabled"));
            mail.From = new MailAddress(FromMail);

            string body = TemplateService.ProcessTemplate(Subscription, emailTemplateRepository, applicationConfigRepository, planEvent, oldValue, newValue);
            mail.Body = body;
            mail.IsBodyHtml = true;

            string toReceipents = string.Empty;

            if (planEvent.ToLower() == "success")
            {
                toReceipents = (planEventsMappingRepository.GetSuccessStateEmails(Subscription.GuidPlanId)
              );
                Subject = emailTemplateRepository.GetSubject(Subscription.SaasSubscriptionStatus.ToString());
                mail.Subject = Subject;
                if (!string.IsNullOrEmpty(toReceipents))
                {
                    string[] ToEmails = toReceipents.Split(';');

                    foreach (string Multimailid in ToEmails)
                    {
                        mail.To.Add(new MailAddress(Multimailid));
                    }

                    if (!string.IsNullOrEmpty(emailTemplateRepository.GetCCRecipients(Subscription.SaasSubscriptionStatus.ToString())))
                    {
                        string[] CcEmails = (emailTemplateRepository.GetCCRecipients(Subscription.SaasSubscriptionStatus.ToString())).Split(';');
                        foreach (string Multimailid in CcEmails)
                        {
                            mail.CC.Add(new MailAddress(Multimailid));
                        }
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

            }
            if (planEvent.ToLower() == "failure")
            {
                toReceipents = (planEventsMappingRepository.GetFailureStateEmails(Subscription.GuidPlanId)
                );
                Subject = emailTemplateRepository.GetSubject(planEvent);
                mail.Subject = Subject;
                if (!string.IsNullOrEmpty(toReceipents))
                {
                    string[] ToEmails = toReceipents.Split(';');

                    foreach (string Multimailid in ToEmails)
                    {
                        mail.To.Add(new MailAddress(Multimailid));
                    }

                    if (!string.IsNullOrEmpty(emailTemplateRepository.GetCCRecipients(planEvent)))
                    {
                        string[] CcEmails = (emailTemplateRepository.GetCCRecipients(planEvent)).Split(';');
                        foreach (string Multimailid in CcEmails)
                        {
                            mail.CC.Add(new MailAddress(Multimailid));
                        }
                    }
                }

                if (!string.IsNullOrEmpty(emailTemplateRepository.GetBccRecipients(planEvent)))
                {
                    string[] BccEmails = (emailTemplateRepository.GetBccRecipients(planEvent)).Split(';');
                    foreach (string Multimailid in BccEmails)
                    {
                        mail.Bcc.Add(new MailAddress(Multimailid));
                    }
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
