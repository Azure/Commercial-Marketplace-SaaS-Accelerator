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

    static class ConversionHelper
    {
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

        public static List<SubscriptionResult> subscriptionResultList(this List<Subscription> subscriptions) 
        {
            return subscriptions.Select(x => x.subscriptionResult()).ToList();
        }


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

        public static List<PlanDetailResult> planResults(this IReadOnlyList<Plan> plans)
        {
            return plans.Select(x => x.planResult()).ToList();
        }

        public static PlanDetailResult planResult(this Plan plan)
        {
            return new PlanDetailResult()
            {
                DisplayName = plan.DisplayName,
                PlanId = plan.PlanId,
                IsPrivate = plan.IsPrivate ?? false
            };
        }

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
