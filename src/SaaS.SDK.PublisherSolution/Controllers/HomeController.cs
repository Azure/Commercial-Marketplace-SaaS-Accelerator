namespace Microsoft.Marketplace.Saas.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text.Json.Serialization;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Logging;
    using Microsoft.Marketplace.Saas.Web.Models;
    using Microsoft.Marketplace.SaaS.SDK.PublisherSolution.Utilities;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
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
        private readonly IUsersRepository usersRepository;

        /// <summary>
        /// Defines the  API Client
        /// </summary>
        private readonly IMeteredBillingApiClient apiClient;

        private readonly IApplicationConfigRepository applicationConfigRepository;


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
                                    IMeteredDimensionsRepository DimensionsRepository, ISubscriptionLogRepository subscriptionLogsRepo, IApplicationConfigRepository applicationConfigRepository)
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
            subscritpionDetail.Quantity = subscription.AmpQuantity;
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
    }
}
