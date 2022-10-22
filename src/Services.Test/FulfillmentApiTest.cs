// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using Marketplace.SaaS.Accelerator.Services.Configurations;
using Marketplace.SaaS.Accelerator.Services.Exceptions;
using Marketplace.SaaS.Accelerator.Services.Services;

namespace Marketplace.SaaS.Accelerator.Services.Test;

using System;
using System.Linq;
using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Marketplace.SaaS;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// FulfillmentApi Test Class.
/// </summary>
[TestClass]
public class FulfillmentApiTest
{
    /// <summary>
    /// The client.
    /// </summary>
    private FulfillmentApiService fulfillApiService;

    /// <summary>
    /// The configuration.
    /// </summary>
    private SaaSApiClientConfiguration configuration = new SaaSApiClientConfiguration();

    /// <summary>
    /// Initializes a new instance of the <see cref="FulfillmentApiTest" /> class.
    /// </summary>
    public FulfillmentApiTest()
    {
        var builder = new ConfigurationBuilder();

        IConfigurationRoot config = new ConfigurationBuilder()
           .AddJsonFile("appsettings.test.json")
           .Build();

        configuration = config.GetSection("AppSetting").Get<SaaSApiClientConfiguration>();
        var creds = new ClientSecretCredential(configuration.TenantId.ToString(), configuration.ClientId.ToString(), configuration.ClientSecret);
        fulfillApiService = new FulfillmentApiService(new MarketplaceSaaSClient(creds), sdkSettings: configuration, null);
    }

    /// <summary>
    /// Gets the subscription by identifier.
    /// </summary>
    /// <returns>Test Subscription By Identifier.</returns>
    [TestMethod]
    public async Task GetSubscriptionByID()
    {
        var allSubscriptions = await fulfillApiService.GetAllSubscriptionAsync().ConfigureAwait(false);
        var subscriptionId = allSubscriptions.FirstOrDefault().Id;
        var subscriptionDetail = await fulfillApiService.GetSubscriptionByIdAsync(subscriptionId);
        Assert.IsNotNull(subscriptionDetail);
        Assert.AreEqual(subscriptionId, subscriptionDetail?.Id);
    }

    /// <summary>
    /// Gets the subscription by identifier exception.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [ExpectedException(typeof(MarketplaceException), "Subscription Not Found")]
    [TestMethod]
    public async Task GetSubscriptionByIDException()
    {
        var subscriptionId = Guid.NewGuid();
        _ = await fulfillApiService.GetSubscriptionByIdAsync(subscriptionId);
    }
}
