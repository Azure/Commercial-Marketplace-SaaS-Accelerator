namespace Microsoft.Marketplace.SaasKit.Provisioning.Webjob.StatusHandlers
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Marketplace.SaaS.SDK.Services.Contracts;
    using Microsoft.Marketplace.SaaS.SDK.Services.Helpers;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using Microsoft.Marketplace.SaaS.SDK.Services.Services;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Contracts;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;


    /// <summary>
    /// Status handler to send out notifications based on the last status of the subscription
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Provisioning.Webjob.StatusHandlers.AbstractSubscriptionStatusHandler" />
    public class NotificationStatusHandler : AbstractSubscriptionStatusHandler
    {
        /// <summary>
        /// The fulfillment API client
        /// </summary>
        protected readonly IFulfillmentApiClient fulfillmentApiClient;

        /// <summary>
        /// The subscription repository
        /// </summary>
        protected readonly ISubscriptionsRepository subscriptionRepository;

        /// <summary>
        /// The application configuration repository
        /// </summary>
        protected readonly IApplicationConfigRepository applicationConfigRepository;

        /// <summary>
        /// The email template repository
        /// </summary>
        protected readonly IEmailTemplateRepository emailTemplateRepository;

        /// <summary>
        /// The plan events mapping repository
        /// </summary>
        protected readonly IPlanEventsMappingRepository planEventsMappingRepository;

        /// <summary>
        /// The offer attributes repository
        /// </summary>
        protected readonly IOfferAttributesRepository offerAttributesRepository;

        /// <summary>
        /// The subscription service
        /// </summary>
        protected SubscriptionService subscriptionService = null;

        /// <summary>
        /// The plan repository
        /// </summary>
        protected readonly IPlansRepository planRepository;

        /// <summary>
        /// The offers repository
        /// </summary>
        protected readonly IOffersRepository offersRepository;

        /// <summary>
        /// The events repository
        /// </summary>
        protected readonly IEventsRepository eventsRepository;

        /// <summary>
        /// The subscription template parameters repository
        /// </summary>
        protected readonly ISubscriptionTemplateParametersRepository subscriptionTemplateParametersRepository;

        /// <summary>
        /// The email service
        /// </summary>
        protected readonly IEmailService emailService;

        /// <summary>
        /// The email helper
        /// </summary>
        protected readonly EmailHelper emailHelper;

        /// <summary>
        /// The logger
        /// </summary>
        protected readonly ILogger<NotificationStatusHandler> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationStatusHandler"/> class.
        /// </summary>
        /// <param name="fulfillApiClient">The fulfill API client.</param>
        /// <param name="planRepository">The plan repository.</param>
        /// <param name="applicationConfigRepository">The application configuration repository.</param>
        /// <param name="emailTemplateRepository">The email template repository.</param>
        /// <param name="planEventsMappingRepository">The plan events mapping repository.</param>
        /// <param name="offerAttributesRepository">The offer attributes repository.</param>
        /// <param name="eventsRepository">The events repository.</param>
        /// <param name="subscriptionRepository">The subscription repository.</param>
        /// <param name="usersRepository">The users repository.</param>
        /// <param name="offersRepository">The offers repository.</param>
        /// <param name="subscriptionTemplateParametersRepository">The subscription template parameters repository.</param>
        /// <param name="emailService">The email service.</param>
        /// <param name="emailHelper">The email helper.</param>
        /// <param name="logger">The logger.</param>
        public NotificationStatusHandler(
                                            IFulfillmentApiClient fulfillApiClient,
                                            IPlansRepository planRepository,
                                            IApplicationConfigRepository applicationConfigRepository,
                                            IEmailTemplateRepository emailTemplateRepository,
                                            IPlanEventsMappingRepository planEventsMappingRepository,
                                            IOfferAttributesRepository offerAttributesRepository,
                                            IEventsRepository eventsRepository,
                                            ISubscriptionsRepository subscriptionRepository,                                            
                                            IUsersRepository usersRepository,
                                            IOffersRepository offersRepository,
                                            ISubscriptionTemplateParametersRepository subscriptionTemplateParametersRepository,
                                            IEmailService emailService,
                                            EmailHelper emailHelper,
                                            ILogger<NotificationStatusHandler> logger) : base(subscriptionRepository, planRepository, usersRepository)
        {
            this.fulfillmentApiClient = fulfillApiClient;
            this.applicationConfigRepository = applicationConfigRepository;
            this.planEventsMappingRepository = planEventsMappingRepository;
            this.offerAttributesRepository = offerAttributesRepository;
            this.eventsRepository = eventsRepository;
            this.emailTemplateRepository = emailTemplateRepository;
            this.subscriptionRepository = subscriptionRepository;
            this.planRepository = planRepository;
            this.subscriptionService = new SubscriptionService(this.subscriptionRepository, this.planRepository);
            this.offersRepository = offersRepository;
            this.subscriptionTemplateParametersRepository = subscriptionTemplateParametersRepository;
            this.emailService = emailService;
            this.emailHelper = new EmailHelper(applicationConfigRepository,subscriptionRepository,emailTemplateRepository,planEventsMappingRepository,eventsRepository);
            this.logger = logger;
        }

        /// <summary>
        /// Processes the specified subscription identifier.
        /// </summary>
        /// <param name="subscriptionID">The subscription identifier.</param>
        public override void Process(Guid subscriptionID)
        {
            this.logger?.LogInformation("ResourceDeploymentStatusHandler Process...");
            this.logger?.LogInformation("Get SubscriptionById");
            var subscription = this.GetSubscriptionById(subscriptionID);
            this.logger?.LogInformation("Get PlanById");
            var planDetails = this.GetPlanById(subscription.AmpplanId);
            this.logger?.LogInformation("Get User");
            var userdeatils = this.GetUserById(subscription.UserId);
            this.logger?.LogInformation("Get Offers");
            var offer = this.offersRepository.GetOfferById(planDetails.OfferId);
            this.logger?.LogInformation("Get Events");
            var events = this.eventsRepository.GetByName("Activate");

            var subscriptionTemplateParameters = this.subscriptionTemplateParametersRepository.GetTemplateParametersBySubscriptionId(subscription.AmpsubscriptionId);
            List<SubscriptionTemplateParametersModel> subscriptionTemplateParametersList = new List<SubscriptionTemplateParametersModel>();
            if (subscriptionTemplateParameters != null)
            {
                var serializedParent = JsonConvert.SerializeObject(subscriptionTemplateParameters.ToList());
                subscriptionTemplateParametersList = JsonConvert.DeserializeObject<List<SubscriptionTemplateParametersModel>>(serializedParent);
            }

            List<SubscriptionParametersModel> subscriptionParametersList = new List<SubscriptionParametersModel>();

            var subscriptionParameters = this.subscriptionRepository.GetSubscriptionsParametersById(subscriptionID, planDetails.PlanGuid);


            var serializedSubscription = JsonConvert.SerializeObject(subscriptionParameters);
            subscriptionParametersList = JsonConvert.DeserializeObject<List<SubscriptionParametersModel>>(serializedSubscription);


            SubscriptionResultExtension subscriptionDetail = new SubscriptionResultExtension()
            {
                Id = subscription.AmpsubscriptionId,
                SubscribeId = subscription.Id,
                PlanId = string.IsNullOrEmpty(subscription.AmpplanId) ? string.Empty : subscription.AmpplanId,
                Quantity = subscription.Ampquantity,
                Name = subscription.Name,
                SubscriptionStatus = this.subscriptionService.GetSubscriptionStatus(subscription.SubscriptionStatus),
                IsActiveSubscription = subscription.IsActive ?? false,
                CustomerEmailAddress = userdeatils.EmailAddress,
                CustomerName = userdeatils.FullName,
                GuidPlanId = planDetails.PlanGuid,
                SubscriptionParameters = subscriptionParametersList,
                ARMTemplateParameters = subscriptionTemplateParametersList
            };


            /*
             *  KB: Trigger the email when the subscription is in one of the following statuses:
             *  PendingFulfillmentStart - Send out the pending activation email.
             *  DeploymentFailed
             *  Subscribed
             *  ActivationFailed
             *  DeleteResourceFailed
             *  Unsubscribed
             *  UnsubscribeFailed
             */
            string planEventName = "Activate";

            if (
             subscription.SubscriptionStatus == SubscriptionStatusEnumExtension.Unsubscribed.ToString() ||
                subscription.SubscriptionStatus == SubscriptionStatusEnumExtension.DeleteResourceFailed.ToString() ||
                subscription.SubscriptionStatus == SubscriptionStatusEnumExtension.UnsubscribeFailed.ToString()
                )
            {
                planEventName = "Unsubscribe";

            }
            subscriptionDetail.EventName = planEventName;

            string processStatus = "success";
            if (
                subscription.SubscriptionStatus == SubscriptionStatusEnumExtension.DeploymentFailed.ToString() ||
                subscription.SubscriptionStatus == SubscriptionStatusEnumExtension.ActivationFailed.ToString() ||
                subscription.SubscriptionStatus == SubscriptionStatusEnumExtension.UnsubscribeFailed.ToString() ||
                subscription.SubscriptionStatus == SubscriptionStatusEnumExtension.DeleteResourceFailed.ToString()
                )
            {
                processStatus = "failure";

            }

            int? eventId = this.eventsRepository.GetByName(planEventName)?.EventsId;

            var planEvents = this.planEventsMappingRepository.GetPlanEvent(planDetails.PlanGuid, eventId.GetValueOrDefault());

            bool isEmailEnabledForUnsubscription = Convert.ToBoolean(this.applicationConfigRepository.GetValueByName("IsEmailEnabledForUnsubscription"));
            bool isEmailEnabledForPendingActivation = Convert.ToBoolean(this.applicationConfigRepository.GetValueByName("IsEmailEnabledForPendingActivation"));
            bool isEmailEnabledForSubscriptionActivation = Convert.ToBoolean(this.applicationConfigRepository.GetValueByName("IsEmailEnabledForSubscriptionActivation"));
            
            bool triggerEmail = false;
            if (planEvents.Isactive == true)
            {
                if (planEventName == "Activate" && (isEmailEnabledForPendingActivation || isEmailEnabledForSubscriptionActivation))
                {
                    triggerEmail = true;
                }
                if (planEventName == "Unsubscribe" && isEmailEnabledForUnsubscription)
                {
                    triggerEmail = true;
                }

            }
            var emailContent = this.emailHelper.PrepareEmailContent(subscriptionDetail, processStatus, subscriptionDetail.SubscriptionStatus, subscriptionDetail.SubscriptionStatus.ToString());

            if (triggerEmail)
            {
                this.emailService.SendEmail(emailContent);

                if (emailContent.CopyToCustomer && !string.IsNullOrEmpty(subscriptionDetail.CustomerEmailAddress))
                {
                    emailContent.ToEmails = subscriptionDetail.CustomerEmailAddress;
                    this.emailService.SendEmail(emailContent);
                }
            }
        }
    }
}