namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

    /// <summary>
    /// SubscriptionLicenses Repository.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.ISubscriptionLicensesRepository" />
    public class SubscriptionLicensesRepository : ISubscriptionLicensesRepository
    {
        /// <summary>
        /// The context.
        /// </summary>
        private readonly SaasKitContext context;

        /// <summary>
        /// The disposed.
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionLicensesRepository" /> class.
        /// </summary>
        /// <param name="context">The this.context.</param>
        public SubscriptionLicensesRepository(SaasKitContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="subscriptionStatus">The subscription status.</param>
        /// <returns>
        /// return all subscription licenses.
        /// </returns>
        public IEnumerable<SubscriptionLicenses> GetSubscriptionLicensesByUser(int userId, string subscriptionStatus)
        {
            var getAllSubscriptionlicenses = this.context.SubscriptionLicenses.Include(s => s.Subscription).Where(s => s.Subscription.UserId == userId
                                                && s.Subscription.SubscriptionStatus == subscriptionStatus && s.IsActive == true);
            return getAllSubscriptionlicenses;
        }

        /// <summary>
        /// Gets all subscription licenses.
        /// </summary>
        /// <param name="subscriptionStatus"> Status of subscription.</param>
        /// <returns>
        /// return get all subscription.
        /// </returns>
        public IEnumerable<SubscriptionLicenses> GetLicensesForSubscriptions(string subscriptionStatus)
        {
            var getAllSubscriptionlicenses = this.context.SubscriptionLicenses.Include(s => s.Subscription).Where(s => s.Subscription.SubscriptionStatus == subscriptionStatus);
            return getAllSubscriptionlicenses;
        }

        /// <summary>
        /// Adds the update subscription licenses.
        /// </summary>
        /// <param name="subscription">The subscription.</param>
        /// <returns>
        /// return subscription Id.
        /// </returns>
        public int AssignLicenseToSubscription(SubscriptionLicenses subscription)
        {
            var existingsubscriptionActive = this.context.SubscriptionLicenses.Where(s => s.SubscriptionId == subscription.SubscriptionId && s.IsActive == true).FirstOrDefault();
            if (existingsubscriptionActive == null)
            {
                var existingsubscription = this.context.SubscriptionLicenses.Where(s => s.Id == subscription.Id).FirstOrDefault();
                if (existingsubscription == null)
                {
                    this.context.SubscriptionLicenses.Add(subscription);
                    this.context.SaveChanges();
                }
            }

            return subscription.Id;
        }

        /// <summary>
        /// Updates the active subscription.
        /// </summary>
        /// <param name="subscription">The subscription.</param>
        /// <returns>
        /// return subscriptionId.
        /// </returns>
        public int UpdateActiveSubscription(SubscriptionLicenses subscription)
        {
            var existingsubscription = this.context.SubscriptionLicenses.Where(s => s.Id == subscription.Id).FirstOrDefault();
            if (existingsubscription != null)
            {
                existingsubscription.IsActive = !existingsubscription.IsActive;
                this.context.SubscriptionLicenses.Update(existingsubscription);
                this.context.SaveChanges();
            }

            return subscription.Id;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.context.Dispose();
                }
            }

            this.disposed = true;
        }
    }
}
