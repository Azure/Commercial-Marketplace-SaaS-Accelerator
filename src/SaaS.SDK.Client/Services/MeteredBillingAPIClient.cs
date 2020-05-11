namespace Microsoft.Marketplace.SaasKit.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Marketplace.SaasKit.Configurations;
    using Microsoft.Marketplace.SaasKit.Contracts;
    using Microsoft.Marketplace.SaasKit.Helpers;
    using Microsoft.Marketplace.SaasKit.Models;
    using Microsoft.Marketplace.SaasKit.Network;

    /// <summary>
    /// Metered Api Client.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Contracts.IMeteredBillingApiClient" />
    public class MeteredBillingApiClient : IMeteredBillingApiClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MeteredBillingApiClient"/> class.
        /// </summary>
        /// <param name="sdkSettings">The SDK settings.</param>
        /// <param name="logger">The logger.</param>
        public MeteredBillingApiClient(SaaSApiClientConfiguration sdkSettings, ILogger logger)
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
        /// Manage Subscription Usage.
        /// </summary>
        /// <param name="subscriptionUsageRequest">The subscription usage request.</param>
        /// <returns>
        /// Subscription Usage.
        /// </returns>
        public async Task<MeteringUsageResult> EmitUsageEventAsync(MeteringUsageRequest subscriptionUsageRequest)
        {
            this.Logger?.Info($"Inside ManageSubscriptionUsageAsync() of FulfillmentApiClient, trying to Manage Subscription Usage :: {subscriptionUsageRequest.ResourceId}");

            var restClient = new MeteringApiRestClient<MeteringUsageResult>(this.ClientConfiguration, this.Logger);

            var url = UrlHelper.GetSaaSApiUrl(this.ClientConfiguration, subscriptionUsageRequest.ResourceId, SaaSResourceActionEnum.SUBSCRIPTION_USAGEEVENT);

            Dictionary<string, object> usageEventRequest = new Dictionary<string, object>();
            usageEventRequest.Add("resourceId", Convert.ToString(subscriptionUsageRequest.ResourceId));
            usageEventRequest.Add("quantity", subscriptionUsageRequest.Quantity);
            usageEventRequest.Add("dimension", subscriptionUsageRequest.Dimension);
            usageEventRequest.Add("effectiveStartTime", subscriptionUsageRequest.EffectiveStartTime);
            usageEventRequest.Add("planId", subscriptionUsageRequest.PlanId);

            var meteringUsageResult = await restClient.DoRequest(url, HttpMethods.POST, usageEventRequest).ConfigureAwait(false);

            return meteringUsageResult;
        }

        /// <summary>
        /// Manage Subscription Batch Usage.
        /// </summary>
        /// <param name="subscriptionBatchUsageRequest">The subscription batch usage request.</param>
        /// <returns>
        /// Subscription Usage.
        /// </returns>
        public async Task<MeteringBatchUsageResult> EmitBatchUsageEventAsync(IEnumerable<MeteringUsageRequest> subscriptionBatchUsageRequest)
        {
            this.Logger?.Info($"Inside ManageSubscriptionUsageAsync() of FulfillmentApiClient, with number of request items :: {subscriptionBatchUsageRequest.Count()} and trying to Manage Subscription Batch Usage :: {subscriptionBatchUsageRequest.FirstOrDefault()?.ResourceId}");

            var restClient = new MeteringApiRestClient<MeteringBatchUsageResult>(this.ClientConfiguration, this.Logger);

            var url = UrlHelper.GetSaaSApiUrl(this.ClientConfiguration, Guid.Empty, SaaSResourceActionEnum.SUBSCRIPTION_BATCHUSAGEEVENT);

            Dictionary<string, object> batchUsageEventRequest = new Dictionary<string, object>
            {
                { "request", subscriptionBatchUsageRequest },
            };

            var meteringBatchUsageResult = await restClient.DoRequest(url, HttpMethods.POST, batchUsageEventRequest).ConfigureAwait(false);

            return meteringBatchUsageResult;
        }
    }
}