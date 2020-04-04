namespace Microsoft.Marketplace.Saas.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Logging;
    using Microsoft.Marketplace.Saas.Web.Models;
    using Microsoft.Marketplace.Saas.Web.Services;
    using Microsoft.Marketplace.SaaS.SDK.PublisherSolution.Utilities;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using Microsoft.Marketplace.SaasKit.Web.Helpers;
    using Microsoft.Marketplace.SaasKit.Client.Models;
    using Microsoft.Marketplace.SaasKit.Client.Services;
    using Microsoft.Marketplace.SaasKit.Contracts;
    using Microsoft.Marketplace.SaasKit.Exceptions;
    using Microsoft.Marketplace.SaasKit.Models;
    using Newtonsoft.Json;

    [ServiceFilter(typeof(KnownUser))]
    /// <summary>
    /// Home Controller
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.Saas.Web.Controllers.BaseController" />
    public class HomeController : BaseController
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<HomeController> _logger;

        /// <summary>
        /// The subscription repository
        /// </summary>
        private readonly ISubscriptionsRepository subscriptionRepo;

        /// <summary>
        /// The subscription logs repository
        /// </summary>
        private readonly ISubscriptionLogRepository subscriptionLogRepository;

        /// <summary>
        /// The plan repository
        /// </summary>
        private readonly IPlansRepository planRepository;


        /// <summary>
        /// The Metered Dimension repository
        /// </summary>
        private readonly IMeteredDimensionsRepository dimensionsRepository;

        /// <summary>
        /// The subscription usage logs repository
        /// </summary>
        private readonly ISubscriptionUsageLogsRepository subscriptionUsageLogsRepository;

        /// <summary>
        /// The users repository
        /// </summary>
        private readonly IUsersRepository userRepository;

        private readonly IUsersRepository usersRepository;

        private readonly IFulfillmentApiClient fulfillApiClient;

        private UserService userService;

        private WebSubscriptionService webSubscriptionService = null;

        private readonly IApplicationLogRepository applicationLogRepository;

        /// <summary>
        /// Defines the  API Client
        /// </summary>
        private readonly IMeteredBillingApiClient apiClient;

        private readonly IApplicationConfigRepository applicationConfigRepository;

        private ApplicationLogService applicationLogService = null;

        private SubscriptionService subscriptionService = null;

        private readonly ISubscriptionsRepository subscriptionRepository;

        private readonly IEmailTemplateRepository emailTemplateRepository;




        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController" /> class.
        /// </summary>
        /// <param name="UsersRepository">The users repository.</param>
        /// <param name="apiClient">The API client.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="SubscriptionRepo">The subscription repo.</param>
        /// <param name="PlanRepository">The plan repository.</param>
        /// <param name="SubscriptionUsageLogsRepository">The subscription usage logs repository.</param>
        public HomeController(IUsersRepository UsersRepository, IMeteredBillingApiClient apiClient, ILogger<HomeController> logger, ISubscriptionsRepository SubscriptionRepo,
                                IPlansRepository PlanRepository, ISubscriptionUsageLogsRepository SubscriptionUsageLogsRepository,
                                    IMeteredDimensionsRepository DimensionsRepository, ISubscriptionLogRepository subscriptionLogsRepo, IApplicationConfigRepository applicationConfigRepository, IUsersRepository userRepository, IFulfillmentApiClient fulfillApiClient,  IApplicationLogRepository applicationLogRepository, IEmailTemplateRepository emailTemplateRepository)
        {
            this.apiClient = apiClient;
            subscriptionRepo = SubscriptionRepo;
            this.subscriptionLogRepository = subscriptionLogsRepo;
            planRepository = PlanRepository;
            subscriptionUsageLogsRepository = SubscriptionUsageLogsRepository;
            dimensionsRepository = DimensionsRepository;
            _logger = logger;
            this.applicationConfigRepository = applicationConfigRepository;
            usersRepository = UsersRepository;
            this.userRepository = userRepository;
            this.userService = new UserService(userRepository);
            this.fulfillApiClient = fulfillApiClient;
            webSubscriptionService = new WebSubscriptionService(this.subscriptionRepo, this.planRepository);
            this.applicationLogRepository = applicationLogRepository;
            this.applicationLogService = new ApplicationLogService(this.applicationLogRepository);
            this.applicationConfigRepository = applicationConfigRepository;
            this.subscriptionRepository = subscriptionRepo;
            this.subscriptionService = new SubscriptionService(this.subscriptionRepository, this.planRepository);
            this.emailTemplateRepository = emailTemplateRepository;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            if (Convert.ToBoolean(applicationConfigRepository.GetValuefromApplicationConfig(MainMenuStatusEnum.IsLicenseManagementEnabled.ToString())) == true)
            {
                this.TempData["ShowLicensesMenu"] = true;
            }
            return View();
        }

        /// <summary>
        /// Subscriptionses this instance.
        /// </summary>
        /// <returns></returns>
        public IActionResult Subscriptions()
        {
            if (Convert.ToBoolean(applicationConfigRepository.GetValuefromApplicationConfig(MainMenuStatusEnum.IsLicenseManagementEnabled.ToString())) == true)
            {
                this.TempData["ShowLicensesMenu"] = true;
            }
            SubscriptionViewModel subscriptionDetail = new SubscriptionViewModel();
            if (User.Identity.IsAuthenticated)
            {
                this.TempData["ShowWelcomeScreen"] = "True";

                List<SubscriptionResult> allSubscriptions = new List<SubscriptionResult>();
                var allSubscriptionDetails = subscriptionRepo.Get().ToList();
                var allPlans = planRepository.Get().ToList();
                foreach (var subscription in allSubscriptionDetails)
                {
                    SubscriptionResult subscritpionDetail = PrepareSubscriptionResponse(subscription, allPlans);
                    subscriptionDetail.SkipActivation = Convert.ToBoolean(applicationConfigRepository.GetValuefromApplicationConfig("SkipActivation"));
                    if (subscritpionDetail != null && subscritpionDetail.SubscribeId > 0)
                        allSubscriptions.Add(subscritpionDetail);
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
                return RedirectToAction(nameof(Index));
            }
            return this.View(subscriptionDetail);
        }

        /// <summary>
        /// Subscriptions the log detail.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns> Subscription log detail</returns>
        public IActionResult SubscriptionLogDetail(Guid subscriptionId)
        {
            if (Convert.ToBoolean(applicationConfigRepository.GetValuefromApplicationConfig(MainMenuStatusEnum.IsLicenseManagementEnabled.ToString())) == true)
            {
                this.TempData["ShowLicensesMenu"] = true;
            }
            if (User.Identity.IsAuthenticated)
            {
                List<SubscriptionAuditLogs> subscriptionAudit = new List<SubscriptionAuditLogs>();
                subscriptionAudit = this.subscriptionLogRepository.GetSubscriptionBySubscriptionId(subscriptionId).ToList();
                return this.View(subscriptionAudit);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }


        /// <summary>
        /// Records the usage.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns></returns>
        public IActionResult RecordUsage(int subscriptionId)
        {
            if (Convert.ToBoolean(applicationConfigRepository.GetValuefromApplicationConfig(MainMenuStatusEnum.IsLicenseManagementEnabled.ToString())) == true)
            {
                this.TempData["ShowLicensesMenu"] = true;
            }
            if (User.Identity.IsAuthenticated)
            {
                var subscriptionDetail = subscriptionRepo.Get(subscriptionId);
                var allDimensionsList = dimensionsRepository.GetDimensionsFromPlanId(subscriptionDetail.AmpplanId);
                SubscriptionUsageViewModel usageViewModel = new SubscriptionUsageViewModel();
                usageViewModel.SubscriptionDetail = subscriptionDetail;
                usageViewModel.MeteredAuditLogs = new List<MeteredAuditLogs>();
                usageViewModel.MeteredAuditLogs = subscriptionUsageLogsRepository.GetMeteredAuditLogsBySubscriptionId(subscriptionId).OrderByDescending(s => s.CreatedDate).ToList();
                usageViewModel.DimensionsList = new SelectList(allDimensionsList, "Dimension", "Description");
                return View(usageViewModel);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Manages the subscription usage.
        /// </summary>
        /// <param name="subscriptionData">The subscription data.</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ManageSubscriptionUsage(SubscriptionUsageViewModel subscriptionData)
        {
            try
            {
                if (Convert.ToBoolean(applicationConfigRepository.GetValuefromApplicationConfig(MainMenuStatusEnum.IsLicenseManagementEnabled.ToString())) == true)
                {
                    this.TempData["ShowLicensesMenu"] = true;
                }
                if (subscriptionData != null && subscriptionData.SubscriptionDetail != null)
                {
                    var currentUserDetail = usersRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);
                    var subscriptionUsageRequest = new MeteringUsageRequest()
                    {
                        Dimension = subscriptionData.SelectedDimension,
                        EffectiveStartTime = DateTime.UtcNow,
                        PlanId = subscriptionData.SubscriptionDetail.AmpplanId,
                        Quantity = Convert.ToDouble(subscriptionData.Quantity ?? "0"),
                        ResourceId = subscriptionData.SubscriptionDetail.AmpsubscriptionId
                    };
                    var meteringUsageResult = new MeteringUsageResult();
                    var requestJson = JsonConvert.SerializeObject(subscriptionUsageRequest);
                    var responseJson = string.Empty;
                    try
                    {
                        meteringUsageResult = apiClient.EmitUsageEventAsync(subscriptionUsageRequest).ConfigureAwait(false).GetAwaiter().GetResult();
                        responseJson = JsonConvert.SerializeObject(meteringUsageResult);
                    }
                    catch (MeteredBillingException mex)
                    {
                        responseJson = JsonConvert.SerializeObject(mex.MeteredBillingErrorDetail);
                        meteringUsageResult.Status = mex.ErrorCode;
                    }

                    var newMeteredAuditLog = new MeteredAuditLogs()
                    {
                        RequestJson = requestJson,
                        ResponseJson = responseJson,
                        StatusCode = meteringUsageResult.Status,
                        SubscriptionId = subscriptionData.SubscriptionDetail.Id,
                        SubscriptionUsageDate = DateTime.UtcNow,
                        CreatedBy = currentUserDetail == null ? 0 : currentUserDetail.UserId,
                        CreatedDate = DateTime.Now
                    };
                    subscriptionUsageLogsRepository.Add(newMeteredAuditLog);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, ex.Message);
            }
            return RedirectToAction(nameof(RecordUsage), new { subscriptionId = subscriptionData.SubscriptionDetail.Id });
        }

        public IActionResult ActivateSubscription(Guid subscriptionId, string planId)
        {
            //this.log.Info("Initializing Index Page");
            SubscriptionResult subscriptionDetail = new SubscriptionResult();

            if (User.Identity.IsAuthenticated)
            {
                var userId = this.userService.AddPartnerDetail(GetCurrentUserDetail());
                var currentUserId = this.userService.GetUserIdFromEmailAddress(this.CurrentUserEmailAddress);
                this.webSubscriptionService = new WebSubscriptionService(this.subscriptionRepo, this.planRepository, userId);

                //this.log.Info("User authenticate successfully");

                this.TempData["ShowWelcomeScreen"] = false;
                var subscriptionData = this.fulfillApiClient.GetSubscriptionByIdAsync(subscriptionId).ConfigureAwait(false).GetAwaiter().GetResult();
                var subscribeId = this.webSubscriptionService.AddUpdatePartnerSubscriptions(subscriptionData);
                var oldValue = this.webSubscriptionService.GetSubscriptionsForSubscriptionId(subscriptionId);

                var serializedParent = JsonConvert.SerializeObject(subscriptionData);
                subscriptionDetail = JsonConvert.DeserializeObject<SubscriptionResult>(serializedParent);
                //subscriptionDetail = (SubscriptionResultExtension)subscriptionData;
                subscriptionDetail.ShowWelcomeScreen = false;
                subscriptionDetail.SaasSubscriptionStatus = SubscriptionStatusEnum.PendingFulfillmentStart;
                subscriptionDetail.CustomerEmailAddress = oldValue.CustomerEmailAddress;
                subscriptionDetail.CustomerName = oldValue.CustomerName;
            }
            return this.View(subscriptionDetail);
        }

        public IActionResult DeActivateSubscription(Guid subscriptionId, string planId, string operation)
        {
            try
            {
                if (Convert.ToBoolean(applicationConfigRepository.GetValuefromApplicationConfig(MainMenuStatusEnum.IsLicenseManagementEnabled.ToString())) == true)
                {
                    this.TempData["ShowLicensesMenu"] = true;
                }
                SubscriptionResult subscriptionDetail = new SubscriptionResult();

                if (User.Identity.IsAuthenticated)
                {
                    var userId = this.userService.AddPartnerDetail(GetCurrentUserDetail());
                    var currentUserId = this.userService.GetUserIdFromEmailAddress(this.CurrentUserEmailAddress);
                    this.subscriptionService = new SubscriptionService(this.subscriptionRepository, this.planRepository, userId);


                    this.TempData["ShowWelcomeScreen"] = false;
                    var subscriptionData = this.fulfillApiClient.GetSubscriptionByIdAsync(subscriptionId).ConfigureAwait(false).GetAwaiter().GetResult();
                    var subscribeId = this.subscriptionService.AddUpdatePartnerSubscriptions(subscriptionData);
                    var oldValue = this.subscriptionService.GetPartnerSubscription(CurrentUserEmailAddress, subscriptionId).FirstOrDefault();

                    //var serializedParent = JsonConvert.SerializeObject(subscriptionData);
                    //subscriptionDetail = JsonConvert.DeserializeObject<SubscriptionResult>(serializedParent);
                    //subscriptionDetail = (SubscriptionResult)subscriptionData;
                    subscriptionDetail = subscriptionData;
                    subscriptionDetail.ShowWelcomeScreen = false;
                    subscriptionDetail.SaasSubscriptionStatus = SubscriptionStatusEnum.Subscribed;
                    subscriptionDetail.CustomerEmailAddress = this.CurrentUserEmailAddress;
                    subscriptionDetail.CustomerName = this.CurrentUserName;
                }
                return this.View("ActivateSubscription", subscriptionDetail);
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }

        public IActionResult SubscriptionOperation(Guid subscriptionId, string planId, string operation, int NumberofProviders)
        {
            bool isSuccess = false;
            if (subscriptionId != default)
            {
                SubscriptionResult subscriptionDetail = new SubscriptionResult();
                var oldValue = this.webSubscriptionService.GetSubscriptionsForSubscriptionId(subscriptionId);
                var currentUserId = userService.GetUserIdFromEmailAddress(this.CurrentUserEmailAddress);

                if (operation == "Activate")
                {

                    var response = this.fulfillApiClient.ActivateSubscriptionAsync(subscriptionId, planId).ConfigureAwait(false).GetAwaiter().GetResult();
                    this.webSubscriptionService.UpdateStateOfSubscription(subscriptionId, SubscriptionStatusEnum.Subscribed, true);

                    isSuccess = true;
                    subscriptionDetail = this.webSubscriptionService.GetSubscriptionsForSubscriptionId(subscriptionId);
                    subscriptionDetail.PlanList = this.webSubscriptionService.GetAllSubscriptionPlans();
                    var subscriptionData = this.fulfillApiClient.GetSubscriptionByIdAsync(subscriptionId).ConfigureAwait(false).GetAwaiter().GetResult();
                    bool checkIsActive = emailTemplateRepository.GetIsActive(subscriptionDetail.SaasSubscriptionStatus.ToString()).HasValue ? emailTemplateRepository.GetIsActive(subscriptionDetail.SaasSubscriptionStatus.ToString()).Value : false;
                    if (Convert.ToBoolean(applicationConfigRepository.GetValuefromApplicationConfig(EmailTriggerStatusEnum.IsEmailEnabledForSubscriptionActivation.ToString())) == true)
                    {
                        EmailHelper.SendEmail(subscriptionDetail, applicationConfigRepository, emailTemplateRepository);
                    }
                }

                if (operation == "Deactivate")
                {
                    try
                    {
                        var response = this.fulfillApiClient.DeleteSubscriptionAsync(subscriptionId, planId).ConfigureAwait(false).GetAwaiter().GetResult();
                        this.webSubscriptionService.UpdateStateOfSubscription(subscriptionId, SubscriptionStatusEnum.Unsubscribed, false);
                        subscriptionDetail.SaasSubscriptionStatus = SubscriptionStatusEnum.Unsubscribed;
                        isSuccess = true;
                        bool checkIsActive = emailTemplateRepository.GetIsActive(subscriptionDetail.SaasSubscriptionStatus.ToString()).HasValue ? emailTemplateRepository.GetIsActive(subscriptionDetail.SaasSubscriptionStatus.ToString()).Value : false;
                        if (Convert.ToBoolean(applicationConfigRepository.GetValuefromApplicationConfig(EmailTriggerStatusEnum.IsEmailEnabledForSubscriptionActivation.ToString())) == true)
                        {
                            EmailHelper.SendEmail(subscriptionDetail, applicationConfigRepository, emailTemplateRepository);
                        }
                    }
                    catch (FulfillmentException fex)
                    {
                        this._logger.LogError($"Deactive Subscription plan Error - {fex.Message} with StackTrace- {fex.StackTrace}.");
                        this.TempData["ErrorMsg"] = fex.Message;
                    }
                }

                var newValue = this.webSubscriptionService.GetSubscriptionsForSubscriptionId(subscriptionId, true);
                if (isSuccess)
                {
                    if (oldValue != null && newValue != null)
                    {
                        SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
                        {
                            Attribute = Convert.ToString(SubscriptionLogAttributes.Status),
                            SubscriptionId = newValue.SubscribeId,
                            NewValue = Convert.ToString(newValue.SaasSubscriptionStatus),
                            OldValue = Convert.ToString(oldValue.SaasSubscriptionStatus),
                            CreateBy = currentUserId,
                            CreateDate = DateTime.Now
                        };
                        this.subscriptionLogRepository.Add(auditLog);

                        //auditLog = new SubscriptionAuditLogs()
                        //{
                        //    Attribute = Convert.ToString(SubscriptionLogAttributes.ProviderCount),
                        //    SubscriptionId = newValue.SubscribeId,
                        //    NewValue = Convert.ToString(newValue.NumberofProviders),
                        //    OldValue = Convert.ToString(oldValue.NumberofProviders),
                        //    CreateBy = currentUserId,
                        //    CreateDate = DateTime.Now
                        //};
                        //this.subscriptionLogRepository.Add(auditLog);
                    }
                }
            }
            return this.RedirectToAction(nameof(this.ActivatedMessage));
        }

        public IActionResult ActivatedMessage()
        {
            try
            {
                return this.View();
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }

        /// <summary>
        /// Prepares the subscription response.
        /// </summary>
        /// <param name="subscription">The subscription.</param>
        /// <param name="allPlanDetails">All plan details.</param>
        /// <returns></returns>
        private SubscriptionResult PrepareSubscriptionResponse(Subscriptions subscription, List<Plans> allPlanDetails)
        {
            SubscriptionResult subscritpionDetail = new SubscriptionResult();
            subscritpionDetail.Id = subscription.AmpsubscriptionId;
            subscritpionDetail.SubscribeId = subscription.Id;
            subscritpionDetail.PlanId = string.IsNullOrEmpty(subscription.AmpplanId) ? string.Empty : subscription.AmpplanId;
            subscritpionDetail.Name = subscription.Name;
            subscritpionDetail.SaasSubscriptionStatus = GetSubscriptionStatus(subscription.SubscriptionStatus);
            subscritpionDetail.IsActiveSubscription = subscription.IsActive ?? false;
            subscritpionDetail.CustomerName = subscription.User?.FullName;
            subscritpionDetail.CustomerEmailAddress = subscription.User?.EmailAddress;
            var existingPlanDetail = allPlanDetails.Where(s => s.PlanId == subscritpionDetail.PlanId).FirstOrDefault();
            subscritpionDetail.IsMeteringSupported = existingPlanDetail != null ? (existingPlanDetail.IsmeteringSupported ?? false) : false;

            return subscritpionDetail;
        }

        /// <summary>
        /// Get All Subscription List for Current Logged in User
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns>
        /// The <see cref="IActionResult" />
        /// </returns>
        public IActionResult SubscriptionDetail(Guid subscriptionId)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Convert.ToBoolean(applicationConfigRepository.GetValuefromApplicationConfig(MainMenuStatusEnum.IsLicenseManagementEnabled.ToString())) == true)
                {
                    this.TempData["ShowLicensesMenu"] = true;
                }
                var subscriptionDetail = this.subscriptionService.GetPartnerSubscription(CurrentUserEmailAddress, subscriptionId).FirstOrDefault();
                subscriptionDetail.PlanList = this.subscriptionService.GetAllSubscriptionPlans();

                return this.View(subscriptionDetail);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Gets the subscription status.
        /// </summary>
        /// <param name="subscriptionStatus">The subscription status.</param>
        /// <returns></returns>
        public SubscriptionStatusEnum GetSubscriptionStatus(string subscriptionStatus)
        {
            if (!string.IsNullOrEmpty(subscriptionStatus))
            {
                if (subscriptionStatus.Trim() == Convert.ToString(SubscriptionStatusEnum.NotStarted)) return SubscriptionStatusEnum.NotStarted;
                if (subscriptionStatus.Trim() == Convert.ToString(SubscriptionStatusEnum.PendingFulfillmentStart)) return SubscriptionStatusEnum.PendingFulfillmentStart;
                if (subscriptionStatus.Trim() == Convert.ToString(SubscriptionStatusEnum.Subscribed)) return SubscriptionStatusEnum.Subscribed;
                if (subscriptionStatus.Trim() == Convert.ToString(SubscriptionStatusEnum.Unsubscribed)) return SubscriptionStatusEnum.Unsubscribed;
                if (subscriptionStatus.Trim() == Convert.ToString(SubscriptionStatusEnum.PendingActivation)) return SubscriptionStatusEnum.PendingActivation;

            }
            return SubscriptionStatusEnum.NotStarted;
        }

        /// <summary>
        /// Privacies this instance.
        /// </summary>
        /// <returns></returns>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// The Error
        /// </summary>
        /// <returns>
        /// The <see cref="IActionResult" />
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
        /// <returns>Changes subscription plan</returns>
        [HttpPost]
        public async Task<IActionResult> ChangeSubscriptionPlan(SubscriptionResult subscriptionDetail)
        {
            var subscriptionId = new Guid();
            var planId = string.Empty;
            if (Convert.ToBoolean(applicationConfigRepository.GetValuefromApplicationConfig(MainMenuStatusEnum.IsLicenseManagementEnabled.ToString())) == true)
            {
                this.TempData["ShowLicensesMenu"] = true;
            }
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
                            this.applicationLogService.AddApplicationLog("Operation Status :  " + changePlanOperationStatus + " For SubscriptionId " + subscriptionId);
                        }

                        var oldValue = this.subscriptionService.GetSubscriptionsForSubscriptionId(subscriptionId);

                        this.subscriptionService.UpdateSubscriptionPlan(subscriptionId, planId);
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
                                CreateDate = DateTime.Now
                            };
                            this.subscriptionLogRepository.Add(auditLog);
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
    }
}
