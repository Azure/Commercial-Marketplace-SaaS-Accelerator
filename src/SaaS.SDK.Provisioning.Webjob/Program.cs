using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Microsoft.Marketplace.SaasKit.Configurations;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Contracts;
using Microsoft.Marketplace.SaasKit.Services;
using Microsoft.Marketplace.SaaS.SDK.Services.Utilities;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Marketplace.SaaS.SDK.Services.Models;
using Microsoft.Marketplace.SaaS.SDK.Services.Contracts;
using Microsoft.Marketplace.SaaS.SDK.Services.Helpers;
using Microsoft.Marketplace.SaaS.SDK.Services.Services;

namespace SaaS.SDK.Provisioning.Webjob
{
    class Program
    {
        private static IConfiguration Configuration { get; set; }

        static async Task Main()
        {
            var builder = new HostBuilder();

            builder.ConfigureLogging((context, b) =>
            {
                b.AddConsole();
            });

            builder.ConfigureWebJobs(b =>
            {
                b.AddAzureStorageCoreServices();
                b.AddAzureStorage();
            });

            // Configure Services
            builder.ConfigureServices((context, s) =>
            {
                ConfigureServices(s);
                s.BuildServiceProvider();
            });

            var host = builder.Build();
            using (host)
            {
                await host.RunAsync();
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddConsole();
            });

            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables() //this doesnt do anything useful notice im setting some env variables explicitly. 
                .Build();  //build it so you can use those config variables down below.

            Environment.SetEnvironmentVariable("MyConnVariable", Configuration["MyConnVariable"]);

            #region RegisterServiceProviders


            var config = new SaaSApiClientConfiguration()
            {
                AdAuthenticationEndPoint = Configuration["SaaSApiConfiguration:AdAuthenticationEndPoint"],
                ClientId = Configuration["SaaSApiConfiguration:ClientId"],
                ClientSecret = Configuration["SaaSApiConfiguration:ClientSecret"],
                FulFillmentAPIBaseURL = Configuration["SaaSApiConfiguration:FulFillmentAPIBaseURL"],
                FulFillmentAPIVersion = Configuration["SaaSApiConfiguration:FulFillmentAPIVersion"],
                GrantType = Configuration["SaaSApiConfiguration:GrantType"],
                Resource = Configuration["SaaSApiConfiguration:Resource"],
                SaaSAppUrl = Configuration["SaaSApiConfiguration:SaaSAppUrl"],
                SignedOutRedirectUri = Configuration["SaaSApiConfiguration:SignedOutRedirectUri"],
                TenantId = Configuration["SaaSApiConfiguration:TenantId"]
            };

            var cloudConfig = new CloudStorageConfigs
            {
                AzureWebJobsStorage = Configuration["AzureWebJobsStorage"],
            };
            var keyVaultConfig = new KeyVaultConfig()
            {
                ClientID = Configuration["KeyVaultConfig:ClientID"],
                ClientSecret = Configuration["KeyVaultConfig:ClientSecret"],
                TenantID = Configuration["KeyVaultConfig:TenantID"],
                KeyVaultUrl = Configuration["KeyVaultConfig:KeyVaultUrl"]
            };
            var azureBlobConfig = new AzureBlobConfig()
            {
                BlobContainer = Configuration["AzureBlobConfig:BlobContainer"],
                BlobConnectionString = Configuration["AzureBlobConfig:BlobConnectionString"]

            };

            services.AddSingleton<IFulfillmentApiClient>(new FulfillmentApiClient(config, new FulfillmentApiClientLogger()));
            services.AddSingleton<SaaSApiClientConfiguration>(config);

            services.AddSingleton<IVaultService>(new AzureKeyVaultClient(keyVaultConfig, loggerFactory.CreateLogger<AzureKeyVaultClient>()));
            services.AddSingleton<IARMTemplateStorageService>(new AzureBlobStorageService(azureBlobConfig));
            services.AddSingleton<KeyVaultConfig>(keyVaultConfig);
            services.AddSingleton<AzureBlobConfig>(azureBlobConfig);

            services.AddDbContext<SaasKitContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            InitializeRepositoryServices(services);


            #endregion

        }

        private static void InitializeRepositoryServices(IServiceCollection services)
        {
            services.AddScoped<ISubscriptionsRepository, SubscriptionsRepository>();
            services.AddScoped<IPlansRepository, PlansRepository>();
            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<ISubscriptionLogRepository, SubscriptionLogRepository>();
            services.AddScoped<IApplicationLogRepository, ApplicationLogRepository>();

            services.AddScoped<ISubscriptionLicensesRepository, SubscriptionLicensesRepository>();
            services.AddScoped<IApplicationConfigRepository, ApplicationConfigRepository>();
            services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
            services.AddScoped<IOffersRepository, OffersRepository>();
            services.AddScoped<IOfferAttributesRepository, OfferAttributesRepository>();
            services.AddScoped<IPlanEventsMappingRepository, PlanEventsMappingRepository>();
            services.AddScoped<IEventsRepository, EventsRepository>();
            services.AddScoped<ISubscriptionTemplateParametersRepository, SubscriptionTemplateParametersRepository>();
            services.AddScoped<IArmTemplateRepository, ArmTemplateRepository>();
            services.AddScoped<IEmailService, SMTPEmailService>();
            services.AddScoped<EmailHelper, EmailHelper>();
        }
    }
}
