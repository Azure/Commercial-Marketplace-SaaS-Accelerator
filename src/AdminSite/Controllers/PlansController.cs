using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Web;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.Services.Models;
using Marketplace.SaaS.Accelerator.Services.Services;
using Marketplace.SaaS.Accelerator.Services.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Marketplace.SaaS.Accelerator.AdminSite.Controllers;

/// <summary>
/// Plans Controller.
/// </summary>
/// <seealso cref="BaseController" />
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

    private readonly SaaSClientLogger<PlansController> logger;

    private PlanService plansService;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlansController" /> class.
    /// </summary>
    /// <param name="subscriptionRepository">The subscription repository.</param>
    /// <param name="usersRepository">The users repository.</param>
    /// <param name="applicationConfigRepository">The application configuration repository.</param>
    /// <param name="plansRepository">The plans repository.</param>
    /// <param name="offerAttributeRepository">The offer attribute repository.</param>
    /// <param name="offerRepository">The offer repository.</param>
    /// <param name="logger">The logger.</param>
    public PlansController(
        ISubscriptionsRepository subscriptionRepository, 
        IUsersRepository usersRepository, 
        IApplicationConfigRepository applicationConfigRepository, 
        IPlansRepository plansRepository, 
        IOfferAttributesRepository offerAttributeRepository, 
        IOffersRepository offerRepository,
        IAppVersionService appVersionService,
        SaaSClientLogger<PlansController> logger):base(applicationConfigRepository, appVersionService)
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
        this.logger.Info("Plans Controller / OfferDetails:  offerGuId");
        try
        {
            List<PlansModel> getAllPlansData = new List<PlansModel>();
            this.TempData["ShowWelcomeScreen"] = "True";
            var currentUserDetail = this.usersRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);


            getAllPlansData = this.plansService.GetPlans();

            return this.View(getAllPlansData);
        }
        catch (Exception ex)
        {
            this.logger.LogError($"Message:{ex.Message} :: {ex.InnerException}");
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
        this.logger.Info(HttpUtility.HtmlEncode($"Plans Controller / PlanDetails:  planGuId {planGuId}"));
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
            this.logger.LogError($"Message:{ex.Message} :: {ex.InnerException}");
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
    [ValidateAntiForgeryToken]
    public IActionResult PlanDetails(PlansModel plans)
    {
        this.logger.Info(HttpUtility.HtmlEncode($"Plans Controller / PlanDetails:  plans {JsonSerializer.Serialize(plans)}"));
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
            this.logger.LogError($"Message:{ex.Message} :: {ex.InnerException}");
            return this.PartialView("Error", ex);
        }
    }


}