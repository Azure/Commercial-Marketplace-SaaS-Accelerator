﻿namespace Microsoft.Marketplace.Saas.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using Microsoft.Marketplace.SaaS.SDK.Services.Services;
    using Microsoft.Marketplace.SaaS.SDK.Services.Utilities;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using Microsoft.AspNetCore.Mvc.Rendering;

    /// <summary>
    /// Plans Controller.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.Saas.Web.Controllers.BaseController" />
    [ServiceFilter(typeof(KnownUserAttribute))]
    public class SchedulerController : BaseController
    {
        /// <summary>
        /// The subscription repository.
        /// </summary>
        private readonly ISubscriptionsRepository subscriptionRepository;
        private readonly IPlansRepository plansRepository;
        private readonly IMeteredDimensionsRepository meteredRepository;
        private readonly ISchedulerFrequencyRepository frequencyRepository;
        private readonly IMeteredPlanSchedulerManagementRepository schedulerRepository;
        private readonly ISchedulerManagerViewRepository schedulerViewRepository;

        private readonly ILogger<OffersController> logger;

        private MeteredPlanSchedulerManagementService scheudelerService;
        private SubscriptionService subscriptionService;

        

        private readonly IUsersRepository usersRepository;
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
        public SchedulerController(ISubscriptionsRepository subscriptionRepository, IMeteredDimensionsRepository meteredRepository,
            ISchedulerFrequencyRepository frequencyRepository, IPlansRepository plansRepository,
            IMeteredPlanSchedulerManagementRepository schedulerRepository,ISchedulerManagerViewRepository schedulerViewRepository, IUsersRepository usersRepository, ILogger<OffersController> logger)
        {
            this.subscriptionRepository = subscriptionRepository;
            this.meteredRepository = meteredRepository;
            this.frequencyRepository = frequencyRepository;
            this.plansRepository = plansRepository;
            this.schedulerRepository = schedulerRepository;
            this.schedulerViewRepository= schedulerViewRepository;
            this.usersRepository= usersRepository;
            this.logger = logger;
            this.scheudelerService = new MeteredPlanSchedulerManagementService(this.frequencyRepository, this.schedulerRepository, this.schedulerViewRepository);
            this.subscriptionService = new SubscriptionService(this.subscriptionRepository,this.plansRepository);

        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>return All subscription.</returns>
        public IActionResult Index()
        {
            this.logger.LogInformation("Scheduler Controller / Get all data");
            try
            {
                List<SchedulerManagerViewModel> getAllSchedulerManagerViewData = new List<SchedulerManagerViewModel>();
                this.TempData["ShowWelcomeScreen"] = "True";
                var currentUserDetail = this.usersRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);

                getAllSchedulerManagerViewData = this.scheudelerService.GetAllSchedulerManagerList();
                return this.View(getAllSchedulerManagerViewData);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                return this.View("Error", ex);
            }
        }

        public IActionResult NewScheduler()
        {
            this.logger.LogInformation("New Scheduler Controller");
            try
            {
                SchedulerUsageViewModel schedulerUsageViewModel = new SchedulerUsageViewModel();
                
                this.TempData["ShowWelcomeScreen"] = "True";
                var currentUserDetail = this.usersRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);
                var allActiveMeteredSubscriptions = this.subscriptionService.GetActiveSubscriptionsWithMeteredPlan();
                List<SchedulerFrequency> getAllFrequency = frequencyRepository.GetAll().ToList();

                // Create Frequency Dropdown list
                List<SelectListItem> SchedulerFrequencyList = new List<SelectListItem>();
                foreach (var item in getAllFrequency)
                {
                    SchedulerFrequencyList.Add(new SelectListItem()
                    {
                        Text=item.Frequency,
                        Value=item.Id.ToString(),
                    });
                }

                // Create Subscription Dropdown list
                List<SelectListItem> SubscriptionList = new List<SelectListItem>();
                foreach (var item in allActiveMeteredSubscriptions)
                {
                    SubscriptionList.Add(new SelectListItem()
                    {
                        Text = item.Name,
                        Value = item.Id.ToString(),
                    });
                }


                // Create Plan Dropdown list
                List<SelectListItem> PlanList = new List<SelectListItem>();
                List<SelectListItem> DimensionsList = new List<SelectListItem>();

                schedulerUsageViewModel.DimensionsList = new SelectList(DimensionsList, "Value", "Text");
                schedulerUsageViewModel.SubscriptionList = new SelectList(SubscriptionList, "Value", "Text");
                schedulerUsageViewModel.SchedulerFrequencyList = new SelectList(SchedulerFrequencyList, "Value", "Text");

                return this.View(schedulerUsageViewModel);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                return this.View("Error", ex);
            }
        }

        public IActionResult GetSubscriptionData(int id)
        {
            var allSubscriptionDetails = this.subscriptionRepository.Get().ToList(); 
            //Get Subscription Plan
            var selectSubscription = allSubscriptionDetails.Where(s => s.Id == id).FirstOrDefault();
            if (selectSubscription != null)
            {
                string planId = selectSubscription.AmpplanId;

                List<Plans> getAllPlans = this.plansRepository.Get().ToList();
                var selectedPlan = getAllPlans.Where(s => s.PlanId == planId).FirstOrDefault();
                if (selectedPlan != null)
                {
                    List<SelectListItem> PlanList = new List<SelectListItem>();
                    PlanList.Add(new SelectListItem()
                    {
                        Text = planId,
                        Value = selectedPlan.Id.ToString(),
                    });


                    // Create Dimension Dropdown list
                    List<MeteredDimensions> getAllDimensions = this.meteredRepository.GetDimensionsByPlanId(planId);
                    if (getAllDimensions != null)
                    {
                        List<SelectListItem> selectedList = new List<SelectListItem>();
                        foreach (var item in getAllDimensions)
                        {
                            selectedList.Add(new SelectListItem()
                            {
                                Text = item.Dimension,
                                Value = item.Id.ToString(),
                            });
                        }

                        return Json(selectedList);
  
                    }
                    return this.PartialView("Error", "Can not find any metered dimension related to selected plan");

                }
                return this.PartialView("Error", "Can not find any plan related to selected subscription");
            }
            return this.PartialView("Error", "Subscription is Invalid");
        }


        public IActionResult AddNewScheduledTrigger(SchedulerUsageViewModel schedulerUsageViewModel)
        {
            try
            {
                var selectedDimension = this.meteredRepository.Get(int.Parse(schedulerUsageViewModel.SelectedDimension));
                MeteredPlanSchedulerManagement schedulerManagement = new MeteredPlanSchedulerManagement()
                {
                    FrequencyId = Convert.ToInt32(schedulerUsageViewModel.SelectedSchedulerFrequency),
                    SubscriptionId = Convert.ToInt32(schedulerUsageViewModel.SelectedSubscription),
                    PlanId = Convert.ToInt32(selectedDimension.PlanId),
                    DimensionId = Convert.ToInt32(schedulerUsageViewModel.SelectedDimension),
                    Quantity = Convert.ToDouble(schedulerUsageViewModel.Quantity),
                    StartDate = schedulerUsageViewModel.FirstRunDate
                };
                this.schedulerRepository.Save(schedulerManagement);
                return this.RedirectToAction(nameof(this.Index));

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
        /// <param id="schedule Id">The plan gu identifier.</param>
        /// <returns>
        /// return All subscription.

        public IActionResult DeleteSchedulerItem(string id)
        {
            this.logger.LogInformation("Scheduler Controller / Remove Schedule Item Details:  Id {0}", JsonSerializer.Serialize(id));
            try
            {
                this.scheudelerService.DeleteSchedulerDetailById(int.Parse(id));
                return this.RedirectToAction(nameof(this.Index));
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                return this.PartialView("Error", ex);
            }
        }



    }
}