namespace Microsoft.Marketplace.SaasKit.Client.Controllers
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.OpenIdConnect;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Marketplace.SaaS.SDK.Services.Contracts;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using Microsoft.Marketplace.SaaS.SDK.Services.Services;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using Microsoft.Marketplace.SaasKit.Contracts;
    using Microsoft.Marketplace.SaasKit.Models;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using SaasKitModels = Microsoft.Marketplace.SaasKit.Models;

    /// <summary>Home Controller</summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Web.Controllers.BaseController"/>
    public class HomeController : BaseController
    {
        /// <summary>
        /// Defines the  API Client
        /// </summary>
        private readonly IFulfillmentApiClient apiClient;

        /// <summary>
        /// The subscription repository
        /// </summary>
        private readonly ISubscriptionsRepository subscriptionRepository;

        /// <summary>
        /// The subscription logs repository
        /// </summary>
        private readonly ISubscriptionLogRepository subscriptionLogRepository;

        /// <summary>
        /// The application log repository
        /// </summary>
        private readonly IApplicationLogRepository applicationLogRepository;

        /// <summary>
        /// The plan repository
        /// </summary>
        private readonly IPlansRepository planRepository;

        /// <summary>
        /// The plan repository
        /// </summary>
        private readonly IOffersRepository offersRepository;

        /// <summary>
        /// The user repository
        /// </summary>
        private readonly IUsersRepository userRepository;

        private readonly ILogger<HomeController> logger;
        /// <summary>
        /// The subscription service
        /// </summary>
        private SubscriptionService subscriptionService = null;

        /// <summary>
        /// The application log service
        /// </summary>
        private ApplicationLogService applicationLogService = null;

        private PlanService planService = null;
        /// <summary>
        /// The user service
        /// </summary>
        private UserService userService;

        private readonly IApplicationConfigRepository applicationConfigRepository;

        private readonly IEmailTemplateRepository emailTemplateRepository;

        private readonly IPlanEventsMappingRepository planEventsMappingRepository;

        private readonly IOfferAttributesRepository offerAttributesRepository;

        private readonly IEventsRepository eventsRepository;
        private readonly CloudStorageConfigs cloudConfigs;

        private readonly IVaultService keyVaultClient;

        private string azureWebJobsStorage;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController" /> class.
        /// </summary>
        /// <param name="apiClient">The API Client<see cref="IFulfilmentApiClient" /></param>
        /// <param name="subscriptionRepo">The subscription repository.</param>
        /// <param name="planRepository">The plan repository.</param>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="applicationLogRepository">The application log repository.</param>
        /// <param name="subscriptionLogsRepo">The subscription logs repository.</param>
        public HomeController(ILogger<HomeController> logger, IFulfillmentApiClient apiClient, ISubscriptionsRepository subscriptionRepo, IPlansRepository planRepository, IUsersRepository userRepository, IApplicationLogRepository applicationLogRepository, ISubscriptionLogRepository subscriptionLogsRepo, IApplicationConfigRepository applicationConfigRepository, IEmailTemplateRepository emailTemplateRepository, IOffersRepository offersRepository, IPlanEventsMappingRepository planEventsMappingRepository, IOfferAttributesRepository offerAttributesRepository, IEventsRepository eventsRepository, CloudStorageConfigs cloudConfigs, IVaultService keyVaultClient)
        {
            this.apiClient = apiClient;
            this.subscriptionRepository = subscriptionRepo;
            this.subscriptionLogRepository = subscriptionLogsRepo;
            this.applicationLogRepository = applicationLogRepository;
            this.planRepository = planRepository;
            this.userRepository = userRepository;
            this.userService = new UserService(this.userRepository);
            this.subscriptionService = new SubscriptionService(this.subscriptionRepository, this.planRepository);
            this.applicationLogService = new ApplicationLogService(this.applicationLogRepository);
            this.applicationConfigRepository = applicationConfigRepository;
            this.emailTemplateRepository = emailTemplateRepository;
            this.planEventsMappingRepository = planEventsMappingRepository;
            this.offerAttributesRepository = offerAttributesRepository;
            this.logger = logger;
            this.offersRepository = offersRepository;
            this.planService = new PlanService(this.planRepository, this.offerAttributesRepository, this.offersRepository);
            this.eventsRepository = eventsRepository;
            this.cloudConfigs = cloudConfigs;
            azureWebJobsStorage = cloudConfigs.AzureWebJobsStorage;
            this.keyVaultClient = keyVaultClient;
        }

        #region View Action Methods

        /// <summary>
        /// Get All Subscription List for Current Logged in User
        /// </summary>
        /// <param name="token">The MS Token<see cref="string" /></param>
        /// <returns>
        /// The <see cref="IActionResult" />
        /// </returns>
        public IActionResult Index(string token = null)
        {
            try
            {
                this.logger.LogInformation($"Landing page with token {token}");
                SubscriptionResult subscriptionDetail = new SaasKitModels.SubscriptionResult();
                SubscriptionResultExtension subscriptionExtension = new SubscriptionResultExtension();

                if (User.Identity.IsAuthenticated)
                {
                    if (Convert.ToBoolean(applicationConfigRepository.GetValueByName(MainMenuStatusEnum.IsLicenseManagementEnabled.ToString())) == true)
                    {
                        this.TempData["ShowLicensesMenu"] = true;
                    }
                    var userId = this.userService.AddUser(GetCurrentUserDetail());
                    var currentUserId = this.userService.GetUserIdFromEmailAddress(this.CurrentUserEmailAddress);
                    this.subscriptionService = new SubscriptionService(this.subscriptionRepository, this.planRepository, userId);
                    this.logger.LogInformation("User authenticated successfully");
                    if (!string.IsNullOrEmpty(token))
                    {
                        this.TempData["ShowWelcomeScreen"] = null;
                        token = token.Replace(' ', '+');
                        var newSubscription = this.apiClient.ResolveAsync(token).ConfigureAwait(false).GetAwaiter().GetResult();
                        if (newSubscription != null && newSubscription.SubscriptionId != default)
                        {
                            var subscriptionPlanDetail = this.apiClient.GetAllPlansForSubscriptionAsync(newSubscription.SubscriptionId).ConfigureAwait(false).GetAwaiter().GetResult();
                            Offers offers = new Offers()
                            {
                                OfferId = newSubscription.OfferId,
                                OfferName = newSubscription.OfferId,
                                UserId = currentUserId,
                                CreateDate = DateTime.Now,
                                OfferGuid = Guid.NewGuid()
                            };
                            Guid newOfferId = this.offersRepository.Add(offers);
                            List<PlanDetailResultExtension> planList = new List<PlanDetailResultExtension>();
                            var serializedPlans = JsonConvert.SerializeObject(subscriptionPlanDetail);
                            planList = JsonConvert.DeserializeObject<List<PlanDetailResultExtension>>(serializedPlans);
                            planList.ForEach(x =>
                            {
                                x.OfferId = newOfferId;
                                x.PlanGUID = Guid.NewGuid();
                            });
                            this.subscriptionService.AddPlanDetailsForSubscription(planList);
                            var deploymentAttributes = this.offerAttributesRepository.GetDeploymentParameters();
                            if (deploymentAttributes != null && deploymentAttributes.Count() > 0)
                            {
                                var attribures = this.offerAttributesRepository.AddDeploymentAttributes(newOfferId, currentUserId, deploymentAttributes.ToList());
                                var allPlansOfSubscription = this.planRepository.GetPlansByOfferId(newOfferId);

                                foreach (var plan in allPlansOfSubscription)
                                {
                                    var deploymentAttributesofPlan = this.planService.SavePlanDeploymentAttributes(plan, currentUserId);
                                }
                            }
                            var currentPlan = this.planRepository.GetById(newSubscription.PlanId);
                            var subscriptionData = this.apiClient.GetSubscriptionByIdAsync(newSubscription.SubscriptionId).ConfigureAwait(false).GetAwaiter().GetResult();
                            var subscribeId = this.subscriptionService.AddOrUpdatePartnerSubscriptions(subscriptionData);
                            if (subscribeId > 0 && subscriptionData.SaasSubscriptionStatus == SubscriptionStatusEnum.PendingFulfillmentStart)
                            {
                                SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
                                {
                                    Attribute = Convert.ToString(SubscriptionLogAttributes.Status),
                                    SubscriptionId = subscribeId,
                                    NewValue = SubscriptionStatusEnum.PendingFulfillmentStart.ToString(),
                                    OldValue = "None",
                                    CreateBy = currentUserId,
                                    CreateDate = DateTime.Now
                                };
                                this.subscriptionLogRepository.Save(auditLog);
                            }
                            subscriptionExtension = this.subscriptionService.GetSubscriptionsBySubscriptionId(newSubscription.SubscriptionId);
                            subscriptionExtension.ShowWelcomeScreen = false;
                            subscriptionExtension.CustomerEmailAddress = this.CurrentUserEmailAddress;
                            subscriptionExtension.CustomerName = this.CurrentUserName;
                            subscriptionExtension.SubscriptionParameters = this.subscriptionService.GetSubscriptionsParametersById(newSubscription.SubscriptionId, currentPlan.PlanGuid);
                            subscriptionExtension.DeployToCustomerSubscription = currentPlan.DeployToCustomerSubscription ?? false;
                        }
                    }
                    else
                    {
                        this.TempData["ShowWelcomeScreen"] = "True";
                        subscriptionExtension.ShowWelcomeScreen = true;
                        return this.View(subscriptionExtension);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(token))
                    {
                        return this.Challenge(new AuthenticationProperties
                        {
                            RedirectUri = "/?token=" + token
                        }, OpenIdConnectDefaults.AuthenticationScheme);
                    }
                    else
                    {
                        this.TempData["ShowWelcomeScreen"] = "True";
                        subscriptionExtension.ShowWelcomeScreen = true;
                        return this.View(subscriptionExtension);
                    }
                }
                return this.View(subscriptionExtension);
            }
            catch (Exception ex)
            {
                this.logger.LogError("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                return View("Error", ex);
            }
        }

        [HttpPost]
        public IActionResult ValidateUserParameters(SubscriptionResultExtension subscriptionResultExtension)
        {
            if (subscriptionResultExtension.SubscriptionParameters != null && subscriptionResultExtension.SubscriptionParameters.Count() > 0)
            {
                var deploymentParms = subscriptionResultExtension.SubscriptionParameters.ToList().Where(s => s.Type.ToLower() == "deployment").ToList();
                IDictionary<string, string> parms = new Dictionary<string, string>();
                foreach (var parm in deploymentParms)
                {
                    parms.Add(parm.DisplayName, parm.Value);
                }
                bool isFileSupported =this.keyVaultClient.ValidateUserParameters(parms);
                if (!isFileSupported)
                {
                    return Json(new { status = false, responseText = "Invalid Credentials." });
                }
                else
                {
                    return Json(new { status = true, responseText = "Valid Credentials" });
                }
            }
            else
            {
                return Json(new { status = false, responseText = "Enter Valid Credentials" });
            }
        }

        /// <summary>
        /// Subscription this instance.
        /// </summary>
        /// <returns> Subscription instance</returns>
        public IActionResult Subscriptions()
        {
            logger.LogInformation("Home Controller / Subscriptions ");
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    if (Convert.ToBoolean(applicationConfigRepository.GetValueByName(MainMenuStatusEnum.IsLicenseManagementEnabled.ToString())) == true)
                    {
                        this.TempData["ShowLicensesMenu"] = true;
                    }
                    this.TempData["ShowWelcomeScreen"] = "True";
                    SubscriptionViewModel subscriptionDetail = new SubscriptionViewModel();
                    subscriptionDetail.Subscriptions = this.subscriptionService.GetPartnerSubscription(CurrentUserEmailAddress, default, true).ToList();
                    foreach (var subscription in subscriptionDetail.Subscriptions)
                    {
                        Plans PlanDetail = this.planRepository.GetById(subscription.PlanId);
                        subscriptionDetail.IsAutomaticProvisioningSupported = Convert.ToBoolean(applicationConfigRepository.GetValueByName("IsAutomaticProvisioningSupported"));
                        subscription.IsPerUserPlan = PlanDetail.IsPerUser.HasValue ? PlanDetail.IsPerUser.Value : false;
                    }
                    subscriptionDetail.SaaSAppUrl = this.apiClient.GetSaaSAppURL();

                    if (this.TempData["ErrorMsg"] != null)
                    {
                        subscriptionDetail.IsSuccess = false;
                        subscriptionDetail.ErrorMessage = Convert.ToString(this.TempData["ErrorMsg"]);
                    }
                    return this.View(subscriptionDetail);
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                return View("Error", ex);
            }
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
            this.logger.LogInformation("Home Controller / SubscriptionDetail subscriptionId:{0}", JsonConvert.SerializeObject(subscriptionId));
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    if (Convert.ToBoolean(applicationConfigRepository.GetValueByName(MainMenuStatusEnum.IsLicenseManagementEnabled.ToString())) == true)
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
            catch (Exception ex)
            {
                this.logger.LogError("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                return View("Error", ex);
            }
        }

        /// <summary>
        /// Get Subscription Details for selected Subscription
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns>
        /// The <see cref="IActionResult" />
        /// </returns>
        public IActionResult SubscriptionQuantityDetail(Guid subscriptionId)
        {
            this.logger.LogInformation("Home Controller / SubscriptionQuantityDetail subscriptionId:{0}", JsonConvert.SerializeObject(subscriptionId));
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var subscriptionDetail = this.subscriptionService.GetPartnerSubscription(CurrentUserEmailAddress, subscriptionId).FirstOrDefault();
                    return this.View(subscriptionDetail);
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                return View("Error", ex);
            }
        }

        /// <summary>
        /// Subscriptions the log detail.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns> Subscription log detail</returns>
        public IActionResult SubscriptionLogDetail(Guid subscriptionId)
        {
            this.logger.LogInformation("Home Controller / SubscriptionQuantityDetail subscriptionId:{0}", JsonConvert.SerializeObject(subscriptionId));
            try
            {
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
            catch (Exception ex)
            {
                this.logger.LogError("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                return View("Error", ex);
            }
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
        #endregion

        #region Operation Methods

        public IActionResult ProcessMessage()
        {
            try
            {
                return this.PartialView();
            }
            catch (Exception ex)
            {
                this.logger.LogInformation("Home Controller / ActivatedMessage Exception: {0}", ex);
                return View("Error", ex);
            }
        }

        public IActionResult SubscriptionDetails(Guid subscriptionId, string planId, string operation)
        {
            this.logger.LogInformation("Home Controller / ActivateSubscription subscriptionId:{0} :: planId : {1} :: operation:{2}", JsonConvert.SerializeObject(subscriptionId), JsonConvert.SerializeObject(planId), JsonConvert.SerializeObject(operation));
            try
            {
                if (Convert.ToBoolean(applicationConfigRepository.GetValueByName(MainMenuStatusEnum.IsLicenseManagementEnabled.ToString())) == true)
                {
                    this.TempData["ShowLicensesMenu"] = true;
                }
                SubscriptionResultExtension subscriptionDetail = new SubscriptionResultExtension();
                if (User.Identity.IsAuthenticated)
                {
                    var userId = this.userService.AddUser(GetCurrentUserDetail());
                    var currentUserId = this.userService.GetUserIdFromEmailAddress(this.CurrentUserEmailAddress);
                    this.subscriptionService = new SubscriptionService(this.subscriptionRepository, this.planRepository, userId);
                    this.TempData["ShowWelcomeScreen"] = false;

                    subscriptionDetail = this.subscriptionService.GetSubscriptionsBySubscriptionId(subscriptionId);
                    var planDetails = this.planRepository.GetById(subscriptionDetail.PlanId);
                    var subscriptionParmaeters = this.subscriptionService.GetSubscriptionsParametersById(subscriptionId, planDetails.PlanGuid);
                    var inputParanetrs = subscriptionParmaeters.Where(s => s.Type.ToLower() == "input");
                    if (inputParanetrs != null && inputParanetrs.ToList().Count() > 0)
                    {
                        subscriptionDetail.SubscriptionParameters = inputParanetrs.ToList();
                    }
                    subscriptionDetail.CustomerEmailAddress = this.CurrentUserEmailAddress;
                    subscriptionDetail.CustomerName = this.CurrentUserName;
                }
                return this.View("Index", subscriptionDetail);
            }
            catch (Exception ex)
            {
                this.logger.LogError("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                return View("Error", ex);
            }
        }

        /// <summary>
        /// Changes the subscription plan.
        /// </summary>
        /// <param name="subscriptionDetail">The subscription detail.</param>
        /// <returns>Changes subscription plan</returns>
        [HttpPost]
        public async Task<IActionResult> ChangeSubscriptionPlan(SubscriptionResult subscriptionDetail)
        {
            this.logger.LogInformation("Home Controller / ChangeSubscriptionPlan  subscriptionDetail:{0}", JsonConvert.SerializeObject(subscriptionDetail));
            try
            {
                var subscriptionId = new Guid();
                var planId = string.Empty;
                if (Convert.ToBoolean(applicationConfigRepository.GetValueByName(MainMenuStatusEnum.IsLicenseManagementEnabled.ToString())) == true)
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

                        var jsonResult = await this.apiClient.ChangePlanForSubscriptionAsync(subscriptionId, planId).ConfigureAwait(false);

                        var changePlanOperationStatus = OperationStatusEnum.InProgress;
                        if (jsonResult != null && jsonResult.OperationId != default)
                        {
                            while (OperationStatusEnum.InProgress.Equals(changePlanOperationStatus) || OperationStatusEnum.NotStarted.Equals(changePlanOperationStatus))
                            {
                                var changePlanOperationResult = await this.apiClient.GetOperationStatusResultAsync(subscriptionId, jsonResult.OperationId).ConfigureAwait(false);
                                changePlanOperationStatus = changePlanOperationResult.Status;

                                this.logger.LogInformation("Operation Status :  " + changePlanOperationStatus + " For SubscriptionId " + subscriptionId + "Model SubscriptionID): {0} :: planID:{1}", JsonConvert.SerializeObject(subscriptionId), JsonConvert.SerializeObject(planId));
                                this.applicationLogService.AddApplicationLog("Operation Status :  " + changePlanOperationStatus + " For SubscriptionId " + subscriptionId);
                            }

                            var oldValue = this.subscriptionService.GetSubscriptionsBySubscriptionId(subscriptionId, true);

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
                                    CreateDate = DateTime.Now
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
                return View("Error", ex);
            }
        }

        /// <summary>
        /// Changes the quantity plan.
        /// </summary>
        /// <param name="subscriptionDetail">The subscription detail.</param>
        /// <returns>Changes subscription quantity</returns>
        [HttpPost]
        public async Task<IActionResult> ChangeSubscriptionQuantity(SubscriptionResult subscriptionDetail)
        {
            this.logger.LogInformation("Home Controller / ChangeSubscriptionPlan  subscriptionDetail:{0}", JsonConvert.SerializeObject(subscriptionDetail));
            try
            {
                if (subscriptionDetail != null && subscriptionDetail.Id != default && subscriptionDetail.Quantity > 0)
                {
                    try
                    {
                        var subscriptionId = subscriptionDetail.Id;
                        var quantity = subscriptionDetail.Quantity;

                        var currentUserId = this.userService.GetUserIdFromEmailAddress(this.CurrentUserEmailAddress);

                        var jsonResult = await this.apiClient.ChangeQuantityForSubscriptionAsync(subscriptionId, quantity).ConfigureAwait(false);

                        var changeQuantityOperationStatus = OperationStatusEnum.InProgress;
                        if (jsonResult != null && jsonResult.OperationId != default)
                        {
                            while (OperationStatusEnum.InProgress.Equals(changeQuantityOperationStatus) || OperationStatusEnum.NotStarted.Equals(changeQuantityOperationStatus))
                            {
                                var changeQuantityOperationResult = await this.apiClient.GetOperationStatusResultAsync(subscriptionId, jsonResult.OperationId).ConfigureAwait(false);
                                changeQuantityOperationStatus = changeQuantityOperationResult.Status;

                                this.logger.LogInformation("changeQuantity Operation Status :  " + changeQuantityOperationStatus + " For SubscriptionId " + subscriptionId + "Model SubscriptionID): {0} :: quantity:{1}", JsonConvert.SerializeObject(subscriptionId), JsonConvert.SerializeObject(quantity));
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
                                    CreateDate = DateTime.Now
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
                return View("Error", ex);
            }
        }
        #endregion

        public IActionResult ViewSubscription(Guid subscriptionId, string planId, string operation)
        {
            try
            {
                if (Convert.ToBoolean(applicationConfigRepository.GetValueByName(MainMenuStatusEnum.IsLicenseManagementEnabled.ToString())) == true)
                {
                    this.TempData["ShowLicensesMenu"] = true;
                }
                SubscriptionResultExtension subscriptionDetail = new SubscriptionResultExtension();

                if (User.Identity.IsAuthenticated)
                {
                    var userId = this.userService.AddUser(GetCurrentUserDetail());
                    var currentUserId = this.userService.GetUserIdFromEmailAddress(this.CurrentUserEmailAddress);
                    this.subscriptionService = new SubscriptionService(this.subscriptionRepository, this.planRepository, userId);
                    var planDetails = this.planRepository.GetById(planId);
                    this.TempData["ShowWelcomeScreen"] = false;
                    subscriptionDetail = this.subscriptionService.GetPartnerSubscription(CurrentUserEmailAddress, subscriptionId).FirstOrDefault();
                    subscriptionDetail.ShowWelcomeScreen = false;
                    subscriptionDetail.CustomerEmailAddress = this.CurrentUserEmailAddress;
                    subscriptionDetail.CustomerName = this.CurrentUserName;
                    subscriptionDetail.SubscriptionParameters = this.subscriptionService.GetSubscriptionsParametersById(subscriptionId, planDetails.PlanGuid);
                }
                return this.View("Index", subscriptionDetail);
            }
            catch (Exception ex)
            {
                this.logger.LogError("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                return View("Error", ex);
            }
        }
    }
}
