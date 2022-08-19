// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.
namespace Microsoft.Marketplace.SaaS.SDK.Services.UnitTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using global::Azure.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Marketplace.Metering;
    using Microsoft.Marketplace.SaaS.SDK.Services.Configurations;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using Microsoft.Marketplace.SaaS.SDK.Services.Services;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Metered API Test.
    /// </summary>
    [TestClass]
    public class MeteredApiTest
    {
        /// <summary>The client.</summary>
        private MeteredBillingApiService merteringService;

        /// <summary>
        /// The client.
        /// </summary>
        private FulfillmentApiService fulfillApiService;

        /// <summary>The configuration.</summary>
        private SaaSApiClientConfiguration configuration = new SaaSApiClientConfiguration();

        /// <summary>Initializes a new instance of the <see cref="MeteredApiTest"/> class.</summary>
        public MeteredApiTest()
        {
            var builder = new ConfigurationBuilder();

            IConfigurationRoot config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.test.json")
               .Build();

            this.configuration = config.GetSection("AppSetting").Get<SaaSApiClientConfiguration>();
            var creds = new ClientSecretCredential(configuration.TenantId.ToString(), configuration.ClientId.ToString(), configuration.ClientSecret);
            this.merteringService = new MeteredBillingApiService(new MarketplaceMeteringClient(creds), this.configuration, null);
            this.fulfillApiService = new FulfillmentApiService(new MarketplaceSaaSClient(creds), sdkSettings: this.configuration, null);
        }

        /// <summary>
        /// Gets the subscription by identifier.
        /// </summary>
        /// <returns>Test Subscription Usage.</returns>
        [TestMethod]
        public async Task TestSubscriptionUsage()
        {
            var allSubscriptions = await this.fulfillApiService.GetAllSubscriptionAsync().ConfigureAwait(false);
            var defaultSubscription = allSubscriptions.FirstOrDefault();

            MeteringUsageRequest subscriptionUsageRequest = new MeteringUsageRequest()
            {
                Dimension = "Test",
                EffectiveStartTime = DateTime.UtcNow,
                PlanId = defaultSubscription?.PlanId,
                Quantity = 5,
                ResourceId = defaultSubscription.Id,
            };
            var response = this.merteringService.EmitUsageEventAsync(subscriptionUsageRequest).Result;
            Assert.AreEqual(response.Status, "Accepted");
            Assert.AreEqual(response.ResourceId, defaultSubscription?.Id);
            Assert.AreEqual(response.PlanId, defaultSubscription?.PlanId);
        }

        /// <summary>
        /// Test subscription batch usage.
        /// </summary>
        /// <returns>Test Subscription Batch Usage.</returns>
        [TestMethod]
        public async Task TestSubscriptionBatchUsage()
        {
            var allSubscriptions = await this.fulfillApiService.GetAllSubscriptionAsync().ConfigureAwait(false);
            var defaultSubscription = allSubscriptions.FirstOrDefault();

            var subscriptionUsageRequest = new List<MeteringUsageRequest>
            {
                new MeteringUsageRequest()
                {
                    Dimension = "Test",
                    EffectiveStartTime = DateTime.UtcNow,
                    PlanId = defaultSubscription?.PlanId,
                    Quantity = 5,
                    ResourceId = defaultSubscription.Id,
                },
                new MeteringUsageRequest()
                {
                    Dimension = "Test",
                    EffectiveStartTime = DateTime.UtcNow,
                    PlanId = defaultSubscription?.PlanId,
                    Quantity = 5,
                    ResourceId = defaultSubscription.Id,
                },
            };
            var response = await this.merteringService.EmitBatchUsageEventAsync(subscriptionUsageRequest);
            Assert.AreEqual(response.Count, 2);
        }
    }
}