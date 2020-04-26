using Microsoft.Azure.WebJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Contracts;
using Microsoft.Marketplace.SaasKit.WebJob.Models;
using Microsoft.Marketplace.SaasKit.WebJob.StatusHandlers;
using Saas.SDK.WebJob;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Marketplace.SaasKit.WebJob
{
    class Program
    {
        private static IFulfillmentApiClient fulfillApiclient;
        private static IApplicationConfigRepository applicationConfigrepository;
        private static ISubscriptionsRepository subscriptionsrepository;
        private static IConfiguration configuration;
        IServiceCollection services;
        static void Main(string[] args)
        {

            IServiceCollection services = new ServiceCollection();
            // Startup.cs finally :)
            Startup startup = new Startup(configuration);
            startup.ConfigureServices(services);
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            //configure console logging
            serviceProvider
                .GetService<ILoggerFactory>();
            //.AddConsole(LogLevel.Debug);

            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();

            logger.LogDebug("Logger is working!");

            // Get Service and call method
            //var service = serviceProvider.GetService<IMyService>();
            //service.MyServiceMethod();

            SubscriptionProcessQueueModel model = new SubscriptionProcessQueueModel()
            {
                SubscriptionID = Guid.Parse("25A8379E-E87E-DDDF-C337-259F4FADB09D"),
                TriggerEvent = "Activate"

            };
            ProcessMethod(model);
            //JobHost jobHost = new JobHost();
            //jobHost.CallAsync(typeof(Functions).GetMethod("ProcessMethod"));
            //jobHost.RunAndBlock();
        }

        public Program(IFulfillmentApiClient fulfillApiClient, IApplicationConfigRepository applicationConfigRepository, ISubscriptionsRepository subscriptionsRepository, IConfiguration Configuration, IServiceCollection Services)
        {
            fulfillApiclient = fulfillApiClient;
            applicationConfigrepository = applicationConfigRepository;
            subscriptionsrepository = subscriptionsRepository;
            Configuration = configuration;
            Services = services;
        }

        protected static List<ISubscriptionStatusHandler> activateStatusHandlers = new List<ISubscriptionStatusHandler>()
        {
            new ResourceDeploymentStatusHandler(fulfillApiclient),
            new PendingActivationStatusHandler(fulfillApiclient,applicationConfigrepository,subscriptionsrepository),
            new ActivatedStatusHandler(fulfillApiclient),
            new NotificationStatusHandler(fulfillApiclient)
        };
        protected static List<ISubscriptionStatusHandler> deactivateStatusHandlers = new List<ISubscriptionStatusHandler>()
        {

            new PendingDeleteStatusHandler(fulfillApiclient),
            new UnsubscribeStatusHandler(fulfillApiclient),
            new NotificationStatusHandler(fulfillApiclient)
        };

        public static void ProcessMethod(SubscriptionProcessQueueModel model)
        {
            if (model.TriggerEvent == "Activate")
            {
                foreach (var subscriptionStatusHandler in activateStatusHandlers)
                {
                    subscriptionStatusHandler.Process(model.SubscriptionID);
                }
            }
            if (model.TriggerEvent == "Unsubscribe")
            {
                foreach (var subscriptionStatusHandler in deactivateStatusHandlers)
                {
                    subscriptionStatusHandler.Process(model.SubscriptionID);
                }
            }
        }
    }
}
