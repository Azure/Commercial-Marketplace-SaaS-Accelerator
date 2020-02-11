namespace Microsoft.Marketplace.SaasKit.UnitTest
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Marketplace.SaasKit.Configurations;
    using Microsoft.Marketplace.SaasKit.Models;
    using Microsoft.Marketplace.SaasKit.Helpers;
    using Microsoft.Marketplace.SaasKit.Services;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// FulfillmentApi Test Class
    /// </summary>
    [TestClass]
    public class FulfillmentApiTest
    {
        /// <summary>
        /// The client
        /// </summary>
        public FulfillmentApiClient Client;

        /// <summary>
        /// The configuration
        /// </summary>
        private SaaSApiClientConfiguration Configuration = new SaaSApiClientConfiguration();

        /// <summary>
        /// Initializes a new instance of the <see cref="FulfillmentApiTest" /> class.
        /// </summary>
        public FulfillmentApiTest()
        {
            var builder = new ConfigurationBuilder();

            IConfigurationRoot config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.test.json")
               .Build();

            this.Configuration = config.GetSection("AppSetting").Get<SaaSApiClientConfiguration>();
            this.Client = new FulfillmentApiClient(Configuration, null);
        }

        /// <summary>Checks the authentication.</summary>
        [TestMethod]
        public async Task CheckAuthentication()
        {
            var accessTokenResult = await ADAuthenticationHelper.GetAccessToken(Configuration).ConfigureAwait(false);
            Assert.IsNotNull(accessTokenResult);
            Assert.IsNotNull(accessTokenResult?.AccessToken);
        }

        /// <summary>
        /// Gets the subscription by identifier.
        /// </summary>
        /// <returns>Test Subscription By Identifier</returns>
        [TestMethod]
        public async Task GetSubscriptionByID()
        {
            var allSubscriptions = await this.Client.GetAllSubscriptionAsync().ConfigureAwait(false);
            var subscriptionId = allSubscriptions.FirstOrDefault().Id;
            var subscriptionDetail = await this.Client.GetSubscriptionByIdAsync(subscriptionId);
            Assert.IsNotNull(subscriptionDetail);
            Assert.AreEqual(subscriptionId, subscriptionDetail?.Id);
        }

        /// <summary>
        /// Gets the subscription by identifier exception.
        /// </summary>
        [ExpectedException(typeof(FulfillmentException), "Subscription Not Found")]
        [TestMethod]
        public async Task GetSubscriptionByIDException()
        {
            var subscriptionId = Guid.NewGuid();
            _ = await this.Client.GetSubscriptionByIdAsync(subscriptionId);
        }
    }
}
