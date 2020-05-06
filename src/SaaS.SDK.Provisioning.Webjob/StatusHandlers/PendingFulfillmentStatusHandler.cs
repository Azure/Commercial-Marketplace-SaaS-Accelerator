namespace Microsoft.Marketplace.SaasKit.Provisioning.Webjob.StatusHandlers
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using Microsoft.Marketplace.SaasKit.Contracts;
    using Newtonsoft.Json;
    using System;

    /// <summary>
    /// Status handler to handle the subscription in PendingFulfillment
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Provisioning.Webjob.StatusHandlers.AbstractSubscriptionStatusHandler" />
    public class PendingFulfillmentStatusHandler : AbstractSubscriptionStatusHandler
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
        /// The logger
        /// </summary>
        protected readonly ILogger<PendingFulfillmentStatusHandler> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PendingFulfillmentStatusHandler"/> class.
        /// </summary>
        /// <param name="fulfillApiClient">The fulfill API client.</param>
        /// <param name="applicationConfigRepository">The application configuration repository.</param>
        /// <param name="subscriptionsRepository">The subscriptions repository.</param>
        /// <param name="subscriptionLogRepository">The subscription log repository.</param>
        public PendingFulfillmentStatusHandler(
                                                IFulfillmentApiClient fulfillApiClient,
                                                IApplicationConfigRepository applicationConfigRepository,
                                                ISubscriptionsRepository subscriptionsRepository,
                                                ISubscriptionLogRepository subscriptionLogRepository,
                                                IPlansRepository plansRepository,
                                                IUsersRepository usersRepository,
                                                ILogger<PendingFulfillmentStatusHandler> logger
                                                ) : base(subscriptionsRepository, plansRepository, usersRepository)
        {
            this.fulfillmentApiClient = fulfillApiClient;
            this.applicationConfigRepository = applicationConfigRepository;            
            this.subscriptionLogRepository = subscriptionLogRepository;
            this.logger = logger;
        }

        /// <summary>
        /// Processes the specified subscription identifier.
        /// </summary>
        /// <param name="subscriptionID">The subscription identifier.</param>
        public override void Process(Guid subscriptionID)
        {
            this.logger?.LogInformation("PendingActivationStatusHandler {0}", subscriptionID);
            var subscription = this.GetSubscriptionById(subscriptionID);
            this.logger?.LogInformation("subscription : {0}", JsonConvert.SerializeObject(subscription));
            
            this.logger?.LogInformation("Get User");
            var userdetails = this.GetUserById(subscription.UserId);

            if (subscription.SubscriptionStatus == SubscriptionStatusEnumExtension.PendingFulfillmentStart.ToString())
            {
                try
                {
                    this.subscriptionsRepository.UpdateStatusForSubscription(subscriptionID, SubscriptionStatusEnumExtension.PendingActivation.ToString(), true);

                    SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
                    {
                        Attribute = SubscriptionLogAttributes.Status.ToString(),
                        SubscriptionId = subscription.Id,
                        NewValue = SubscriptionStatusEnumExtension.PendingActivation.ToString(),
                        OldValue = subscription.SubscriptionStatus,
                        CreateBy = userdetails.UserId,
                        CreateDate = DateTime.Now
                    };
                    this.subscriptionLogRepository.Save(auditLog);
                }
                catch (Exception ex)
                {
                    string errorDescription = string.Format("Exception: {0} :: Innser Exception:{1}", ex.Message, ex.InnerException);

                    this.logger?.LogInformation(errorDescription);

                    this.subscriptionsRepository.UpdateStatusForSubscription(subscriptionID, SubscriptionStatusEnumExtension.PendingActivation.ToString(), true);

                    SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
                    {
                        Attribute = SubscriptionLogAttributes.Status.ToString(),
                        SubscriptionId = subscription.Id,
                        NewValue = SubscriptionStatusEnumExtension.PendingActivation.ToString(),
                        OldValue = subscription.SubscriptionStatus,
                        CreateBy = userdetails.UserId,
                        CreateDate = DateTime.Now
                    };
                    this.subscriptionLogRepository.Save(auditLog);
                }
            }
        }
    }
}