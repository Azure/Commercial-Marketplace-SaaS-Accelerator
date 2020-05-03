using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using Microsoft.Marketplace.SaasKit.Contracts;
using Microsoft.Marketplace.SaaS.SDK.Services;
using Microsoft.Marketplace.SaaS.SDK.Services.Helpers;
using Microsoft.Marketplace.SaaS.SDK.Services.Models;
using Microsoft.Marketplace.SaaS.SDK.Services.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.Provisioning.Webjob.StatusHandlers
{

    class NotificationStatusHandler : AbstractSubscriptionStatusHandler
    {

        protected readonly IFulfillmentApiClient fulfillApiclient;

        protected readonly ISubscriptionsRepository SubscriptionRepository;

        protected readonly IApplicationConfigRepository applicationConfigRepository;

        protected readonly IEmailTemplateRepository emailTemplateRepository;

        protected readonly IPlanEventsMappingRepository planEventsMappingRepository;

        protected readonly IOfferAttributesRepository offerAttributesRepository;

        private SubscriptionService subscriptionService = null;

        protected readonly IPlansRepository planRepository;

        protected readonly IOffersRepository offersRepository;

        protected readonly IEventsRepository eventsRepository;

        protected readonly ISubscriptionTemplateParametersRepository subscriptionTemplateParametersRepository;

        public NotificationStatusHandler(IFulfillmentApiClient fulfillApiClient, IPlansRepository planRepository,
            IApplicationConfigRepository applicationConfigRepository, IEmailTemplateRepository emailTemplateRepository, IPlanEventsMappingRepository planEventsMappingRepository, IOfferAttributesRepository offerAttributesRepository, IEventsRepository eventsRepository, ISubscriptionsRepository SubscriptionRepository, IOffersRepository offersRepository, ISubscriptionTemplateParametersRepository subscriptionTemplateParametersRepository) : base(new SaasKitContext())
        {
            this.fulfillApiclient = fulfillApiClient;
            this.applicationConfigRepository = applicationConfigRepository;
            this.planEventsMappingRepository = planEventsMappingRepository;
            this.offerAttributesRepository = offerAttributesRepository;
            this.eventsRepository = eventsRepository;
            this.emailTemplateRepository = emailTemplateRepository;
            this.SubscriptionRepository = SubscriptionRepository;
            this.planRepository = planRepository;
            this.subscriptionService = new SubscriptionService(this.SubscriptionRepository, this.planRepository);
            this.offersRepository = offersRepository;
            this.subscriptionTemplateParametersRepository = subscriptionTemplateParametersRepository;

        }
        public override void Process(Guid subscriptionID)
        {
            Console.WriteLine("ResourceDeploymentStatusHandler Process...");
            Console.WriteLine("Get SubscriptionById");
            var subscription = this.GetSubscriptionById(subscriptionID);
            Console.WriteLine("Get PlanById");
            var planDetails = this.GetPlanById(subscription.AmpplanId);
            Console.WriteLine("Get User");
            var userdeatils = this.GetUserById(subscription.UserId);
            Console.WriteLine("Get Offers");
            var offer = this.offersRepository.GetOfferDetailByOfferId(planDetails.OfferId);
            Console.WriteLine("Get Events");
            var events = this.eventsRepository.GetEventID("Activate");

            var subscriptionTemplateParameters = this.subscriptionTemplateParametersRepository.GetSubscriptionTemplateParameters(subscription.AmpsubscriptionId);
            List<SubscriptionTemplateParametersModel> subscriptionTemplateParametersList = new List<SubscriptionTemplateParametersModel>();
            if (subscriptionTemplateParameters != null)
            {
                var serializedParent = JsonConvert.SerializeObject(subscriptionTemplateParameters.ToList());
                subscriptionTemplateParametersList = JsonConvert.DeserializeObject<List<SubscriptionTemplateParametersModel>>(serializedParent);
            }

            List<SubscriptionParametersModel> subscriptionParametersList = new List<SubscriptionParametersModel>();

            var subscriptionParameters = this.SubscriptionRepository.GetSubscriptionsParametersById(subscriptionID, planDetails.PlanGuid);


            var serializedSubscription = JsonConvert.SerializeObject(subscriptionParameters);
            subscriptionParametersList = JsonConvert.DeserializeObject<List<SubscriptionParametersModel>>(serializedSubscription);



            SubscriptionResultExtension subscriptionDetail = new SubscriptionResultExtension();

            subscriptionDetail.Id = subscription.AmpsubscriptionId;
            subscriptionDetail.SubscribeId = subscription.Id;
            subscriptionDetail.PlanId = string.IsNullOrEmpty(subscription.AmpplanId) ? string.Empty : subscription.AmpplanId;
            subscriptionDetail.Quantity = subscription.Ampquantity;
            subscriptionDetail.Name = subscription.Name;
            subscriptionDetail.SubscriptionStatus = this.subscriptionService.GetSubscriptionStatus(subscription.SubscriptionStatus);
            subscriptionDetail.IsActiveSubscription = subscription.IsActive ?? false;
            subscriptionDetail.CustomerEmailAddress = userdeatils.EmailAddress;
            subscriptionDetail.CustomerName = userdeatils.FullName;
            subscriptionDetail.GuidPlanId = planDetails.PlanGuid;
            subscriptionDetail.SubscriptionParameters = subscriptionParametersList;
            subscriptionDetail.ARMTemplateParameters = subscriptionTemplateParametersList;


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
            string eventStatus = "Activate";


            if (
             subscription.SubscriptionStatus == SubscriptionStatusEnumExtension.Unsubscribed.ToString() ||
                subscription.SubscriptionStatus == SubscriptionStatusEnumExtension.DeleteResourceFailed.ToString() ||
                subscription.SubscriptionStatus == SubscriptionStatusEnumExtension.UnsubscribeFailed.ToString()
                )
            {
                eventStatus = "Unsubscribe";

            }
            subscriptionDetail.EventName = eventStatus;

            string emailStatusKey = "success";
            if (
                subscription.SubscriptionStatus == SubscriptionStatusEnumExtension.DeploymentFailed.ToString() ||
                subscription.SubscriptionStatus == SubscriptionStatusEnumExtension.ActivationFailed.ToString() ||
                subscription.SubscriptionStatus == SubscriptionStatusEnumExtension.UnsubscribeFailed.ToString() ||
                subscription.SubscriptionStatus == SubscriptionStatusEnumExtension.DeleteResourceFailed.ToString()
                )
            {
                emailStatusKey = "failure";

            }
            EmailHelper emial = new EmailHelper(this.applicationConfigRepository, this.SubscriptionRepository, this.emailTemplateRepository, this.planEventsMappingRepository, this.eventsRepository);
            emial.SendEmail(subscriptionDetail, emailStatusKey, SubscriptionStatusEnumExtension.PendingActivation, "");

        }


    }
}
