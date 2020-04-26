using Microsoft.EntityFrameworkCore;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using Microsoft.Marketplace.SaasKit.Contracts;
using Microsoft.Marketplace.SaasKit.WebJob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.WebJob.StatusHandlers
{

    class ResourceDeploymentStatusHandler : AbstractSubscriptionStatusHandler
    {

        readonly IFulfillmentApiClient fulfillApiclient;

        public ResourceDeploymentStatusHandler(IFulfillmentApiClient fulfillApiClient) : base(new SaasKitContext())
        {
            this.fulfillApiclient = fulfillApiClient;

        }
        public override void Process(Guid subscriptionID)
        {
            Console.Write("Process...");
            Console.Write("GetSubscriptionById");
            var subscription = this.GetSubscriptionById(subscriptionID);
            Console.Write("GetPlanById");
            var planDetails = this.GetPlanById(subscription.AmpplanId);
            Console.Write("Offers");
            var offer = Context.Offers.Where(s => s.OfferGuid == planDetails.OfferId).FirstOrDefault();
            Console.Write("Events");
            var events = Context.Events.Where(s => s.EventsName == "Activate").FirstOrDefault();
            Console.Write("Events");

            Console.Write("subscription.SubscriptionStatus: SubscriptionStatus: {0}", subscription.SubscriptionStatus);
            if (subscription.SubscriptionStatus == "PendingActivation")
            {
                try
                {
                    // Check if arm template is available fo rthe plan in Plan Event Mapping table with isactive=1

                    var planEvent = Context.PlanEventsMapping.Where(s => s.PlanId == planDetails.PlanGuid && s.EventId == events.EventsId).FirstOrDefault();
                    var armTemplate = Context.Armtemplates.Where(s => s.ArmtempalteId == planEvent.ArmtemplateId).FirstOrDefault();


                    var subscriptionAttributes = Context.SubscriptionParametersOutput.FromSqlRaw("dbo.spGetSubscriptionParameters {0},{1}", subscriptionID, planDetails.PlanGuid);

                    if (armTemplate != null && subscriptionAttributes != null && subscriptionAttributes.Count() > 0)
                    {
                        var credenitals = subscriptionAttributes.Where(s => s.Type.ToLower() == "deployment").ToList();
                        var templateParameters = Context.SubscriptionTemplateParameters.Where(
                           s => s.PlanGuid == planDetails.PlanGuid &&
                           s.ArmtemplateId == armTemplate.ArmtempalteId &&
                           s.AmpsubscriptionId == subscriptionID &&
                           s.OfferGuid == offer.OfferGuid);

                        if (templateParameters != null && templateParameters.Count() > 0)
                        {
                            var parametersList = templateParameters.ToList();
                            WebJobSubscriptionStatus status = new WebJobSubscriptionStatus()
                            {

                                SubscriptionId = subscriptionID,
                                ArmtemplateId = armTemplate.ArmtempalteId,
                                SubscriptionStatus = subscription.SubscriptionStatus,
                                DeploymentStatus = "ARMTemplateDeploymentPending",
                                Description = "Start Deployment",
                                InsertDate = System.DateTime.Now

                            };

                            Context.WebJobSubscriptionStatus.Add(status);
                            Console.Write("Start Deployment");
                            Deploy obj = new Deploy();
                            obj.DeployTemplate(armTemplate, parametersList, credenitals);
                        }

                    }



                    //Start ARM template Deployment
                    //Change status to ARMTemplateDeploymentSuccess

                }

                }
                catch (Exception ex)
            {
                //Change status to ARMTemplateDeploymentFailure

            }

        }
    }

}
}
