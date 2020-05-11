namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    using System.Collections.Generic;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

    /// <summary>
    /// ISubscriptionLicenses Repository.
    /// </summary>
    public interface ISubscriptionLicensesRepository
    {
        /// <summary>
        /// Gets all.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="subscriptionStatus">The subscription status.</param>
        /// <returns>
        /// return get all subscription licenses.
        /// </returns>
        IEnumerable<SubscriptionLicenses> GetSubscriptionLicensesByUser(int userId, string subscriptionStatus);

        /// <summary>
        /// Gets all subscription licenses.
        /// </summary>
        /// <param name="subscriptionStatus">The subscription status.</param>
        /// <returns>
        /// return get all subscription.
        /// </returns>
        IEnumerable<SubscriptionLicenses> GetLicensesForSubscriptions(string subscriptionStatus);

        /// <summary>
        /// Adds the update subscription licenses.
        /// </summary>
        /// <param name="subscription">The subscription.</param>
        /// <returns>
        /// return subscription Id.
        /// </returns>
        int AssignLicenseToSubscription(SubscriptionLicenses subscription);

        /// <summary>
        /// Updates the active subscription.
        /// </summary>
        /// <param name="subscription">The subscription.</param>
        /// <returns>
        /// return subscriptionId.
        /// </returns>
        int UpdateActiveSubscription(SubscriptionLicenses subscription);
    }
}
