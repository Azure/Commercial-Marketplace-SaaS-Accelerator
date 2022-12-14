// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using Xunit;

namespace Microsoft.Marketplace.SaasKit.UnitTest
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using global::Azure.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Marketplace.SaaS;
    using Microsoft.Marketplace.SaaS.SDK.Services.Configurations;
    using Microsoft.Marketplace.SaaS.SDK.Services.Exceptions;
    using Microsoft.Marketplace.SaaS.SDK.Services.Services;

    /// <summary>
    /// FulfillmentApi Test Class.
    /// </summary>
    [Collection("FulfillmentApiTest collection")]
   public class FulfillmentApiTest
{
    private FulfillmentApiService fulfillApiService;
    private SaaSApiClientConfiguration configuration = new SaaSApiClientConfiguration();

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

    [Fact]
    public async Task GetSubscriptionByID()
    {
        var allSubscriptions = await this.fulfillApiService.GetAllSubscriptionAsync().ConfigureAwait(false);
        var subscriptionId = allSubscriptions.FirstOrDefault().Id;
        var subscriptionDetail = await this.fulfillApiService.GetSubscriptionByIdAsync(subscriptionId);
        Assert.NotNull(subscriptionDetail);
        Assert.Equal(subscriptionId, subscriptionDetail?.Id);
    }

    [Fact]
    public async Task GetSubscriptionByIDException()
    {
        var subscriptionId = Guid.NewGuid();

        var ex = await Assert.ThrowsAsync<MarketplaceException>(() => this.fulfillApiService.GetSubscriptionByIdAsync(subscriptionId));
        Assert.Equal("Subscription Not Found", ex.Message);
    }
}

}
