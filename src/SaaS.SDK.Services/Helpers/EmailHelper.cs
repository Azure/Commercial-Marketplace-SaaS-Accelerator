
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using Microsoft.Marketplace.SaasKit.Models;
using Microsoft.Marketplace.SaasKit.Services;
using Microsoft.Marketplace.SaaS.SDK.Services.Models;
using Microsoft.Marketplace.SaaS.SDK.Services.Services;
using System;
//using SendGrid;
//using SendGrid.Helpers.Mail;
using System.Net;
using System.Net.Mail;

namespace Microsoft.Marketplace.SaaS.SDK.Services.Helpers
{
    public class EmailHelper
    {

        private readonly IApplicationConfigRepository applicationConfigRepository;
        private readonly ISubscriptionsRepository subscriptionsRepository;
        //private readonly ISubscriptionLogRepository subscriptionLogRepository;
        private readonly IEmailTemplateRepository emailTemplateRepository;
        private readonly IEventsRepository eventsRepository;
        private readonly IPlanEventsMappingRepository planEventsMappingRepository;

        public EmailHelper(IApplicationConfigRepository applicationConfigRepository, ISubscriptionsRepository subscriptionsRepository, IEmailTemplateRepository emailTemplateRepository, IPlanEventsMappingRepository planEventsMappingRepository, IEventsRepository eventsRepository)
        {
            this.applicationConfigRepository = applicationConfigRepository;
            this.subscriptionsRepository = subscriptionsRepository;
            //this.subscriptionLogRepository = subscriptionLogRepository;
            this.emailTemplateRepository = emailTemplateRepository;
            this.eventsRepository = eventsRepository;
            this.planEventsMappingRepository = planEventsMappingRepository;
        }

        public void SendEmail(SubscriptionResultExtension Subscription, string planEvent = "success", SubscriptionStatusEnumExtension oldValue = SubscriptionStatusEnumExtension.PendingFulfillmentStart, string newValue = null)
        {
            MailMessage mail = new MailMessage();
            string FromMail = this.applicationConfigRepository.GetValuefromApplicationConfig("SMTPFromEmail");
            string password = applicationConfigRepository.GetValuefromApplicationConfig("SMTPPassword");
            string username = applicationConfigRepository.GetValuefromApplicationConfig("SMTPUserName");
            string Subject = string.Empty;
            bool smtpSsl = bool.Parse(applicationConfigRepository.GetValuefromApplicationConfig("SMTPSslEnabled"));
            mail.From = new MailAddress(FromMail);


            string body = TemplateService.ProcessTemplate(Subscription, emailTemplateRepository, applicationConfigRepository, planEvent, oldValue, newValue);
            mail.Body = body;
            mail.IsBodyHtml = true;

            int eventID = this.eventsRepository.GetEventID(Subscription.EventName);

            string toReceipents = string.Empty;
            bool CustomerToCopy = false;
            bool isActive = false;
            var eventrep = planEventsMappingRepository.GetPlanEventsMappingEmails(Subscription.GuidPlanId, eventID);
            if (eventrep != null)
            {
                CustomerToCopy = eventrep.CopyToCustomer ?? false;
                isActive = eventrep.Isactive;
            }

            if (isActive)
            {
                if (CustomerToCopy && planEvent.ToLower() == "success")
                {
                    toReceipents = Subscription.CustomerEmailAddress;
                    if (string.IsNullOrEmpty(toReceipents))
                    {
                        throw new Exception(" Error while sending an email, please check the configuration. ");
                    }
                    if (Subscription.SubscriptionStatus.ToString() == "PendingActivation")
                    {
                        Subject = "Pending Activation";
                    }
                    else if (Subscription.SubscriptionStatus.ToString() == "Subscribed")
                    {
                        Subject = "Subscription Activation";
                    }
                    else if (Subscription.SubscriptionStatus.ToString() == "Unsubscribed")
                    {
                        Subject = "Unsubscription";
                    }
                    mail.Subject = Subject;
                    mail.To.Add(toReceipents);
                    SmtpClient copy = new SmtpClient();
                    copy.Host = applicationConfigRepository.GetValuefromApplicationConfig("SMTPHost");
                    copy.Port = int.Parse(applicationConfigRepository.GetValuefromApplicationConfig("SMTPPort"));
                    copy.UseDefaultCredentials = false;
                    copy.Credentials = new NetworkCredential(
                        username, password);
                    copy.EnableSsl = smtpSsl;
                    //copy.Send(mail);
                }

                if (CustomerToCopy && planEvent.ToLower() == "failure" && isActive)
                {
                    toReceipents = Subscription.CustomerEmailAddress;
                    if (string.IsNullOrEmpty(toReceipents))
                    {
                        throw new Exception(" Error while sending an email, please check the configuration. ");
                    }
                    if (Subscription.SubscriptionStatus.ToString() == "DeploymentFailed")
                    {
                        Subject = "Deployment Failed";
                    }
                    else if (Subscription.SubscriptionStatus.ToString() == "ActivationFailed")
                    {
                        Subject = "Activation Failed";
                    }
                    else if (Subscription.SubscriptionStatus.ToString() == "UnsubscribeFailed")
                    {
                        Subject = "Unsubscribe Failed";
                    }
                    else if (Subscription.SubscriptionStatus.ToString() == "DeleteResourceFailed")
                    {
                        Subject = "Delete Resource Failed";
                    }
                    mail.Subject = Subject;
                    mail.To.Add(toReceipents);
                    SmtpClient copy = new SmtpClient();
                    copy.Host = applicationConfigRepository.GetValuefromApplicationConfig("SMTPHost");
                    copy.Port = int.Parse(applicationConfigRepository.GetValuefromApplicationConfig("SMTPPort"));
                    copy.UseDefaultCredentials = false;
                    copy.Credentials = new NetworkCredential(
                        username, password);
                    copy.EnableSsl = smtpSsl;
                    //copy.Send(mail);
                }

                mail.To.Clear();

                if (planEvent.ToLower() == "success")
                {
                    toReceipents = (planEventsMappingRepository.GetPlanEventsMappingEmails(Subscription.GuidPlanId, eventID).SuccessStateEmails
                  );
                    if (string.IsNullOrEmpty(toReceipents))
                    {
                        throw new Exception(" Error while sending an email, please check the configuration. ");
                    }
                    if (Subscription.SubscriptionStatus.ToString() == "PendingActivation")
                    {
                        Subject = "Pending Activation";
                    }
                    else if (Subscription.SubscriptionStatus.ToString() == "Subscribed")
                    {
                        Subject = "Subscription Activation";
                    }
                    else if (Subscription.SubscriptionStatus.ToString() == "Unsubscribed")
                    {
                        Subject = "Unsubscription";
                    }
                    mail.Subject = Subject;
                    if (!string.IsNullOrEmpty(toReceipents))
                    {
                        string[] ToEmails = toReceipents.Split(';');

                        foreach (string Multimailid in ToEmails)
                        {
                            mail.To.Add(new MailAddress(Multimailid));
                        }

                        if (!string.IsNullOrEmpty(emailTemplateRepository.GetCCRecipients(Subscription.SubscriptionStatus.ToString())))
                        {
                            string[] CcEmails = (emailTemplateRepository.GetCCRecipients(Subscription.SubscriptionStatus.ToString())).Split(';');
                            foreach (string Multimailid in CcEmails)
                            {
                                mail.CC.Add(new MailAddress(Multimailid));
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(emailTemplateRepository.GetBccRecipients(Subscription.SubscriptionStatus.ToString())))
                    {
                        string[] BccEmails = (emailTemplateRepository.GetBccRecipients(Subscription.SubscriptionStatus.ToString())).Split(';');
                        foreach (string Multimailid in BccEmails)
                        {
                            mail.Bcc.Add(new MailAddress(Multimailid));
                        }
                    }

                }

                if (planEvent.ToLower() == "failure")
                {
                    toReceipents = (planEventsMappingRepository.GetPlanEventsMappingEmails(Subscription.GuidPlanId, eventID).FailureStateEmails
                    );
                    if (string.IsNullOrEmpty(toReceipents))
                    {
                        throw new Exception(" Error while sending an email, please check the configuration. ");
                    }
                    if (Subscription.SubscriptionStatus.ToString() == "DeploymentFailed")
                    {
                        Subject = "Deployment Failed";
                    }
                    else if (Subscription.SubscriptionStatus.ToString() == "ActivationFailed")
                    {
                        Subject = "Activation Failed";
                    }
                    else if (Subscription.SubscriptionStatus.ToString() == "UnsubscribeFailed")
                    {
                        Subject = "Unsubscribe Failed";
                    }
                    else if (Subscription.SubscriptionStatus.ToString() == "DeleteResourceFailed")
                    {
                        Subject = "Delete Resource Failed";
                    }
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
}
