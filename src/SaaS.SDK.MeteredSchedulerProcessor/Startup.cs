// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.
    using global::Azure.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Marketplace.Metering;
    using Microsoft.Marketplace.SaaS.SDK.Services.Configurations;
    using Microsoft.Marketplace.SaaS.SDK.Services.Contracts;
    using Microsoft.Marketplace.SaaS.SDK.Services.Services;
    using Microsoft.Marketplace.SaaS.SDK.Services.Utilities;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Services;
    using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using System.IO;
using System;

/// <summary>
/// Startup.
/// </summary>
[assembly: FunctionsStartup(typeof(SaaS.SDK.MeteredSchedulerProcessor.Startup))]
    namespace SaaS.SDK.MeteredSchedulerProcessor
    {
        public class Startup : FunctionsStartup
        {
            public override void Configure(IFunctionsHostBuilder builder)
            {
                ConfigureServices(builder.Services);
            }

        public void ConfigureServices(IServiceCollection services)
        {
            // Configurations
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"local.settings.json", optional: true, reloadOnChange: true)
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
            
            services.AddLogging();
            services.AddSingleton<SaaSApiClientConfiguration>(config);
            services.AddDbContext<SaasKitContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            services.AddSingleton<IMeteredBillingApiService>(new MeteredBillingApiService(new MarketplaceMeteringClient(creds), config, new MeteringApiClientLogger()));

            //Register DB Repositories
            services.AddScoped<ISchedulerFrequencyRepository, SchedulerFrequencyRepository>();
            services.AddScoped<IMeteredPlanSchedulerManagementRepository, MeteredPlanSchedulerManagementRepository>();
            services.AddScoped<ISchedulerManagerViewRepository, SchedulerManagerViewRepository>();
            services.AddScoped<ISubscriptionUsageLogsRepository, SubscriptionUsageLogsRepository>();

        }

    }
}
