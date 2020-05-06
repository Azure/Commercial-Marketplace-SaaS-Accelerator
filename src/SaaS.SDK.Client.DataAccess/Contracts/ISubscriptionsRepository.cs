namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///  SubscriptionsRepository Interface
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    /// <seealso cref="Microsoft.Marketplace.SaasKit.DataAccess.Contracts.IBaseRepository{Microsoft.Marketplace.SaasKit.DataAccess.Entities.Subscriptions}" />
    public interface ISubscriptionsRepository : IDisposable, IBaseRepository<Subscriptions>
    {
        /// <summary>
        /// Gets the subscriptions by email address.
        /// </summary>
        /// <param name="partnerEmailAddress">The partner email address.</param>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="isIncludeDeactvated">if set to <c>true</c> [is include deactvated].</param>
        /// <returns></returns>
        IEnumerable<Subscriptions> GetSubscriptionsByEmailAddress(string partnerEmailAddress, Guid subscriptionId, bool isIncludeDeactvated = false);

        /// <summary>
        /// Gets the subscriptions by schedule identifier.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="isIncludeDeactvated">if set to <c>true</c> [is include deactvated].</param>
        /// <returns></returns>
        Subscriptions GetById(Guid subscriptionId, bool isIncludeDeactvated = false);

        /// <summary>
        /// Updates the status for subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="subscriptionStatus">The subscription status.</param>
        /// <param name="isActive">if set to <c>true</c> [is active].</param>
        void UpdateStatusForSubscription(Guid subscriptionId, string subscriptionStatus, bool isActive);

        /// <summary>
        /// Updates the plan for subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="planId">The plan identifier.</param>
        void UpdatePlanForSubscription(Guid subscriptionId, string planId);

        /// <summary>
        /// Updates the quantity for subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="quantity">The quantity identifier.</param>
        void UpdateQuantityForSubscription(Guid subscriptionId, int quantity);

        /// <summary>
        /// Saves the deployment credentials.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="keyVaultSecret">The key vault secret.</param>
        /// <param name="userId">The user identifier.</param>
        void SaveDeploymentCredentials(Guid subscriptionId, string keyVaultSecret,int userId);

        /// <summary>
        /// Gets the subscriptions parameters by identifier.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="planId">The plan identifier.</param>
        /// <returns>List of parameters related to the subscription</returns>
        List<SubscriptionParametersOutput> GetSubscriptionsParametersById(Guid subscriptionId, Guid planId);

        /// <summary>
        /// Adds the subscription parameters.
        /// </summary>
        /// <param name="subscriptionParametersOutput">The subscription parameters output.</param>
        void AddSubscriptionParameters(SubscriptionParametersOutput subscriptionParametersOutput);

        /// <summary>
        /// Gets the deployment configuration.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns></returns>
        SubscriptionKeyValut GetDeploymentConfig(Guid subscriptionId);
    }
}
