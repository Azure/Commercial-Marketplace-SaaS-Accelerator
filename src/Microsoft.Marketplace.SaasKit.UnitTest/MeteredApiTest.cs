namespace Microsoft.Marketplace.SaasKit.UnitTest
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Marketplace.SaasKit.Configurations;
    using Microsoft.Marketplace.SaasKit.Helpers;
    using Microsoft.Marketplace.SaasKit.Models;
    using Microsoft.Marketplace.SaasKit.Services;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Metered API Test
    /// </summary>
    [TestClass]
    public class MeteredApiTest
    {
        /// <summary>The client</summary>
        private MeteredBillingApiClient client;

        /// <summary>The client</summary>
        private FulfillmentApiClient fulfillmentClient;

        /// <summary>The configuration</summary>
        private SaaSApiClientConfiguration configuration = new SaaSApiClientConfiguration();

        /// <summary>Initializes a new instance of the <see cref="MeteredApiTest"/> class.</summary>
        public MeteredApiTest()
        {
            var builder = new ConfigurationBuilder();

            IConfigurationRoot config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.test.json")
               .Build();

            this.configuration = config.GetSection("AppSetting").Get<SaaSApiClientConfiguration>();
            this.client = new MeteredBillingApiClient(this.configuration, null);
            this.fulfillmentClient = new FulfillmentApiClient(this.configuration, null);
        }

        /// <summary>
        /// Check Authentication.
        /// </summary>
        /// <returns>Test Authentication</returns>
        [TestMethod]
        public async Task CheckAuthentication()
        {
            var accessTokenResult = await ADAuthenticationHelper.GetAccessToken(this.configuration).ConfigureAwait(false);
            Assert.IsNotNull(accessTokenResult);
            Assert.IsNotNull(accessTokenResult?.AccessToken);
        }

        /// <summary>
        /// Gets the subscription by identifier.
        /// </summary>
        /// <returns>Test Subscription Usage</returns>
        [TestMethod]
        public async Task TestSubscriptionUsage()
        {
            var allSubscriptions = await this.fulfillmentClient.GetAllSubscriptionAsync().ConfigureAwait(false);
            var defaultSubscription = allSubscriptions.FirstOrDefault();

            MeteringUsageRequest subscriptionUsageRequest = new MeteringUsageRequest()
            {
                Dimension = "Test",
                EffectiveStartTime = DateTime.UtcNow,
                PlanId = defaultSubscription?.PlanId,
                Quantity = 5,
                ResourceId = defaultSubscription.Id
            };
            var response = this.client.EmitUsageEventAsync(subscriptionUsageRequest).Result;            
            Assert.AreEqual(response.Status, "Accepted");
            Assert.AreEqual(response.ResourceId, defaultSubscription?.Id);
            Assert.AreEqual(response.PlanId, defaultSubscription?.PlanId);
        }
    }
}