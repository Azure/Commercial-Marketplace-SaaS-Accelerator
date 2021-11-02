// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.
namespace Microsoft.Marketplace.SaaS.SDK.Services.Helpers
{
    using Microsoft.Marketplace.SaaS.Models;
    using Microsoft.Marketplace.SaaS.SDK.Services.Exceptions;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Conversion Helper.
    /// </summary>
    static class ConversionHelper
    {

        /// <summary>
        /// Convert Subscription model to SubscriptionResult model.
        /// </summary>
        /// <param name="subscription">The subscription model.</param>
        /// <returns>
        /// SubscriptionResult.
        /// </returns>
        public static SubscriptionResult subscriptionResult(this Subscription subscription)
        {
            var subscriptionResult = new SubscriptionResult()
            {
                Id = subscription.Id ?? throw new MarketplaceException("Subscription Id cannot be null"),
                PublisherId = subscription.PublisherId,
                OfferId = subscription.OfferId,
                Name = subscription.Name,
                SaasSubscriptionStatus = (Models.SubscriptionStatusEnum)Enum.Parse(typeof(Models.SubscriptionStatusEnum), subscription.SaasSubscriptionStatus.ToString()),
                PlanId = subscription.PlanId,
                Quantity = subscription.Quantity ?? 0,
                Purchaser = new PurchaserResult()
                {
                    EmailId = subscription.Purchaser.EmailId,
                    ObjectId = subscription.Purchaser.ObjectId ?? throw new MarketplaceException("Purchaser ObjectId cannot be null"),
                    TenantId = subscription.Purchaser.TenantId ?? throw new MarketplaceException("Purchaser Tenant Id cannot be null"),
                },
                Beneficiary = new BeneficiaryResult()
                {
                    TenantId = subscription.Beneficiary.TenantId ?? throw new MarketplaceException("Beneficiary Tenant Id cannot be null"),
                },
                Term = new TermResult()
                {
                    StartDate = subscription.Term.StartDate ?? default(DateTimeOffset),
                    EndDate = subscription.Term.EndDate ?? default(DateTimeOffset),
                }
            };
            return subscriptionResult;
        }

        /// <summary>
        /// Convert Subscription model list to SubscriptionResult model list.
        /// </summary>
        /// <param name="subscriptions">The subscription model list.</param>
        /// <returns>
        /// SubscriptionResult list.
        /// </returns>
        public static List<SubscriptionResult> subscriptionResultList(this List<Subscription> subscriptions) 
        {
            return subscriptions.Select(x => x.subscriptionResult()).ToList();
        }

        /// <summary>
        /// Convert ResolvedSubscription model to ResolvedSubscriptionResult model.
        /// </summary>
        /// <param name="resolvedSubscription">The ResolvedSubscription model.</param>
        /// <returns>
        /// ResolvedSubscriptionResult.
        /// </returns>
        public static ResolvedSubscriptionResult resolvedSubscriptionResult(this ResolvedSubscription resolvedSubscription)
        {
            return new ResolvedSubscriptionResult() 
            {
                SubscriptionId = resolvedSubscription.Id ?? throw new MarketplaceException("Subscription Id cannot be null"),
                SubscriptionName = resolvedSubscription.SubscriptionName,
                OfferId = resolvedSubscription.OfferId,
                PlanId = resolvedSubscription.PlanId,
                Quantity = (int)(resolvedSubscription?.Quantity ?? 0),
            };
        }

        /// <summary>
        /// Convert Plan model list to PlanResult model list.
        /// </summary>
        /// <param name="plans">The Plan model list.</param>
        /// <returns>
        /// PlanDetailResult List.
        /// </returns>
        public static List<PlanDetailResultExtension> planResults(this IReadOnlyList<Plan> plans)
        {
            return plans.Select(x => x.planResult()).ToList();
        }

        /// <summary>
        /// Convert Plan model  to PlanResult model .
        /// </summary>
        /// <param name="plan">The plan model.</param>
        /// <returns>
        /// PlanDetailResult.
        /// </returns>
        public static PlanDetailResultExtension planResult(this Plan plan)
        {
            return new PlanDetailResultExtension()
            {
                DisplayName = plan.DisplayName,
                PlanId = plan.PlanId,
                IsPrivate = plan.IsPrivate ?? false,
                IsPerUserPlan = plan.IsPricePerSeat ?? false,
            };
        }

        /// <summary>
        /// Convert operation model to operationResult model.
        /// </summary>
        /// <param name="operation">The operation model.</param>
        /// <returns>
        /// OperationResult Model.
        /// </returns>
        public static OperationResult operationResult(this Operation operation)
        {
            return new OperationResult() 
            { 
              ID = operation.Id?.ToString(),
              Status = (Models.OperationStatusEnum)Enum.Parse(typeof(Models.OperationStatusEnum), operation.Status.ToString()),
              Created = operation.TimeStamp.Value.UtcDateTime
            };
        }
    }
}
