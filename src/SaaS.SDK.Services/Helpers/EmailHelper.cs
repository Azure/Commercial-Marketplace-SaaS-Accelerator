namespace Microsoft.Marketplace.SaaS.SDK.Services.Helpers
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Linq;
    using Commons.Collections;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using NVelocity;
    using NVelocity.App;

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
        /// <param name="subscriptionResultExtension">The subscription result extension.</param>
        /// <param name="processStatus">The process status.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns>
        /// Email Content Model.
        /// </returns>
        /// <exception cref="Exception">Error while sending an email, please check the configuration.
        /// or
        /// Error while sending an email, please check the configuration.</exception>
        public EmailContentModel PrepareEmailContent(
                                                        SubscriptionResultExtension subscriptionResultExtension,
                                                        string processStatus = "success",
                                                        SubscriptionStatusEnumExtension oldValue = SubscriptionStatusEnumExtension.PendingFulfillmentStart,
                                                        string newValue = null)
        {
            EmailContentModel emailContent = new EmailContentModel();
            string body = this.ProcessTemplate(subscriptionResultExtension, processStatus, oldValue, newValue);
            var subscriptionEvent = this.eventsRepository.GetByName(subscriptionResultExtension.EventName);
            var emailTemplateData = this.emailTemplateRepository.GetTemplateForStatus(subscriptionResultExtension.SubscriptionStatus.ToString());
            string subject = string.Empty;

            bool copyToCustomer = false;
            bool isActive = false;
            string toReceipents = string.Empty;
            string ccReceipents = string.Empty;
            string bccReceipents = string.Empty;

            string fromMail = this.applicationConfigRepository.GetValueByName("SMTPFromEmail");
            string password = this.applicationConfigRepository.GetValueByName("SMTPPassword");
            string username = this.applicationConfigRepository.GetValueByName("SMTPUserName");
            bool smtpSsl = bool.Parse(this.applicationConfigRepository.GetValueByName("SMTPSslEnabled"));
            int port = int.Parse(this.applicationConfigRepository.GetValueByName("SMTPPort"));
            string smtpHost = this.applicationConfigRepository.GetValueByName("SMTPHost");
            if (processStatus.ToLower() == "success")
            {
                var successEventData = this.planEventsMappingRepository.GetPlanEvent(subscriptionResultExtension.GuidPlanId, subscriptionEvent.EventsId);

                if (successEventData != null)
                {
                    toReceipents = successEventData.SuccessStateEmails;
                    copyToCustomer = Convert.ToBoolean(successEventData.CopyToCustomer);
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
                var failureStateEmails = this.planEventsMappingRepository.GetPlanEvent(subscriptionResultExtension.GuidPlanId, subscriptionEvent.EventsId);

                if (failureStateEmails != null)
                {
                    toReceipents = failureStateEmails.FailureStateEmails;
                    copyToCustomer = Convert.ToBoolean(failureStateEmails.CopyToCustomer);
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

            if (subscriptionResultExtension.SubscriptionStatus.ToString() == "PendingActivation")
            {
                subject = "Pending Activation";
            }
            else if (subscriptionResultExtension.SubscriptionStatus.ToString() == "Subscribed")
            {
                subject = "Subscription Activation";
            }
            else if (subscriptionResultExtension.SubscriptionStatus.ToString() == "Unsubscribed")
            {
                subject = "Unsubscription";
            }
            else if (subscriptionResultExtension.SubscriptionStatus.ToString() == "DeploymentFailed")
            {
                subject = "Deployment Failed";
            }
            else if (subscriptionResultExtension.SubscriptionStatus.ToString() == "ActivationFailed")
            {
                subject = "Activation Failed";
            }
            else if (subscriptionResultExtension.SubscriptionStatus.ToString() == "UnsubscribeFailed")
            {
                subject = "Unsubscribe Failed";
            }
            else if (subscriptionResultExtension.SubscriptionStatus.ToString() == "DeleteResourceFailed")
            {
                subject = "Delete Resource Failed";
            }

            emailContent.BCCEmails = bccReceipents;
            emailContent.CCEmails = ccReceipents;
            emailContent.ToEmails = toReceipents;
            emailContent.Body = body;
            emailContent.Subject = subject;
            emailContent.CopyToCustomer = copyToCustomer;
            emailContent.IsActive = isActive;
            emailContent.FromEmail = fromMail;
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
        /// <param name="subscription">The subscription.</param>
        /// <param name="processStatus">The process status.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns> string.</returns>
        public string ProcessTemplate(SubscriptionResultExtension subscription, string processStatus, SubscriptionStatusEnumExtension oldValue, string newValue)
        {
            string parameter = string.Empty;
            string value = string.Empty;
            string parameterType = string.Empty;

            string body = string.Empty;
            EmailTemplate templateDetails = this.emailTemplateRepository.GetTemplateForStatus("Template");
            body = templateDetails.TemplateBody;
            string applicationName = this.applicationConfigRepository.GetValueByName("ApplicationName");
            Hashtable hashTable = new Hashtable();
            hashTable.Add("ApplicationName", applicationName);
            hashTable.Add("CustomerEmailAddress", subscription.CustomerEmailAddress);
            hashTable.Add("CustomerName", subscription.CustomerName);
            hashTable.Add("Id", subscription.Id);
            hashTable.Add("SubscriptionName", subscription.Name);
            hashTable.Add("SaasSubscriptionStatus", subscription.SubscriptionStatus);
            hashTable.Add("oldValue", oldValue);
            hashTable.Add("newValue", newValue);
            hashTable.Add("planevent", processStatus);
            hashTable.Add("PurchaserEmail", subscription.Purchaser.EmailId ?? " ");
            hashTable.Add("PurchaserTenant", Convert.ToString(subscription.Purchaser.TenantId) ?? " ");

            ExtendedProperties p = new ExtendedProperties();

            VelocityEngine v = new VelocityEngine();
            v.Init(p);

            VelocityContext context = new VelocityContext(hashTable);
            IList list;
            //IList arminputlist;
            //IList armoutputlist;
            if (subscription.SubscriptionParameters != null && subscription.SubscriptionParameters.Count > 0)
            {
                list = subscription.SubscriptionParameters.Where(s => s.Type.ToLower() == "input").ToList();
                if (list.Count > 0)
                {
                    context.Put("parms", list);
                }
            }

            /*  Indra
             *  if (subscription.ARMTemplateParameters != null && subscription.ARMTemplateParameters.Count > 0)
                 {
                     arminputlist = subscription.ARMTemplateParameters.Where(s => s.ParameterType.ToLower() == "input").ToList();
                     if (arminputlist.Count > 0)
                     {
                         context.Put("arminputparms", arminputlist);
                     }
                 }

                 if (subscription.ARMTemplateParameters != null && subscription.ARMTemplateParameters.Count > 0)
                 {
                     armoutputlist = subscription.ARMTemplateParameters.Where(s => s.ParameterType.ToLower() == "output").ToList();
                     if (armoutputlist.Count > 0)
                     {
                         context.Put("armoutputparms", armoutputlist);
                     }
                 }

         */
            StringWriter writer = new StringWriter();
            v.Evaluate(context, writer, string.Empty, body);
            return writer.ToString();
        }
    }
}
