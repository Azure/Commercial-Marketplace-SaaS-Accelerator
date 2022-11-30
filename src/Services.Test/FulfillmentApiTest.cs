// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Azure.Identity;
using Marketplace.SaaS.Accelerator.Services.Configurations;
using Marketplace.SaaS.Accelerator.Services.Exceptions;
using Marketplace.SaaS.Accelerator.Services.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Marketplace.SaaS;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Marketplace.SaaS.Accelerator.Services.Test;

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

        this.configuration = config.GetSection("AppSetting").Get<SaaSApiClientConfiguration>();
        var creds = new ClientSecretCredential(configuration.TenantId.ToString(), configuration.ClientId.ToString(), configuration.ClientSecret);
        this.fulfillApiService = new FulfillmentApiService(new MarketplaceSaaSClient(creds), sdkSettings:this.configuration, null);
    }

    /// <summary>
    /// Gets the subscription by identifier.
    /// </summary>
    /// <returns>Test Subscription By Identifier.</returns>
    [TestMethod]
    public async Task GetSubscriptionByID()
    {
        var allSubscriptions = await this.fulfillApiService.GetAllSubscriptionAsync().ConfigureAwait(false);
        var subscriptionId = allSubscriptions.FirstOrDefault().Id;
        var subscriptionDetail = await this.fulfillApiService.GetSubscriptionByIdAsync(subscriptionId);
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
        _ = await this.fulfillApiService.GetSubscriptionByIdAsync(subscriptionId);
    }
}