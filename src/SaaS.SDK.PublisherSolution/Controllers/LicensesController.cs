namespace Microsoft.Marketplace.Saas.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using Microsoft.Marketplace.SaaS.SDK.Services.Utilities;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using Microsoft.Marketplace.SaasKit.Models;

    [ServiceFilter(typeof(KnownUserAttribute))]

    /// <summary>
    /// The Licneses controller.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.Saas.Web.Controllers.BaseController" />
    public class LicensesController : BaseController
    {
        /// <summary>
        /// The subscription licenses repository.
        /// </summary>
        private readonly ISubscriptionLicensesRepository subscriptionLicensesRepository;

        /// <summary>
        /// The subscription repository.
        /// </summary>
        private readonly ISubscriptionsRepository subscriptionRepository;

        /// <summary>
        /// The users repository.
        /// </summary>
        private readonly IUsersRepository usersRepository;

        private readonly IApplicationConfigRepository applicationConfigRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="LicensesController" /> class.
        /// </summary>
        /// <param name="subscriptionLicenses">The subscription licenses.</param>
        /// <param name="subscriptionRepository">The subscription repository.</param>
        /// <param name="usersRepository">The users repository.</param>
        /// <param name="applicationConfigRepository">The application configuration repository.</param>
        public LicensesController(ISubscriptionLicensesRepository subscriptionLicenses, ISubscriptionsRepository subscriptionRepository, IUsersRepository usersRepository, IApplicationConfigRepository applicationConfigRepository)
        {
            this.subscriptionLicensesRepository = subscriptionLicenses;
            this.subscriptionRepository = subscriptionRepository;
            this.usersRepository = usersRepository;
            this.applicationConfigRepository = applicationConfigRepository;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns> Return All Licenses.</returns>
        public IActionResult Index()
        {
            if (Convert.ToBoolean(this.applicationConfigRepository.GetValueByName(MainMenuStatusEnum.IsLicenseManagementEnabled.ToString())) == true)
            {
                this.TempData["ShowLicensesMenu"] = true;
            }

            SubscriptionLicensesModel subscriptionLicenses = new SubscriptionLicensesModel();
            subscriptionLicenses.SubscriptionList = new SelectList(this.subscriptionRepository.Get().Where(s => s.SubscriptionStatus == Convert.ToString(SubscriptionStatusEnum.Subscribed)), "Id", "Name");
            List<SubscriptionLicensesViewModel> subscriptionlist = new List<SubscriptionLicensesViewModel>();
            var getsubscriptionList = this.subscriptionLicensesRepository.GetLicensesForSubscriptions(Convert.ToString(SubscriptionStatusEnum.Subscribed));
            foreach (var item in getsubscriptionList)
            {
                SubscriptionLicensesViewModel subscription = new SubscriptionLicensesViewModel();
                subscription.Id = item.Id;
                subscription.AmpsubscriptionId = Convert.ToString(item.Subscription.AmpsubscriptionId);
                subscription.LicenseKey = item.LicenseKey;
                subscription.PlanName = item.Subscription?.AmpplanId;
                subscription.SubscriptionName = item.Subscription?.Name;
                subscription.Status = item.IsActive.GetValueOrDefault();
                subscription.SubScriptionID = item.SubscriptionId ?? 0;
                subscriptionlist.Add(subscription);
            }

            subscriptionLicenses.Licenses = subscriptionlist;
            return this.View(subscriptionLicenses);
        }

        /// <summary>
        /// Updates the active Subscription.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns>
        /// return Active Subscription.
        /// </returns>
        public JsonResult UpdateActiveSubscription(int id, int subscriptionId)
        {
            if (Convert.ToBoolean(this.applicationConfigRepository.GetValueByName(MainMenuStatusEnum.IsLicenseManagementEnabled.ToString())) == true)
            {
                this.TempData["ShowLicensesMenu"] = true;
            }

            var subscriptionDetails = this.subscriptionLicensesRepository.GetLicensesForSubscriptions(Convert.ToString(SubscriptionStatusEnum.Subscribed))
                .Where(s => s.SubscriptionId == subscriptionId).ToList();

            var requestedUpdateSubscription = subscriptionDetails.Where(s => s.Id == id).FirstOrDefault();

            if (requestedUpdateSubscription != null && !subscriptionDetails.Any(s => s.Id != id && s.IsActive == true))
            {
                this.subscriptionLicensesRepository.UpdateActiveSubscription(requestedUpdateSubscription);
            }
            else
            {
                if (requestedUpdateSubscription != null && requestedUpdateSubscription.Subscription != null)
                {
                    return new JsonResult("There is already a license associated with the subscription " + requestedUpdateSubscription.Subscription.Name);
                }
            }

            return new JsonResult(0);
        }

        /// <summary>
        /// Adds the subscription license.
        /// </summary>
        /// <returns>return subscription list.</returns>
        public PartialViewResult AddSubscriptionLicense()
        {
            SubscriptionLicensesViewModel subscription = new SubscriptionLicensesViewModel();
            subscription.SubscriptionList = new SelectList(this.subscriptionRepository.Get().Where(s => s.SubscriptionStatus == Convert.ToString(SubscriptionStatusEnum.Subscribed)), "Id", "Name");
            return this.PartialView(subscription);
        }

        /// <summary>
        /// Adds the License detail.
        /// </summary>
        /// <param name="subscriptionLicenses">The subscription licenses.</param>
        /// <returns>return subscription.</returns>
        [HttpPost]
        public IActionResult AddLicenseDetail(SubscriptionLicensesViewModel subscriptionLicenses)
        {
            if (Convert.ToBoolean(this.applicationConfigRepository.GetValueByName(MainMenuStatusEnum.IsLicenseManagementEnabled.ToString())) == true)
            {
                this.TempData["ShowLicensesMenu"] = true;
            }

            if (subscriptionLicenses != null)
            {
                var currentUserDetail = this.usersRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);
                var subscriptionLicense = new SubscriptionLicenses()
                {
                    LicenseKey = subscriptionLicenses.LicenseKey,
                    SubscriptionId = subscriptionLicenses.SubScriptionID,
                    IsActive = true,
                    CreatedDate = DateTime.Now,
                    CreatedBy = currentUserDetail == null ? 0 : currentUserDetail.UserId,
                };

                var getsubscriptionDetails = this.subscriptionLicensesRepository.GetLicensesForSubscriptions(Convert.ToString(SubscriptionStatusEnum.Subscribed))
               .Where(s => s.SubscriptionId == subscriptionLicenses.SubScriptionID).ToList();

                if (getsubscriptionDetails.Any(s => s.IsActive == true))
                {
                    if (getsubscriptionDetails.FirstOrDefault() != null && getsubscriptionDetails.FirstOrDefault().Subscription != null)
                    {
                        this.TempData["msg"] = "<script>alert('There is already a license associated with the subscription" + getsubscriptionDetails.FirstOrDefault().Subscription.Name + "');</script>";
                        return this.RedirectToAction(nameof(this.Index));
                    }
                }

                this.subscriptionLicensesRepository.AssignLicenseToSubscription(subscriptionLicense);
            }

            return this.RedirectToAction(nameof(this.Index));
        }
    }
}