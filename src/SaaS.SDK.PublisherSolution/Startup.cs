// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.
namespace Microsoft.Marketplace.Saas.Web
{
    using global::Azure.Identity;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authentication.OpenIdConnect;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Protocols.OpenIdConnect;
    using Microsoft.Marketplace.Metering;
    using Microsoft.Marketplace.SaaS;
    using Microsoft.Marketplace.SaaS.SDK.Services.Configurations;
    using Microsoft.Marketplace.SaaS.SDK.Services.Contracts;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using Microsoft.Marketplace.SaaS.SDK.Services.Services;
    using Microsoft.Marketplace.SaaS.SDK.Services.Utilities;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Services;
    /// <summary>
    /// Startup.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddConsole();
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            var config = new SaaSApiClientConfiguration()
            {
                AdAuthenticationEndPoint = this.Configuration["SaaSApiConfiguration:AdAuthenticationEndPoint"],
                ClientId = this.Configuration["SaaSApiConfiguration:ClientId"],
                ClientSecret = this.Configuration["SaaSApiConfiguration:ClientSecret"],
                FulFillmentAPIBaseURL = this.Configuration["SaaSApiConfiguration:FulFillmentAPIBaseURL"],
                FulFillmentAPIVersion = this.Configuration["SaaSApiConfiguration:FulFillmentAPIVersion"],
                GrantType = this.Configuration["SaaSApiConfiguration:GrantType"],
                Resource = this.Configuration["SaaSApiConfiguration:Resource"],
                SaaSAppUrl = this.Configuration["SaaSApiConfiguration:SaaSAppUrl"],
                SignedOutRedirectUri = this.Configuration["SaaSApiConfiguration:SignedOutRedirectUri"],
                TenantId = this.Configuration["SaaSApiConfiguration:TenantId"],
            };
            var knownUsers = new KnownUsersModel()
            {
                KnownUsers = this.Configuration["KnownUsers"],

            };
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = OpenIdConnectDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })

   .AddOpenIdConnect(options =>
   {
       options.Authority = $"{config.AdAuthenticationEndPoint}/common";
       options.ClientId = config.MTClientId;
       options.ResponseType = OpenIdConnectResponseType.IdToken;
       options.CallbackPath = "/Home/Index";
       options.SignedOutRedirectUri = config.SignedOutRedirectUri;
       options.TokenValidationParameters.NameClaimType = "name";
       options.TokenValidationParameters.ValidateIssuer = false;
   })
   .AddCookie();

            var creds = new ClientSecretCredential(config.TenantId.ToString(), config.ClientId.ToString(), config.ClientSecret);
            services.AddSingleton<IFulfillmentApiService>(new FulfillmentApiService(new MarketplaceSaaSClient(creds), config, new FulfillmentApiClientLogger()));
            services.AddSingleton<IMeteredBillingApiService>( new MeteredBillingApiService(new MarketplaceMeteringClient(creds), config, new MeteringApiClientLogger()));
            services.AddSingleton<SaaSApiClientConfiguration>(config);
            services.AddSingleton<KnownUsersModel>(knownUsers);
            services.AddDbContext<SaasKitContext>(options =>
               options.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection")));

            InitializeRepositoryServices(services);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddMvc(option => option.EnableEndpointRouting = false);
            services.AddControllersWithViews();
        }

        /// <summary>
        /// Configures the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The env.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        /// <summary>
        /// Initializes the repository services.
        /// </summary>
        /// <param name="services">The services.</param>
        private static void InitializeRepositoryServices(IServiceCollection services)
        {
            services.AddScoped<ISubscriptionsRepository, SubscriptionsRepository>();
            services.AddScoped<IPlansRepository, PlansRepository>();
            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<ISubscriptionLogRepository, SubscriptionLogRepository>();
            services.AddScoped<IApplicationConfigRepository, ApplicationConfigRepository>();
            services.AddScoped<IApplicationLogRepository, ApplicationLogRepository>();
            services.AddScoped<ISubscriptionUsageLogsRepository, SubscriptionUsageLogsRepository>();
            services.AddScoped<IMeteredDimensionsRepository, MeteredDimensionsRepository>();
            services.AddScoped<IKnownUsersRepository, KnownUsersRepository>();
            services.AddScoped<IOffersRepository, OffersRepository>();
            services.AddScoped<IValueTypesRepository, ValueTypesRepository>();
            services.AddScoped<IOfferAttributesRepository, OfferAttributesRepository>();
            services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
            services.AddScoped<IPlanEventsMappingRepository, PlanEventsMappingRepository>();
            services.AddScoped<IEventsRepository, EventsRepository>();
            services.AddScoped<KnownUserAttribute>();
            services.AddScoped<IEmailService, SMTPEmailService>();
        }
    }
}
