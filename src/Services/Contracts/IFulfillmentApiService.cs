// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using Marketplace.SaaS.Accelerator.Services.Models;
using Microsoft.Marketplace.SaaS.Models;

namespace Marketplace.SaaS.Accelerator.Services.Contracts;

/// <summary>
/// Interface AMPClient.
/// </summary>
public interface IFulfillmentApiService
{
    /// <summary>
    /// Defines the CONTENTTYPE_URLENCODED.
    /// </summary>
    const string CONTENTTYPEURLENCODED = "application/x-www-form-urlencoded";

    /// <summary>
    /// Defines the CONTENTTYPE_APPLICATIONJSON.
    /// </summary>
    const string CONTENTTYPEAPPLICATIONJSON = "application/json";

    /// <summary>
    /// Defines the MARKETPLACE_TOKEN.
    /// </summary>
    const string MARKETPLACETOKEN = "x-ms-marketplace-token";

    /// <summary>
    /// Gets the subscription by subscription identifier.
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <returns>Subscription Detail By SubscriptionId. </returns>
    Task<SubscriptionResult> GetSubscriptionByIdAsync(Guid subscriptionId);

    /// <summary>
    /// Gets the subscription by subscription identifier.
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <returns>Subscription Detail By SubscriptionId. </returns>
    SubscriptionResult GetSubscriptionById(Guid subscriptionId);

    /// <summary>
    /// Resolves the Subscription.
    /// </summary>
    /// <param name="marketPlaceAccessToken">The Market Place access token.</param>
    /// <returns>Resolve Subscription.</returns>
    Task<ResolvedSubscriptionResult> ResolveAsync(string marketPlaceAccessToken);

    /// <summary>
    /// Gets all plans for subscription.
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <returns>Get All Plans For Subscription.</returns>
    Task<List<PlanDetailResultExtension>> GetAllPlansForSubscriptionAsync(Guid subscriptionId);

    /// <summary>
    /// Activates the subscription.
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <param name="subscriptionPlanID">The subscription plan identifier.</param>
    /// <returns>Activate SubscriptionAsync.</returns>
    Task<Response> ActivateSubscriptionAsync(Guid subscriptionId, string subscriptionPlanID);

    /// <summary>
    /// Changes the plan for subscription.
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <param name="subscriptionPlanID">The subscription plan identifier.</param>
    /// <returns>Change Plan For Subscription.</returns>
    Task<SubscriptionUpdateResult> ChangePlanForSubscriptionAsync(Guid subscriptionId, string subscriptionPlanID);

    /// <summary>
    /// Changes the plan for subscription.
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <param name="subscriptionQuantity">The subscription plan identifier.</param>
    /// <returns>Change Plan For Subscription.</returns>
    Task<SubscriptionUpdateResult> ChangeQuantityForSubscriptionAsync(Guid subscriptionId, int? subscriptionQuantity);

    /// <summary>
    /// Gets the operation status result.
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <param name="operationId">The operation identifier.</param>
    /// <returns>
    /// Get Operation Status Result.
    /// </returns>
    Task<OperationResult> GetOperationStatusResultAsync(Guid subscriptionId, Guid operationId);

    /// <summary>
    /// Update the operation status result.
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <param name="operationId">The operation identifier.</param>
    /// <param name="updateOperationStatus">The operation status to patch with.</param>
    /// <returns>
    /// Get Operation Status Result.
    /// </returns>
    Task<Response> PatchOperationStatusResultAsync(Guid subscriptionId, Guid operationId, UpdateOperationStatusEnum updateOperationStatus);

    /// <summary>
    /// Deletes the subscription.
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <param name="subscriptionPlanID">The subscription plan identifier.</param>
    /// <returns>Delete Subscription.</returns>
    Task<SubscriptionUpdateResult> DeleteSubscriptionAsync(Guid subscriptionId, string subscriptionPlanID);

    /// <summary>
    /// Get all subscriptions asynchronously.
    /// </summary>
    /// <returns> List of subscriptions.</returns>
    Task<List<SubscriptionResult>> GetAllSubscriptionAsync();

    /// <summary>
    /// Gets the saas application URL.
    /// </summary>
    /// <returns> Saas URL.</returns>
    string GetSaaSAppURL();
}