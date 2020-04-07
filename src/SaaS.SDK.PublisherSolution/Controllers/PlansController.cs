namespace Microsoft.Marketplace.Saas.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Marketplace.Saas.Web.Models;
    using Microsoft.Marketplace.SaaS.SDK.PublisherSolution.Utilities;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using Microsoft.Marketplace.SaasKit.Client.Services;
    using Microsoft.Marketplace.SaasKit.Models;

    [ServiceFilter(typeof(KnownUser))]
    /// <summary>
    /// Licenses Controller
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.Saas.Web.Controllers.BaseController" />
    public class PlansController : BaseController
    {
        /// <summary>
        /// The subscription licenses repository
        /// </summary>
        private readonly ISubscriptionLicensesRepository subscriptionLicensesRepository;

        /// <summary>
        /// The subscription repository
        /// </summary>
        private readonly ISubscriptionsRepository subscriptionRepository;

        /// <summary>
        /// The users repository
        /// </summary>
        private readonly IUsersRepository usersRepository;

        private readonly IApplicationConfigRepository applicationConfigRepository;

        private readonly IPlansRepository plansRepository;

        private PlansService plansService;

        private IOffersRepository offerRepository;

        public IOfferAttributesRepository offerAttributeRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="LicensesController"/> class.
        /// </summary>
        /// <param name="subscriptionLicenses">The subscription licenses.</param>
        /// <param name="subscriptionRepository">The subscription repository.</param>
        /// <param name="usersRepository">The users repository.</param>
        public PlansController(ISubscriptionLicensesRepository subscriptionLicenses, ISubscriptionsRepository subscriptionRepository, IUsersRepository usersRepository, IApplicationConfigRepository applicationConfigRepository, IPlansRepository plansRepository, IOfferAttributesRepository offerAttributeRepository, IOffersRepository offerRepository)
        {
            this.subscriptionLicensesRepository = subscriptionLicenses;
            this.subscriptionRepository = subscriptionRepository;
            this.usersRepository = usersRepository;
            this.applicationConfigRepository = applicationConfigRepository;
            this.plansRepository = plansRepository;
            this.offerAttributeRepository = offerAttributeRepository;
            this.offerRepository = offerRepository;

            this.plansService = new PlansService(this.plansRepository, this.offerAttributeRepository, this.offerRepository);
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>return All subscription</returns>
        public IActionResult Index()
        {
            if (Convert.ToBoolean(applicationConfigRepository.GetValuefromApplicationConfig(MainMenuStatusEnum.IsLicenseManagementEnabled.ToString())) == true)
            {
                this.TempData["ShowPlansMenu"] = true;
            }
            List<PlansModel> getAllPlansData = new List<PlansModel>();
            this.TempData["ShowWelcomeScreen"] = "True";
            var currentUserDetail = usersRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);
            if (currentUserDetail != null)
            {
                getAllPlansData = this.plansService.GetPlans();
            }
            return this.View(getAllPlansData);
        }


        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>return All subscription</returns>
        public IActionResult PlanDetails(Guid planGuId)
        {
            //OffersViewModel OffersData = new OffersViewModel();
            PlansModel plans = new PlansModel();
            this.TempData["ShowWelcomeScreen"] = "True";
            var currentUserDetail = usersRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);
            plans = this.plansService.GetPlanDetailByPlanGuId(planGuId);
            return this.PartialView(plans);
        }

    }
}