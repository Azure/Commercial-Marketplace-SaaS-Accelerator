using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.WebJob.Helpers
{
    public class StatusUpadeHelpers
    {
        public static void UpdateWebJobSubscriptionStatus(Guid subscriptionID, Guid? ArmtempalteId, string deploymentStatus, string errorDescription, SaasKitContext context, string subscriptionStatus)
        {
            var subscription = context.Subscriptions.Where(s => s.AmpsubscriptionId == subscriptionID).FirstOrDefault();
            var existingWebJobStatus = context.WebJobSubscriptionStatus.Where(s => s.SubscriptionId == subscriptionID).FirstOrDefault();
            if (existingWebJobStatus == null)
            {
                WebJobSubscriptionStatus status = new WebJobSubscriptionStatus()
                {
                    SubscriptionId = subscriptionID,
                    ArmtemplateId = ArmtempalteId,
                    SubscriptionStatus = subscriptionStatus,
                    DeploymentStatus = deploymentStatus,
                    Description = errorDescription,
                    InsertDate = DateTime.Now
                };
                Console.WriteLine("New WebJobSubscriptionStatus Status {0}:", JsonConvert.SerializeObject(status));
                context.WebJobSubscriptionStatus.Add(status);
                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("Existing WebJobSubscriptionStatus Status {0}:", JsonConvert.SerializeObject(existingWebJobStatus));

                existingWebJobStatus.SubscriptionId = subscriptionID;
                if (ArmtempalteId != default)
                {
                    existingWebJobStatus.ArmtemplateId = ArmtempalteId;
                }
                existingWebJobStatus.SubscriptionStatus = subscription.SubscriptionStatus;
                existingWebJobStatus.DeploymentStatus = deploymentStatus;
                existingWebJobStatus.Description = errorDescription;

                Console.WriteLine("Existing WebJobSubscriptionStatus Status Updated {0}:", JsonConvert.SerializeObject(existingWebJobStatus));
                context.WebJobSubscriptionStatus.Update(existingWebJobStatus);
                context.SaveChanges();

            }

        }

    }
}
