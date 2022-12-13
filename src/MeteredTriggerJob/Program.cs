using System;
using Azure.Identity;
using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Services;
using MeteredTriggerHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Marketplace.Metering;
using Microsoft.Marketplace.SaaS.SDK.Services.Configurations;
using Microsoft.Marketplace.SaaS.SDK.Services.Contracts;
using Microsoft.Marketplace.SaaS.SDK.Services.Services;
using Microsoft.Marketplace.SaaS.SDK.Services.Utilities;

namespace Marketplace.SaaS.Accelerator.MeteredTriggerJob
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
                            .AddScoped<IApplicationConfigRepository, ApplicationConfigRepository>()
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