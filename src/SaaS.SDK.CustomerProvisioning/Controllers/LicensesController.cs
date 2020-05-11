namespace Microsoft.Marketplace.SaasKit.Client.Controllers
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using Microsoft.Marketplace.SaaS.SDK.Services.Services;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;

    /// <summary>
    /// Licenses Controller.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Client.Controllers.BaseController" />
    public class LicensesController : BaseController
    {
        /// <summary>
        /// The subscription licenses repository.
        /// </summary>
        private readonly ISubscriptionLicensesRepository subscriptionLicensesRepository;

        /// <summary>
        /// The subscriptions repository.
        /// </summary>
        private readonly ISubscriptionsRepository subscriptionsRepository;

        /// <summary>
        /// The users repository.
        /// </summary>
        private readonly IUsersRepository usersRepository;

        private readonly IApplicationConfigRepository applicationConfigRepository;

        /// <summary>
        /// The subscription licenses service.
        /// </summary>
        private SubscriptionLicensesService subscriptionLicensesService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LicensesController" /> class.
        /// </summary>
        /// <param name="subscriptionLicensesRepository">The subscription licenses repository.</param>
        /// <param name="subscriptionsRepository">The subscriptions repository.</param>
        /// <param name="usersRepository">The users repository.</param>
        /// <param name="applicationConfigRepository">The application configuration repository.</param>
        public LicensesController(ISubscriptionLicensesRepository subscriptionLicensesRepository, ISubscriptionsRepository subscriptionsRepository, IUsersRepository usersRepository, IApplicationConfigRepository applicationConfigRepository)
        {
            this.subscriptionLicensesRepository = subscriptionLicensesRepository;
            this.subscriptionsRepository = subscriptionsRepository;
            this.usersRepository = usersRepository;
            this.applicationConfigRepository = applicationConfigRepository;
            this.subscriptionLicensesService = new SubscriptionLicensesService(this.subscriptionLicensesRepository, this.subscriptionsRepository);
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>
        /// return get all subscription.
        /// </returns>
        public IActionResult Index()
        {
            if (Convert.ToBoolean(this.applicationConfigRepository.GetValueByName(MainMenuStatusEnum.IsLicenseManagementEnabled.ToString())) == true)
            {
                this.TempData["ShowLicensesMenu"] = true;
            }

            List<SubscriptionLicensesViewModel> getAllSubScriptionData = new List<SubscriptionLicensesViewModel>();
            this.TempData["ShowWelcomeScreen"] = "True";
            var currentUserDetail = this.usersRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);
            if (currentUserDetail != null)
            {
                getAllSubScriptionData = this.subscriptionLicensesService.GetSubScriptionLicensesbyUser(currentUserDetail.UserId);
            }

            return this.View(getAllSubScriptionData);
        }
    }
}