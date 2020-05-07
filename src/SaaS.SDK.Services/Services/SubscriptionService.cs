namespace Microsoft.Marketplace.SaaS.SDK.Services.Services
{
    using Microsoft.Azure.Management.ResourceManager.Models;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using Microsoft.Marketplace.SaasKit.Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SaasKitModels = Microsoft.Marketplace.SaasKit.Models;

    public class SubscriptionService
    {
        /// <summary>
        /// The subscription repository
        /// </summary>
        public ISubscriptionsRepository subscriptionRepository;

        /// <summary>
        /// The plan repository
        /// </summary>
        public IPlansRepository planRepository;

        /// <summary>
        /// The plan repository
        /// </summary>
        public ISubscriptionLogRepository subscriptionLogRepository;

        /// <summary>
        /// The current user identifier
        /// </summary>
        public int currentUserId;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionService" /> class.
        /// </summary>
        /// <param name="subscriptionRepo">The subscription repository.</param>
        /// <param name="planRepository">The plan repository.</param>
        /// <param name="currentUserId">The current user identifier.</param>
        public SubscriptionService(ISubscriptionsRepository subscriptionRepo, IPlansRepository planRepository, int currentUserId = 0)
        {
            subscriptionRepository = subscriptionRepo;
            this.planRepository = planRepository;
            this.currentUserId = currentUserId;
        }

        /// <summary>
        /// Adds/Update partner subscriptions.
        /// </summary>
        /// <param name="subscriptionDetail">The subscription detail.</param>
        public int AddOrUpdatePartnerSubscriptions(SaasKitModels.SubscriptionResult subscriptionDetail)
        {
            var isActive = IsSubscriptionDeleted(Convert.ToString(subscriptionDetail.SaasSubscriptionStatus));
            Subscriptions newSubscription = new Subscriptions()
            {
                Id = 0,
                AmpplanId = subscriptionDetail.PlanId,
                Ampquantity = subscriptionDetail.Quantity,
                AmpsubscriptionId = subscriptionDetail.Id,
                CreateBy = currentUserId,
                CreateDate = DateTime.Now,
                IsActive = isActive,
                ModifyDate = DateTime.Now,
                Name = subscriptionDetail.Name,
                SubscriptionStatus = Convert.ToString(subscriptionDetail.SaasSubscriptionStatus),
                UserId = currentUserId,
                PurchaserEmail = subscriptionDetail.Purchaser.EmailId,
                PurchaserTenantId = subscriptionDetail.Purchaser.TenantId
        };
            return subscriptionRepository.Save(newSubscription);
        }

    /// <summary>
    /// Binds the subscriptions.
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <param name="status">The status.</param>
    /// <param name="isActivate">if set to <c>true</c> [is activate].</param>
    public void UpdateStateOfSubscription(Guid subscriptionId, string status, bool isActivate)
    {
        subscriptionRepository.UpdateStatusForSubscription(subscriptionId, status, isActivate);
    }

    /// <summary>
    /// Subscriptions state from status.
    /// </summary>
    /// <param name="status">The status.</param>
    /// <returns></returns>
    public bool IsSubscriptionDeleted(string status)
    {
        return SubscriptionStatusEnum.Unsubscribed.ToString().Equals(status, StringComparison.InvariantCultureIgnoreCase);
    }

    /// <summary>
    /// Gets the subscriptions for partner.
    /// </summary>
    /// <param name="partnerEmailAddress">The partner email address.</param>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <param name="includeUnsubscribed">if set to <c>true</c> [include unsubscribed].</param>
    /// <returns></returns>
    public List<SubscriptionResultExtension> GetPartnerSubscription(string partnerEmailAddress, Guid subscriptionId, bool includeUnsubscribed = true)
    {
        List<SubscriptionResultExtension> allSubscriptions = new List<SubscriptionResultExtension>();
        var allSubscriptionsForEmail = subscriptionRepository.GetSubscriptionsByEmailAddress(partnerEmailAddress, subscriptionId, includeUnsubscribed).OrderByDescending(s => s.CreateDate).ToList();

        foreach (var subscription in allSubscriptionsForEmail)
        {
            SubscriptionResultExtension subscritpionDetail = PrepareSubscriptionResponse(subscription);
            if (subscritpionDetail != null && subscritpionDetail.SubscribeId > 0)
                allSubscriptions.Add(subscritpionDetail);
        }
        return allSubscriptions;
    }

    /// <summary>
    /// Gets the subscriptions for subscription identifier.
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <param name="includeUnsubscribed">if set to <c>true</c> [include unsubscribed].</param>
    /// <returns></returns>
    public SubscriptionResultExtension GetSubscriptionsBySubscriptionId(Guid subscriptionId, bool includeUnsubscribed = true)
    {
        var subscriptionDetail = subscriptionRepository.GetById(subscriptionId, includeUnsubscribed);
        if (subscriptionDetail != null)
        {
            SubscriptionResultExtension subscritpionDetail = PrepareSubscriptionResponse(subscriptionDetail);
            if (subscritpionDetail != null)
                return subscritpionDetail;
        }
        return new SubscriptionResultExtension();
    }

    /// <summary>
    /// Prepares the subscription response.
    /// </summary>
    /// <param name="subscription">The subscription.</param>
    /// <returns></returns>
    public SubscriptionResultExtension PrepareSubscriptionResponse(Subscriptions subscription)
    {
        var existingPlanDetail = this.planRepository.GetById(subscription.AmpplanId);

        SubscriptionResultExtension subscritpionDetail = new SubscriptionResultExtension
        {
            Id = subscription.AmpsubscriptionId,
            SubscribeId = subscription.Id,
            PlanId = string.IsNullOrEmpty(subscription.AmpplanId) ? string.Empty : subscription.AmpplanId,
            Quantity = subscription.Ampquantity,
            Name = subscription.Name,
            SubscriptionStatus = GetSubscriptionStatus(subscription.SubscriptionStatus),
            IsActiveSubscription = subscription.IsActive ?? false,
            CustomerEmailAddress = subscription.User?.EmailAddress,
            CustomerName = subscription.User?.FullName,
            IsMeteringSupported = existingPlanDetail != null ? (existingPlanDetail.IsmeteringSupported ?? false) : false

        };
        return subscritpionDetail;
    }

    /// <summary>
    /// Gets the subscriptions for partner.
    /// </summary>
    /// <param name="partnerEmailAddress">The partner email address.</param>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <param name="includeUnsubscribed">if set to <c>true</c> [include unsubscribed].</param>
    /// <returns></returns>
    public List<SubscriptionResult> GetPartnerSubscriptions(string partnerEmailAddress, Guid subscriptionId, bool includeUnsubscribed = true)
    {
        List<SubscriptionResult> allSubscriptions = new List<SubscriptionResult>();
        var allSubscriptionsForEmail = subscriptionRepository.GetSubscriptionsByEmailAddress(partnerEmailAddress, subscriptionId, includeUnsubscribed).OrderByDescending(s => s.CreateDate).ToList();

        foreach (var subscription in allSubscriptionsForEmail)
        {
            SubscriptionResult subscritpionDetail = PrepareSubscriptionResponse(subscription);
            if (subscritpionDetail != null && subscritpionDetail.SubscribeId > 0)
                allSubscriptions.Add(subscritpionDetail);
        }

        return allSubscriptions;
    }

    /// <summary>
    /// Gets the subscription status.
    /// </summary>
    /// <param name="subscriptionStatus">The subscription status.</param>
    /// <returns></returns>
    public SubscriptionStatusEnumExtension GetSubscriptionStatus(string subscriptionStatus)
    {
        var status = SubscriptionStatusEnumExtension.NotStarted;
        Enum.TryParse(subscriptionStatus, out status);
        return status;
    }

    /// <summary>
    /// Updates the subscription plan.
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <param name="planId">The plan identifier.</param>
    /// <returns></returns>
    public void UpdateSubscriptionPlan(Guid subscriptionId, string planId)
    {
        if (subscriptionId != default && !string.IsNullOrWhiteSpace(planId))
        {
            subscriptionRepository.UpdatePlanForSubscription(subscriptionId, planId);
        }
    }

    /// <summary>
    /// Updates the subscription quantity.
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <param name="quantity">The quantity identifier.</param>
    /// <returns></returns>
    public void UpdateSubscriptionQuantity(Guid subscriptionId, int quantity)
    {
        if (subscriptionId != default && quantity > 0)
            subscriptionRepository.UpdateQuantityForSubscription(subscriptionId, quantity);

    }

    /// <summary>
    /// Adds the plan details for subscription.
    /// </summary>
    /// <param name="allPlanDetail">All plan detail.</param>
    public void AddPlanDetailsForSubscription(List<PlanDetailResultExtension> allPlanDetail)
    {
        foreach (var planDetail in allPlanDetail)
        {
            planRepository.Save(new Plans
            {
                PlanId = planDetail.PlanId,
                DisplayName = planDetail.PlanId,
                Description = planDetail.DisplayName,
                OfferId = planDetail.OfferId,
                PlanGuid = planDetail.PlanGUID
            });
        }
    }

    /// <summary>
    /// Get the plan details for subscription.
    /// </summary>
    /// <returns></returns>
    public List<SaasKitModels.PlanDetailResult> GetAllSubscriptionPlans()
    {
        var allPlans = planRepository.Get();

        return (from plan in allPlans
                select new SaasKitModels.PlanDetailResult()
                {
                    Id = plan.Id,
                    PlanId = plan.PlanId,
                    DisplayName = plan.DisplayName,

                }).ToList();
    }

    /// <summary>
    /// Get the plan details for subscription.
    /// </summary>
    /// <returns></returns>
    public List<SubscriptionParametersModel> GetSubscriptionsParametersById(Guid subscriptionId, Guid planId)
    {
        List<SubscriptionParametersModel> subscriptionParametersList = new List<SubscriptionParametersModel>();

        var subscriptionParameters = subscriptionRepository.GetSubscriptionsParametersById(subscriptionId, planId);

        var serializedSubscription = JsonConvert.SerializeObject(subscriptionParameters);
        subscriptionParametersList = JsonConvert.DeserializeObject<List<SubscriptionParametersModel>>(serializedSubscription);

        return subscriptionParametersList;
    }

    /// <summary>
    /// Adds the plan details for subscription.
    /// </summary>
    /// <param name="allPlanDetail">All plan detail.</param>
    public void AddSubscriptionParameters(List<SubscriptionParametersModel> subscriptionParameters, int? currentUserId)
    {
        foreach (var parameters in subscriptionParameters)
        {
            subscriptionRepository.AddSubscriptionParameters(new SubscriptionParametersOutput
            {
                Id = parameters.Id,
                PlanId = parameters.PlanId,
                DisplayName = parameters.DisplayName,
                PlanAttributeId = parameters.PlanAttributeId,
                SubscriptionId = parameters.SubscriptionId,
                OfferId = parameters.OfferId,
                Value = parameters.Value,
                UserId = currentUserId,
                CreateDate = DateTime.Now


            }); ;
        }
    }


    public List<SubscriptionTemplateParameters> GenerateParmlistFromResponse(DeploymentExtended outputstring)
    {
        List<SubscriptionTemplateParameters> childlist = new List<SubscriptionTemplateParameters>();

        JObject templateOutputs = (JObject)outputstring.Properties.Outputs;


        foreach (JToken child in templateOutputs.Children())
        {
            SubscriptionTemplateParameters childparms = new SubscriptionTemplateParameters();
            childparms = new SubscriptionTemplateParameters();
            childparms.ParameterType = "output";
            var paramName = (child as JProperty).Name;
            childparms.Parameter = paramName;
            object paramValue = string.Empty;

            foreach (JToken grandChild in child)
            {
                foreach (JToken grandGrandChild in grandChild)
                {
                    var property = grandGrandChild as JProperty;

                    if (property != null && property.Name == "value")
                    {
                        var type = property.Value.GetType();

                        if (type == typeof(JValue) || type == typeof(JArray) ||
                        property.Value.Type == JTokenType.Object ||
                        property.Value.Type == JTokenType.Date)
                        {
                            paramValue = property.Value;
                            if (paramValue != null)
                            {
                                childparms.Value = paramValue.ToString();
                            }
                        }

                    }
                }
            }
            childlist.Add(childparms);
        }
        return childlist;
    }

}
}