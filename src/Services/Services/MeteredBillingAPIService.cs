// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.
namespace Microsoft.Marketplace.SaaS.SDK.Services.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Marketplace.Metering;
    using Microsoft.Marketplace.Metering.Models;
    using Microsoft.Marketplace.SaaS.SDK.Services.Configurations;
    using Microsoft.Marketplace.SaaS.SDK.Services.Contracts;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;

    /// <summary>
    /// Metered Api Client.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaaS.SDK.Services.Contracts.IMeteredBillingApiService" />
    public class MeteredBillingApiService : BaseApiService, IMeteredBillingApiService
    {
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
        /// Gets or sets the Marketplace Metering client.
        /// </summary>
        /// <value>
        /// The SDK settings.
        /// </value>
        private readonly IMarketplaceMeteringClient meteringClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="MeteredBillingApiClient"/> class.
        /// </summary>
        /// <param name="sdkSettings">The SDK settings.</param>
        /// <param name="logger">The logger.</param>
        public MeteredBillingApiService(IMarketplaceMeteringClient meteringClient, SaaSApiClientConfiguration sdkSettings, ILogger logger):base(logger)
        {
            this.meteringClient = meteringClient;
            this.ClientConfiguration = sdkSettings;
            this.Logger = logger;
        }

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

            var usage = new UsageEvent()
            {
                ResourceId = subscriptionUsageRequest.ResourceId,
                PlanId = subscriptionUsageRequest.PlanId,
                Dimension = subscriptionUsageRequest.Dimension,
                Quantity = subscriptionUsageRequest.Quantity,
                EffectiveStartTime = subscriptionUsageRequest.EffectiveStartTime,
            };

            try
            {
                var updateResult = (await this.meteringClient.Metering.PostUsageEventAsync(usage)).Value;
                return new MeteringUsageResult()
                {
                    Dimension = updateResult.Dimension,
                    MessageTime = updateResult.MessageTime.Value.UtcDateTime,
                    PlanId = updateResult.PlanId,
                    Quantity = (long)updateResult.Quantity,
                    ResourceId = updateResult.ResourceId.Value,
                    Status = updateResult.Status.ToString(),
                    UsagePostedDate = updateResult.EffectiveStartTime.Value.UtcDateTime,
                    UsageEventId = updateResult.UsageEventId.Value
                };
            }
            catch (Exception ex)
            {
                this.ProcessErrorResponse(MarketplaceActionEnum.SUBSCRIPTION_USAGEEVENT, ex);
                return null;
            }
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

            BatchUsageEvent batchUsageEvent = new BatchUsageEvent();
            foreach(MeteringUsageRequest usage in subscriptionBatchUsageRequest)
            {
                batchUsageEvent.Request.Add(new UsageEvent()
                {
                    ResourceId = usage.ResourceId,
                    PlanId = usage.PlanId,
                    Dimension = usage.Dimension,
                    Quantity = usage.Quantity,
                    EffectiveStartTime = usage.EffectiveStartTime,
                });
            }

            try
            {
                var updateResult = (await this.meteringClient.Metering.PostBatchUsageEventAsync(batchUsageEvent)).Value;
            }
            catch (Exception ex)
            {
                this.ProcessErrorResponse(MarketplaceActionEnum.SUBSCRIPTION_BATCHUSAGEEVENT, ex);
                return null;
            }

            return new MeteringBatchUsageResult();
        }
    }
}