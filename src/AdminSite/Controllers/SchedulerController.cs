using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Web;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.Services.Models;
using Marketplace.SaaS.Accelerator.Services.Services;
using Marketplace.SaaS.Accelerator.Services.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace Marketplace.SaaS.Accelerator.AdminSite.Controllers;

/// <summary>
/// Scheduler Controller.
/// </summary>
/// <seealso cref="BaseController" />
[ServiceFilter(typeof(KnownUserAttribute))]
public class SchedulerController : BaseController
{
    /// <summary>
    /// The dimension repository
    /// </summary>
    private readonly IMeteredDimensionsRepository meteredRepository;

    /// <summary>
    /// logger controller
    /// </summary>
    private readonly SaaSClientLogger<SchedulerController> logger;

    /// <summary>
    /// the shceduler service
    /// </summary>
    private MeteredPlanSchedulerManagementService schedulerService;

    /// <summary>
    /// the subscription service
    /// </summary>
    private SubscriptionService subscriptionService;

    /// <summary>
    /// the user repository
    /// </summary>
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
    public SchedulerController(
        ISubscriptionsRepository subscriptionRepository,
        IMeteredDimensionsRepository meteredRepository,
        ISchedulerFrequencyRepository frequencyRepository,
        IPlansRepository plansRepository,
        IMeteredPlanSchedulerManagementRepository schedulerRepository,
        ISchedulerManagerViewRepository schedulerViewRepository, 
        IUsersRepository usersRepository,
        SaaSClientLogger<SchedulerController> logger,
        IAppVersionService appVersionService,
        ISubscriptionUsageLogsRepository subscriptionUsageLogsRepository,IApplicationConfigRepository applicationConfigRepository):base(applicationConfigRepository, appVersionService)

    {
        this.usersRepository = usersRepository;
        this.logger = logger;
        this.meteredRepository = meteredRepository;
        this.schedulerService = new MeteredPlanSchedulerManagementService(frequencyRepository, schedulerRepository, schedulerViewRepository,subscriptionUsageLogsRepository,applicationConfigRepository);
        this.subscriptionService = new SubscriptionService(subscriptionRepository,plansRepository);

    }

    /// <summary>
    /// Indexes this instance.
    /// </summary>
    /// <returns>return All subscription.</returns>
    public IActionResult Index()
    {
        this.logger.Info("Scheduler Controller / Get all data");
        try
        {
            List<SchedulerManagerViewModel> getAllSchedulerManagerViewData = new List<SchedulerManagerViewModel>();
            this.TempData["ShowWelcomeScreen"] = "True";
            var currentUserDetail = this.usersRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);

            getAllSchedulerManagerViewData = this.schedulerService.GetAllSchedulerManagerList();

            return this.View(getAllSchedulerManagerViewData);
        }
        catch (Exception ex)
        {
            this.logger.LogError($"Message:{ex.Message} :: {ex.InnerException}");
            return this.View("Error", ex);
        }
    }

    public IActionResult NewScheduler(string subscriptionId, string dimId, string quantity)
    {
        this.logger.Info("New Scheduler Controller");
        try
        {
            SchedulerUsageViewModel schedulerUsageViewModel = new SchedulerUsageViewModel();

            this.TempData["ShowWelcomeScreen"] = "True";
            var currentUserDetail = this.usersRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);
            var allActiveMeteredSubscriptions = this.subscriptionService.GetActiveSubscriptionsWithMeteredPlan();
            List<SchedulerFrequencyModel> getAllEnabledFrequency = this.schedulerService.GetAllEnabledFrequency();

            // Create Frequency Dropdown list
            List<SelectListItem> SchedulerFrequencyList = new List<SelectListItem>();
            foreach (var item in getAllEnabledFrequency)
            {
                SchedulerFrequencyList.Add(new SelectListItem()
                {
                    Text = item.Frequency,
                    Value = item.Id.ToString(),
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


            schedulerUsageViewModel.SelectedSubscription = subscriptionId;
            schedulerUsageViewModel.Quantity = (Int32)Math.Round(Convert.ToDecimal(quantity), 0);

            if (!String.IsNullOrEmpty(dimId))
            {
                var dimensions = this.meteredRepository.Get().Where(d => d.Dimension == dimId).FirstOrDefault();
                if (dimensions != null)
                {
                    DimensionsList.Add(new SelectListItem()
                    {
                        Text = dimId,
                        Value = dimensions.Id.ToString(),
                        Selected = true
                    });
                    schedulerUsageViewModel.DimensionsList = new SelectList(DimensionsList, "Value", "Text");
                    schedulerUsageViewModel.SelectedDimension = dimensions.Id.ToString();
                }
            }
            else
            {
                if (subscriptionId != null)
                {
                    var selectSubscription = allActiveMeteredSubscriptions.Where(s => s.Id == int.Parse(subscriptionId)).FirstOrDefault();
                    if (selectSubscription != null)
                    {
                        // Create Dimension Dropdown list
                        List<MeteredDimensions> getAllDimensions = this.meteredRepository.GetDimensionsByPlanId(selectSubscription.AmpplanId);
                        if (getAllDimensions != null)
                        {

                            foreach (var item in getAllDimensions)
                            {
                                DimensionsList.Add(new SelectListItem()
                                {
                                    Text = item.Dimension,
                                    Value = item.Id.ToString(),
                                });
                            }
                        }
                    }
                }

            }
            return this.View(schedulerUsageViewModel);
        }
        catch (Exception ex)
        {
            this.logger.LogError($"Message:{ex.Message} :: {ex.InnerException}");
            return this.View("Error", ex);
        }
    }

    public IActionResult GetSubscriptionData(int id)
    {
        var allSubscriptionDetails = this.subscriptionService.GetActiveSubscriptionsWithMeteredPlan();
        var selectSubscription = allSubscriptionDetails.Where(s => s.Id == id).FirstOrDefault();
        if (selectSubscription != null)
        {
            // Create Dimension Dropdown list
            List<MeteredDimensions> getAllDimensions = this.meteredRepository.GetDimensionsByPlanId(selectSubscription.AmpplanId);
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
        return this.PartialView("Error", "Subscription is Invalid");
    }

    public IActionResult AddNewScheduledTrigger(SchedulerUsageViewModel schedulerUsageViewModel)
    {
        try
        {
            var selectedDimension = this.meteredRepository.Get(int.Parse(schedulerUsageViewModel.SelectedDimension));
            MeteredPlanSchedulerManagementModel schedulerManagement = new MeteredPlanSchedulerManagementModel()
            {
                FrequencyId = Convert.ToInt32(schedulerUsageViewModel.SelectedSchedulerFrequency),
                SchedulerName = Convert.ToString(schedulerUsageViewModel.SchedulerName),
                SubscriptionId = Convert.ToInt32(schedulerUsageViewModel.SelectedSubscription),
                PlanId = Convert.ToInt32(selectedDimension.PlanId),
                DimensionId = Convert.ToInt32(schedulerUsageViewModel.SelectedDimension),
                Quantity = Convert.ToDouble(schedulerUsageViewModel.Quantity),
                StartDate = schedulerUsageViewModel.FirstRunDate.AddHours(schedulerUsageViewModel.TimezoneOffset)
            };
            this.schedulerService.SaveSchedulerDetail(schedulerManagement);
            return this.RedirectToAction(nameof(this.Index));

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
    /// <param id="schedule Id">The plan gu identifier.</param>
    /// <returns>
    /// return All subscription.

    public IActionResult DeleteSchedulerItem(string id)
    {
        this.logger.Info(HttpUtility.HtmlEncode($"Scheduler Controller / Remove Schedule Item Details:  Id {id}"));
        try
        {
            this.schedulerService.DeleteSchedulerDetailById(int.Parse(id));
            return this.RedirectToAction(nameof(this.Index));
        }
        catch (Exception ex)
        {
            this.logger.LogError($"Message:{ex.Message} :: {ex.InnerException}");
            return this.PartialView("Error", ex);
        }
    }

    public IActionResult SchedulerLogDetail(string id)
    {
        this.logger.Info(HttpUtility.HtmlEncode($"Scheduler Controller / Get Schedule Item Details:  Id {id}"));
        try
        {
            var SchedulerItem = this.schedulerService.GetSchedulerManagerById(int.Parse(id));
            var detail = this.schedulerService.GetSchedulerItemRunHistory(int.Parse(id));
            SchedulerUsageViewModel schedulerUsageViewModel = new SchedulerUsageViewModel();
            schedulerUsageViewModel.SelectedSubscription = SchedulerItem.AMPSubscriptionId.ToString();
            schedulerUsageViewModel.SelectedDimension = SchedulerItem.Dimension;
            schedulerUsageViewModel.SelectedSchedulerFrequency = SchedulerItem.Frequency;
            schedulerUsageViewModel.Quantity = Convert.ToInt32(SchedulerItem.Quantity);
            schedulerUsageViewModel.FirstRunDate = SchedulerItem.StartDate;
            if (SchedulerItem.NextRunTime.HasValue)
            {
                schedulerUsageViewModel.NextRunDate = SchedulerItem.NextRunTime.Value;
            }

            schedulerUsageViewModel.MeteredAuditLogs = detail;

            return this.View(schedulerUsageViewModel);



        }
        catch (Exception ex)
        {
            this.logger.LogError($"Message:{ex.Message} :: {ex.InnerException}");
            return this.PartialView("Error", ex);
        }
    }



}