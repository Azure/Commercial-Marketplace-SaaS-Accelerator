using Microsoft.Marketplace.SaaS.SDK.Services.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using Microsoft.Marketplace.SaasKit.Contracts;
using Microsoft.Marketplace.SaaS.SDK.Services;
using Microsoft.Marketplace.SaaS.SDK.Services.Helpers;
using Microsoft.Marketplace.SaaS.SDK.Services.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Microsoft.Marketplace.SaasKit.Provisioning.Webjob.StatusHandlers
{
    class PendingDeleteStatusHandler : AbstractSubscriptionStatusHandler
    {

        protected readonly IFulfillmentApiClient fulfillApiclient;
        protected readonly IApplicationConfigRepository applicationConfigRepository;
        protected readonly ISubscriptionLogRepository subscriptionLogRepository;
        protected readonly ISubscriptionsRepository subscriptionsRepository;
        protected readonly IVaultService azureKeyVaultClient;
        protected readonly KeyVaultConfig keyVaultConfig;
        protected readonly ISubscriptionTemplateParametersRepository subscriptionsTemplateRepository;
        protected readonly ILogger<PendingDeleteStatusHandler> logger;
        protected readonly ARMTemplateDeploymentManager aRMTemplateDeploymentManager;

        public PendingDeleteStatusHandler(IFulfillmentApiClient fulfillApiClient,
                                            IApplicationConfigRepository applicationConfigRepository,
                                            ISubscriptionLogRepository subscriptionLogRepository,
                                            ISubscriptionsRepository subscriptionsRepository,
                                            IVaultService azureKeyVaultClient,
                                              KeyVaultConfig keyVaultConfig,
                                              ISubscriptionTemplateParametersRepository subscriptionsTemplateRepository,
                                              ILogger<PendingDeleteStatusHandler> logger,
                                              ARMTemplateDeploymentManager aRMTemplateDeploymentManager) : base(new SaasKitContext())
        {
            this.fulfillApiclient = fulfillApiClient;
            this.applicationConfigRepository = applicationConfigRepository;
            this.subscriptionLogRepository = subscriptionLogRepository;
            this.subscriptionsRepository = subscriptionsRepository;
            this.azureKeyVaultClient = azureKeyVaultClient;
            this.keyVaultConfig = keyVaultConfig;
            this.subscriptionsTemplateRepository = subscriptionsTemplateRepository;
            this.logger = logger;
            this.aRMTemplateDeploymentManager = aRMTemplateDeploymentManager;

        }
        public override void Process(Guid subscriptionID)
        {
            this.logger.LogInformation("Get GetSubscriptionById");
            var subscription = this.GetSubscriptionById(subscriptionID);
            this.logger.LogInformation("Get PlanById");
            var planDetails = this.GetPlanById(subscription.AmpplanId);
            this.logger.LogInformation("Get User");
            var userdeatils = this.GetUserById(subscription.UserId);

            if (subscription.SubscriptionStatus == SubscriptionStatusEnumExtension.PendingUnsubscribe.ToString())
            {
                try
                {

                    var subscriptionParameters = subscriptionsTemplateRepository.GetTemplateParametersBySubscriptionId(subscriptionID);
                    if (subscriptionParameters != null)
                    {
                        var parametersList = subscriptionParameters.ToList();
                        if (parametersList.Count() > 0)
                        {

                            this.subscriptionLogRepository.LogStatusDuringProvisioning(subscriptionID, default, DeploymentStatusEnum.DeleteResourceGroupPending.ToString(), "Delete Resource Group Begin", subscription.SubscriptionStatus);
                            this.subscriptionsRepository.UpdateStatusForSubscription(subscriptionID, SubscriptionStatusEnumExtension.DeleteResourcePendign.ToString(), true);

                            var resourceGroup = parametersList.Where(s => s.Parameter.ToLower() == "resourcegroup").FirstOrDefault();
                            this.logger.LogInformation("Get SubscriptionKeyValut");
                            string secretKey = "";
                            if (planDetails.DeployToCustomerSubscription != null && planDetails.DeployToCustomerSubscription == true)
                            {
                                var keyvault = Context.SubscriptionKeyValut.Where(s => s.SubscriptionId == subscriptionID).FirstOrDefault();
                                secretKey = keyvault.SecureId;
                            }
                            else
                            {
                                secretKey = string.Format("{0}secrets/HostedsubscriptionCredentials", keyVaultConfig.KeyVaultUrl);
                            }
                            this.logger.LogInformation("Get DoVault");
                            string secretValue = azureKeyVaultClient.GetKeyAsync(secretKey).ConfigureAwait(false).GetAwaiter().GetResult();

                            var credenitals = JsonConvert.DeserializeObject<CredentialsModel>(secretValue);
                            this.logger.LogInformation("SecretValue : {0}", secretValue);
                                                        
                            this.aRMTemplateDeploymentManager.DeleteResoureGroup(parametersList, credenitals);

                            this.subscriptionLogRepository.LogStatusDuringProvisioning(subscriptionID, default, DeploymentStatusEnum.DeleteResourceGroupSuccess.ToString(), string.Format("Delete Resource Group: {0} End", resourceGroup), subscription.SubscriptionStatus.ToString());

                            this.subscriptionsRepository.UpdateStatusForSubscription(subscriptionID, SubscriptionStatusEnumExtension.DeleteResourceSuccess.ToString(), true);

                            SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
                            {
                                Attribute = SubscriptionLogAttributes.Deployment.ToString(),
                                SubscriptionId = subscription.Id,
                                NewValue = SubscriptionStatusEnumExtension.DeleteResourceSuccess.ToString(),
                                OldValue = SubscriptionStatusEnumExtension.DeleteResourcePendign.ToString(),
                                CreateBy = userdeatils.UserId,
                                CreateDate = DateTime.Now
                            };
                            this.subscriptionLogRepository.Add(auditLog);
                        }
                    }
                }

                catch (Exception ex)
                {
                    string errorDescriptin = string.Format("Exception: {0} :: Innser Exception:{1}", ex.Message, ex.InnerException);
                    this.subscriptionLogRepository.LogStatusDuringProvisioning(subscriptionID, default, DeploymentStatusEnum.DeleteResourceGroupFailure.ToString(), errorDescriptin, subscription.SubscriptionStatus.ToString());
                    this.logger.LogInformation(errorDescriptin);

                    this.subscriptionsRepository.UpdateStatusForSubscription(subscriptionID, SubscriptionStatusEnumExtension.DeleteResourceFailed.ToString(), true);

                    SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
                    {
                        Attribute = SubscriptionLogAttributes.Deployment.ToString(),
                        SubscriptionId = subscription.Id,
                        NewValue = SubscriptionStatusEnumExtension.DeleteResourceFailed.ToString(),
                        OldValue = subscription.SubscriptionStatus.ToString(),
                        CreateBy = userdeatils.UserId,
                        CreateDate = DateTime.Now
                    };
                    this.subscriptionLogRepository.Add(auditLog);
                }

            }
        }

    }
}
