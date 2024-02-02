// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Marketplace.SaaS.Accelerator.Services.Exceptions;
using Marketplace.SaaS.Accelerator.Services.Models;
using Microsoft.Marketplace.SaaS.Models;

namespace Marketplace.SaaS.Accelerator.Services.Helpers;

using MeteringDimension = Models.MeteringDimension;
using RecurrentBillingTerm = Models.RecurrentBillingTerm;
using MeteringedQuantityIncluded = Models.MeteringedQuantityIncluded;
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
                EmailId = subscription.Beneficiary.EmailId ?? throw new MarketplaceException("Beneficiary Email Id cannot be null"),
                ObjectId = subscription.Beneficiary.ObjectId ?? throw new MarketplaceException("Beneficiary Object Id cannot be null"),
                TenantId = subscription.Beneficiary.TenantId ?? throw new MarketplaceException("Beneficiary Tenant Id cannot be null"),
            },
            Term = new TermResult()
            {
                TermUnit = subscription.Term.TermUnit.HasValue ? (Models.TermUnitEnum)subscription.Term.TermUnit :         Models.TermUnitEnum.P1M,
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
            Description = plan.Description,
            DisplayName = plan.DisplayName,
            PlanId = plan.PlanId,
            IsPrivate = plan.IsPrivate ?? false,
            HasFreeTrials = plan.HasFreeTrials ?? false,
            IsPerUserPlan = plan.IsPricePerSeat ?? false,
            IsStopSell = plan.IsStopSell ?? false,
            Market = plan.Market,
            PlanComponents = getPlanComponentsFromPlan(plan)
        };
    }
    /// <summary>
    /// Exctract Meter and Billing dimi
    /// </summary>
    /// <param name="plan"></param>
    /// <returns></returns>
    public static Models.PlanComponents getPlanComponentsFromPlan(this Plan plan)
    {
        Models.PlanComponents components = new Models.PlanComponents();
        components.RecurrentBillingTerms = new List<RecurrentBillingTerm>();
        components.MeteringDimensions = new List<MeteringDimension>();
        //Map MeteringDimesion array

        foreach (Microsoft.Marketplace.SaaS.Models.MeteringDimension meterDim in plan.PlanComponents.MeteringDimensions)
        {
            components.MeteringDimensions.Add(
                new MeteringDimension()
                {
                    Currency = meterDim.Currency,
                    PricePerUnit = meterDim.PricePerUnit,
                    UnitOfMeasure = meterDim.UnitOfMeasure,
                    DisplayName = meterDim.DisplayName,
                    Id = meterDim.Id
                });
        }

        //Map RecurrentBillingTerms array
        foreach (Microsoft.Marketplace.SaaS.Models.RecurrentBillingTerm recurrentBilling in plan.PlanComponents.RecurrentBillingTerms)
        {
            RecurrentBillingTerm recurrentBillingTerm = new RecurrentBillingTerm();
            recurrentBillingTerm.MeteredQuantityIncluded = new List<MeteringedQuantityIncluded>();
            recurrentBillingTerm.Currency = recurrentBilling.Currency;
            recurrentBillingTerm.Price = recurrentBilling.Price;
            recurrentBillingTerm.TermDescription = recurrentBilling.TermDescription;
            recurrentBillingTerm.TermUnit = recurrentBilling.TermUnit.ToString();
            foreach (Microsoft.Marketplace.SaaS.Models.MeteringedQuantityIncluded metering in recurrentBilling.MeteredQuantityIncluded)
            {
                recurrentBillingTerm.MeteredQuantityIncluded.Add(new MeteringedQuantityIncluded()
                {
                    DimensionId = metering.DimensionId,
                    Units = metering.Units
                });
            }

            components.RecurrentBillingTerms.Add(recurrentBillingTerm);
        }

        return components;
    }

    /// <summary>
    /// Convert operation model to operationResult model.
    /// </summary>
    /// <param name="operation">The operation model.</param>
    /// <returns>
    /// OperationResult Model.
    /// </returns>
    public static OperationResult operationResult(this SaaSOperation operation)
    {
        return new OperationResult() 
        { 
            ID = operation.Id?.ToString(),
            Status = (Models.OperationStatusEnum)Enum.Parse(typeof(Models.OperationStatusEnum), operation.Status.ToString()),
            Created = operation.TimeStamp.Value.UtcDateTime,
            SubscriptionId = operation.SubscriptionId?.ToString(), 
            ActionType = operation.Action?.ToString()
        };
    }
}