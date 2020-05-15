namespace Microsoft.Marketplace.Saas.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Logging;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using Microsoft.Marketplace.SaaS.SDK.Services.Services;
    using Microsoft.Marketplace.SaaS.SDK.Services.Utilities;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using System.Text.Json;

    /// <summary>
    /// Licenses Controller.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.Saas.Web.Controllers.BaseController" />
    [ServiceFilter(typeof(KnownUserAttribute))]
    public class PlansController : BaseController
    {

        /// <summary>
        /// The subscription repository.
        /// </summary>
        private readonly ISubscriptionsRepository subscriptionRepository;

        /// <summary>
        /// The users repository.
        /// </summary>
        private readonly IUsersRepository usersRepository;

        private readonly IApplicationConfigRepository applicationConfigRepository;

        private readonly IPlansRepository plansRepository;

        private readonly IOffersRepository offerRepository;

        private readonly IOfferAttributesRepository offerAttributeRepository;

        private readonly ILogger<OffersController> logger;

        private PlanService plansService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlansController"/> class.
        /// </summary>
        /// <param name="subscriptionLicenses">The subscription licenses.</param>
        /// <param name="subscriptionRepository">The subscription repository.</param>
        /// <param name="usersRepository">The users repository.</param>
        /// <param name="applicationConfigRepository">The application configuration repository.</param>
        /// <param name="plansRepository">The plans repository.</param>
        /// <param name="offerAttributeRepository">The offer attribute repository.</param>
        /// <param name="offerRepository">The offer repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="armTemplateRepository">The arm template repository.</param>
        public PlansController(ISubscriptionsRepository subscriptionRepository, IUsersRepository usersRepository, IApplicationConfigRepository applicationConfigRepository, IPlansRepository plansRepository, IOfferAttributesRepository offerAttributeRepository, IOffersRepository offerRepository, ILogger<OffersController> logger)
        {
            this.subscriptionRepository = subscriptionRepository;
            this.usersRepository = usersRepository;
            this.applicationConfigRepository = applicationConfigRepository;
            this.plansRepository = plansRepository;
            this.offerAttributeRepository = offerAttributeRepository;
            this.offerRepository = offerRepository;
            this.logger = logger;
            this.plansService = new PlanService(this.plansRepository, this.offerAttributeRepository, this.offerRepository);
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>return All subscription.</returns>
        public IActionResult Index()
        {
            this.logger.LogInformation("Plans Controller / OfferDetails:  offerGuId");
            try
            {
                if (Convert.ToBoolean(this.applicationConfigRepository.GetValueByName(MainMenuStatusEnum.IsLicenseManagementEnabled.ToString())) == true)
                {
                    this.TempData["ShowPlansMenu"] = true;
                }

                List<PlansModel> getAllPlansData = new List<PlansModel>();
                this.TempData["ShowWelcomeScreen"] = "True";
                var currentUserDetail = this.usersRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);

                getAllPlansData = this.plansService.GetPlans();

                return this.View(getAllPlansData);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                return this.View("Error", ex);
            }
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <param name="planGuId">The plan gu identifier.</param>
        /// <returns>
        /// return All subscription.
        /// </returns>
        public IActionResult PlanDetails(Guid planGuId)
        {
            this.logger.LogInformation("Plans Controller / PlanDetails:  planGuId {0}", planGuId);
            try
            {
                PlansModel plans = new PlansModel();
                this.TempData["ShowWelcomeScreen"] = "True";
                var currentUserDetail = this.usersRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);
                plans = this.plansService.GetPlanDetailByPlanGuId(planGuId);
                return this.PartialView(plans);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                return this.View("Error", ex);
            }
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <param name="plans">The plans.</param>
        /// <returns>
        /// return All subscription.
        /// </returns>
        [HttpPost]
        public IActionResult PlanDetails(PlansModel plans)
        {
            this.logger.LogInformation("Plans Controller / PlanDetails:  plans {0}",  JsonSerializer.Serialize(plans));
            try
            {
                var currentUserDetail = this.usersRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);
                if (plans != null)
                {
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

                this.ModelState.Clear();
                return this.RedirectToAction(nameof(this.PlanDetails), new { @planGuId = plans.PlanGUID });
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                return this.PartialView("Error", ex);
            }
        }
    }
}