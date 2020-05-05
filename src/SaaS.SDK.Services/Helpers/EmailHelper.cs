
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

        public EmailContentModel PrepareEmailContent(SubscriptionResultExtension Subscription, string planEvent = "success", SubscriptionStatusEnumExtension oldValue = SubscriptionStatusEnumExtension.PendingFulfillmentStart, string newValue = null)
        {
            EmailContentModel emailContent = new EmailContentModel();
            string body = ProcessTemplate(Subscription, planEvent, oldValue, newValue);
            int eventID = this.eventsRepository.GetEventID(Subscription.EventName);
            var emailTemplateData = emailTemplateRepository.GetEmailTemplateOnStatus(Subscription.SubscriptionStatus.ToString());
            string Subject = string.Empty;
            bool smtpSsl = true;

            bool CustomerToCopy = false;
            bool isActive = false;
            string toReceipents = string.Empty;
            string ccReceipents = string.Empty;
            string bccReceipents = string.Empty;


            /* 
            Cases
             * Activate - Success  
             * Activate - Failure
             * Unsubscribe - Success 
             * Unsubscribe - Failure
            Conditions:
            Is Active in Plan Event Mapping 
            Is Email Enabled for Subscribtio status in App config
          
            Copy to customer.

            If to email is null then don't pull CC and checkfor bcc

             */


            var eventMappings = planEventsMappingRepository.GetPlanEventsMappingEmails(Subscription.GuidPlanId, eventID);
            if (eventMappings != null)
            {
                CustomerToCopy = eventMappings.CopyToCustomer ?? false;
                isActive = eventMappings.Isactive;
            }
            toReceipents = Subscription.CustomerEmailAddress;

            if (planEvent.ToLower() == "success")
            {
                var successEventData = planEventsMappingRepository.GetPlanEventsMappingEmails(Subscription.GuidPlanId, eventID);

                if (string.IsNullOrEmpty(toReceipents))
                {
                    throw new Exception(" Error while sending an email, please check the configuration. ");
                }


                if (successEventData != null)
                {
                    toReceipents = successEventData.SuccessStateEmails;
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
                }
            }

            if (planEvent.ToLower() == "failure")
            {
                var failureStateEmails = planEventsMappingRepository.GetPlanEventsMappingEmails(Subscription.GuidPlanId, eventID);
                if (string.IsNullOrEmpty(toReceipents))
                {
                    throw new Exception(" Error while sending an email, please check the configuration. ");
                }

                if (failureStateEmails != null)
                {
                    toReceipents = failureStateEmails.FailureStateEmails;
                }
                if (emailTemplateData != null)
                {
                    if (!string.IsNullOrEmpty(toReceipents) && !string.IsNullOrEmpty(emailTemplateData.Cc))
                    {

                        ccReceipents = emailTemplateData.Cc;
                    }

                    if (string.IsNullOrEmpty(emailTemplateData.Bcc))
                    {
                        bccReceipents = emailTemplateData.Bcc;
                    }
                }
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
            else if (Subscription.SubscriptionStatus.ToString() == "DeploymentFailed")
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



            return emailContent;

        }



        public string ProcessTemplate(SubscriptionResultExtension Subscription, string planEvent, SubscriptionStatusEnumExtension oldValue, string newValue)
        {

            string parameter = string.Empty;
            string value = string.Empty;
            string parameterType = string.Empty;


            string body = string.Empty;
            EmailTemplate templateDetails = emailTemplateRepository.GetEmailTemplateOnStatus("Template");

            string applicationName = applicationConfigRepository.GetValueByName("ApplicationName");
            Hashtable hashTable = new Hashtable();
            hashTable.Add("ApplicationName", applicationName);
            hashTable.Add("CustomerEmailAddress", Subscription.CustomerEmailAddress);
            hashTable.Add("CustomerName", Subscription.CustomerName);
            hashTable.Add("Id", Subscription.Id);
            hashTable.Add("SubscriptionName", Subscription.Name);
            hashTable.Add("SaasSubscriptionStatus", Subscription.SubscriptionStatus);
            hashTable.Add("oldValue", oldValue);
            hashTable.Add("newValue", newValue);
            hashTable.Add("planevent", planEvent);


            ExtendedProperties p = new ExtendedProperties();

            VelocityEngine v = new VelocityEngine();
            v.Init(p);

            VelocityContext context = new VelocityContext(hashTable);
            IList list;
            IList arminputlist;
            IList armoutputlist;
            if (Subscription.SubscriptionParameters != null && Subscription.SubscriptionParameters.Count > 0)
            {
                list = Subscription.SubscriptionParameters.Where(s => s.Type.ToLower() == "input").ToList();
                if (list.Count > 0)
                    context.Put("parms", list);
            }
            if (Subscription.ARMTemplateParameters != null && Subscription.ARMTemplateParameters.Count > 0)
            {
                arminputlist = Subscription.ARMTemplateParameters.Where(s => s.ParameterType.ToLower() == "input"
                /*&& s.EventsName == "Active"*/
                ).ToList();
                if (arminputlist.Count > 0)
                    context.Put("arminputparms", arminputlist);
            }
            if (Subscription.ARMTemplateParameters != null && Subscription.ARMTemplateParameters.Count > 0)
            {
                armoutputlist = Subscription.ARMTemplateParameters.Where(s => s.ParameterType.ToLower() == "output"
                /*&& s.EventsName == "Active"*/
                ).ToList();
                if (armoutputlist.Count > 0)
                    context.Put("armoutputparms", armoutputlist);
            }
            StringWriter writer = new StringWriter();
            v.Evaluate(context, writer, string.Empty, body);
            return writer.ToString();
        }
    }
}
