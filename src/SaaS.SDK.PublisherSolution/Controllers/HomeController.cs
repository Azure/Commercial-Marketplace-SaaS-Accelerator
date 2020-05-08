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
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using Microsoft.Marketplace.SaaS.SDK.Services.Services;
    using Microsoft.Marketplace.SaaS.SDK.Services.Utilities;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using Microsoft.Marketplace.SaaS.SDK.Services.Helpers;
    using Microsoft.Marketplace.SaasKit.Contracts;
    using Microsoft.Marketplace.SaasKit.Exceptions;
    using Microsoft.Marketplace.SaasKit.Models;
    using Newtonsoft.Json;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.DataModel;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Microsoft.Marketplace.SaasKit.Configurations;
    using Microsoft.Extensions.Options;

    [ServiceFilter(typeof(KnownUserAttribute))]
    /// <summary>
    /// Home Controller
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.Saas.Web.Controllers.BaseController" />
    public class HomeController : BaseController
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<HomeController> logger;

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

        private SubscriptionService subscriptionService = null;

        private readonly IApplicationLogRepository applicationLogRepository;

        /// <summary>
        /// Defines the  API Client
        /// </summary>
        private readonly IMeteredBillingApiClient apiClient;

        private readonly IApplicationConfigRepository applicationConfigRepository;

        private ApplicationLogService applicationLogService = null;

        //private SubscriptionService subscriptionService = null;

        private readonly ISubscriptionsRepository subscriptionRepository;

        private readonly IEmailTemplateRepository emailTemplateRepository;

        private readonly IPlanEventsMappingRepository planEventsMappingRepository;

        private readonly IEventsRepository eventsRepository;

        private readonly IOptions<SaaSApiClientConfiguration> options;

        private readonly ISubscriptionTemplateParametersRepository subscriptionTemplateParametersRepository;

        private readonly CloudStorageConfigs cloudConfigs;
        private string azureWebJobsStorage;

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
                                    IMeteredDimensionsRepository DimensionsRepository, ISubscriptionLogRepository subscriptionLogsRepo, IApplicationConfigRepository applicationConfigRepository, IUsersRepository userRepository, IFulfillmentApiClient fulfillApiClient, IApplicationLogRepository applicationLogRepository, IEmailTemplateRepository emailTemplateRepository, IPlanEventsMappingRepository planEventsMappingRepository, IEventsRepository eventsRepository, IOptions<SaaSApiClientConfiguration> options,
                                    ISubscriptionTemplateParametersRepository subscriptionTemplateParametersRepository,
                                     CloudStorageConfigs cloudConfigs)
        {
            this.apiClient = apiClient;
            subscriptionRepo = SubscriptionRepo;
            this.subscriptionLogRepository = subscriptionLogsRepo;
            planRepository = PlanRepository;
            subscriptionUsageLogsRepository = SubscriptionUsageLogsRepository;
            dimensionsRepository = DimensionsRepository;
            this.logger = logger;
            this.applicationConfigRepository = applicationConfigRepository;
            usersRepository = UsersRepository;
            this.userRepository = userRepository;
            this.userService = new UserService(userRepository);
            this.fulfillApiClient = fulfillApiClient;
            //webSubscriptionService = new WebSubscriptionService(this.subscriptionRepo, this.planRepository);
            this.applicationLogRepository = applicationLogRepository;
            this.applicationLogService = new ApplicationLogService(this.applicationLogRepository);
            this.applicationConfigRepository = applicationConfigRepository;
            this.subscriptionRepository = subscriptionRepo;
            this.subscriptionService = new SubscriptionService(this.subscriptionRepository, this.planRepository);
            this.emailTemplateRepository = emailTemplateRepository;
            this.planEventsMappingRepository = planEventsMappingRepository;
            this.eventsRepository = eventsRepository;
            this.options = options;
            this.cloudConfigs = cloudConfigs;
            azureWebJobsStorage = cloudConfigs.AzureWebJobsStorage;
            this.subscriptionTemplateParametersRepository = subscriptionTemplateParametersRepository;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
       {
            this.logger.LogInformation("Home Controller / Index ");
            try
            {
                var userId = this.userService.AddUser(GetCurrentUserDetail());
                if (Convert.ToBoolean(applicationConfigRepository.GetValueByName(MainMenuStatusEnum.IsLicenseManagementEnabled.ToString())) == true)
                {
                    this.TempData["ShowLicensesMenu"] = true;
                }
                return View();
            }
            catch (Exception ex)
            {
                this.logger.LogError("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                return View("Error", ex);
            }
        }

        /// <summary>
        /// Subscriptionses this instance.
        /// </summary>
        /// <returns></returns>
        public IActionResult Subscriptions()
        {
            this.logger.LogInformation("Home Controller / Subscriptions ");
            try
            {
                if (Convert.ToBoolean(applicationConfigRepository.GetValueByName(MainMenuStatusEnum.IsLicenseManagementEnabled.ToString())) == true)
                {
                    this.TempData["ShowLicensesMenu"] = true;
                }
                SubscriptionViewModel subscriptionDetail = new SubscriptionViewModel();
                if (User.Identity.IsAuthenticated)
                {
                    this.TempData["ShowWelcomeScreen"] = "True";

                    List<SubscriptionResultExtension> allSubscriptions = new List<SubscriptionResultExtension>();
                    var allSubscriptionDetails = subscriptionRepo.Get().ToList();
                    var allPlans = planRepository.Get().ToList();
                    foreach (var subscription in allSubscriptionDetails)
                    {
                        SubscriptionResultExtension subscriptionDetailExtension = this.subscriptionService.PrepareSubscriptionResponse(subscription);
                        Plans PlanDetail = this.planRepository.GetById(subscriptionDetailExtension.PlanId);
                        subscriptionDetailExtension.IsPerUserPlan = PlanDetail.IsPerUser.HasValue ? PlanDetail.IsPerUser.Value : false;
                        subscriptionDetail.IsAutomaticProvisioningSupported = Convert.ToBoolean(applicationConfigRepository.GetValueByName("IsAutomaticProvisioningSupported"));
                        if (subscriptionDetailExtension != null && subscriptionDetailExtension.SubscribeId > 0)
                            allSubscriptions.Add(subscriptionDetailExtension);
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
            catch (Exception ex)
            {
                logger.LogError("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
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
            this.logger.LogInformation("Home Controller / RecordUsage : subscriptionId: {0}", JsonConvert.SerializeObject(subscriptionId));
            try
            {
                if (Convert.ToBoolean(applicationConfigRepository.GetValueByName(MainMenuStatusEnum.IsLicenseManagementEnabled.ToString())) == true)
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
            catch (Exception ex)
            {
                this.logger.LogInformation("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                return View("Error", ex);
            }
        }

        /// <summary>
        /// Subscriptions the log detail.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns> Subscription log detail</returns>
        public IActionResult SubscriptionTemplateParmeters(Guid subscriptionId, Guid planId)
        {
            this.logger.LogInformation("Home Controller / SubscriptionTemplateParmeters subscriptionId : {0}", JsonConvert.SerializeObject(subscriptionId));
            try
            {
                if (Convert.ToBoolean(applicationConfigRepository.GetValueByName(MainMenuStatusEnum.IsLicenseManagementEnabled.ToString())) == true)
                {
                    this.TempData["ShowLicensesMenu"] = true;
                }
                if (User.Identity.IsAuthenticated)
                {
                    List<SubscriptionTemplateParameters> subscriptionTemplateParms = new List<SubscriptionTemplateParameters>();
                    subscriptionTemplateParms = this.subscriptionTemplateParametersRepository.GetById(subscriptionId, planId).ToList();
                    return this.View(subscriptionTemplateParms);
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                this.logger.LogInformation("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                return View("Error", ex);
            }
        }

        public IActionResult SubscriptionDetails(Guid subscriptionId, string planId)
        {
            this.logger.LogInformation("Home Controller / ActivateSubscription subscriptionId:{0} :: planId:{1}", subscriptionId, planId);
            SubscriptionResultExtension subscriptionDetail = new SubscriptionResultExtension();

            if (User.Identity.IsAuthenticated)
            {
                var userId = this.userService.AddUser(GetCurrentUserDetail());
                var currentUserId = this.userService.GetUserIdFromEmailAddress(this.CurrentUserEmailAddress);
                this.subscriptionService = new SubscriptionService(this.subscriptionRepo, this.planRepository, userId);
                this.logger.LogInformation("User authenticate successfully & GetSubscriptionByIdAsync  SubscriptionID :{0}", JsonConvert.SerializeObject(subscriptionId));
                this.TempData["ShowWelcomeScreen"] = false;
                var oldValue = this.subscriptionService.GetSubscriptionsBySubscriptionId(subscriptionId);
                var serializedParent = JsonConvert.SerializeObject(oldValue);
                subscriptionDetail = JsonConvert.DeserializeObject<SubscriptionResultExtension>(serializedParent);
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

        public IActionResult DeActivateSubscription(Guid subscriptionId, string planId, string operation)
        {
            this.logger.LogInformation("Home Controller / ActivateSubscription subscriptionId:{0} :: planId:{1} :: operation:{2}", subscriptionId, planId, operation);
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
                    this.logger.LogInformation("GetSubscriptionByIdAsync SubscriptionID :{0} :: planID:{1}:: operation:{2}", JsonConvert.SerializeObject(subscriptionId), JsonConvert.SerializeObject(operation));

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
                return View("Error", ex);
            }
        }

        public IActionResult SubscriptionOperation(Guid subscriptionId, string planId, string operation, int NumberofProviders)
        {
            this.logger.LogInformation("Home Controller / SubscriptionOperation subscriptionId:{0} :: planId : {1} :: operation:{2} :: NumberofProviders : {3}", JsonConvert.SerializeObject(subscriptionId), JsonConvert.SerializeObject(planId), JsonConvert.SerializeObject(operation), JsonConvert.SerializeObject(NumberofProviders));
            try
            {
                var userDetails = this.userRepository.GetPartnerDetailFromEmail(CurrentUserEmailAddress);
                var oldValue = this.subscriptionService.GetSubscriptionsBySubscriptionId(subscriptionId);
                SubscriptionProcessQueueModel queueObject = new SubscriptionProcessQueueModel();
                if (operation == "Activate")
                {
                    this.subscriptionRepository.UpdateStatusForSubscription(subscriptionId, SubscriptionStatusEnumExtension.PendingActivation.ToString(), true);

                    queueObject.SubscriptionID = subscriptionId;
                    queueObject.TriggerEvent = "Activate";
                    queueObject.UserId = userDetails.UserId;
                    queueObject.PortalName = "Admin";


                }
                if (operation == "Deactivate")
                {

                    this.subscriptionRepository.UpdateStatusForSubscription(subscriptionId, SubscriptionStatusEnumExtension.PendingUnsubscribe.ToString(), true);
                    queueObject.SubscriptionID = subscriptionId;
                    queueObject.TriggerEvent = "Unsubscribe";
                    queueObject.UserId = userDetails.UserId;
                    queueObject.PortalName = "Admin";
                }

                var newValue = this.subscriptionService.GetSubscriptionsBySubscriptionId(subscriptionId);
                SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
                {
                    Attribute = Convert.ToString(SubscriptionLogAttributes.Status),
                    SubscriptionId = oldValue.SubscribeId,
                    NewValue = newValue.SubscriptionStatus.ToString(),
                    OldValue = oldValue.SubscriptionStatus.ToString(),
                    CreateBy = userDetails.UserId,
                    CreateDate = DateTime.Now
                };
                this.subscriptionLogRepository.Save(auditLog);

                string queueMessage = JsonConvert.SerializeObject(queueObject);
                string StorageConnectionString = this.cloudConfigs.AzureWebJobsStorage ?? azureWebJobsStorage;
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);

                //// Create the queue client.
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                CloudQueue queue = queueClient.GetQueueReference("saas-provisioning-queue");

                ////Create the queue if it doesn't already exist
                queue.CreateIfNotExistsAsync();

                //// Create a message and add it to the queue.
                CloudQueueMessage message = new CloudQueueMessage(queueMessage);
                queue.AddMessageAsync(message);

                return this.RedirectToAction(nameof(this.ActivatedMessage));
            }
            catch (Exception ex)
            {
                this.logger.LogInformation("Message:{0} :: {1}", ex.Message, ex.InnerException);
                return View("Error");
            }
        }

        public IActionResult ActivatedMessage()
        {
            try
            {
                return this.View("ProcessMessage");
            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }

        /// <summary>
        /// Records the usage.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns></returns>
        public IActionResult RecordUsage(int subscriptionId)
        {
            this.logger.LogInformation("Home Controller / RecordUsage ");
            try
            {
                if (Convert.ToBoolean(applicationConfigRepository.GetValueByName(MainMenuStatusEnum.IsLicenseManagementEnabled.ToString())) == true)
                {
                    this.TempData["ShowLicensesMenu"] = true;
                }
                if (User.Identity.IsAuthenticated)
                {
                    var subscriptionDetail = subscriptionRepo.Get(subscriptionId);
                    var allDimensionsList = dimensionsRepository.GetDimensionsByPlanId(subscriptionDetail.AmpplanId);
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
            catch (Exception ex)
            {
                this.logger.LogInformation("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                return View("Error", ex);
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
            this.logger.LogInformation("Home Controller / ManageSubscriptionUsage  subscriptionData: {0}", JsonConvert.SerializeObject(subscriptionData));
            try
            {
                if (Convert.ToBoolean(applicationConfigRepository.GetValueByName(MainMenuStatusEnum.IsLicenseManagementEnabled.ToString())) == true)
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
                        this.logger.LogInformation("EmitUsageEventAsync");
                        meteringUsageResult = apiClient.EmitUsageEventAsync(subscriptionUsageRequest).ConfigureAwait(false).GetAwaiter().GetResult();
                        responseJson = JsonConvert.SerializeObject(meteringUsageResult);
                        this.logger.LogInformation(responseJson);
                    }
                    catch (MeteredBillingException mex)
                    {
                        responseJson = JsonConvert.SerializeObject(mex.MeteredBillingErrorDetail);
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
                        CreatedDate = DateTime.Now
                    };
                    subscriptionUsageLogsRepository.Save(newMeteredAuditLog);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
            }
            return RedirectToAction(nameof(RecordUsage), new { subscriptionId = subscriptionData.SubscriptionDetail.Id });
        }



        /// <summary>
        /// Get All Subscription List for Current Logged in User
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns>
        /// The <see cref="IActionResult" />
        /// </returns>
        public IActionResult ViewSubscriptionDetail(Guid subscriptionId)
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
                    var subscriptionDetail = this.subscriptionService.GetSubscriptionsBySubscriptionId(subscriptionId);
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

                        var jsonResult = await this.fulfillApiClient.ChangePlanForSubscriptionAsync(subscriptionId, planId).ConfigureAwait(false);

                        var changePlanOperationStatus = OperationStatusEnum.InProgress;
                        if (jsonResult != null && jsonResult.OperationId != default)
                        {
                            while (OperationStatusEnum.InProgress.Equals(changePlanOperationStatus) || OperationStatusEnum.NotStarted.Equals(changePlanOperationStatus))
                            {
                                var changePlanOperationResult = await this.fulfillApiClient.GetOperationStatusResultAsync(subscriptionId, jsonResult.OperationId).ConfigureAwait(false);
                                changePlanOperationStatus = changePlanOperationResult.Status;
                                this.logger.LogInformation("Operation Status :  " + changePlanOperationStatus + " For SubscriptionId " + subscriptionId + "Model SubscriptionID): {0} :: planID:{1}", JsonConvert.SerializeObject(subscriptionId), JsonConvert.SerializeObject(planId));
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
    }
}
