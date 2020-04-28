using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using Microsoft.Marketplace.SaasKit.Contracts;
using Microsoft.Marketplace.SaasKit.WebJob;
using Microsoft.Marketplace.SaasKit.WebJob.Helpers;
using Microsoft.Marketplace.SaasKit.WebJob.Models;
using Microsoft.Marketplace.SaasKit.WebJob.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.WebJob.StatusHandlers
{

    class NotificationStatusHandler : AbstractSubscriptionStatusHandler
    {

        readonly IFulfillmentApiClient fulfillApiclient;

        readonly ISubscriptionsRepository SubscriptionRepository;

        readonly IApplicationConfigRepository applicationConfigRepository;

        readonly IEmailTemplateRepository emailTemplateRepository;

        readonly IPlanEventsMappingRepository planEventsMappingRepository;

        readonly IOfferAttributesRepository offerAttributesRepository;

        readonly IEventsRepository eventsRepository;
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



            SubscriptionResultExtension subscriptionDetail = new SubscriptionResultExtension
            {
                Id = subscription.AmpsubscriptionId,
                SubscribeId = subscription.Id,
                PlanId = string.IsNullOrEmpty(subscription.AmpplanId) ? string.Empty : subscription.AmpplanId,
                Quantity = subscription.Ampquantity,
                Name = subscription.Name,
                SubscriptionStatus = subscription.SubscriptionStatus,
                IsActiveSubscription = subscription.IsActive ?? false,
                CustomerEmailAddress = subscription.User?.EmailAddress,
                CustomerName = subscription.User?.FullName,
                GuidPlanId = planDetails.PlanGuid,
                SubscriptionParameters = subscriptionParametersList,
                ARMTemplateParameters = subscriptionTemplateParametersList
            };





            EmailHelper.SendEmail(subscriptionDetail, applicationConfigRepository, emailTemplateRepository, planEventsMappingRepository, eventsRepository, "failure", SubscriptionWebJobStatusEnum.PendingActivation, "");

        }


    }
}
