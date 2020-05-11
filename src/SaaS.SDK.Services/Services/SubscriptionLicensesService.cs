namespace Microsoft.Marketplace.SaaS.SDK.Services.Services
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Models;

    /// <summary>
    /// SubscriptionLicenses Service.
    /// </summary>
    public class SubscriptionLicensesService
    {
        /// <summary>
        /// The subscription licenses repository.
        /// </summary>
        private ISubscriptionLicensesRepository subscriptionLicensesRepository;

        /// <summary>
        /// The subscriptions repository.
        /// </summary>
        private ISubscriptionsRepository subscriptionsRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionLicensesService" /> class.
        /// </summary>
        /// <param name="subscriptionLicensesRepository">The subscription licenses repository.</param>
        /// <param name="subscriptionsRepository">The subscriptions repository.</param>
        public SubscriptionLicensesService(ISubscriptionLicensesRepository subscriptionLicensesRepository, ISubscriptionsRepository subscriptionsRepository)
        {
            this.subscriptionLicensesRepository = subscriptionLicensesRepository;
            this.subscriptionsRepository = subscriptionsRepository;
        }

        /// <summary>
        /// Gets subscription licenses by user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>
        /// return subscription licenses by user.
        /// </returns>
        public List<SubscriptionLicensesViewModel> GetSubScriptionLicensesbyUser(int userId)
        {
            List<SubscriptionLicensesViewModel> subscriptionLicensesList = new List<SubscriptionLicensesViewModel>();
            var allsubscriptionData = this.subscriptionLicensesRepository.GetSubscriptionLicensesByUser(userId, Convert.ToString(SubscriptionStatusEnum.Subscribed));
            foreach (var item in allsubscriptionData)
            {
                SubscriptionLicensesViewModel subscription = new SubscriptionLicensesViewModel();
                subscription.AmpsubscriptionId = Convert.ToString(item.Subscription.AmpsubscriptionId);
                subscription.SubscriptionName = item.Subscription.Name;
                subscription.PlanName = item.Subscription.AmpplanId;
                subscription.LicenseKey = item.LicenseKey;
                subscriptionLicensesList.Add(subscription);
            }

            return subscriptionLicensesList;
        }
    }
}
