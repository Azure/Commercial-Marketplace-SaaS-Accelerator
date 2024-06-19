// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using Azure.Identity;
using Marketplace.SaaS.Accelerator.CustomerSite.Controllers;
using Marketplace.SaaS.Accelerator.CustomerSite.WebHook;
using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Services;
using Marketplace.SaaS.Accelerator.Services.Configurations;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.Services;
using Marketplace.SaaS.Accelerator.Services.Utilities;
using Marketplace.SaaS.Accelerator.Services.WebHook;
using Microsoft.AspNetCore.Authentication;
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
using Microsoft.Marketplace.SaaS;
using System;

namespace Marketplace.SaaS.Accelerator.CustomerSite;

/// <summary>
/// Defines the <see cref="Startup" />.
/// </summary>
public class Startup
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Startup"/> class.
    /// </summary>
    /// <param name="configuration">The configuration<see cref="IConfiguration"/>.</param>
    public Startup(IConfiguration configuration)
    {
        this.Configuration = configuration;
    }

    /// <summary>
    /// Gets the Configuration.
    /// </summary>
    public IConfiguration Configuration { get; }

    /// <summary>
    /// The ConfigureServices.
    /// </summary>
    /// <param name="services">The services<see cref="IServiceCollection"/>.</param>
    public void ConfigureServices(IServiceCollection services)
    {
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
            MTClientId = this.Configuration["SaaSApiConfiguration:MTClientId"],
            FulFillmentAPIBaseURL = this.Configuration["SaaSApiConfiguration:FulFillmentAPIBaseURL"],
            FulFillmentAPIVersion = this.Configuration["SaaSApiConfiguration:FulFillmentAPIVersion"],
            GrantType = this.Configuration["SaaSApiConfiguration:GrantType"],
            Resource = this.Configuration["SaaSApiConfiguration:Resource"],
            SaaSAppUrl = this.Configuration["SaaSApiConfiguration:SaaSAppUrl"],
            SignedOutRedirectUri = this.Configuration["SaaSApiConfiguration:SignedOutRedirectUri"],
            TenantId = this.Configuration["SaaSApiConfiguration:TenantId"],
            Environment = this.Configuration["SaaSApiConfiguration:Environment"]
        };
        var creds = new ClientSecretCredential(config.TenantId.ToString(), config.ClientId.ToString(), config.ClientSecret);

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = OpenIdConnectDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.Cookie.MaxAge = options.ExpireTimeSpan;
                options.SlidingExpiration = true;
            })
            .AddOpenIdConnect(options =>
            {
                options.Authority = $"{config.AdAuthenticationEndPoint}/common/v2.0";
                options.ClientId = config.MTClientId;
                options.ResponseType = OpenIdConnectResponseType.IdToken;
                options.CallbackPath = "/Home/Index";
                options.SignedOutRedirectUri = config.SignedOutRedirectUri;
                options.TokenValidationParameters.NameClaimType = ClaimConstants.CLAIM_SHORT_NAME;
                options.TokenValidationParameters.ValidateIssuer = false;
            });
        services
            .AddTransient<IClaimsTransformation, CustomClaimsTransformation>()
            .AddScoped<ExceptionHandlerAttribute>()
            .AddScoped<RequestLoggerActionFilter>();

        if (!Uri.TryCreate(config.FulFillmentAPIBaseURL, UriKind.Absolute, out var fulfillmentBaseApi)) 
        {
            fulfillmentBaseApi = new Uri("https://marketplaceapi.microsoft.com/api");
        }

        services
            .AddSingleton<IFulfillmentApiService>(new FulfillmentApiService(new MarketplaceSaaSClient(fulfillmentBaseApi, creds), config, new FulfillmentApiClientLogger()))
            .AddSingleton<SaaSApiClientConfiguration>(config)
            .AddSingleton<ValidateJwtToken>();

        services
            .AddDbContext<SaasKitContext>(options => options.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection")));

        InitializeRepositoryServices(services);

        services.AddMvc(option => {
            option.EnableEndpointRouting = false;
            option.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
        });
    }

    /// <summary>
    /// The Configure.
    /// </summary>
    /// <param name="app">The app<see cref="IApplicationBuilder" />.</param>
    /// <param name="env">The env<see cref="IWebHostEnvironment" />.</param>
    /// <param name="loggerFactory">The loggerFactory<see cref="ILoggerFactory" />.</param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
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

    private static void InitializeRepositoryServices(IServiceCollection services)
    {
        services.AddScoped<ISubscriptionsRepository, SubscriptionsRepository>();
        services.AddScoped<IPlansRepository, PlansRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<ISubscriptionLogRepository, SubscriptionLogRepository>();
        services.AddScoped<IApplicationLogRepository, ApplicationLogRepository>();
        services.AddScoped<IWebhookProcessor, WebhookProcessor>();
        services.AddScoped<IWebhookHandler, WebHookHandler>();
        services.AddScoped<IApplicationConfigRepository, ApplicationConfigRepository>();
        services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
        services.AddScoped<IOffersRepository, OffersRepository>();
        services.AddScoped<IOfferAttributesRepository, OfferAttributesRepository>();
        services.AddScoped<IPlanEventsMappingRepository, PlanEventsMappingRepository>();
        services.AddScoped<IEventsRepository, EventsRepository>();
        services.AddScoped<IEmailService, SMTPEmailService>();
        services.AddScoped<SaaSClientLogger<HomeController>>();
        services.AddScoped<IWebNotificationService, WebNotificationService>();
    }
}