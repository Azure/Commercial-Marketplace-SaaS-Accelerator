// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.Services.Models;

namespace Marketplace.SaaS.Accelerator.Services.Services;

/// <summary>
/// Subscriptions Service.
/// </summary>
public class SubscriptionService
{
    /// <summary>
    /// The subscription repository.
    /// </summary>
    private ISubscriptionsRepository subscriptionRepository;

    /// <summary>
    /// The plan repository.
    /// </summary>
    private IPlansRepository planRepository;

    /// <summary>
    /// The current user identifier.
    /// </summary>
    private int currentUserId;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubscriptionService"/> class.
    /// </summary>
    /// <param name="subscriptionRepo">The subscription repo.</param>
    /// <param name="planRepository">The plan repository.</param>
    /// <param name="currentUserId">The current user identifier.</param>
    public SubscriptionService(ISubscriptionsRepository subscriptionRepo, IPlansRepository planRepository, int currentUserId = 0)
    {
        this.subscriptionRepository = subscriptionRepo;
        this.planRepository = planRepository;
        this.currentUserId = currentUserId;
    }

    /// <summary>
    /// Adds/Update partner subscriptions.
    /// </summary>
    /// <param name="subscriptionDetail">The subscription detail.</param>
    /// <returns>Subscription Id.</returns>
    public int AddOrUpdatePartnerSubscriptions(SubscriptionResult subscriptionDetail, int customerUserId = 0)
    {
        var isActive = this.IsSubscriptionDeleted(Convert.ToString(subscriptionDetail.SaasSubscriptionStatus));
        Subscriptions newSubscription = new Subscriptions()
        {
            Id = 0,
            AmpplanId = subscriptionDetail.PlanId,
            Ampquantity = subscriptionDetail.Quantity,
            AmpsubscriptionId = subscriptionDetail.Id,
            CreateBy = this.currentUserId,
            CreateDate = DateTime.Now,
            IsActive = isActive,
            ModifyDate = DateTime.Now,
            Name = subscriptionDetail.Name,
            SubscriptionStatus = Convert.ToString(subscriptionDetail.SaasSubscriptionStatus),
            UserId = customerUserId == 0 ? this.currentUserId : customerUserId,
            PurchaserEmail = subscriptionDetail.Purchaser.EmailId,
            PurchaserTenantId = subscriptionDetail.Purchaser.TenantId,
            AmpOfferId = subscriptionDetail.OfferId,
            Term = subscriptionDetail.Term.TermUnit.ToString(),
            StartDate = subscriptionDetail.Term.StartDate.ToUniversalTime().DateTime,
            EndDate = subscriptionDetail.Term.EndDate.ToUniversalTime().DateTime
        };
        return this.subscriptionRepository.Save(newSubscription);
    }

    /// <summary>
    /// Binds the subscriptions.
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <param name="status">The status.</param>
    /// <param name="isActivate">if set to <c>true</c> [is activate].</param>
    public void UpdateStateOfSubscription(Guid subscriptionId, string status, bool isActivate)
    {
        this.subscriptionRepository.UpdateStatusForSubscription(subscriptionId, status, isActivate);
    }

    /// <summary>
    /// Subscriptions state from status.
    /// </summary>
    /// <param name="status">The status.</param>
    /// <returns> check if subscription deleted.</returns>
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
    /// <returns> subscription status.</returns>
    public List<SubscriptionResultExtension> GetPartnerSubscription(string partnerEmailAddress, Guid subscriptionId, bool includeUnsubscribed = true)
    {
        List<SubscriptionResultExtension> allSubscriptions = new List<SubscriptionResultExtension>();
        var allSubscriptionsForEmail = this.subscriptionRepository.GetSubscriptionsByEmailAddress(partnerEmailAddress, subscriptionId, includeUnsubscribed).OrderByDescending(s => s.CreateDate).ToList();

        foreach (var subscription in allSubscriptionsForEmail)
        {
            SubscriptionResultExtension subscritpionDetail = this.PrepareSubscriptionResponse(subscription);
            if (subscritpionDetail != null && subscritpionDetail.SubscribeId > 0)
            {
                allSubscriptions.Add(subscritpionDetail);
            }
        }

        return allSubscriptions;
    }

    /// <summary>
    /// Gets the subscriptions for subscription identifier.
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <param name="includeUnsubscribed">if set to <c>true</c> [include unsubscribed].</param>
    /// <returns> Subscription ResultExtension.</returns>
    public SubscriptionResultExtension GetSubscriptionsBySubscriptionId(Guid subscriptionId, bool includeUnsubscribed = true)
    {
        var subscriptionDetail = this.subscriptionRepository.GetById(subscriptionId, includeUnsubscribed);
        if (subscriptionDetail != null)
        {
            SubscriptionResultExtension subscritpionDetail = this.PrepareSubscriptionResponse(subscriptionDetail);
            if (subscritpionDetail != null)
            {
                return subscritpionDetail;
            }
        }

        return new SubscriptionResultExtension();
    }

    /// <summary>
    /// Prepares the subscription response.
    /// </summary>
    /// <param name="subscription">The subscription.</param>
    /// <returns> Subscription.</returns>
    public SubscriptionResultExtension PrepareSubscriptionResponse(Subscriptions subscription, Plans existingPlanDetail = null)
    {
        if(existingPlanDetail == null)
        {
            existingPlanDetail = this.planRepository.GetById(subscription.AmpplanId);
        }

        SubscriptionResultExtension subscritpionDetail = new SubscriptionResultExtension
        {
            Id = subscription.AmpsubscriptionId,
            SubscribeId = subscription.Id,
            PlanId = string.IsNullOrEmpty(subscription.AmpplanId) ? string.Empty : subscription.AmpplanId,
            OfferId = subscription.AmpOfferId,
            Term = new TermResult
            {
                StartDate = subscription.StartDate.GetValueOrDefault(),
                EndDate = subscription.EndDate.GetValueOrDefault(),
            },
            Quantity = subscription.Ampquantity,
            Name = subscription.Name,
            SubscriptionStatus = this.GetSubscriptionStatus(subscription.SubscriptionStatus),
            IsActiveSubscription = subscription.IsActive ?? false,
            CustomerEmailAddress = subscription.User?.EmailAddress,
            CustomerName = subscription.User?.FullName,
            IsMeteringSupported = existingPlanDetail != null ? (existingPlanDetail.IsmeteringSupported ?? false) : false,
        };

        if (!Enum.TryParse<TermUnitEnum>(subscription.Term, out var termUnit))
            termUnit = TermUnitEnum.P1M;
        subscritpionDetail.Term.TermUnit = termUnit;

        subscritpionDetail.Purchaser = new PurchaserResult();
        subscritpionDetail.Purchaser.EmailId = subscription.PurchaserEmail;
        subscritpionDetail.Purchaser.TenantId = subscription.PurchaserTenantId ?? default;
        return subscritpionDetail;
    }

    /// <summary>
    /// Gets the subscriptions for partner.
    /// </summary>
    /// <param name="partnerEmailAddress">The partner email address.</param>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <param name="includeUnsubscribed">if set to <c>true</c> [include unsubscribed].</param>
    /// <returns> SubscriptionResult.</returns>
    public List<SubscriptionResult> GetPartnerSubscriptions(string partnerEmailAddress, Guid subscriptionId, bool includeUnsubscribed = true)
    {
        List<SubscriptionResult> allSubscriptions = new List<SubscriptionResult>();
        var allSubscriptionsForEmail = this.subscriptionRepository.GetSubscriptionsByEmailAddress(partnerEmailAddress, subscriptionId, includeUnsubscribed).OrderByDescending(s => s.CreateDate).ToList();

        foreach (var subscription in allSubscriptionsForEmail)
        {
            SubscriptionResult subscritpionDetail = this.PrepareSubscriptionResponse(subscription);
            if (subscritpionDetail != null && subscritpionDetail.SubscribeId > 0)
            {
                allSubscriptions.Add(subscritpionDetail);
            }
        }

        return allSubscriptions;
    }

    /// <summary>
    /// Gets the subscription status.
    /// </summary>
    /// <param name="subscriptionStatus">The subscription status.</param>
    /// <returns> Subscription Status EnumExtension.</returns>
    public SubscriptionStatusEnumExtension GetSubscriptionStatus(string subscriptionStatus)
    {
        var parseSuccessfull = Enum.TryParse(subscriptionStatus, out SubscriptionStatusEnumExtension status);
        return parseSuccessfull ? status : SubscriptionStatusEnumExtension.UnRecognized;
    }

    /// <summary>
    /// Updates the subscription plan.
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <param name="planId">The plan identifier.</param>
    public void UpdateSubscriptionPlan(Guid subscriptionId, string planId)
    {
        if (subscriptionId != default && !string.IsNullOrWhiteSpace(planId))
        {
            this.subscriptionRepository.UpdatePlanForSubscription(subscriptionId, planId);
        }
    }

    /// <summary>
    /// Updates the subscription quantity.
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <param name="quantity">The quantity identifier.</param>
    public void UpdateSubscriptionQuantity(Guid subscriptionId, int quantity)
    {
        if (subscriptionId != default && quantity > 0)
        {
            this.subscriptionRepository.UpdateQuantityForSubscription(subscriptionId, quantity);
        }
    }

    /// <summary>
    /// Adds/Updates all plans details for subscription.
    /// </summary>
    /// <param name="allPlanDetail">All plan detail.</param>
    public void AddUpdateAllPlanDetailsForSubscription(List<PlanDetailResultExtension> allPlanDetail)
    {
        foreach (var planDetail in allPlanDetail)
        {
            this.planRepository.Save(new Plans
            {
                PlanId = planDetail.PlanId,
                DisplayName = planDetail.DisplayName,
                Description = "",
                OfferId = planDetail.OfferId,
                PlanGuid = planDetail.PlanGUID,
                MeteredDimensions = planDetail.GetmeteredDimensions(),
                IsmeteringSupported = planDetail.IsmeteringSupported,
                IsPerUser = planDetail.IsPerUserPlan,
            });
        }
    }

    /// <summary>
    /// Only Add current subscription plan. This is more relevent when an unsubscribed subscription gets created
    /// As the ListAvailableplans API is not available, we only add current plan from Subscription
    /// </summary>
    /// <param name="allPlanDetail">All plan detail.</param>
    public void AddPlanDetailsForSubscription(PlanDetailResultExtension planDetail)
    {
        this.planRepository.Add(new Plans
        {
            PlanId = planDetail.PlanId,
            DisplayName = planDetail.DisplayName,
            Description = "",
            OfferId = planDetail.OfferId,
            PlanGuid = planDetail.PlanGUID,
            IsPerUser = planDetail.IsPerUserPlan,
            // Setting to false to avoid NULL in the DB. This only applies
            // when creating a plan for an unsubscribed subscription, so it is fine.
            IsmeteringSupported = false   
        });
    }

    /// <summary>
    /// Get the plan details for subscription.
    /// </summary>
    /// <returns> Plan Details.</returns>
    public List<PlanDetailResult> GetAllSubscriptionPlans()
    {
        var allPlans = this.planRepository.Get();

        return (from plan in allPlans
            select new PlanDetailResult()
            {
                Id = plan.Id,
                PlanId = plan.PlanId,
                DisplayName = plan.DisplayName,
                Description = plan.Description

            }).ToList();
    }

    /// <summary>
    /// Get the plan details for subscription.
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <param name="planId">The plan identifier.</param>
    /// <returns>
    /// Subscription Parameters Model.
    /// </returns>
    public List<SubscriptionParametersModel> GetSubscriptionsParametersById(Guid subscriptionId, Guid planId)
    {
        List<SubscriptionParametersModel> subscriptionParametersList = new List<SubscriptionParametersModel>();

        var subscriptionParameters = this.subscriptionRepository.GetSubscriptionsParametersById(subscriptionId, planId);

        var serializedSubscription = JsonSerializer.Serialize(subscriptionParameters);
        subscriptionParametersList = JsonSerializer.Deserialize<List<SubscriptionParametersModel>>(serializedSubscription);

        return subscriptionParametersList;
    }

    /// <summary>
    /// Adds the plan details for subscription.
    /// </summary>
    /// <param name="subscriptionParameters">The subscription parameters.</param>
    /// <param name="currentUserId">The current user identifier.</param>
    public void AddSubscriptionParameters(List<SubscriptionParametersModel> subscriptionParameters, int? currentUserId)
    {
        foreach (var parameters in subscriptionParameters)
        {
            this.subscriptionRepository.AddSubscriptionParameters(new SubscriptionParametersOutput
            {
                Id = parameters.Id,
                PlanId = parameters.PlanId,
                DisplayName = parameters.DisplayName,
                PlanAttributeId = parameters.PlanAttributeId,
                SubscriptionId = parameters.SubscriptionId,
                OfferId = parameters.OfferId,
                Value = parameters.Value,
                UserId = currentUserId,
                CreateDate = DateTime.Now,
            });
        }
    }

    /// <summary>
    /// Get all Active subscription with Metered plan
    /// </summary>
    /// <returns>a list of subscription with metered plan</returns>
    public List<Subscriptions> GetActiveSubscriptionsWithMeteredPlan()
    {
        var allActiveSubscription = this.subscriptionRepository.Get().ToList().Where(s => s.SubscriptionStatus == "Subscribed").ToList();
        var allPlansData = this.planRepository.Get().ToList().Where(p => p.IsmeteringSupported == true).ToList();
        var meteredSubscriptions = from subscription in allActiveSubscription
            join plan in allPlansData
                on subscription.AmpplanId equals plan.PlanId
            select subscription;
        return meteredSubscriptions.ToList();
    }
    /*
    /// <summary>
    /// Generates the parmlist from response.
    /// </summary>
    /// <param name="outputstring">The outputstring.</param>
    /// <returns> Subscription Template Parameters.</returns>
    //    public List<SubscriptionTemplateParameters> GenerateParmlistFromResponse(DeploymentExtended outputstring)
    //    {
    //        List<SubscriptionTemplateParameters> childlist = new List<SubscriptionTemplateParameters>();
    //        JObject templateOutputs = (JObject)outputstring.Properties.Outputs;
    //        foreach (JToken child in templateOutputs.Children())
    //        {
    //            SubscriptionTemplateParameters childparms = new SubscriptionTemplateParameters();
    //            childparms = new SubscriptionTemplateParameters();
    //            childparms.ParameterType = "output";
    //            var paramName = (child as JProperty).Name;
    //            childparms.Parameter = paramName;
    //            object paramValue = string.Empty;

    //            foreach (JToken grandChild in child)
    //            {
    //                foreach (JToken grandGrandChild in grandChild)
    //                {
    //                    var property = grandGrandChild as JProperty;

    //                    if (property != null && property.Name == "value")
    //                    {
    //                        var type = property.Value.GetType();

    //                        if (type == typeof(JValue) || type == typeof(JArray) ||
    //                        property.Value.Type == JTokenType.Object ||
    //                        property.Value.Type == JTokenType.Date)
    //                        {
    //                            paramValue = property.Value;
    //                            if (paramValue != null)
    //                            {
    //                                childparms.Value = paramValue.ToString();
    //                            }
    //                        }
    //                    }
    //                }
    //            }

    //            childlist.Add(childparms);
    //        }

        return childlist;
    }
    */
}