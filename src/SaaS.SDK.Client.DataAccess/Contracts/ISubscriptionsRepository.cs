using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System;
using System.Collections.Generic;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
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
        Subscriptions GetSubscriptionsByScheduleId(Guid subscriptionId, bool isIncludeDeactvated = false);

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

        //Subscriptions GetSubscriptionsBySubscriptionId(Guid subscriptionId, bool isIncludeDeactvated = false);

    }
}
