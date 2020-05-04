
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
using Commons.Collections;
using NVelocity.App;
using System.Collections;
using NVelocity;
using System.IO;
using System.Linq;

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

        public void PrepareEmailContent(SubscriptionResultExtension Subscription, string planEvent = "success", SubscriptionStatusEnumExtension oldValue = SubscriptionStatusEnumExtension.PendingFulfillmentStart, string newValue = null)
        {

        //    string Subject = "";
        //    string body = ProcessTemplate(Subscription, emailTemplateRepository, applicationConfigRepository, planEvent, oldValue, newValue);
        //    //TemplateService.ProcessTemplate(Subscription, emailTemplateRepository, applicationConfigRepository, planEvent, oldValue, newValue);
        //    //mail.Body = body;

        //    bool smtpSsl = true;
        //    int eventID = this.eventsRepository.GetEventID(Subscription.EventName);

        //    string toReceipents = string.Empty;
        //    bool CustomerToCopy = false;
        //    bool isActive = false;
        //    var eventrep = planEventsMappingRepository.GetPlanEventsMappingEmails(Subscription.GuidPlanId, eventID);
        //    if (eventrep != null)
        //    {
        //        CustomerToCopy = eventrep.CopyToCustomer ?? false;
        //        isActive = eventrep.Isactive;
        //    }

        //    if (isActive)
        //    {
        //        if (CustomerToCopy && planEvent.ToLower() == "success")
        //        {
        //            toReceipents = Subscription.CustomerEmailAddress;
        //            if (string.IsNullOrEmpty(toReceipents))
        //            {
        //                throw new Exception(" Error while sending an email, please check the configuration. ");
        //            }
        //            if (Subscription.SubscriptionStatus.ToString() == "PendingActivation")
        //            {
        //                Subject = "Pending Activation";
        //            }
        //            else if (Subscription.SubscriptionStatus.ToString() == "Subscribed")
        //            {
        //                Subject = "Subscription Activation";
        //            }
        //            else if (Subscription.SubscriptionStatus.ToString() == "Unsubscribed")
        //            {
        //                Subject = "Unsubscription";
        //            }
        //            //mail.Subject = Subject;
        //            //mail.To.Add(toReceipents);
        //            //SmtpClient copy = new SmtpClient();
        //            //copy.Host = applicationConfigRepository.GetValuefromApplicationConfig("SMTPHost");
        //            //copy.Port = int.Parse(applicationConfigRepository.GetValuefromApplicationConfig("SMTPPort"));
        //            //copy.UseDefaultCredentials = false;
        //            //copy.Credentials = new NetworkCredential(
        //            //    username, password);
        //            //copy.EnableSsl = smtpSsl;
        //            //copy.Send(mail);
        //        }

        //        if (CustomerToCopy && planEvent.ToLower() == "failure" && isActive)
        //        {
        //            toReceipents = Subscription.CustomerEmailAddress;
        //            if (string.IsNullOrEmpty(toReceipents))
        //            {
        //                throw new Exception(" Error while sending an email, please check the configuration. ");
        //            }
        //            if (Subscription.SubscriptionStatus.ToString() == "DeploymentFailed")
        //            {
        //                Subject = "Deployment Failed";
        //            }
        //            else if (Subscription.SubscriptionStatus.ToString() == "ActivationFailed")
        //            {
        //                Subject = "Activation Failed";
        //            }
        //            else if (Subscription.SubscriptionStatus.ToString() == "UnsubscribeFailed")
        //            {
        //                Subject = "Unsubscribe Failed";
        //            }
        //            else if (Subscription.SubscriptionStatus.ToString() == "DeleteResourceFailed")
        //            {
        //                Subject = "Delete Resource Failed";
        //            }
        //            //mail.Subject = Subject;
        //            //mail.To.Add(toReceipents);
        //            //SmtpClient copy = new SmtpClient();
        //            //copy.Host = applicationConfigRepository.GetValuefromApplicationConfig("SMTPHost");
        //            //copy.Port = int.Parse(applicationConfigRepository.GetValuefromApplicationConfig("SMTPPort"));
        //            //copy.UseDefaultCredentials = false;
        //            //copy.Credentials = new NetworkCredential(
        //            //    username, password);
        //            //copy.EnableSsl = smtpSsl;
        //            //copy.Send(mail);
        //        }



        //        if (planEvent.ToLower() == "success")
        //        {
        //            toReceipents = (planEventsMappingRepository.GetPlanEventsMappingEmails(Subscription.GuidPlanId, eventID).SuccessStateEmails
        //          );
        //            if (string.IsNullOrEmpty(toReceipents))
        //            {
        //                throw new Exception(" Error while sending an email, please check the configuration. ");
        //            }
        //            if (Subscription.SubscriptionStatus.ToString() == "PendingActivation")
        //            {
        //                Subject = "Pending Activation";
        //            }
        //            else if (Subscription.SubscriptionStatus.ToString() == "Subscribed")
        //            {
        //                Subject = "Subscription Activation";
        //            }
        //            else if (Subscription.SubscriptionStatus.ToString() == "Unsubscribed")
        //            {
        //                Subject = "Unsubscription";
        //            }

        //            if (!string.IsNullOrEmpty(toReceipents))
        //            {
        //                string[] ToEmails = toReceipents.Split(';');

        //                foreach (string Multimailid in ToEmails)
        //                {
        //                    to = Multimailid;
        //                }

        //                if (!string.IsNullOrEmpty(emailTemplateRepository.GetCCRecipients(Subscription.SubscriptionStatus.ToString())))
        //                {
        //                    string[] CcEmails = (emailTemplateRepository.GetCCRecipients(Subscription.SubscriptionStatus.ToString())).Split(';');
        //                    foreach (string Multimailid in CcEmails)
        //                    {
        //                        CC = Multimailid));
        //                    }
        //                }
        //            }

        //            if (!string.IsNullOrEmpty(emailTemplateRepository.GetBccRecipients(Subscription.SubscriptionStatus.ToString())))
        //            {
        //                string[] BccEmails = (emailTemplateRepository.GetBccRecipients(Subscription.SubscriptionStatus.ToString())).Split(';');
        //                foreach (string Multimailid in BccEmails)
        //                {
        //                    mail.Bcc.Add(new MailAddress(Multimailid));
        //                }
        //            }

        //        }

        //        if (planEvent.ToLower() == "failure")
        //        {
        //            toReceipents = (planEventsMappingRepository.GetPlanEventsMappingEmails(Subscription.GuidPlanId, eventID).FailureStateEmails
        //            );
        //            if (string.IsNullOrEmpty(toReceipents))
        //            {
        //                throw new Exception(" Error while sending an email, please check the configuration. ");
        //            }
        //            if (Subscription.SubscriptionStatus.ToString() == "DeploymentFailed")
        //            {
        //                Subject = "Deployment Failed";
        //            }
        //            else if (Subscription.SubscriptionStatus.ToString() == "ActivationFailed")
        //            {
        //                Subject = "Activation Failed";
        //            }
        //            else if (Subscription.SubscriptionStatus.ToString() == "UnsubscribeFailed")
        //            {
        //                Subject = "Unsubscribe Failed";
        //            }
        //            else if (Subscription.SubscriptionStatus.ToString() == "DeleteResourceFailed")
        //            {
        //                Subject = "Delete Resource Failed";
        //            }
        //            mail.Subject = Subject;
        //            if (!string.IsNullOrEmpty(toReceipents))
        //            {
        //                string[] ToEmails = toReceipents.Split(';');

        //                foreach (string Multimailid in ToEmails)
        //                {
        //                    mail.To.Add(new MailAddress(Multimailid));
        //                }

        //                if (!string.IsNullOrEmpty(emailTemplateRepository.GetCCRecipients(planEvent)))
        //                {
        //                    string[] CcEmails = (emailTemplateRepository.GetCCRecipients(planEvent)).Split(';');
        //                    foreach (string Multimailid in CcEmails)
        //                    {
        //                        mail.CC.Add(new MailAddress(Multimailid));
        //                    }
        //                }
        //            }

        //            if (!string.IsNullOrEmpty(emailTemplateRepository.GetBccRecipients(planEvent)))
        //            {
        //                string[] BccEmails = (emailTemplateRepository.GetBccRecipients(planEvent)).Split(';');
        //                foreach (string Multimailid in BccEmails)
        //                {
        //                    mail.Bcc.Add(new MailAddress(Multimailid));
        //                }
        //            }
        //        }


        //    }
        //}


        //public static string ProcessTemplate(SubscriptionResultExtension Subscription, IEmailTemplateRepository emailTemplateRepository, IApplicationConfigRepository applicationConfigRepository, string planEvent, SubscriptionStatusEnumExtension oldValue, string newValue)
        //{

        //    string parameter = string.Empty;
        //    string value = string.Empty;
        //    string parameterType = string.Empty;


        //    string body = string.Empty;
        //    EmailTemplate templateDetails = emailTemplateRepository.GetEmailTemplateOnStatus("Template");

        //    string applicationName = applicationConfigRepository.GetValuefromApplicationConfig("ApplicationName");
        //    Hashtable hashTable = new Hashtable();
        //    hashTable.Add("ApplicationName", applicationName);
        //    hashTable.Add("CustomerEmailAddress", Subscription.CustomerEmailAddress);
        //    hashTable.Add("CustomerName", Subscription.CustomerName);
        //    hashTable.Add("Id", Subscription.Id);
        //    hashTable.Add("SubscriptionName", Subscription.Name);
        //    hashTable.Add("SaasSubscriptionStatus", Subscription.SubscriptionStatus);
        //    hashTable.Add("oldValue", oldValue);
        //    hashTable.Add("newValue", newValue);
        //    hashTable.Add("planevent", planEvent);


        //    ExtendedProperties p = new ExtendedProperties();

        //    VelocityEngine v = new VelocityEngine();
        //    v.Init(p);

        //    VelocityContext context = new VelocityContext(hashTable);
        //    IList list;
        //    IList arminputlist;
        //    IList armoutputlist;
        //    if (Subscription.SubscriptionParameters != null && Subscription.SubscriptionParameters.Count > 0)
        //    {
        //        list = Subscription.SubscriptionParameters.Where(s => s.Type.ToLower() == "input").ToList();
        //        if (list.Count > 0)
        //            context.Put("parms", list);
        //    }
        //    if (Subscription.ARMTemplateParameters != null && Subscription.ARMTemplateParameters.Count > 0)
        //    {
        //        arminputlist = Subscription.ARMTemplateParameters.Where(s => s.ParameterType.ToLower() == "input"
        //        /*&& s.EventsName == "Active"*/
        //        ).ToList();
        //        if (arminputlist.Count > 0)
        //            context.Put("arminputparms", arminputlist);
        //    }
        //    if (Subscription.ARMTemplateParameters != null && Subscription.ARMTemplateParameters.Count > 0)
        //    {
        //        armoutputlist = Subscription.ARMTemplateParameters.Where(s => s.ParameterType.ToLower() == "output"
        //        /*&& s.EventsName == "Active"*/
        //        ).ToList();
        //        if (armoutputlist.Count > 0)
        //            context.Put("armoutputparms", armoutputlist);
        //    }
        //    StringWriter writer = new StringWriter();
        //    v.Evaluate(context, writer, string.Empty, body);
        //    return writer.ToString();
        }
    }
}
