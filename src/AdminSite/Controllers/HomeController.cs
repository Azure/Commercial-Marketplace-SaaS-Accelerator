// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.Services.Configurations;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.Exceptions;
using Marketplace.SaaS.Accelerator.Services.Models;
using Marketplace.SaaS.Accelerator.Services.Services;
using Marketplace.SaaS.Accelerator.Services.StatusHandlers;
using Marketplace.SaaS.Accelerator.Services.Utilities;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace Marketplace.SaaS.Accelerator.AdminSite.Controllers;

/// <summary>
/// Home Controller.
/// </summary>
/// <seealso cref="BaseController" />
[ServiceFilter(typeof(KnownUserAttribute))]
public class HomeController : BaseController
{
    /// <summary>
    /// The logger.
    /// </summary>
    private readonly SaaSClientLogger<HomeController> logger;

    /// <summary>
    /// The subscription repository.
    /// </summary>
    private readonly ISubscriptionsRepository subscriptionRepo;

    /// <summary>
    /// The subscription logs repository.
    /// </summary>
    private readonly ISubscriptionLogRepository subscriptionLogRepository;

    /// <summary>
    /// The plan repository.
    /// </summary>
    private readonly IPlansRepository planRepository;

    /// <summary>
    /// The Metered Dimension repository.
    /// </summary>
    private readonly IMeteredDimensionsRepository dimensionsRepository;

    /// <summary>
    /// The subscription usage logs repository.
    /// </summary>
    private readonly ISubscriptionUsageLogsRepository subscriptionUsageLogsRepository;

    /// <summary>
    /// The users repository.
    /// </summary>
    private readonly IUsersRepository userRepository;

    private readonly IFulfillmentApiService fulfillApiService;

    private readonly IApplicationLogRepository applicationLogRepository;

    private readonly IMeteredBillingApiService billingApiService;

    private readonly IApplicationConfigRepository applicationConfigRepository;

    private readonly ISubscriptionsRepository subscriptionRepository;

    private readonly IEmailTemplateRepository emailTemplateRepository;

    private readonly IPlanEventsMappingRepository planEventsMappingRepository;

    private readonly IEventsRepository eventsRepository;

    private readonly IEmailService emailService;

    private readonly ISubscriptionStatusHandler pendingFulfillmentStatusHandlers;

    private readonly ISubscriptionStatusHandler pendingActivationStatusHandlers;

    private readonly ISubscriptionStatusHandler unsubscribeStatusHandlers;

    private readonly ISubscriptionStatusHandler notificationStatusHandlers;

    private readonly ILoggerFactory loggerFactory;

    private readonly IOffersRepository offersRepository;

    private readonly IOfferAttributesRepository offersAttributeRepository;

    private readonly ApplicationConfigService applicationConfigService;

    private UserService userService;

    private SubscriptionService subscriptionService = null;

    private ApplicationLogService applicationLogService = null;
    private SaaSApiClientConfiguration saaSApiClientConfiguration;

    /// <summary>
    /// Initializes a new instance of the <see cref="HomeController" /> class.
    /// </summary>
    /// <param name="usersRepository">The users repository.</param>
    /// <param name="billingApiService">The billing API service.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="subscriptionRepo">The subscription repo.</param>
    /// <param name="planRepository">The plan repository.</param>
    /// <param name="subscriptionUsageLogsRepository">The subscription usage logs repository.</param>
    /// <param name="dimensionsRepository">The dimensions repository.</param>
    /// <param name="subscriptionLogsRepo">The subscription logs repo.</param>
    /// <param name="applicationConfigRepository">The application configuration repository.</param>
    /// <param name="userRepository">The user repository.</param>
    /// <param name="fulfillApiService">The fulfill API client.</param>
    /// <param name="applicationLogRepository">The application log repository.</param>
    /// <param name="emailTemplateRepository">The email template repository.</param>
    /// <param name="planEventsMappingRepository">The plan events mapping repository.</param>
    /// <param name="eventsRepository">The events repository.</param>
    /// <param name="SaaSApiClientConfiguration">The SaaSApiClientConfiguration.</param>
    /// <param name="cloudConfigs">The cloud configs.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="emailService">The email service.</param>
    /// <param name="offersRepository">The offers repository.</param>
    /// <param name="offersAttributeRepository">The offers attribute repository.</param>
    public HomeController(
        IUsersRepository usersRepository, IMeteredBillingApiService billingApiService,  ISubscriptionsRepository subscriptionRepo, IPlansRepository planRepository, ISubscriptionUsageLogsRepository subscriptionUsageLogsRepository, IMeteredDimensionsRepository dimensionsRepository, ISubscriptionLogRepository subscriptionLogsRepo, IApplicationConfigRepository applicationConfigRepository, IUsersRepository userRepository, IFulfillmentApiService fulfillApiService, IApplicationLogRepository applicationLogRepository, IEmailTemplateRepository emailTemplateRepository, IPlanEventsMappingRepository planEventsMappingRepository, IEventsRepository eventsRepository, SaaSApiClientConfiguration saaSApiClientConfiguration, ILoggerFactory loggerFactory, IEmailService emailService, IOffersRepository offersRepository, IOfferAttributesRepository offersAttributeRepository, SaaSClientLogger<HomeController> logger) : base(applicationConfigRepository)
    {
        this.billingApiService = billingApiService;
        this.subscriptionRepo = subscriptionRepo;
        this.subscriptionLogRepository = subscriptionLogsRepo;
        this.planRepository = planRepository;
        this.subscriptionUsageLogsRepository = subscriptionUsageLogsRepository;
        this.dimensionsRepository = dimensionsRepository;
        this.logger = logger;
        this.applicationConfigRepository = applicationConfigRepository;
        this.applicationConfigService = new ApplicationConfigService(this.applicationConfigRepository);
        this.userRepository = userRepository;
        this.userService = new UserService(userRepository);
        this.fulfillApiService = fulfillApiService;
        this.applicationLogRepository = applicationLogRepository;
        this.applicationLogService = new ApplicationLogService(this.applicationLogRepository);
        this.subscriptionRepository = this.subscriptionRepo;
        this.subscriptionService = new SubscriptionService(this.subscriptionRepository, this.planRepository);
        this.emailTemplateRepository = emailTemplateRepository;
        this.planEventsMappingRepository = planEventsMappingRepository;
        this.eventsRepository = eventsRepository;
        this.emailService = emailService;
        this.offersRepository = offersRepository;
        this.offersAttributeRepository = offersAttributeRepository;
        this.loggerFactory = loggerFactory;
        this.saaSApiClientConfiguration = saaSApiClientConfiguration;

        this.pendingActivationStatusHandlers = new PendingActivationStatusHandler(
            fulfillApiService,
            subscriptionRepo,
            subscriptionLogsRepo,
            planRepository,
            userRepository,
            loggerFactory.CreateLogger<PendingActivationStatusHandler>());

        this.pendingFulfillmentStatusHandlers = new PendingFulfillmentStatusHandler(
            fulfillApiService,
            applicationConfigRepository,
            subscriptionRepo,
            subscriptionLogsRepo,
            planRepository,
            userRepository,
            this.loggerFactory.CreateLogger<PendingFulfillmentStatusHandler>());

        this.notificationStatusHandlers = new NotificationStatusHandler(
            fulfillApiService,
            planRepository,
            applicationConfigRepository,
            emailTemplateRepository,
            planEventsMappingRepository,
            offersAttributeRepository,
            eventsRepository,
            subscriptionRepo,
            userRepository,
            offersRepository,
            emailService,
            this.loggerFactory.CreateLogger<NotificationStatusHandler>());

        this.unsubscribeStatusHandlers = new UnsubscribeStatusHandler(
            fulfillApiService,
            subscriptionRepo,
            subscriptionLogsRepo,
            planRepository,
            userRepository,
            this.loggerFactory.CreateLogger<UnsubscribeStatusHandler>());
    }

    /// <summary>
    /// Indexes this instance.
    /// </summary>
    /// <returns> The <see cref="IActionResult" />.</returns>
    public IActionResult Index()
    {
        this.logger.Info("Home Controller / Index ");
        try
        {
            this.applicationConfigService.SaveFileToDisk("LogoFile", "contoso-sales.png");
            this.applicationConfigService.SaveFileToDisk("FaviconFile", "favicon.ico");

            var userId = this.userService.AddUser(this.GetCurrentUserDetail());
            return this.View();
        }
        catch (Exception ex)
        {
            this.logger.LogError($"Message:{ex.Message} :: {ex.InnerException}");
            return this.View("Error", ex);
        }
    }

    /// <summary>
    /// Subscriptionses this instance.
    /// </summary>
    /// <returns> The <see cref="IActionResult" />.</returns>
    public IActionResult Subscriptions()
    {
        this.logger.Info("Home Controller / Subscriptions ");
        try
        {
            SubscriptionViewModel subscriptionDetail = new SubscriptionViewModel();
            if (this.User.Identity.IsAuthenticated)
            {
                this.TempData["ShowWelcomeScreen"] = "True";

                List<SubscriptionResultExtension> allSubscriptions = new List<SubscriptionResultExtension>();
                var allSubscriptionDetails = this.subscriptionRepo.Get().ToList();
                var allPlans = this.planRepository.Get().ToList();
                foreach (var subscription in allSubscriptionDetails)
                {
                    Plans planDetail = allPlans.FirstOrDefault(p => p.PlanId == subscription.AmpplanId);
                    SubscriptionResultExtension subscriptionDetailExtension = this.subscriptionService.PrepareSubscriptionResponse(subscription, planDetail);
                    subscriptionDetailExtension.IsPerUserPlan = planDetail.IsPerUser.HasValue ? planDetail.IsPerUser.Value : false;
                    if (subscriptionDetailExtension != null && subscriptionDetailExtension.SubscribeId > 0)
                    {
                        allSubscriptions.Add(subscriptionDetailExtension);
                    }
                }

                subscriptionDetail.Subscriptions = allSubscriptions;

                if (this.TempData["ErrorMsg"] != null)
                {
                    subscriptionDetail.IsSuccess = false;
                    subscriptionDetail.ErrorMessage = Convert.ToString(this.TempData["ErrorMsg"]);
                }
            }
            else
            {
                return this.RedirectToAction(nameof(this.Index));
            }

            return this.View(subscriptionDetail);
        }
        catch (Exception ex)
        {
            this.logger.LogError($"Message:{ex.Message} :: {ex.InnerException}");
            return this.View("Error", ex);
        }
    }

    /// <summary>
    /// Subscriptions the log detail.
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <returns>
    /// Subscription log detail.
    /// </returns>
    public IActionResult SubscriptionLogDetail(Guid subscriptionId)
    {
        this.logger.Info(HttpUtility.HtmlEncode($"Home Controller / SubscriptionLogDetail : subscriptionId: {subscriptionId}"));
        try
        {
            if (this.User.Identity.IsAuthenticated)
            {
                List<SubscriptionAuditLogs> subscriptionAudit = new List<SubscriptionAuditLogs>();
                subscriptionAudit = this.subscriptionLogRepository.GetSubscriptionBySubscriptionId(subscriptionId).ToList();
                return this.PartialView(subscriptionAudit);
            }
            else
            {
                return this.RedirectToAction(nameof(this.Index));
            }
        }
        catch (Exception ex)
        {
            this.logger.LogError($"Message:{ex.Message} :: {ex.InnerException}");
            return this.View("Error", ex);
        }
    }

    /// <summary>
    /// Subscriptions the details.
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <param name="planId">The plan identifier.</param>
    /// <returns> The <see cref="IActionResult" />.</returns>
    public async Task<IActionResult> SubscriptionDetails(Guid subscriptionId, string planId)
    {
        this.logger.Info(HttpUtility.HtmlEncode($"Home Controller / ActivateSubscription subscriptionId:{subscriptionId} :: planId:{planId}"));
        SubscriptionResultExtension subscriptionDetail = new SubscriptionResultExtension();

        if (this.User.Identity.IsAuthenticated)
        {
            var userId = this.userService.AddUser(this.GetCurrentUserDetail());
            var currentUserId = this.userService.GetUserIdFromEmailAddress(this.CurrentUserEmailAddress);
            this.subscriptionService = new SubscriptionService(this.subscriptionRepo, this.planRepository, userId);
            this.logger.Info(HttpUtility.HtmlEncode($"User authenticate successfully & GetSubscriptionByIdAsync  SubscriptionID :{subscriptionId}"));
            this.TempData["ShowWelcomeScreen"] = false;
            var oldValue = this.subscriptionService.GetSubscriptionsBySubscriptionId(subscriptionId);
            var serializedParent = JsonSerializer.Serialize(oldValue);
            subscriptionDetail = JsonSerializer.Deserialize<SubscriptionResultExtension>(serializedParent);
            this.logger.Info(HttpUtility.HtmlEncode($"serializedParent :{serializedParent}"));
            subscriptionDetail.ShowWelcomeScreen = false;
            subscriptionDetail.SubscriptionStatus = oldValue.SubscriptionStatus;
            subscriptionDetail.CustomerEmailAddress = oldValue.CustomerEmailAddress;
            subscriptionDetail.CustomerName = oldValue.CustomerName;
            var plandetails = this.planRepository.GetById(oldValue.PlanId);
            subscriptionDetail = this.subscriptionService.GetSubscriptionsBySubscriptionId(subscriptionId);
            subscriptionDetail.SubscriptionParameters = this.subscriptionService.GetSubscriptionsParametersById(subscriptionId, plandetails.PlanGuid);
            subscriptionDetail.SubscriptionParameters = this.subscriptionService.GetSubscriptionsParametersById(subscriptionId, plandetails.PlanGuid);
            var detailsFromAPI = await this.fulfillApiService.GetSubscriptionByIdAsync(subscriptionId).ConfigureAwait(false);
            subscriptionDetail.Beneficiary = detailsFromAPI.Beneficiary;
        }

        return this.View(subscriptionDetail);
    }

    /// <summary>
    /// Des the activate subscription.
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <param name="planId">The plan identifier.</param>
    /// <param name="operation">The operation.</param>
    /// <returns> The <see cref="IActionResult" />.</returns>
    public IActionResult DeActivateSubscription(Guid subscriptionId, string planId, string operation)
    {
        this.logger.Info(HttpUtility.HtmlEncode($"Home Controller / ActivateSubscription subscriptionId::{subscriptionId} :: planID:{planId}:: operation:{operation}"));
        try
        {
            SubscriptionResultExtension subscriptionDetail = new SubscriptionResultExtension();

            if (this.User.Identity.IsAuthenticated)
            {
                var userId = this.userService.AddUser(this.GetCurrentUserDetail());
                var currentUserId = this.userService.GetUserIdFromEmailAddress(this.CurrentUserEmailAddress);
                this.subscriptionService = new SubscriptionService(this.subscriptionRepository, this.planRepository, userId);
                this.logger.Info(HttpUtility.HtmlEncode($"GetSubscriptionByIdAsync SubscriptionID :{subscriptionId} :: planID:{planId}:: operation:{operation}"));

                this.TempData["ShowWelcomeScreen"] = false;
                var oldValue = this.subscriptionService.GetSubscriptionsBySubscriptionId(subscriptionId);
                var plandetails = this.planRepository.GetById(oldValue.PlanId);
                subscriptionDetail = this.subscriptionService.GetSubscriptionsBySubscriptionId(subscriptionId);
                subscriptionDetail.ShowWelcomeScreen = false;
                subscriptionDetail.CustomerEmailAddress = this.CurrentUserEmailAddress;
                subscriptionDetail.CustomerName = this.CurrentUserName;
                subscriptionDetail.SubscriptionParameters = this.subscriptionService.GetSubscriptionsParametersById(subscriptionId, plandetails.PlanGuid);
            }

            return this.View("ActivateSubscription", subscriptionDetail);
        }
        catch (Exception ex)
        {
            this.logger.LogError($"Error while deactivating subscription :{ex.Message} :: {ex.InnerException}");
            return this.View("Error", ex);
        }
    }

    /// <summary>
    /// Subscriptions the operation.
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <param name="planId">The plan identifier.</param>
    /// <param name="operation">The operation.</param>
    /// <param name="numberofProviders">The numberof providers.</param>
    /// <returns> The <see cref="IActionResult" />.</returns>
    public IActionResult SubscriptionOperation(Guid subscriptionId, string planId, string operation, int numberofProviders)
    {
        this.logger.Info(HttpUtility.HtmlEncode($"Home Controller / SubscriptionOperation subscriptionId:{subscriptionId} :: planId : {planId} :: operation:{operation} :: NumberofProviders : {numberofProviders}"));
        try
        {
            var userDetails = this.userRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);
            var oldValue = this.subscriptionService.GetSubscriptionsBySubscriptionId(subscriptionId);
            SubscriptionProcessQueueModel queueObject = new SubscriptionProcessQueueModel();
            if (operation == "Activate")
            {
                if (oldValue.SubscriptionStatus.ToString() != SubscriptionStatusEnumExtension.PendingActivation.ToString())
                {
                    this.subscriptionRepository.UpdateStatusForSubscription(subscriptionId, SubscriptionStatusEnumExtension.PendingActivation.ToString(), true);

                    SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
                    {
                        Attribute = Convert.ToString(SubscriptionLogAttributes.Status),
                        SubscriptionId = oldValue.SubscribeId,
                        NewValue = SubscriptionStatusEnumExtension.PendingActivation.ToString(),
                        OldValue = oldValue.SubscriptionStatus.ToString(),
                        CreateBy = userDetails.UserId,
                        CreateDate = DateTime.Now,
                    };
                    this.subscriptionLogRepository.Save(auditLog);
                }

                this.pendingActivationStatusHandlers.Process(subscriptionId);
            }

            if (operation == "Deactivate")
            {
                this.subscriptionRepository.UpdateStatusForSubscription(subscriptionId, SubscriptionStatusEnumExtension.PendingUnsubscribe.ToString(), true);
                SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
                {
                    Attribute = Convert.ToString(SubscriptionLogAttributes.Status),
                    SubscriptionId = oldValue.SubscribeId,
                    NewValue = SubscriptionStatusEnumExtension.PendingUnsubscribe.ToString(),
                    OldValue = oldValue.SubscriptionStatus.ToString(),
                    CreateBy = userDetails.UserId,
                    CreateDate = DateTime.Now,
                };
                this.subscriptionLogRepository.Save(auditLog);

                this.unsubscribeStatusHandlers.Process(subscriptionId);
            }

            this.notificationStatusHandlers.Process(subscriptionId);

            return this.RedirectToAction(nameof(this.ActivatedMessage));
        }
        catch (Exception ex)
        {
            this.logger.LogError($"Message:{ex.Message} :: {ex.InnerException}");
            return this.View("Error");
        }
    }

    /// <summary>
    /// Gets or sets the <see cref="Microsoft.AspNetCore.Mvc.IActionResult" /> with the specified error.
    /// </summary>
    /// <returns>
    /// The <see cref="IActionResult" />.
    /// </returns>
    /// <value>
    /// The <see cref="Microsoft.AspNetCore.Mvc.IActionResult" />.
    /// </value>
    public IActionResult ActivatedMessage()
    {
        try
        {
            return this.View("ProcessMessage");
        }
        catch (Exception ex)
        {
            return this.View("Error", ex);
        }
    }

    /// <summary>
    /// Records the usage.
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <returns> The <see cref="IActionResult" />.</returns>
    public IActionResult RecordUsage(int subscriptionId)
    {
        this.logger.Info("Home Controller / RecordUsage ");
        try
        {
            if (this.User.Identity.IsAuthenticated)
            {
                var subscriptionDetail = this.subscriptionRepo.Get(subscriptionId);
                var allDimensionsList = this.dimensionsRepository.GetDimensionsByPlanId(subscriptionDetail.AmpplanId);
                SubscriptionUsageViewModel usageViewModel = new SubscriptionUsageViewModel();
                usageViewModel.SubscriptionDetail = subscriptionDetail;
                usageViewModel.MeteredAuditLogs = new List<MeteredAuditLogs>();
                usageViewModel.MeteredAuditLogs = this.subscriptionUsageLogsRepository.GetMeteredAuditLogsBySubscriptionId(subscriptionId, true).OrderByDescending(s => s.CreatedDate).ToList();
                usageViewModel.DimensionsList = new SelectList(allDimensionsList, "Dimension", "Description");
                return this.View(usageViewModel);
            }
            else
            {
                return this.RedirectToAction(nameof(this.Index));
            }
        }
        catch (Exception ex)
        {
            this.logger.LogError($"Message:{ex.Message} :: {ex.InnerException}");
            return this.View("Error", ex);
        }
    }
    /// <summary>
    /// Records the usage.
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <returns> The <see cref="IActionResult" />.</returns>
    public IActionResult RecordUsageNow(int subscriptionId, string dimId, string quantity)
    {
        this.logger.Info("Home Controller / RecordUsage ");
        try
        {
            if (this.User.Identity.IsAuthenticated)
            {
                var subscriptionDetail = this.subscriptionRepo.Get(subscriptionId);
                var allDimensionsList = this.dimensionsRepository.GetDimensionsByPlanId(subscriptionDetail.AmpplanId);
                SubscriptionUsageViewModel usageViewModel = new SubscriptionUsageViewModel();
                usageViewModel.SubscriptionDetail = subscriptionDetail;
                usageViewModel.MeteredAuditLogs = new List<MeteredAuditLogs>();
                usageViewModel.MeteredAuditLogs = this.subscriptionUsageLogsRepository.GetMeteredAuditLogsBySubscriptionId(subscriptionId).OrderByDescending(s => s.CreatedDate).ToList();
                usageViewModel.DimensionsList = new SelectList(allDimensionsList, "Dimension", "Description");

                usageViewModel.SelectedDimension = dimId;
                usageViewModel.Quantity = quantity;
                return this.View("RecordUsage", usageViewModel);
            }
            else
            {
                return this.RedirectToAction(nameof(this.Index));
            }
        }
        catch (Exception ex)
        {
            this.logger.LogError($"Message:{ex.Message} :: {ex.InnerException}");
            return this.View("Error", ex);
        }
    }
    /// <summary>
    /// Get Subscription Details for selected Subscription.
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <returns>
    /// The <see cref="IActionResult" />.
    /// </returns>
    public IActionResult SubscriptionQuantityDetail(Guid subscriptionId)
    {
        this.logger.Info(HttpUtility.HtmlEncode($"Home Controller / SubscriptionQuantityDetail subscriptionId:{subscriptionId}"));
        try
        {
            if (this.User.Identity.IsAuthenticated)
            {
                var subscriptionDetail = this.subscriptionService.GetSubscriptionsBySubscriptionId(subscriptionId);
                return this.View(subscriptionDetail);
            }
            else
            {
                return this.RedirectToAction(nameof(this.Index));
            }
        }
        catch (Exception ex)
        {
            this.logger.LogError($"Message:{ex.Message} :: {ex.InnerException}");
            return this.View("Error", ex);
        }
    }

    /// <summary>
    /// Manages the subscription usage.
    /// </summary>
    /// <param name="subscriptionData">The subscription data.</param>
    /// <returns> The <see cref="IActionResult" />.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ManageSubscriptionUsage(SubscriptionUsageViewModel subscriptionData)
    {
        this.logger.Info(HttpUtility.HtmlEncode($"Home Controller / ManageSubscriptionUsage  subscriptionData: {JsonSerializer.Serialize(subscriptionData)}"));
        try
        {
            if (subscriptionData != null && subscriptionData.SubscriptionDetail != null)
            {
                var currentUserDetail = this.userRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);
                var subscriptionUsageRequest = new MeteringUsageRequest()
                {
                    Dimension = subscriptionData.SelectedDimension,
                    EffectiveStartTime = DateTime.UtcNow,
                    PlanId = subscriptionData.SubscriptionDetail.AmpplanId,
                    Quantity = Convert.ToDouble(subscriptionData.Quantity ?? "0"),
                    ResourceId = subscriptionData.SubscriptionDetail.AmpsubscriptionId,
                };
                var meteringUsageResult = new MeteringUsageResult();
                var requestJson = JsonSerializer.Serialize(subscriptionUsageRequest);
                var responseJson = string.Empty;
                try
                {
                    this.logger.Info("EmitUsageEventAsync");
                    meteringUsageResult = this.billingApiService.EmitUsageEventAsync(subscriptionUsageRequest).ConfigureAwait(false).GetAwaiter().GetResult();
                    responseJson = JsonSerializer.Serialize(meteringUsageResult);
                    this.logger.Info(responseJson);
                }
                catch (MarketplaceException mex)
                {
                    responseJson = JsonSerializer.Serialize(mex.MeteredBillingErrorDetail);
                    meteringUsageResult.Status = mex.ErrorCode;
                    this.logger.Info(responseJson);
                }

                var newMeteredAuditLog = new MeteredAuditLogs()
                {
                    RequestJson = requestJson,
                    ResponseJson = responseJson,
                    StatusCode = meteringUsageResult.Status,
                    RunBy = "Manual",
                    SubscriptionId = subscriptionData.SubscriptionDetail.Id,
                    SubscriptionUsageDate = DateTime.UtcNow,
                    CreatedBy = currentUserDetail == null ? 0 : currentUserDetail.UserId,
                    CreatedDate = DateTime.Now,
                };
                this.subscriptionUsageLogsRepository.Save(newMeteredAuditLog);
            }
        }
        catch (Exception ex)
        {
            this.logger.LogError($"Message:{ex.Message} :: {ex.InnerException}");
        }

        return this.RedirectToAction(nameof(this.RecordUsage), new { subscriptionId = subscriptionData.SubscriptionDetail.Id });
    }

    /// <summary>
    /// Get All Subscription List for Current Logged in User.
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <returns>
    /// The <see cref="IActionResult" />.
    /// </returns>
    public IActionResult ViewSubscriptionDetail(Guid subscriptionId)
    {
        this.logger.Info(HttpUtility.HtmlEncode($"Home Controller / SubscriptionDetail subscriptionId:{subscriptionId}"));
        try
        {
            if (this.User.Identity.IsAuthenticated)
            {
                var subscriptionDetail = this.subscriptionService.GetSubscriptionsBySubscriptionId(subscriptionId);
                subscriptionDetail.PlanList = this.subscriptionService.GetAllSubscriptionPlans();

                return this.View(subscriptionDetail);
            }
            else
            {
                return this.RedirectToAction(nameof(this.Index));
            }
        }
        catch (Exception ex)
        {
            this.logger.LogError($"Message:{ex.Message} :: {ex.InnerException}");
            return this.View("Error", ex);
        }
    }

    /// <summary>
    /// Privacies this instance.
    /// </summary>
    /// <returns> The <see cref="IActionResult" />.</returns>
    public IActionResult Privacy()
    {
        return this.View();
    }

    /// <summary>
    /// The Error.
    /// </summary>
    /// <returns>
    /// The <see cref="IActionResult" />.
    /// </returns>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var exceptionDetail = this.HttpContext.Features.Get<IExceptionHandlerFeature>();
        return this.View(exceptionDetail?.Error);
    }

    /// <summary>
    /// Changes the subscription plan.
    /// </summary>
    /// <param name="subscriptionDetail">The subscription detail.</param>
    /// <returns> IActionResult.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeSubscriptionPlan(SubscriptionResult subscriptionDetail)
    {
        this.logger.Info(HttpUtility.HtmlEncode($"Home Controller / ChangeSubscriptionPlan  subscriptionDetail:{JsonSerializer.Serialize(subscriptionDetail)}"));
        try
        {
            if (subscriptionDetail.Id != default && !string.IsNullOrEmpty(subscriptionDetail.PlanId))
            {
                try
                {
                    //initiate change plan
                    var currentUserId = this.userService.GetUserIdFromEmailAddress(this.CurrentUserEmailAddress);
                    
                    var jsonResult = await this.fulfillApiService.ChangePlanForSubscriptionAsync(subscriptionDetail.Id, subscriptionDetail.PlanId).ConfigureAwait(false);
                    var changePlanOperationStatus = OperationStatusEnum.InProgress;

                    if (jsonResult != null && jsonResult.OperationId != default)
                    {
                        int _counter = 0;

                        //loop untill the operation status has moved away from inprogress or notstarted, generally this will be the result of webhooks' action aganist this operation
                        while (OperationStatusEnum.InProgress.Equals(changePlanOperationStatus) || OperationStatusEnum.NotStarted.Equals(changePlanOperationStatus))
                        {
                            var changePlanOperationResult = await this.fulfillApiService.GetOperationStatusResultAsync(subscriptionDetail.Id, jsonResult.OperationId).ConfigureAwait(false);
                            changePlanOperationStatus = changePlanOperationResult.Status;

                            this.logger.Info(HttpUtility.HtmlEncode($"Plan Change Progress. SubscriptionId: {subscriptionDetail.Id} ToPlan: {subscriptionDetail.PlanId} UserId: ****** OperationId: {jsonResult.OperationId} Operationstatus: {changePlanOperationStatus}."));
                            await this.applicationLogService.AddApplicationLog($"Plan Change Progress. SubscriptionId: {subscriptionDetail.Id} ToPlan: {subscriptionDetail.PlanId} UserId: {currentUserId} OperationId: {jsonResult.OperationId} Operationstatus: {changePlanOperationStatus}.").ConfigureAwait(false);

                            //wait and check every 5secs
                            await Task.Delay(5000);
                            _counter++;
                            if (_counter > 100)
                            {
                                //if loop has been executed for more than 100 times then break, to avoid infinite loop just in case
                                break;
                            }
                        }

                        if (changePlanOperationStatus == OperationStatusEnum.Succeeded)
                        {
                            this.logger.Info(HttpUtility.HtmlEncode($"Plan Change Success. SubscriptionId: {subscriptionDetail.Id} ToPlan : {subscriptionDetail.PlanId} UserId: ***** OperationId: {jsonResult.OperationId}."));
                            await this.applicationLogService.AddApplicationLog($"Plan Change Success. SubscriptionId: {subscriptionDetail.Id} ToPlan: {subscriptionDetail.PlanId} UserId: {currentUserId} OperationId: {jsonResult.OperationId}.").ConfigureAwait(false);
                        }
                        else
                        {
                            this.logger.Info(HttpUtility.HtmlEncode($"Plan Change Failed. SubscriptionId: {subscriptionDetail.Id} ToPlan : {subscriptionDetail.PlanId} UserId: **** OperationId: {jsonResult.OperationId} Operation status {changePlanOperationStatus}."));
                            await this.applicationLogService.AddApplicationLog($"Plan Change Failed. SubscriptionId: {subscriptionDetail.Id} ToPlan: {subscriptionDetail.PlanId} UserId: {currentUserId} OperationId: {jsonResult.OperationId} Operation status {changePlanOperationStatus}.").ConfigureAwait(false);

                            throw new MarketplaceException($"Plan change operation failed with operation status {changePlanOperationStatus}. Check if the updates are allowed in the App config \"AcceptSubscriptionUpdates\" key or db application log for more information.");
                        }
                    }
                }
                catch (MarketplaceException fex)
                {
                    this.TempData["ErrorMsg"] = fex.Message;
                }
            }

            return this.RedirectToAction(nameof(this.Subscriptions));
        }
        catch (Exception ex)
        {
            this.logger.LogError($"Message:{ex.Message} :: {ex.InnerException}");
            return this.View("Error", ex);
        }
    }

    /// <summary>
    /// Changes the quantity plan.
    /// </summary>
    /// <param name="subscriptionDetail">The subscription detail.</param>
    /// <returns>Changes subscription quantity.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeSubscriptionQuantity(SubscriptionResult subscriptionDetail)
    {
        this.logger.Info(HttpUtility.HtmlEncode($"Home Controller / ChangeSubscriptionPlan  subscriptionDetail:{JsonSerializer.Serialize(subscriptionDetail)}"));
        if (this.User.Identity.IsAuthenticated)
        {
            try
            {
                if (subscriptionDetail != null && subscriptionDetail.Id != default && subscriptionDetail.Quantity > 0)
                {
                    try
                    {
                        //initiate change quantity
                        var currentUserId = this.userService.GetUserIdFromEmailAddress(this.CurrentUserEmailAddress);
                        
                        var jsonResult = await this.fulfillApiService.ChangeQuantityForSubscriptionAsync(subscriptionDetail.Id, subscriptionDetail.Quantity).ConfigureAwait(false);
                        var changeQuantityOperationStatus = OperationStatusEnum.InProgress;

                        if (jsonResult != null && jsonResult.OperationId != default)
                        {
                            int _counter = 0;

                            //loop untill the operation status has moved away from inprogress or notstarted, generally this will be the result of webhooks' action aganist this operation
                            while (OperationStatusEnum.InProgress.Equals(changeQuantityOperationStatus) || OperationStatusEnum.NotStarted.Equals(changeQuantityOperationStatus))
                            {
                                var changeQuantityOperationResult = await this.fulfillApiService.GetOperationStatusResultAsync(subscriptionDetail.Id, jsonResult.OperationId).ConfigureAwait(false);
                                changeQuantityOperationStatus = changeQuantityOperationResult.Status;

                                this.logger.Info(HttpUtility.HtmlEncode($"Quantity Change Progress. SubscriptionId: {subscriptionDetail.Id} ToQuantity: {subscriptionDetail.Quantity}  OperationId: {jsonResult.OperationId} Operationstatus: {changeQuantityOperationStatus}."));
                                await this.applicationLogService.AddApplicationLog($"Quantity Change Progress. SubscriptionId: {subscriptionDetail.Id} ToQuantity: {subscriptionDetail.Quantity} UserId: {currentUserId} OperationId: {jsonResult.OperationId} Operationstatus: {changeQuantityOperationStatus}.").ConfigureAwait(false);

                                //wait and check every 5secs
                                await Task.Delay(5000);
                                _counter++;
                                if (_counter > 100)
                                {
                                    //if loop has been executed for more than 100 times then break, to avoid infinite loop just in case
                                    break;
                                }
                            }

                            if (changeQuantityOperationStatus == OperationStatusEnum.Succeeded)
                            {
                                this.logger.Info(HttpUtility.HtmlEncode($"Quantity Change Success. SubscriptionId: {subscriptionDetail.Id} ToQuantity: {subscriptionDetail.Quantity} UserId: ****** OperationId: {jsonResult.OperationId}."));
                                await this.applicationLogService.AddApplicationLog($"Quantity Change Success. SubscriptionId: {subscriptionDetail.Id} ToQuantity: {subscriptionDetail.Quantity} UserId: {currentUserId} OperationId: {jsonResult.OperationId}.").ConfigureAwait(false);
                            }
                            else
                            {
                                this.logger.Info(HttpUtility.HtmlEncode($"Quantity Change Failed. SubscriptionId: {subscriptionDetail.Id} ToQuantity: {subscriptionDetail.Quantity} UserId: ***** OperationId: {jsonResult.OperationId} Operationstatus: {changeQuantityOperationStatus}."));
                                await this.applicationLogService.AddApplicationLog($"Quantity Change Failed. SubscriptionId: {subscriptionDetail.Id} ToQuantity: {subscriptionDetail.Quantity} UserId: {currentUserId} OperationId: {jsonResult.OperationId} Operationstatus: {changeQuantityOperationStatus}.").ConfigureAwait(false);

                                throw new MarketplaceException($"Quantity Change operation failed with operation status {changeQuantityOperationStatus}. Check if the updates are allowed in the App config \"AcceptSubscriptionUpdates\" key or db application log for more information.");
                            }
                        }
                    }
                    catch (MarketplaceException fex)
                    {
                        this.TempData["ErrorMsg"] = fex.Message;
                        this.logger.LogError($"Message:{fex.Message} :: {fex.InnerException}   ");
                    }
                }

                return this.RedirectToAction(nameof(this.Subscriptions));
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Message:{ex.Message} :: {ex.InnerException}"); 
                return this.View("Error", ex);
            }
        }
        else
        {
            return this.RedirectToAction(nameof(this.Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult FetchAllSubscriptions()
    {
        var currentUserId = this.userService.GetUserIdFromEmailAddress(this.CurrentUserEmailAddress);

        try
        {
            this.subscriptionService = new SubscriptionService(this.subscriptionRepository, this.planRepository, currentUserId);

            // Step 1: Get all subscriptions from the API
            var subscriptions = this.fulfillApiService.GetAllSubscriptionAsync().GetAwaiter().GetResult();
            foreach (SubscriptionResult subscription in subscriptions)
            {
                var customerUserId = 0;
                var currentSubscription = this.subscriptionService.GetSubscriptionsBySubscriptionId(subscription.Id);

                // Step 2: Check if they Exist in DB - Create if dont exist
                if (currentSubscription.Name == null)
                {
                    // Step 3: Add/Update the Offer
                    Guid OfferId = this.offersRepository.Add(new Offers()
                    {
                        OfferId = subscription.OfferId,
                        OfferName = subscription.OfferId,
                        UserId = currentUserId,
                        CreateDate = DateTime.Now,
                        OfferGuid = Guid.NewGuid(),
                    });

                    // Step 4: Add/Update the Plans. For Unsubscribed Only Add current plan from subscription information
                    if (subscription.SaasSubscriptionStatus == SubscriptionStatusEnum.Unsubscribed)
                    {
                        PlanDetailResultExtension planDetails = new PlanDetailResultExtension
                        {
                            PlanId = subscription.PlanId,
                            DisplayName = subscription.PlanId,
                            Description = "",
                            OfferId = OfferId,
                            PlanGUID = Guid.NewGuid(),
                            IsPerUserPlan = subscription.Quantity > 0,
                        };
                        this.subscriptionService.AddPlanDetailsForSubscription(planDetails);
                    }
                    else
                    {
                        var subscriptionPlanDetail = this.fulfillApiService.GetAllPlansForSubscriptionAsync(subscription.Id).ConfigureAwait(false).GetAwaiter().GetResult();
                        subscriptionPlanDetail.ForEach(x =>
                        {
                            x.OfferId = OfferId;
                            x.PlanGUID = Guid.NewGuid();
                        });
                        this.subscriptionService.AddUpdateAllPlanDetailsForSubscription(subscriptionPlanDetail);
                    }

                    // Step 5: Add/Update the current user from Subscription information
                    customerUserId = this.userService.AddUser(new PartnerDetailViewModel { FullName = subscription.Beneficiary.EmailId, EmailAddress = subscription.Beneficiary.EmailId });
                }

                // Step 6: Add Subscription
                var subscriptionId = this.subscriptionService.AddOrUpdatePartnerSubscriptions(subscription, customerUserId);

                // Step 7: Add Subscription Audit
                if (currentSubscription != null && subscription.SaasSubscriptionStatus.ToString() != currentSubscription.SubscriptionStatus.ToString())
                {
                    this.subscriptionLogRepository.Save(new SubscriptionAuditLogs()
                    {
                        Attribute = $"{Convert.ToString(SubscriptionLogAttributes.Status)}-Refresh",
                        SubscriptionId = subscriptionId,
                        NewValue = subscription.SaasSubscriptionStatus.ToString(),
                        OldValue = currentSubscription.SubscriptionStatus.ToString(),
                        CreateBy = currentUserId,
                        CreateDate = DateTime.Now
                    });
                }
                if (currentSubscription != null && subscription.PlanId != currentSubscription.PlanId)
                {
                    this.subscriptionLogRepository.Save(new SubscriptionAuditLogs()
                    {
                        Attribute = $"{Convert.ToString(SubscriptionLogAttributes.Plan)}-Refresh",
                        SubscriptionId = subscriptionId,
                        NewValue = subscription.PlanId.ToString(),
                        OldValue = currentSubscription.PlanId,
                        CreateBy = currentUserId,
                        CreateDate = DateTime.Now
                    });
                }
                if (currentSubscription != null && subscription.Quantity != currentSubscription.Quantity)
                {
                    this.subscriptionLogRepository.Save(new SubscriptionAuditLogs()
                    {
                        Attribute = $"{Convert.ToString(SubscriptionLogAttributes.Quantity)}-Refresh",
                        SubscriptionId = subscriptionId,
                        NewValue = subscription.Quantity.ToString(),
                        OldValue = currentSubscription.Quantity.ToString(),
                        CreateBy = currentUserId,
                        CreateDate = DateTime.Now
                    });
                }

            }
        }
        catch (Exception ex)
        {
            var errorMessage = $"Message: {ex.Message} ({ex.InnerException})";
            this.logger.LogError(errorMessage);
            applicationLogService.AddApplicationLog(errorMessage).GetAwaiter().GetResult();

            return BadRequest();
        }

        return Ok();
    }

}