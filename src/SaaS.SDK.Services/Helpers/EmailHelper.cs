namespace Microsoft.Marketplace.SaaS.SDK.Services.Helpers
{
    using Commons.Collections;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using NVelocity;
    using NVelocity.App;
    using System;
    using System.Collections;
    using System.IO;
    using System.Linq;

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
                            IEventsRepository eventsRepository
                            )
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
        /// <param name="Subscription">The subscription.</param>
        /// <param name="planEvent">The plan event.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns></returns>
        /// <exception cref="Exception">
        /// Error while sending an email, please check the configuration.
        /// or
        /// Error while sending an email, please check the configuration.
        /// </exception>
        public EmailContentModel PrepareEmailContent(
                                                        SubscriptionResultExtension Subscription,
                                                        string processStatus = "success",
                                                        SubscriptionStatusEnumExtension oldValue = SubscriptionStatusEnumExtension.PendingFulfillmentStart,
                                                        string newValue = null)
        {
            EmailContentModel emailContent = new EmailContentModel();
            string body = ProcessTemplate(Subscription, processStatus, oldValue, newValue);
            var subscriptionEvent = this.eventsRepository.GetByName(Subscription.EventName);
            var emailTemplateData = emailTemplateRepository.GetTemplateForStatus(Subscription.SubscriptionStatus.ToString());
            string Subject = string.Empty;

            bool CopyToCustomer = false;
            bool isActive = false;
            string toReceipents = string.Empty;
            string ccReceipents = string.Empty;
            string bccReceipents = string.Empty;

            string FromMail = this.applicationConfigRepository.GetValueByName("SMTPFromEmail");
            string password = applicationConfigRepository.GetValueByName("SMTPPassword");
            string username = applicationConfigRepository.GetValueByName("SMTPUserName");
            bool smtpSsl = bool.Parse(applicationConfigRepository.GetValueByName("SMTPSslEnabled"));
            int port = int.Parse(applicationConfigRepository.GetValueByName("SMTPPort"));
            string smtpHost = applicationConfigRepository.GetValueByName("SMTPHost");
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

            if (processStatus.ToLower() == "success")
            {
                var successEventData = planEventsMappingRepository.GetPlanEvent(Subscription.GuidPlanId, subscriptionEvent.EventsId);

                if (successEventData != null)
                {
                    toReceipents = successEventData.SuccessStateEmails;
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
                }
            }

            if (processStatus.ToLower() == "failure")
            {
                var failureStateEmails = planEventsMappingRepository.GetPlanEvent(Subscription.GuidPlanId, subscriptionEvent.EventsId);

                if (failureStateEmails != null)
                {
                    toReceipents = failureStateEmails.FailureStateEmails;
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

            emailContent.BCCEmails = bccReceipents;
            emailContent.CCEmails = ccReceipents;
            emailContent.ToEmails = toReceipents;
            emailContent.Body = body;
            emailContent.Subject = Subject;
            emailContent.CopyToCustomer = CopyToCustomer;
            emailContent.IsActive = isActive;
            emailContent.FromEmail = FromMail;
            emailContent.Password = password;
            emailContent.SSL = smtpSsl;
            emailContent.UserName = username;
            emailContent.Port = port;
            emailContent.SMTPHost = smtpHost;

            return emailContent;

        }

        /// <summary>
        /// Processes the template.
        /// </summary>
        /// <param name="Subscription">The subscription.</param>
        /// <param name="planEvent">The plan event.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns></returns>
        public string ProcessTemplate(SubscriptionResultExtension Subscription, string processStatus, SubscriptionStatusEnumExtension oldValue, string newValue)
        {

            string parameter = string.Empty;
            string value = string.Empty;
            string parameterType = string.Empty;

            string body = string.Empty;
            EmailTemplate templateDetails = emailTemplateRepository.GetTemplateForStatus("Template");
            body = templateDetails.TemplateBody;
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
            hashTable.Add("planevent", processStatus);

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
