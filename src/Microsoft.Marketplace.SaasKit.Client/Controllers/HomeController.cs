namespace Microsoft.Marketplace.SaasKit.Client.Controllers
{
    using log4net;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.OpenIdConnect;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using Microsoft.Marketplace.SaasKit.Client.Models;
    using Microsoft.Marketplace.SaasKit.Client.Services;
    using Microsoft.Marketplace.SaasKit.Contracts;
    using Microsoft.Marketplace.SaasKit.Models;
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
        /// The user repository
        /// </summary>
        private readonly IUsersRepository userRepository;

        /// <summary>
        /// The log
        /// </summary>
        private readonly ILog log = LogManager.GetLogger(typeof(HomeController));

        /// <summary>
        /// The subscription service
        /// </summary>
        private SubscriptionService subscriptionService = null;

        /// <summary>
        /// The application log service
        /// </summary>
        private ApplicationLogService applicationLogService = null;

        /// <summary>
        /// The user service
        /// </summary>
        private UserService userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController" /> class.
        /// </summary>
        /// <param name="apiClient">The API Client<see cref="IFulfilmentApiClient" /></param>
        /// <param name="subscriptionRepo">The subscription repository.</param>
        /// <param name="planRepository">The plan repository.</param>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="applicationLogRepository">The application log repository.</param>
        /// <param name="subscriptionLogsRepo">The subscription logs repository.</param>
        public HomeController(IFulfillmentApiClient apiClient, ISubscriptionsRepository subscriptionRepo, IPlansRepository planRepository, IUsersRepository userRepository, IApplicationLogRepository applicationLogRepository, ISubscriptionLogRepository subscriptionLogsRepo)
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
            this.log.Info("Initializing Index Page");
            SubscriptionResult subscriptionDetail = new SaasKitModels.SubscriptionResult();

            if (User.Identity.IsAuthenticated)
            {
                var userId = this.userService.AddPartnerDetail(GetCurrentUserDetail());
                var currentUserId = this.userService.GetUserIdFromEmailAddress(this.CurrentUserEmailAddress);
                this.subscriptionService = new SubscriptionService(this.subscriptionRepository, this.planRepository, userId);

                this.log.Info("User authenticate successfully");

                if (!string.IsNullOrEmpty(token))
                {
                    this.TempData["ShowWelcomeScreen"] = null;
                    token = token.Replace(' ', '+');
                    var newSubscription = this.apiClient.ResolveAsync(token).ConfigureAwait(false).GetAwaiter().GetResult();

                    if (newSubscription != null && newSubscription.SubscriptionId != default)
                    {
                        var subscriptionPlanDetail = this.apiClient.GetAllPlansForSubscriptionAsync(newSubscription.SubscriptionId).ConfigureAwait(false).GetAwaiter().GetResult();
                        this.subscriptionService.AddPlanDetailsForSubscription(subscriptionPlanDetail);

                        // GetSubscriptionBy SubscriptionId
                        var subscriptionData = this.apiClient.GetSubscriptionByIdAsync(newSubscription.SubscriptionId).ConfigureAwait(false).GetAwaiter().GetResult();
                        var subscribeId = this.subscriptionService.AddUpdatePartnerSubscriptions(subscriptionData);

                        if (subscribeId > 0 && subscriptionData.SaasSubscriptionStatus == SubscriptionStatusEnum.PendingFulfillmentStart)
                        {
                            SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
                            {
                                Attribute = Convert.ToString(SubscriptionLogAttributes.Status),
                                SubscriptionId = subscribeId,
                                NewValue = "Pending Activation",
                                OldValue = "None",
                                CreateBy = currentUserId,
                                CreateDate = DateTime.Now
                            };
                            this.subscriptionLogRepository.Add(auditLog);
                        }

                        subscriptionDetail = subscriptionData;
                        subscriptionDetail.ShowWelcomeScreen = false;
                        subscriptionDetail.CustomerEmailAddress = this.CurrentUserEmailAddress;
                        subscriptionDetail.CustomerName = this.CurrentUserName;
                    }
                }
                else
                {
                    this.TempData["ShowWelcomeScreen"] = "True";
                    subscriptionDetail.ShowWelcomeScreen = true;
                    return this.View(subscriptionDetail);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(token))
                {
                    return this.Challenge(new AuthenticationProperties { RedirectUri = "/?token=" + token }, OpenIdConnectDefaults.AuthenticationScheme);
                }
                else
                {
                    this.TempData["ShowWelcomeScreen"] = "True";
                    subscriptionDetail.ShowWelcomeScreen = true;
                    return this.View(subscriptionDetail);
                }
            }

            return this.View(subscriptionDetail);
        }

        /// <summary>
        /// Subscription this instance.
        /// </summary>
        /// <returns> Subscription instance</returns>
        public IActionResult Subscriptions()
        {
            if (User.Identity.IsAuthenticated)
            {
                this.TempData["ShowWelcomeScreen"] = "True";
                SubscriptionViewModel subscriptionDetail = new SubscriptionViewModel();
                subscriptionDetail.Subscriptions = this.subscriptionService.GetPartnerSubscription(CurrentUserEmailAddress, default, true).ToList();
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
        /// Subscriptions the log detail.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns> Subscription log detail</returns>
        public IActionResult SubscriptionLogDetail(Guid subscriptionId)
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

        /// <summary>
        /// Subscriptions the operation.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="planId">The plan identifier.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>Subscriptions operation</returns>
        [HttpPost]
        public IActionResult SubscriptionOperation(Guid subscriptionId, string planId, string operation)
        {
            bool isSuccess = false;
            if (subscriptionId != default)
            {
                var oldValue = this.subscriptionService.GetPartnerSubscription(CurrentUserEmailAddress, subscriptionId).FirstOrDefault();
                var currentUserId = this.userService.GetUserIdFromEmailAddress(this.CurrentUserEmailAddress);

                if (operation == "Activate")
                {
                    try
                    {
                        var response = this.apiClient.ActivateSubscriptionAsync(subscriptionId, planId).ConfigureAwait(false).GetAwaiter().GetResult();
                        this.subscriptionService.UpdateStateOfSubscription(subscriptionId, SubscriptionStatusEnum.Subscribed, true);
                        isSuccess = true;
                    }
                    catch (FulfillmentException fex)
                    {
                        this.TempData["ErrorMsg"] = fex.Message;
                    }
                }

                if (operation == "Deactivate")
                {
                    try
                    {
                        var response = this.apiClient.DeleteSubscriptionAsync(subscriptionId, planId).ConfigureAwait(false).GetAwaiter().GetResult();
                        this.subscriptionService.UpdateStateOfSubscription(subscriptionId, SubscriptionStatusEnum.Unsubscribed, false);
                        isSuccess = true;
                    }
                    catch (FulfillmentException fex)
                    {
                        this.TempData["ErrorMsg"] = fex.Message;
                    }
                }

                var newValue = this.subscriptionService.GetPartnerSubscription(CurrentUserEmailAddress, subscriptionId, true).FirstOrDefault();
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
                    }
                }
            }

            return this.RedirectToAction(nameof(this.Subscriptions));
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

        #endregion
    }
}
