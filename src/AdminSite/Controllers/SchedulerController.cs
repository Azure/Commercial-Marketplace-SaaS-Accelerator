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
    /// the scheduler service
    /// </summary>
    private readonly MeteredPlanSchedulerManagementService schedulerService;

    /// <summary>
    /// the subscription service
    /// </summary>
    private readonly SubscriptionService subscriptionService;

    /// <summary>
    /// the user repository
    /// </summary>
    private readonly IUsersRepository usersRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlansController" /> class.
    /// </summary>
    /// <param name="subscriptionRepository">The subscription repository.</param>
    /// <param name="meteredRepository">The metered dimension repository.</param>
    /// <param name="frequencyRepository">The scheduler frequency repository.</param>
    /// <param name="usersRepository">The users repository.</param>
    /// <param name="applicationConfigRepository">The application configuration repository.</param>
    /// <param name="plansRepository">The plans repository.</param>
    /// <param name="schedulerRepository">The metered plan scheduler management repository.</param>
    /// <param name="schedulerViewRepository">The scheduler manager view repository.</param>
    /// <param name="subscriptionUsageLogsRepository">The subscription usage logs repository.</param>
    /// <param name="logger">The logger.</param>
    public SchedulerController(ISubscriptionsRepository subscriptionRepository,
        IMeteredDimensionsRepository meteredRepository,
        ISchedulerFrequencyRepository frequencyRepository,
        IPlansRepository plansRepository,
        IMeteredPlanSchedulerManagementRepository schedulerRepository,
        ISchedulerManagerViewRepository schedulerViewRepository, 
        IUsersRepository usersRepository,
        SaaSClientLogger<SchedulerController> logger,
        ISubscriptionUsageLogsRepository subscriptionUsageLogsRepository,
        IApplicationConfigRepository applicationConfigRepository)
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
            this.TempData["ShowWelcomeScreen"] = "True";
            var getAllSchedulerManagerViewData = this.schedulerService.GetAllSchedulerManagerList();
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
            this.TempData["ShowWelcomeScreen"] = "True";
            var allActiveMeteredSubscriptions = this.subscriptionService.GetActiveSubscriptionsWithMeteredPlan();
            var getAllEnabledFrequency = this.schedulerService.GetAllEnabledFrequency();

            // Create Frequency Dropdown list
            var schedulerFrequencyList = getAllEnabledFrequency.Select(item => new SelectListItem
            {
                Text = item.Frequency,
                Value = item.Id.ToString(),
            }).ToList();

            // Create Subscription Dropdown list
            var subscriptionList = allActiveMeteredSubscriptions.Select(item => new SelectListItem
            {
                Text = item.Name,
                Value = item.Id.ToString(),
            }).ToList();
            
            // Create Plan Dropdown list
            var dimensionsList = new List<SelectListItem>();

            var schedulerUsageViewModel = new SchedulerUsageViewModel
            {
                DimensionsList = new SelectList(dimensionsList, "Value", "Text"),
                SubscriptionList = new SelectList(subscriptionList, "Value", "Text"),
                SchedulerFrequencyList = new SelectList(schedulerFrequencyList, "Value", "Text"),
                SelectedSubscription = subscriptionId,
                Quantity = (int)Math.Round(Convert.ToDecimal(quantity), 0)
            };

            if (!string.IsNullOrEmpty(dimId))
            {
                var dimensions = this.meteredRepository.Get().FirstOrDefault(d => d.Dimension == dimId);
                if (dimensions != null)
                {
                    dimensionsList.Add(new SelectListItem
                    {
                        Text = dimId,
                        Value = dimensions.Id.ToString(),
                        Selected = true
                    });
                    schedulerUsageViewModel.SelectedDimension = dimensions.Id.ToString();
                }
            }
            else
            {
                if (subscriptionId != null)
                {
                    var selectSubscription = allActiveMeteredSubscriptions.FirstOrDefault(s => s.Id == int.Parse(subscriptionId));
                    if (selectSubscription != null)
                    {
                        // Create Dimension Dropdown list
                        var getAllDimensions = this.meteredRepository.GetDimensionsByPlanId(selectSubscription.AmpplanId);
                        if (getAllDimensions != null)
                        {

                            foreach (var item in getAllDimensions)
                            {
                                dimensionsList.Add(new SelectListItem
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
        var selectSubscription = allSubscriptionDetails.FirstOrDefault(s => s.Id == id);
        if (selectSubscription != null)
        {
            // Create Dimension Dropdown list
            var getAllDimensions = this.meteredRepository.GetDimensionsByPlanId(selectSubscription.AmpplanId);
            if (getAllDimensions != null)
            {
                var selectedList = new List<SelectListItem>();
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
            var schedulerItem = this.schedulerService.GetSchedulerManagerById(int.Parse(id));
            var detail = this.schedulerService.GetSchedulerItemRunHistory(int.Parse(id));
            var schedulerUsageViewModel = new SchedulerUsageViewModel
            {
                SelectedSubscription = schedulerItem.AMPSubscriptionId.ToString(),
                SelectedDimension = schedulerItem.Dimension,
                SelectedSchedulerFrequency = schedulerItem.Frequency,
                Quantity = Convert.ToInt32(schedulerItem.Quantity),
                FirstRunDate = schedulerItem.StartDate,
                MeteredAuditLogs = detail,
            };
            if (schedulerItem.NextRunTime.HasValue)
            {
                schedulerUsageViewModel.NextRunDate = schedulerItem.NextRunTime.Value;
            }

            return this.View(schedulerUsageViewModel);
        }
        catch (Exception ex)
        {
            this.logger.LogError($"Message:{ex.Message} :: {ex.InnerException}");
            return this.PartialView("Error", ex);
        }
    }
}