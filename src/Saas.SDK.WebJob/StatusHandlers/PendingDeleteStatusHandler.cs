﻿using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
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
        readonly IApplicationConfigRepository applicationConfigRepository;
        readonly ISubscriptionLogRepository subscriptionLogRepository;
        readonly ISubscriptionsRepository subscriptionsRepository;

        public PendingDeleteStatusHandler(IFulfillmentApiClient fulfillApiClient, IApplicationConfigRepository applicationConfigRepository, ISubscriptionLogRepository subscriptionLogRepository, ISubscriptionsRepository subscriptionsRepository) : base(new SaasKitContext())
        {
            this.fulfillApiclient = fulfillApiClient;
            this.applicationConfigRepository = applicationConfigRepository;
            this.subscriptionLogRepository = subscriptionLogRepository;
            this.subscriptionsRepository = subscriptionsRepository;

        }
        public override void Process(Guid subscriptionID)
        {
            var subscription = this.GetSubscriptionById(subscriptionID);

            if (subscription.SubscriptionStatus == SubscriptionWebJobStatusEnum.PendingUnsubscribe.ToString())
            {
                try
                {
                    this.subscriptionsRepository.UpdateStatusForSubscription(subscriptionID, SubscriptionWebJobStatusEnum.DeleteResourcePendign.ToString(), true);

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
                            string secretValue = AzureKeyVaultHelper.DoVault(keyvaultUrl.SecureId);

                            var credenitals = JsonConvert.DeserializeObject<CredentialsModel>(secretValue);
                            Console.WriteLine("SecretValue : {0}", secretValue);

                            Deploy deploy = new Deploy();
                            deploy.DeleteResoureGroup(parametersList, credenitals);

                            StatusUpadeHelpers.UpdateWebJobSubscriptionStatus(subscriptionID, default, DeploymentStatusEnum.DeleteResourceGroupSuccess.ToString(), string.Format("Delete Resource Group: {0} End", resourceGroup), Context, subscription.SubscriptionStatus.ToString());

                            this.subscriptionsRepository.UpdateStatusForSubscription(subscriptionID, SubscriptionWebJobStatusEnum.DeleteResourceSuccess.ToString(), true);

                            SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
                            {
                                Attribute = SubscriptionLogAttributes.Deployment.ToString(),
                                SubscriptionId = subscription.Id,
                                NewValue = SubscriptionWebJobStatusEnum.DeleteResourceSuccess.ToString(),
                                OldValue = SubscriptionWebJobStatusEnum.DeleteResourcePendign.ToString(),
                                CreateBy = 0,
                                CreateDate = DateTime.Now
                            };
                            this.subscriptionLogRepository.Add(auditLog);
                        }
                    }
                }

                catch (Exception ex)
                {
                    string errorDescriptin = string.Format("Exception: {0} :: Innser Exception:{1}", ex.Message, ex.InnerException);
                    StatusUpadeHelpers.UpdateWebJobSubscriptionStatus(subscriptionID, default, DeploymentStatusEnum.DeleteResourceGroupFailure.ToString(), errorDescriptin, Context, subscription.SubscriptionStatus.ToString());
                    Console.WriteLine(errorDescriptin);

                    this.subscriptionsRepository.UpdateStatusForSubscription(subscriptionID, SubscriptionWebJobStatusEnum.DeleteResourceFailed.ToString(), true);

                    SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
                    {
                        Attribute = SubscriptionLogAttributes.Deployment.ToString(),
                        SubscriptionId = subscription.Id,
                        NewValue = SubscriptionWebJobStatusEnum.DeleteResourceFailed.ToString(),
                        OldValue = subscription.SubscriptionStatus.ToString(),
                        CreateBy = 0,
                        CreateDate = DateTime.Now
                    };
                    this.subscriptionLogRepository.Add(auditLog);
                }

            }
        }

    }
}
