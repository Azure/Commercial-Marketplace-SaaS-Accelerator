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
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

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
        public IArmTemplateRepository armTemplateRepository;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<OffersController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LicensesController"/> class.
        /// </summary>
        /// <param name="subscriptionLicenses">The subscription licenses.</param>
        /// <param name="subscriptionRepository">The subscription repository.</param>
        /// <param name="usersRepository">The users repository.</param>
        public PlansController(ISubscriptionLicensesRepository subscriptionLicenses, ISubscriptionsRepository subscriptionRepository, IUsersRepository usersRepository, IApplicationConfigRepository applicationConfigRepository, IPlansRepository plansRepository, IOfferAttributesRepository offerAttributeRepository, IOffersRepository offerRepository, ILogger<OffersController> logger, IArmTemplateRepository armTemplateRepository)
        {
            this.subscriptionLicensesRepository = subscriptionLicenses;
            this.subscriptionRepository = subscriptionRepository;
            this.usersRepository = usersRepository;
            this.applicationConfigRepository = applicationConfigRepository;
            this.plansRepository = plansRepository;
            this.offerAttributeRepository = offerAttributeRepository;
            this.offerRepository = offerRepository;
            this.logger = logger;
            this.armTemplateRepository = armTemplateRepository;
            this.plansService = new PlansService(this.plansRepository, this.offerAttributeRepository, this.offerRepository);
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>return All subscription</returns>
        public IActionResult Index()
        {
            this.logger.LogInformation("Plans Controller / OfferDetails:  offerGuId");
            try
            {
                if (Convert.ToBoolean(applicationConfigRepository.GetValuefromApplicationConfig(MainMenuStatusEnum.IsLicenseManagementEnabled.ToString())) == true)
                {
                    this.TempData["ShowPlansMenu"] = true;
                }
                List<PlansModel> getAllPlansData = new List<PlansModel>();
                this.TempData["ShowWelcomeScreen"] = "True";
                var currentUserDetail = usersRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);
                
                    getAllPlansData = this.plansService.GetPlans();
                
                return this.View(getAllPlansData);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                return View("Error", ex);
            }
        }


        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>return All subscription</returns>
        public IActionResult PlanDetails(Guid planGuId)
        {
            this.logger.LogInformation("Plans Controller / PlanDetails:  planGuId {0}", planGuId);
            try
            {
                //OffersViewModel OffersData = new OffersViewModel();
                PlansModel plans = new PlansModel();
                this.TempData["ShowWelcomeScreen"] = "True";
                var currentUserDetail = usersRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);
                plans = this.plansService.GetPlanDetailByPlanGuId(planGuId);
                var armTemplates = armTemplateRepository.Get().ToList();
                ViewBag.ARMTempleate = new SelectList(armTemplates, "ArmtempalteId", "ArmtempalteName");
                return this.PartialView(plans);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                return View("Error", ex);
            }
        }


        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>return All subscription</returns>
        [HttpPost]
        public IActionResult PlanDetails(PlansModel plans)
        {
            this.logger.LogInformation("Plans Controller / PlanDetails:  plans {0}", JsonConvert.SerializeObject(plans));
            try
            {
                var currentUserDetail = usersRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);
                if (plans != null)
                {
                    this.plansService.SavePlanDeploymentParameter(plans);
                    if (plans.PlanAttributes != null)
                    {
                        var inputAtttributes = plans.PlanAttributes.Where(s => s.Type.ToLower() == "input").ToList();
                        foreach (var attributes in inputAtttributes)
                        {
                            attributes.UserId = currentUserDetail.UserId;
                            this.plansService.SavePlanAttributes(attributes);
                        }
                    }
                    if (plans.PlanEvents != null)
                    {
                        foreach (var events in plans.PlanEvents)
                        {
                            events.UserId = currentUserDetail.UserId;
                            this.plansService.SavePlanEvents(events);
                        }
                    }
                }
                ModelState.Clear();
                //return RedirectToAction(nameof(Index));
                return RedirectToAction(nameof(PlanDetails), new { @planGuId = plans.PlanGUID });
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                return this.PartialView("Error", ex);
            }


        }

    }
}