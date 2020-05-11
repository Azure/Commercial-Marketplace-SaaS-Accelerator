namespace SaaS.SDK.Provisioning.Webjob
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Marketplace.SaaS.SDK.Services.Contracts;
    using Microsoft.Marketplace.SaaS.SDK.Services.Helpers;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using Microsoft.Marketplace.SaaS.SDK.Services.Services;
    using Microsoft.Marketplace.SaaS.SDK.Services.Utilities;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Services;
    using Microsoft.Marketplace.SaasKit.Configurations;
    using Microsoft.Marketplace.SaasKit.Contracts;
    using Microsoft.Marketplace.SaasKit.Services;

    /// <summary>
    /// Entry point Program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        private static IConfiguration Configuration { get; set; }

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task Main()
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
                .AddEnvironmentVariables() // this doesn't do anything useful notice in setting some env variables explicitly.
                .Build();  // build it so you can use those config variables down below.

            Environment.SetEnvironmentVariable("MyConnVariable", Configuration["MyConnVariable"]);

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
                TenantId = Configuration["SaaSApiConfiguration:TenantId"],
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
                KeyVaultUrl = Configuration["KeyVaultConfig:KeyVaultUrl"],
            };
            var azureBlobConfig = new AzureBlobConfig()
            {
                BlobContainer = Configuration["AzureBlobConfig:BlobContainer"],
                BlobConnectionString = Configuration["AzureBlobConfig:BlobConnectionString"],
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
