using System;
using Azure.Identity;
using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Services;
using Marketplace.SaaS.Accelerator.Services.Configurations;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.Services;
using Marketplace.SaaS.Accelerator.Services.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Marketplace.Metering;

namespace Marketplace.SaaS.Accelerator.MeteredTriggerJob;

class Program
{
    /// <summary>
    /// Entery point to the scheduler engine
    /// </summary>
    /// <param name="args"></param>
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
            TenantId = configuration["SaaSApiConfiguration:TenantId"]
        };

        var creds = new ClientSecretCredential(config.TenantId.ToString(), config.ClientId.ToString(), config.ClientSecret);

        var services = new ServiceCollection()
            .AddDbContext<SaasKitContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient)
            .AddScoped<ISchedulerFrequencyRepository, SchedulerFrequencyRepository>()
            .AddScoped<IMeteredPlanSchedulerManagementRepository, MeteredPlanSchedulerManagementRepository>()
            .AddScoped<ISchedulerManagerViewRepository, SchedulerManagerViewRepository>()
            .AddScoped<ISubscriptionUsageLogsRepository, SubscriptionUsageLogsRepository>()
            .AddScoped<IApplicationLogRepository, ApplicationLogRepository>()
            .AddScoped<IEmailService, SMTPEmailService>()
            .AddScoped<IEmailTemplateRepository, EmailTemplateRepository>()
            .AddScoped<IApplicationConfigRepository, ApplicationConfigRepository>()
            .AddSingleton<IMeteredBillingApiService>(new MeteredBillingApiService(new MarketplaceMeteringClient(creds), config, new SaaSClientLogger<MeteredBillingApiService>()))
            .AddSingleton<Executor, Executor>()
            .BuildServiceProvider();

        services
            .GetService<Executor>()
            .Execute();
        Console.WriteLine($"MeteredExecutor Webjob Ended at: {DateTime.Now}");

    }
}