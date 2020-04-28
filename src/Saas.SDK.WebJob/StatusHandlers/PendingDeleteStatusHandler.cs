using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Contracts;
using Microsoft.Marketplace.SaasKit.WebJob;
using Microsoft.Marketplace.SaasKit.WebJob.Helpers;
using Microsoft.Marketplace.SaasKit.WebJob.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.WebJob.StatusHandlers
{

    class PendingDeleteStatusHandler : AbstractSubscriptionStatusHandler
    {

        readonly IFulfillmentApiClient fulfillApiclient;

        public PendingDeleteStatusHandler(IFulfillmentApiClient fulfillApiClient) : base(new SaasKitContext())
        {
            this.fulfillApiclient = fulfillApiClient;

        }
        public override void Process(Guid subscriptionID)
        {
            var subscription = this.GetSubscriptionById(subscriptionID);

            if (subscription.SubscriptionStatus == "Subscribed")
            {
                try
                {

                    StatusUpadeHelpers.UpdateWebJobSubscriptionStatus(subscriptionID, default, DeploymentStatusEnum.DeleteResourceGroupPending.ToString(), "Delete Resource Group Begin", Context, subscription.SubscriptionStatus);
                    var subscriptionParameters = Context.SubscriptionTemplateParameters.Where(s => s.AmpsubscriptionId == subscriptionID);
                    if (subscriptionParameters != null)
                    {
                        var parametersList = subscriptionParameters.ToList();
                        if (parametersList.Count() > 0)
                        {
                            var resourceGroup = parametersList.Where(s => s.Parameter.ToLower() == "resourcegroup").FirstOrDefault();
                            Console.WriteLine("Get SubscriptionKeyValut");
                            var keyvaultUrl = Context.SubscriptionKeyValut.Where(s => s.SubscriptionId == subscriptionID).FirstOrDefault();

                            Console.WriteLine("Get DoVault");
                            string secretValue = AzureKeyVaultHelper.DoVault(keyvaultUrl.SecuteId);

                            var credenitals = JsonConvert.DeserializeObject<CredentialsModel>(secretValue);
                            Console.WriteLine("SecretValue : {0}", secretValue);

                            Deploy deploy = new Deploy();
                            deploy.DeleteResoureGroup(parametersList, credenitals);

                            StatusUpadeHelpers.UpdateWebJobSubscriptionStatus(subscriptionID, default, DeploymentStatusEnum.DeleteResourceGroupSuccess.ToString(), string.Format("Delete Resource Group: {0} End", resourceGroup), Context, subscription.SubscriptionStatus.ToString());

                        }
                    }
                }

                catch (Exception ex)
                {
                    string errorDescriptin = string.Format("Exception: {0} :: Innser Exception:{1}", ex.Message, ex.InnerException);
                    StatusUpadeHelpers.UpdateWebJobSubscriptionStatus(subscriptionID, default, DeploymentStatusEnum.DeleteResourceGroupFailure.ToString(), errorDescriptin, Context, subscription.SubscriptionStatus.ToString());
                    Console.WriteLine(errorDescriptin);

                }

            }
        }

    }
}
