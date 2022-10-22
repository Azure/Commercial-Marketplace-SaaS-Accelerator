// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using Marketplace.SaaS.Accelerator.Services.Configurations;
using Marketplace.SaaS.Accelerator.Services.Models;
using Marketplace.SaaS.Accelerator.Services.Services;

namespace Marketplace.SaaS.Accelerator.Services.Test;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Marketplace.Metering;
using Microsoft.Marketplace.SaaS;
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

        configuration = config.GetSection("AppSetting").Get<SaaSApiClientConfiguration>();
        var creds = new ClientSecretCredential(configuration.TenantId.ToString(), configuration.ClientId.ToString(), configuration.ClientSecret);
        merteringService = new MeteredBillingApiService(new MarketplaceMeteringClient(creds), configuration, null);
        fulfillApiService = new FulfillmentApiService(new MarketplaceSaaSClient(creds), sdkSettings: configuration, null);
    }

    /// <summary>
    /// Gets the subscription by identifier.
    /// </summary>
    /// <returns>Test Subscription Usage.</returns>
    [TestMethod]
    public async Task TestSubscriptionUsage()
    {
        var allSubscriptions = await fulfillApiService.GetAllSubscriptionAsync().ConfigureAwait(false);
        var defaultSubscription = allSubscriptions.FirstOrDefault();

        MeteringUsageRequest subscriptionUsageRequest = new MeteringUsageRequest()
        {
            Dimension = "Test",
            EffectiveStartTime = DateTime.UtcNow,
            PlanId = defaultSubscription?.PlanId,
            Quantity = 5,
            ResourceId = defaultSubscription.Id,
        };
        var response = merteringService.EmitUsageEventAsync(subscriptionUsageRequest).Result;
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
        var allSubscriptions = await fulfillApiService.GetAllSubscriptionAsync().ConfigureAwait(false);
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
        var response = await merteringService.EmitBatchUsageEventAsync(subscriptionUsageRequest);
        Assert.AreEqual(response.Count, 2);
    }
}