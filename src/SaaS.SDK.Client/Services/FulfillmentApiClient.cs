namespace Microsoft.Marketplace.SaasKit.Services
{
    using Microsoft.Marketplace.SaasKit.Configurations;
    using Microsoft.Marketplace.SaasKit.Contracts;
    using Microsoft.Marketplace.SaasKit.Helpers;
    using Microsoft.Marketplace.SaasKit.Models;
    using Microsoft.Marketplace.SaasKit.Network;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Fulfillment API Client Action-List For Subscriptions
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Contracts.IFulfilmentApiClient" />
    public class FulfillmentApiClient : IFulfillmentApiClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FulfillmentApiClient" /> class.
        /// </summary>
        /// <param name="sdkSettings">The SDK settings.</param>
        /// <param name="logger">The logger.</param>
        public FulfillmentApiClient(SaaSApiClientConfiguration sdkSettings, ILogger logger)
        {
            this.ClientConfiguration = sdkSettings;
            this.Logger = logger;
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        protected ILogger Logger { get; set; }

        /// <summary>
        /// Gets or sets the SDK settings.
        /// </summary>
        /// <value>
        /// The SDK settings.
        /// </value>
        protected SaaSApiClientConfiguration ClientConfiguration { get; set; }

        /// <summary>
        /// Get all subscriptions asynchronously.
        /// </summary>
        /// <returns> List of subscriptions</returns>
        public async Task<List<SubscriptionResult>> GetAllSubscriptionAsync()
        {
            this.Logger?.Info($"Inside GetAllSubscriptionAsync() of FulfillmentApiClient, trying to get All Subscriptions.");
            var restClient = new FulfillmentApiRestClient<SubscriptionListResult>(this.ClientConfiguration, this.Logger);
            var url = UrlHelper.GetSaaSApiUrl(this.ClientConfiguration, default, SaaSResourceActionEnum.ALL_SUBSCRIPTIONS, null);
            var subscriptResult = await restClient.DoRequest(url, HttpMethods.GET, null).ConfigureAwait(false);
            return subscriptResult.SubscriptionsResult;
        }

        /// <summary>
        /// Gets Subscription By SubscriptionId.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns>
        /// Returns Subscription By SubscriptionId
        /// </returns>
        public async Task<SubscriptionResult> GetSubscriptionByIdAsync(Guid subscriptionId)
        {
            this.Logger?.Info($"Inside GetSubscriptionByIdAsync() of FulfillmentApiClient, trying to gets the Subscription Detail by subscriptionId : {subscriptionId}");
            var restClient = new FulfillmentApiRestClient<SubscriptionResult>(this.ClientConfiguration, this.Logger);
            var url = UrlHelper.GetSaaSApiUrl(this.ClientConfiguration, subscriptionId, null);
            var subscriptResult = await restClient.DoRequest(url, HttpMethods.GET, null).ConfigureAwait(false);
            return subscriptResult;
        }

        /// <summary>
        /// Resolves the Subscription
        /// </summary>
        /// <param name="marketPlaceAccessToken">The marketPlace access token.</param>
        /// <returns>
        /// Resolve Subscription
        /// </returns>
        public async Task<ResolvedSubscriptionResult> ResolveAsync(string marketPlaceAccessToken)
        {
            this.Logger?.Info($"Inside ResolveAsync() of FulfillmentApiClient, trying to resolve the Subscription by MarketPlaceToken");
            var resolvedSubscription = default(ResolvedSubscriptionResult);

            if (!string.IsNullOrEmpty(marketPlaceAccessToken))
            {
                var restClient = new FulfillmentApiRestClient<ResolvedSubscriptionResult>(this.ClientConfiguration, this.Logger);
                var url = UrlHelper.GetSaaSApiUrl(this.ClientConfiguration, new Guid(), SaaSResourceActionEnum.RESOLVE);
                var resolveTokenHeaders = new Dictionary<string, object>();
                resolveTokenHeaders.Add(IFulfillmentApiClient.MARKETPLACETOKEN, marketPlaceAccessToken);
                resolvedSubscription = await restClient.DoRequest(url, HttpMethods.POST, null /* parameters */, resolveTokenHeaders).ConfigureAwait(false); ;
            }

            return resolvedSubscription;
        }

        /// <summary>
        /// GetAllPlansForSubscription By SubscriptionId
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns>
        /// Get AllPlans For SubscriptionId
        /// </returns>
        /// <exception cref="FulfillmentException">Invalid subscription ID</exception>
        public async Task<List<PlanDetailResult>> GetAllPlansForSubscriptionAsync(Guid subscriptionId)
        {
            this.Logger?.Info($"Inside GetAllPlansForSubscriptionAsync() of FulfillmentApiClient, trying to Get All Plans for {subscriptionId}");
            if (subscriptionId != default)
            {
                var restClient = new FulfillmentApiRestClient<PlanResult>(this.ClientConfiguration, this.Logger);
                var url = UrlHelper.GetSaaSApiUrl(this.ClientConfiguration, subscriptionId, SaaSResourceActionEnum.LISTALLPLAN);
                var result = await restClient.DoRequest(url, HttpMethods.GET, null).ConfigureAwait(false);
                return result?.Plans;
            }

            throw new FulfillmentException("Invalid subscription ID", SaasApiErrorCode.BadRequest);
        }

        /// <summary>
        /// Changes the plan for subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="subscriptionPlanID">The subscription plan identifier.</param>
        /// <returns>
        /// Change Plan For Subscription
        /// </returns>
        /// <exception cref="FulfillmentException">Invalid subscription ID</exception>
        public async Task<SubscriptionUpdateResult> ChangePlanForSubscriptionAsync(Guid subscriptionId, string subscriptionPlanID)
        {
            this.Logger?.Info($"Inside ChangePlanForSubscriptionAsync() of FulfillmentApiClient, trying to Change Plan By {subscriptionId} with New Plan {subscriptionPlanID}");
            if (subscriptionId != default)
            {
                var restClient = new FulfillmentApiRestClient<SubscriptionUpdateResult>(this.ClientConfiguration, this.Logger);
                var payload = new Dictionary<string, object>();
                payload.Add("planId", subscriptionPlanID);
                var url = UrlHelper.GetSaaSApiUrl(this.ClientConfiguration, subscriptionId, null);
                var subscriptionUpdateResult = await restClient.DoRequest(url, HttpMethods.PATCH, payload).ConfigureAwait(false);
                
                return subscriptionUpdateResult;
            }

            throw new FulfillmentException("Invalid subscription ID", SaasApiErrorCode.BadRequest);
        }

        /// <summary>
        /// Gets the operation status result.
        /// </summary>
        /// <param name="subscriptionId">The subscription</param>
        /// <param name="operationId">The operation location.</param>
        /// <returns>
        /// Get Operation Status Result
        /// </returns>
        /// <exception cref="System.Exception">Error occurred while getting the operation result</exception>
        public async Task<OperationResult> GetOperationStatusResultAsync(Guid subscriptionId, Guid operationId)
        {
            this.Logger?.Info($"Inside GetOperationStatusResultAsync() of FulfillmentApiClient, trying to Get Operation Status By Operation ID : {operationId}");
            var restClient = new FulfillmentApiRestClient<OperationResult>(this.ClientConfiguration, this.Logger);

            // TODO: Build url for operation status
            var url = UrlHelper.GetSaaSApiUrl(this.ClientConfiguration, subscriptionId, SaaSResourceActionEnum.OPERATION_STATUS, operationId);
            var operationResult = await restClient.DoRequest(url, HttpMethods.GET, null).ConfigureAwait(false);

            return operationResult;
        }

        /// <summary>
        /// Deletes the subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="subscriptionPlanID">The subscription plan identifier.</param>
        /// <returns>
        /// Delete Subscription
        /// </returns>
        public async Task<SubscriptionUpdateResult> DeleteSubscriptionAsync(Guid subscriptionId, string subscriptionPlanID)
        {
            this.Logger?.Info($"Inside DeleteSubscriptionAsync() of FulfillmentApiClient, trying to Delete Subscription :: {subscriptionId}");
            var restClient = new FulfillmentApiRestClient<SubscriptionUpdateResult>(this.ClientConfiguration, this.Logger);
            var url = UrlHelper.GetSaaSApiUrl(this.ClientConfiguration, subscriptionId, null);

            var subscriptionDeleteResult = await restClient.DoRequest(url, HttpMethods.DELETE, null).ConfigureAwait(false);
            return subscriptionDeleteResult;
        }

        /// <summary>
        /// Activates the subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="subscriptionPlanId">The subscription plan identifier.</param>
        /// <returns>
        /// Activate Subscription
        /// </returns>
        public async Task<SubscriptionUpdateResult> ActivateSubscriptionAsync(Guid subscriptionId, string subscriptionPlanId)
        {
            this.Logger?.Info($"Inside ActivateSubscriptionAsync() of FulfillmentApiClient, trying to Activate Subscription :: {subscriptionId}");
            var restClient = new FulfillmentApiRestClient<SubscriptionUpdateResult>(this.ClientConfiguration, this.Logger);
            var url = UrlHelper.GetSaaSApiUrl(this.ClientConfiguration, subscriptionId, SaaSResourceActionEnum.ACTIVATE);
            var payload = new Dictionary<string, object>();
            payload.Add("planId", subscriptionPlanId);
            var subscriptionActivationResult = await restClient.DoRequest(url, HttpMethods.POST, payload).ConfigureAwait(false);

            return subscriptionActivationResult;
        }

        /// <summary>
        /// Gets the saas application URL.
        /// </summary>
        /// <returns>SaaS App URL</returns>
        public string GetSaaSAppURL()
        {
            try
            {
                return this.ClientConfiguration.SaaSAppUrl;
            }
            catch (Exception) { }
            return string.Empty;
        }
    }
}