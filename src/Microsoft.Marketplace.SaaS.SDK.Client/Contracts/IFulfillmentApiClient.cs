namespace Microsoft.Marketplace.SaasKit.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Marketplace.SaasKit.Models;

    /// <summary>
    /// Interface AMPClient
    /// </summary>
    public interface IFulfillmentApiClient
    {
        /// <summary>
        /// Defines the CONTENTTYPE_URLENCODED
        /// </summary>
        const string CONTENTTYPEURLENCODED = "application/x-www-form-urlencoded";

        /// <summary>
        /// Defines the CONTENTTYPE_APPLICATIONJSON
        /// </summary>
        const string CONTENTTYPEAPPLICATIONJSON = "application/json";

        /// <summary>
        /// Defines the MARKETPLACE_TOKEN
        /// </summary>
        const string MARKETPLACETOKEN = "x-ms-marketplace-token";

        /// <summary>
        /// Gets the subscription by subscription identifier.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns>Subscription Detail By SubscriptionId </returns>
        Task<SubscriptionResult> GetSubscriptionByIdAsync(Guid subscriptionId);

        /// <summary>
        /// Resolves the Subscription.
        /// </summary>
        /// <param name="marketPlaceAccessToken">The Market Place access token.</param>
        /// <returns>Resolve Subscription</returns>
        Task<ResolvedSubscriptionResult> ResolveAsync(string marketPlaceAccessToken);

        /// <summary>
        /// Gets all plans for subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns>Get All Plans For Subscription</returns>
        Task<List<PlanDetailResult>> GetAllPlansForSubscriptionAsync(Guid subscriptionId);

        /// <summary>
        /// Activates the subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="subscriptionPlanID">The subscription plan identifier.</param>
        /// <returns>Activate SubscriptionAsync</returns>
        Task<SubscriptionUpdateResult> ActivateSubscriptionAsync(Guid subscriptionId, string subscriptionPlanID);

        /// <summary>
        /// Changes the plan for subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="subscriptionPlanID">The subscription plan identifier.</param>
        /// <returns>Change Plan For Subscription</returns>
        Task<SubscriptionUpdateResult> ChangePlanForSubscriptionAsync(Guid subscriptionId, string subscriptionPlanID);

        /// <summary>
        /// Gets the operation status result.
        /// </summary>
        /// <param name="operationLocation">The operation location.</param>
        /// <returns>Get Operation Status Result</returns>
        Task<OperationResult> GetOperationStatusResultAsync(Guid subscriptionId, Guid operationId);

        /// <summary>
        /// Deletes the subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="subscriptionPlanID">The subscription plan identifier.</param>
        /// <returns>Delete Subscription</returns>
        Task<SubscriptionUpdateResult> DeleteSubscriptionAsync(Guid subscriptionId, string subscriptionPlanID);

        /// <summary>
        /// Get all subscriptions asynchronously.
        /// </summary>
        /// <returns> List of subscriptions</returns>
        Task<List<SubscriptionResult>> GetAllSubscriptionAsync();

        /// <summary>
        /// Gets the saas application URL.
        /// </summary>
        /// <returns></returns>
        string GetSaaSAppURL();
    }
}