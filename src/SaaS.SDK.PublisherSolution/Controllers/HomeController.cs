namespace Microsoft.Marketplace.Saas.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Marketplace.SaaS.SDK.Services.Contracts;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using Microsoft.Marketplace.SaaS.SDK.Services.Services;
    using Microsoft.Marketplace.SaaS.SDK.Services.StatusHandlers;
    using Microsoft.Marketplace.SaaS.SDK.Services.Utilities;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using Microsoft.Marketplace.SaasKit.Configurations;
    using Microsoft.Marketplace.SaasKit.Contracts;
    using Microsoft.Marketplace.SaasKit.Exceptions;
    using Microsoft.Marketplace.SaasKit.Models;

    /// <summary>
    /// Home Controller.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.Saas.Web.Controllers.BaseController" />
    [ServiceFilter(typeof(KnownUserAttribute))]
    public class HomeController : BaseController
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<HomeController> logger;

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

        private readonly IFulfillmentApiClient fulfillApiClient;

        private readonly IApplicationLogRepository applicationLogRepository;

        private readonly IMeteredBillingApiClient apiClient;

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

        private UserService userService;

        private SubscriptionService subscriptionService = null;

        private ApplicationLogService applicationLogService = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController" /> class.
        /// </summary>
        /// <param name="usersRepository">The users repository.</param>
        /// <param name="apiClient">The API client.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="subscriptionRepo">The subscription repo.</param>
        /// <param name="planRepository">The plan repository.</param>
        /// <param name="subscriptionUsageLogsRepository">The subscription usage logs repository.</param>
        /// <param name="dimensionsRepository">The dimensions repository.</param>
        /// <param name="subscriptionLogsRepo">The subscription logs repo.</param>
        /// <param name="applicationConfigRepository">The application configuration repository.</param>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="fulfillApiClient">The fulfill API client.</param>
        /// <param name="applicationLogRepository">The application log repository.</param>
        /// <param name="emailTemplateRepository">The email template repository.</param>
        /// <param name="planEventsMappingRepository">The plan events mapping repository.</param>
        /// <param name="eventsRepository">The events repository.</param>
        /// <param name="options">The options.</param>
        /// <param name="cloudConfigs">The cloud configs.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="emailService">The email service.</param>
        /// <param name="offersRepository">The offers repository.</param>
        /// <param name="offersAttributeRepository">The offers attribute repository.</param>
        public HomeController(
                        IUsersRepository usersRepository, IMeteredBillingApiClient apiClient, ILogger<HomeController> logger, ISubscriptionsRepository subscriptionRepo, IPlansRepository planRepository, ISubscriptionUsageLogsRepository subscriptionUsageLogsRepository, IMeteredDimensionsRepository dimensionsRepository, ISubscriptionLogRepository subscriptionLogsRepo, IApplicationConfigRepository applicationConfigRepository, IUsersRepository userRepository, IFulfillmentApiClient fulfillApiClient, IApplicationLogRepository applicationLogRepository, IEmailTemplateRepository emailTemplateRepository, IPlanEventsMappingRepository planEventsMappingRepository, IEventsRepository eventsRepository, IOptions<SaaSApiClientConfiguration> options, ILoggerFactory loggerFactory, IEmailService emailService, IOffersRepository offersRepository, IOfferAttributesRepository offersAttributeRepository)
        {
            this.apiClient = apiClient;
            this.subscriptionRepo = subscriptionRepo;
            this.subscriptionLogRepository = subscriptionLogsRepo;
            this.planRepository = planRepository;
            this.subscriptionUsageLogsRepository = subscriptionUsageLogsRepository;
            this.dimensionsRepository = dimensionsRepository;
            this.logger = logger;
            this.applicationConfigRepository = applicationConfigRepository;
            this.userRepository = userRepository;
            this.userService = new UserService(userRepository);
            this.fulfillApiClient = fulfillApiClient;
            this.applicationLogRepository = applicationLogRepository;
            this.applicationLogService = new ApplicationLogService(this.applicationLogRepository);
            this.applicationConfigRepository = applicationConfigRepository;
            this.subscriptionRepository = this.subscriptionRepo;
            this.subscriptionService = new SubscriptionService(this.subscriptionRepository, this.planRepository);
            this.emailTemplateRepository = emailTemplateRepository;
            this.planEventsMappingRepository = planEventsMappingRepository;
            this.eventsRepository = eventsRepository;
            this.emailService = emailService;
            this.offersRepository = offersRepository;
            this.offersAttributeRepository = offersAttributeRepository;
            this.loggerFactory = loggerFactory;

            this.pendingActivationStatusHandlers = new PendingActivationStatusHandler(
                                                                          fulfillApiClient,
                                                                          subscriptionRepo,
                                                                          subscriptionLogsRepo,
                                                                          planRepository,
                                                                          userRepository,
                                                                          loggerFactory.CreateLogger<PendingActivationStatusHandler>());

            this.pendingFulfillmentStatusHandlers = new PendingFulfillmentStatusHandler(
                                                                           fulfillApiClient,
                                                                           applicationConfigRepository,
                                                                           subscriptionRepo,
                                                                           subscriptionLogsRepo,
                                                                           planRepository,
                                                                           userRepository,
                                                                           this.loggerFactory.CreateLogger<PendingFulfillmentStatusHandler>());

            this.notificationStatusHandlers = new NotificationStatusHandler(
                                                                        fulfillApiClient,
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
                                                                        fulfillApiClient,
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
            this.logger.LogInformation("Home Controller / Index ");
            try
            {
                var userId = this.userService.AddUser(this.GetCurrentUserDetail());
                return this.View();
            }
            catch (Exception ex)
            {
                this.logger.LogError("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                return this.View("Error", ex);
            }
        }

        /// <summary>
        /// Subscriptionses this instance.
        /// </summary>
        /// <returns> The <see cref="IActionResult" />.</returns>
        public IActionResult Subscriptions()
        {
            this.logger.LogInformation("Home Controller / Subscriptions ");
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
                        SubscriptionResultExtension subscriptionDetailExtension = this.subscriptionService.PrepareSubscriptionResponse(subscription);
                        Plans planDetail = this.planRepository.GetById(subscriptionDetailExtension.PlanId);
                        subscriptionDetailExtension.IsPerUserPlan = planDetail.IsPerUser.HasValue ? planDetail.IsPerUser.Value : false;
                        subscriptionDetail.IsAutomaticProvisioningSupported = Convert.ToBoolean(this.applicationConfigRepository.GetValueByName("IsAutomaticProvisioningSupported"));
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
                this.logger.LogError("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
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
            this.logger.LogInformation("Home Controller / RecordUsage : subscriptionId: {0}", JsonSerializer.Serialize(subscriptionId));
            try
            {
                if (this.User.Identity.IsAuthenticated)
                {
                    List<SubscriptionAuditLogs> subscriptionAudit = new List<SubscriptionAuditLogs>();
                    subscriptionAudit = this.subscriptionLogRepository.GetSubscriptionBySubscriptionId(subscriptionId).ToList();
                    return this.View(subscriptionAudit);
                }
                else
                {
                    return this.RedirectToAction(nameof(this.Index));
                }
            }
            catch (Exception ex)
            {
                this.logger.LogInformation("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                return this.View("Error", ex);
            }
        }

        /// <summary>
        /// Subscriptions the details.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="planId">The plan identifier.</param>
        /// <returns> The <see cref="IActionResult" />.</returns>
        public IActionResult SubscriptionDetails(Guid subscriptionId, string planId)
        {
            this.logger.LogInformation("Home Controller / ActivateSubscription subscriptionId:{0} :: planId:{1}", subscriptionId, planId);
            SubscriptionResultExtension subscriptionDetail = new SubscriptionResultExtension();

            if (this.User.Identity.IsAuthenticated)
            {
                var userId = this.userService.AddUser(this.GetCurrentUserDetail());
                var currentUserId = this.userService.GetUserIdFromEmailAddress(this.CurrentUserEmailAddress);
                this.subscriptionService = new SubscriptionService(this.subscriptionRepo, this.planRepository, userId);
                this.logger.LogInformation("User authenticate successfully & GetSubscriptionByIdAsync  SubscriptionID :{0}", JsonSerializer.Serialize(subscriptionId));
                this.TempData["ShowWelcomeScreen"] = false;
                var oldValue = this.subscriptionService.GetSubscriptionsBySubscriptionId(subscriptionId);
                var serializedParent = JsonSerializer.Serialize(oldValue);
                subscriptionDetail = JsonSerializer.Deserialize<SubscriptionResultExtension>(serializedParent);
                this.logger.LogInformation("serializedParent :{0}", serializedParent);
                subscriptionDetail.ShowWelcomeScreen = false;
                subscriptionDetail.SubscriptionStatus = oldValue.SubscriptionStatus;
                subscriptionDetail.CustomerEmailAddress = oldValue.CustomerEmailAddress;
                subscriptionDetail.CustomerName = oldValue.CustomerName;
                var plandetails = this.planRepository.GetById(oldValue.PlanId);
                subscriptionDetail = this.subscriptionService.GetSubscriptionsBySubscriptionId(subscriptionId);
                subscriptionDetail.SubscriptionParameters = this.subscriptionService.GetSubscriptionsParametersById(subscriptionId, plandetails.PlanGuid);
                subscriptionDetail.SubscriptionParameters = this.subscriptionService.GetSubscriptionsParametersById(subscriptionId, plandetails.PlanGuid);
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
            this.logger.LogInformation("Home Controller / ActivateSubscription subscriptionId:{0} :: planId:{1} :: operation:{2}", subscriptionId, planId, operation);
            try
            {
                SubscriptionResultExtension subscriptionDetail = new SubscriptionResultExtension();

                if (this.User.Identity.IsAuthenticated)
                {
                    var userId = this.userService.AddUser(this.GetCurrentUserDetail());
                    var currentUserId = this.userService.GetUserIdFromEmailAddress(this.CurrentUserEmailAddress);
                    this.subscriptionService = new SubscriptionService(this.subscriptionRepository, this.planRepository, userId);
                    this.logger.LogInformation("GetSubscriptionByIdAsync SubscriptionID :{0} :: planID:{1}:: operation:{2}", JsonSerializer.Serialize(subscriptionId), JsonSerializer.Serialize(operation));

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
                this.logger.LogError(ex, "Error while deactivating subscription");
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
            this.logger.LogInformation("Home Controller / SubscriptionOperation subscriptionId:{0} :: planId : {1} :: operation:{2} :: NumberofProviders : {3}", JsonSerializer.Serialize(subscriptionId), JsonSerializer.Serialize(planId), JsonSerializer.Serialize(operation), JsonSerializer.Serialize(numberofProviders));
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
                this.logger.LogInformation("Message:{0} :: {1}", ex.Message, ex.InnerException);
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
            this.logger.LogInformation("Home Controller / RecordUsage ");
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
                    return this.View(usageViewModel);
                }
                else
                {
                    return this.RedirectToAction(nameof(this.Index));
                }
            }
            catch (Exception ex)
            {
                this.logger.LogInformation("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
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
            this.logger.LogInformation("Home Controller / SubscriptionQuantityDetail subscriptionId:{0}", JsonSerializer.Serialize(subscriptionId));
            try
            {
                if (this.User.Identity.IsAuthenticated)
                {
                    var subscriptionDetail = this.subscriptionService.GetPartnerSubscription(this.CurrentUserEmailAddress, subscriptionId).FirstOrDefault();
                    return this.View(subscriptionDetail);
                }
                else
                {
                    return this.RedirectToAction(nameof(this.Index));
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                return this.View("Error", ex);
            }
        }

        /// <summary>
        /// Manages the subscription usage.
        /// </summary>
        /// <param name="subscriptionData">The subscription data.</param>
        /// <returns> The <see cref="IActionResult" />.</returns>
        [HttpPost]
        public IActionResult ManageSubscriptionUsage(SubscriptionUsageViewModel subscriptionData)
        {
            this.logger.LogInformation("Home Controller / ManageSubscriptionUsage  subscriptionData: {0}", JsonSerializer.Serialize(subscriptionData));
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
                        this.logger.LogInformation("EmitUsageEventAsync");
                        meteringUsageResult = this.apiClient.EmitUsageEventAsync(subscriptionUsageRequest).ConfigureAwait(false).GetAwaiter().GetResult();
                        responseJson = JsonSerializer.Serialize(meteringUsageResult);
                        this.logger.LogInformation(responseJson);
                    }
                    catch (MeteredBillingException mex)
                    {
                        responseJson = JsonSerializer.Serialize(mex.MeteredBillingErrorDetail);
                        meteringUsageResult.Status = mex.ErrorCode;
                        this.logger.LogInformation(responseJson);
                    }

                    var newMeteredAuditLog = new MeteredAuditLogs()
                    {
                        RequestJson = requestJson,
                        ResponseJson = responseJson,
                        StatusCode = meteringUsageResult.Status,
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
                this.logger.LogError(ex, ex.Message);
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
            this.logger.LogInformation("Home Controller / SubscriptionDetail subscriptionId:{0}", JsonSerializer.Serialize(subscriptionId));
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
                this.logger.LogError("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
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
        public async Task<IActionResult> ChangeSubscriptionPlan(SubscriptionResult subscriptionDetail)
        {
            this.logger.LogInformation("Home Controller / ChangeSubscriptionPlan  subscriptionDetail:{0}", JsonSerializer.Serialize(subscriptionDetail));
            try
            {
                var subscriptionId = Guid.Empty;
                var planId = string.Empty;

                if (subscriptionDetail != null)
                {
                    subscriptionId = subscriptionDetail.Id;
                    planId = subscriptionDetail.PlanId;
                }

                if (subscriptionId != default && !string.IsNullOrEmpty(planId))
                {
                    try
                    {
                        var currentUserId = this.userService.GetUserIdFromEmailAddress(this.CurrentUserEmailAddress);

                        var jsonResult = await this.fulfillApiClient.ChangePlanForSubscriptionAsync(subscriptionId, planId).ConfigureAwait(false);

                        var changePlanOperationStatus = OperationStatusEnum.InProgress;
                        if (jsonResult != null && jsonResult.OperationId != default)
                        {
                            while (OperationStatusEnum.InProgress.Equals(changePlanOperationStatus) || OperationStatusEnum.NotStarted.Equals(changePlanOperationStatus))
                            {
                                var changePlanOperationResult = await this.fulfillApiClient.GetOperationStatusResultAsync(subscriptionId, jsonResult.OperationId).ConfigureAwait(false);
                                changePlanOperationStatus = changePlanOperationResult.Status;
                                this.logger.LogInformation("Operation Status :  " + changePlanOperationStatus + " For SubscriptionId " + subscriptionId + "Model SubscriptionID): {0} :: planID:{1}", JsonSerializer.Serialize(subscriptionId), JsonSerializer.Serialize(planId));
                                this.applicationLogService.AddApplicationLog("Operation Status :  " + changePlanOperationStatus + " For SubscriptionId " + subscriptionId);
                            }

                            var oldValue = this.subscriptionService.GetSubscriptionsBySubscriptionId(subscriptionId);
                            this.subscriptionService.UpdateSubscriptionPlan(subscriptionId, planId);
                            this.logger.LogInformation("Plan Successfully Changed.");
                            this.applicationLogService.AddApplicationLog("Plan Successfully Changed.");

                            if (oldValue != null)
                            {
                                SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
                                {
                                    Attribute = Convert.ToString(SubscriptionLogAttributes.Plan),
                                    SubscriptionId = oldValue.SubscribeId,
                                    NewValue = planId,
                                    OldValue = oldValue.PlanId,
                                    CreateBy = currentUserId,
                                    CreateDate = DateTime.Now,
                                };
                                this.subscriptionLogRepository.Save(auditLog);
                            }
                        }
                    }
                    catch (FulfillmentException fex)
                    {
                        this.TempData["ErrorMsg"] = fex.Message;
                    }
                }

                return this.RedirectToAction(nameof(this.Subscriptions));
            }
            catch (Exception ex)
            {
                this.logger.LogError("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                return this.View("Error", ex);
            }
        }

        /// <summary>
        /// Changes the quantity plan.
        /// </summary>
        /// <param name="subscriptionDetail">The subscription detail.</param>
        /// <returns>Changes subscription quantity.</returns>
        [HttpPost]
        public async Task<IActionResult> ChangeSubscriptionQuantity(SubscriptionResult subscriptionDetail)
        {
            this.logger.LogInformation("Home Controller / ChangeSubscriptionPlan  subscriptionDetail:{0}", JsonSerializer.Serialize(subscriptionDetail));
            if (this.User.Identity.IsAuthenticated)
            {
                try
                {
                    if (subscriptionDetail != null && subscriptionDetail.Id != default && subscriptionDetail.Quantity > 0)
                    {
                        try
                        {
                            var subscriptionId = subscriptionDetail.Id;
                            var quantity = subscriptionDetail.Quantity;

                            var currentUserId = this.userService.GetUserIdFromEmailAddress(this.CurrentUserEmailAddress);

                            var jsonResult = await this.fulfillApiClient.ChangeQuantityForSubscriptionAsync(subscriptionId, quantity).ConfigureAwait(false);

                            var changeQuantityOperationStatus = OperationStatusEnum.InProgress;
                            if (jsonResult != null && jsonResult.OperationId != default)
                            {
                                while (OperationStatusEnum.InProgress.Equals(changeQuantityOperationStatus) || OperationStatusEnum.NotStarted.Equals(changeQuantityOperationStatus))
                                {
                                    var changeQuantityOperationResult = await this.fulfillApiClient.GetOperationStatusResultAsync(subscriptionId, jsonResult.OperationId).ConfigureAwait(false);
                                    changeQuantityOperationStatus = changeQuantityOperationResult.Status;

                                    this.logger.LogInformation("changeQuantity Operation Status :  " + changeQuantityOperationStatus + " For SubscriptionId " + subscriptionId + "Model SubscriptionID): {0} :: quantity:{1}", JsonSerializer.Serialize(subscriptionId), JsonSerializer.Serialize(quantity));
                                    this.applicationLogService.AddApplicationLog("Operation Status :  " + changeQuantityOperationStatus + " For SubscriptionId " + subscriptionId);
                                }

                                var oldValue = this.subscriptionService.GetSubscriptionsBySubscriptionId(subscriptionId, true);

                                this.subscriptionService.UpdateSubscriptionQuantity(subscriptionId, quantity);
                                this.logger.LogInformation("Quantity Successfully Changed.");
                                this.applicationLogService.AddApplicationLog("Quantity Successfully Changed.");

                                if (oldValue != null)
                                {
                                    SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
                                    {
                                        Attribute = Convert.ToString(SubscriptionLogAttributes.Quantity),
                                        SubscriptionId = oldValue.SubscribeId,
                                        NewValue = quantity.ToString(),
                                        OldValue = oldValue.Quantity.ToString(),
                                        CreateBy = currentUserId,
                                        CreateDate = DateTime.Now,
                                    };
                                    this.subscriptionLogRepository.Save(auditLog);
                                }
                            }
                        }
                        catch (FulfillmentException fex)
                        {
                            this.TempData["ErrorMsg"] = fex.Message;
                            this.logger.LogError("Message:{0} :: {1}   ", fex.Message, fex.InnerException);
                        }
                    }

                    return this.RedirectToAction(nameof(this.Subscriptions));
                }
                catch (Exception ex)
                {
                    this.logger.LogError("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                    return this.View("Error", ex);
                }
            }
            else
            {
                return this.RedirectToAction(nameof(this.Index));
            }
        }
    }
}