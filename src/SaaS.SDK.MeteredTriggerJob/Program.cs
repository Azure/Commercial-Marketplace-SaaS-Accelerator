using System;
using System.Collections;
using System.Collections.Generic;
using global::Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Marketplace.SaaS.SDK.Services.Models;
using Microsoft.Marketplace.SaaS.SDK.Services.Services;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaaS.SDK.Services.Contracts;
using Microsoft.Marketplace.SaaS.SDK.Services.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Marketplace.Metering;
using Microsoft.Marketplace.SaaS.SDK.Services.Utilities;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Services;
using MeteredTriggerHelper;
namespace MeteredTriggerHelper
{
    class Program
    {
        static void Main (string[] args)
        {

            Console.WriteLine($"MeteredExecutor Webjob Started at: {DateTime.Now}");

            IConfiguration configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            var config = new SaaSApiClientConfiguration()
            {
                AdAuthenticationEndPoint = configuration["SaaSApiConfiguration:AdAuthenticationEndPoint"],
                ClientId = configuration["SaaSApiConfiguration:ClientId"],
                ClientSecret = configuration["SaaSApiConfiguration:ClientSecret"],
                GrantType = configuration["SaaSApiConfiguration:GrantType"],
                Resource = configuration["SaaSApiConfiguration:Resource"],
                TenantId = configuration["SaaSApiConfiguration:TenantId"],
                SupportMeteredBilling = Convert.ToBoolean(configuration["SaaSApiConfiguration:SupportMeteredBilling"])
            };

            var creds = new ClientSecretCredential(config.TenantId.ToString(), config.ClientId.ToString(), config.ClientSecret);

            var services = new ServiceCollection()
                            .AddDbContext<SaasKitContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")))
                            .AddScoped<ISchedulerFrequencyRepository, SchedulerFrequencyRepository>()
                            .AddScoped<IMeteredPlanSchedulerManagementRepository, MeteredPlanSchedulerManagementRepository>()
                            .AddScoped<ISchedulerManagerViewRepository, SchedulerManagerViewRepository>()
                            .AddScoped<ISubscriptionUsageLogsRepository, SubscriptionUsageLogsRepository>()
                            .AddSingleton<IMeteredBillingApiService>(new MeteredBillingApiService(new MarketplaceMeteringClient(creds), config, new MeteringApiClientLogger()))
                            .AddSingleton<Executor, Executor>()
                            .BuildServiceProvider();

            services
                .GetService<Executor>()
                    .Execute();
            Console.WriteLine($"MeteredExecutor Webjob Ended at: {DateTime.Now}");

        }
    }
}