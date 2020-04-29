using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using Microsoft.Marketplace.SaasKit.Contracts;
using Microsoft.Marketplace.SaasKit.Provisioning.Webjob;
using Microsoft.Marketplace.SaasKit.Provisioning.Webjob.Helpers;
using Microsoft.Marketplace.SaasKit.Provisioning.Webjob.Models;
using Microsoft.Marketplace.SaasKit.Provisioning.Webjob.Models;
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

        protected readonly IEventsRepository eventsRepository;
        public NotificationStatusHandler(IFulfillmentApiClient fulfillApiClient,
            IApplicationConfigRepository applicationConfigRepository, IEmailTemplateRepository emailTemplateRepository, IPlanEventsMappingRepository planEventsMappingRepository, IOfferAttributesRepository offerAttributesRepository, IEventsRepository eventsRepository, ISubscriptionsRepository SubscriptionRepository) : base(new SaasKitContext())
        {
            this.fulfillApiclient = fulfillApiClient;
            this.applicationConfigRepository = applicationConfigRepository;
            this.planEventsMappingRepository = planEventsMappingRepository;
            this.offerAttributesRepository = offerAttributesRepository;
            this.eventsRepository = eventsRepository;
            this.emailTemplateRepository = emailTemplateRepository;
            this.SubscriptionRepository = SubscriptionRepository;

        }
        public override void Process(Guid subscriptionID)
        {
            Console.WriteLine("ResourceDeploymentStatusHandler Process...");
            Console.WriteLine("Get SubscriptionById");
            var subscription = this.GetSubscriptionById(subscriptionID);
            Console.WriteLine("Get PlanById");
            var planDetails = this.GetPlanById(subscription.AmpplanId);
            Console.WriteLine("Get Offers");
            var offer = Context.Offers.Where(s => s.OfferGuid == planDetails.OfferId).FirstOrDefault();
            Console.WriteLine("Get Events");
            var events = Context.Events.Where(s => s.EventsName == "Activate").FirstOrDefault();

            //if (subscription.SubscriptionStatus == "Subscribed")
            //{
            //    var subscriptionData = this.fulfillApiclient.GetSubscriptionByIdAsync(subscriptionID).ConfigureAwait(false).GetAwaiter().GetResult();
            //}

            var subscriptionTemplateParameters = Context.SubscriptionTemplateParameters.Where(s => s.AmpsubscriptionId == subscriptionID);
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
            subscriptionDetail.SubscriptionStatus = subscription.SubscriptionStatus;
            subscriptionDetail.IsActiveSubscription = subscription.IsActive ?? false;
            subscriptionDetail.CustomerEmailAddress = subscription.User?.EmailAddress;
            subscriptionDetail.CustomerName = subscription.User?.FullName;
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
             subscription.SubscriptionStatus == SubscriptionWebJobStatusEnum.Unsubscribed.ToString() ||
                subscription.SubscriptionStatus == SubscriptionWebJobStatusEnum.DeleteResourceFailed.ToString() ||
                subscription.SubscriptionStatus == SubscriptionWebJobStatusEnum.UnsubscribeFailed.ToString()
                )
            {
                eventStatus = "Unsubscribe";

            }
            subscriptionDetail.EventName = eventStatus;

            string emailStatusKey = "success";
            if (
                subscription.SubscriptionStatus == SubscriptionWebJobStatusEnum.DeploymentFailed.ToString() ||
                subscription.SubscriptionStatus == SubscriptionWebJobStatusEnum.ActivationFailed.ToString() ||
                subscription.SubscriptionStatus == SubscriptionWebJobStatusEnum.UnsubscribeFailed.ToString() ||
                subscription.SubscriptionStatus == SubscriptionWebJobStatusEnum.DeleteResourceFailed.ToString()
                )
            {
                emailStatusKey = "failure";

            }
            EmailHelper emial = new EmailHelper(this.applicationConfigRepository, this.SubscriptionRepository, this.emailTemplateRepository, this.planEventsMappingRepository, this.eventsRepository);
            emial.SendEmail(subscriptionDetail, emailStatusKey, SubscriptionWebJobStatusEnum.PendingActivation, "");

        }


    }
}
