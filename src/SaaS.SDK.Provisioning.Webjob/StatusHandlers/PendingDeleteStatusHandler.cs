namespace Microsoft.Marketplace.SaasKit.Provisioning.Webjob.StatusHandlers
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Marketplace.SaaS.SDK.Services.Contracts;
    using Microsoft.Marketplace.SaaS.SDK.Services.Helpers;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using Microsoft.Marketplace.SaasKit.Contracts;
    using Newtonsoft.Json;
    using System;
    using System.Linq;

    /// <summary>
    /// Status handler to handle the subscription that is in pending delete status
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Provisioning.Webjob.StatusHandlers.AbstractSubscriptionStatusHandler" />
    public class PendingDeleteStatusHandler : AbstractSubscriptionStatusHandler
    {
        /// <summary>
        /// The fulfillment API client
        /// </summary>
        protected readonly IFulfillmentApiClient fulfillmentApiClient;

        /// <summary>
        /// The application configuration repository
        /// </summary>
        protected readonly IApplicationConfigRepository applicationConfigRepository;

        /// <summary>
        /// The subscription log repository
        /// </summary>
        protected readonly ISubscriptionLogRepository subscriptionLogRepository;

        /// <summary>
        /// The azure key vault client
        /// </summary>
        protected readonly IVaultService azureKeyVaultClient;

        /// <summary>
        /// The key vault configuration
        /// </summary>
        protected readonly KeyVaultConfig keyVaultConfig;

        /// <summary>
        /// The subscriptions template repository
        /// </summary>
        protected readonly ISubscriptionTemplateParametersRepository subscriptionsTemplateRepository;

        /// <summary>
        /// The logger
        /// </summary>
        protected readonly ILogger<PendingDeleteStatusHandler> logger;

        /// <summary>
        /// ARM template deployment manager
        /// </summary>
        protected readonly ARMTemplateDeploymentManager aRMTemplateDeploymentManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="PendingDeleteStatusHandler"/> class.
        /// </summary>
        /// <param name="fulfillApiClient">The fulfill API client.</param>
        /// <param name="applicationConfigRepository">The application configuration repository.</param>
        /// <param name="subscriptionLogRepository">The subscription log repository.</param>
        /// <param name="subscriptionsRepository">The subscriptions repository.</param>
        /// <param name="azureKeyVaultClient">The azure key vault client.</param>
        /// <param name="keyVaultConfig">The key vault configuration.</param>
        /// <param name="subscriptionsTemplateRepository">The subscriptions template repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="aRMTemplateDeploymentManager">a rm template deployment manager.</param>
        public PendingDeleteStatusHandler(
                                            IFulfillmentApiClient fulfillApiClient,
                                            IApplicationConfigRepository applicationConfigRepository,
                                            ISubscriptionLogRepository subscriptionLogRepository,
                                            ISubscriptionsRepository subscriptionsRepository,
                                            IVaultService azureKeyVaultClient,
                                            KeyVaultConfig keyVaultConfig,
                                            ISubscriptionTemplateParametersRepository subscriptionsTemplateRepository,
                                            IPlansRepository plansRepository,
                                            IUsersRepository usersRepository,
                                            ILogger<PendingDeleteStatusHandler> logger,
                                            ARMTemplateDeploymentManager aRMTemplateDeploymentManager) : base(subscriptionsRepository, plansRepository, usersRepository)
        {
            this.fulfillmentApiClient = fulfillApiClient;
            this.applicationConfigRepository = applicationConfigRepository;
            this.subscriptionLogRepository = subscriptionLogRepository;
            this.azureKeyVaultClient = azureKeyVaultClient;
            this.keyVaultConfig = keyVaultConfig;
            this.subscriptionsTemplateRepository = subscriptionsTemplateRepository;
            this.logger = logger;
            this.aRMTemplateDeploymentManager = aRMTemplateDeploymentManager;

        }

        /// <summary>
        /// Processes the specified subscription identifier.
        /// </summary>
        /// <param name="subscriptionID">The subscription identifier.</param>
        public override void Process(Guid subscriptionID)
        {
            this.logger?.LogInformation("Get GetSubscriptionById");
            var subscription = this.GetSubscriptionById(subscriptionID);
            this.logger?.LogInformation("Get PlanById");
            var planDetails = this.GetPlanById(subscription.AmpplanId);
            this.logger?.LogInformation("Get User");
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
                            this.subscriptionsRepository.UpdateStatusForSubscription(subscriptionID, SubscriptionStatusEnumExtension.DeleteResourcePending.ToString(), true);

                            var resourceGroup = parametersList.Where(s => s.Parameter.ToLower() == "resourcegroup").FirstOrDefault();
                            this.logger?.LogInformation("Get SubscriptionKeyValut");
                            string secretKey = "";
                            if (planDetails.DeployToCustomerSubscription != null && planDetails.DeployToCustomerSubscription == true)
                            {
                                var keyvault = subscriptionsRepository.GetDeploymentConfig(subscriptionID);
                                secretKey = keyvault.SecureId;
                            }
                            else
                            {
                                secretKey = string.Format("{0}secrets/HostedsubscriptionCredentials", keyVaultConfig.KeyVaultUrl);
                            }
                            this.logger?.LogInformation("Get DoVault");
                            string secretValue = azureKeyVaultClient.GetKeyAsync(secretKey).ConfigureAwait(false).GetAwaiter().GetResult();

                            var credenitals = JsonConvert.DeserializeObject<CredentialsModel>(secretValue);
                            this.logger?.LogInformation("SecretValue : {0}", secretValue);

                            this.aRMTemplateDeploymentManager.DeleteResoureGroup(parametersList, credenitals);

                            this.subscriptionLogRepository.LogStatusDuringProvisioning(subscriptionID, default, DeploymentStatusEnum.DeleteResourceGroupSuccess.ToString(), string.Format("Delete Resource Group: {0} End", resourceGroup), subscription.SubscriptionStatus.ToString());

                            this.subscriptionsRepository.UpdateStatusForSubscription(subscriptionID, SubscriptionStatusEnumExtension.DeleteResourceSuccess.ToString(), true);

                            SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
                            {
                                Attribute = SubscriptionLogAttributes.Deployment.ToString(),
                                SubscriptionId = subscription.Id,
                                NewValue = SubscriptionStatusEnumExtension.DeleteResourceSuccess.ToString(),
                                OldValue = SubscriptionStatusEnumExtension.DeleteResourcePending.ToString(),
                                CreateBy = userdeatils.UserId,
                                CreateDate = DateTime.Now
                            };
                            this.subscriptionLogRepository.Save(auditLog);
                        }
                    }
                }

                catch (Exception ex)
                {
                    string errorDescriptin = string.Format("Exception: {0} :: Innser Exception:{1}", ex.Message, ex.InnerException);
                    this.subscriptionLogRepository.LogStatusDuringProvisioning(subscriptionID, default, DeploymentStatusEnum.DeleteResourceGroupFailure.ToString(), errorDescriptin, subscription.SubscriptionStatus.ToString());
                    this.logger?.LogInformation(errorDescriptin);

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
                    this.subscriptionLogRepository.Save(auditLog);
                }

            }
        }
    }
}