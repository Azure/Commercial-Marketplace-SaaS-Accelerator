namespace Microsoft.Marketplace.Saas.Web
{
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
    using Microsoft.IdentityModel.Protocols.OpenIdConnect;
    using Microsoft.Marketplace.Saas.Web.Utlities;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Services;
    using Microsoft.Marketplace.SaasKit.Configurations;
    using Microsoft.Marketplace.SaasKit.Contracts;
    using Microsoft.Marketplace.SaasKit.Services;
    using Microsoft.Marketplace.SaaS.SDK.PublisherSolution.Utilities;

    /// <summary>
    /// Startup
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
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
                FulFillmentAPIBaseURL = this.Configuration["SaaSApiConfiguration:FulFillmentAPIBaseURL"],
                FulFillmentAPIVersion = this.Configuration["SaaSApiConfiguration:FulFillmentAPIVersion"],
                GrantType = this.Configuration["SaaSApiConfiguration:GrantType"],
                Resource = this.Configuration["SaaSApiConfiguration:Resource"],
                SaaSAppUrl = this.Configuration["SaaSApiConfiguration:SaaSAppUrl"],
                SignedOutRedirectUri = this.Configuration["SaaSApiConfiguration:SignedOutRedirectUri"],
                TenantId = this.Configuration["SaaSApiConfiguration:TenantId"]
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = OpenIdConnectDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            ///OPEN ID Authentication
   .AddOpenIdConnect(options =>
   {
       options.Authority = $"{config.AdAuthenticationEndPoint}/common";
       options.ClientId = config.ClientId;
       options.ResponseType = OpenIdConnectResponseType.IdToken;
       options.CallbackPath = "/Home/Index";
       options.SignedOutRedirectUri = config.SignedOutRedirectUri;
       options.TokenValidationParameters.NameClaimType = "name";
       options.TokenValidationParameters.ValidateIssuer = false;
   })
   .AddCookie();

            services.AddSingleton<IFulfillmentApiClient>(new FulfillmentApiClient(config, new FulfillmentApiClientLogger()));
            services.AddSingleton<IMeteredBillingApiClient>(new MeteredBillingApiClient(config, new MeteringApiClientLogger()));
            services.AddSingleton<SaaSApiClientConfiguration>(config);

            services.AddDbContext<SaasKitContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            InitializeRepositoryServices(services);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddMvc(option => option.EnableEndpointRouting = false);

            // Add our Config object so it can be injected

            services.AddControllersWithViews();
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
            services.AddScoped<ISubscriptionLicensesRepository, SubscriptionLicensesRepository>();
            services.AddScoped<IKnownUsersRepository, KnownUsersRepository>();
            services.AddScoped<IOffersRepository, OffersRepository>();
            services.AddScoped<IArmTemplateRepository, ArmTemplateRepository>();
            services.AddScoped<IValueTypesRepository, ValueTypesRepository>();
            services.AddScoped<IOfferAttributesRepository, OfferAttributesRepository>();
            services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
            services.AddScoped<IPlanEventsMappingRepository, PlanEventsMappingRepository>();
            services.AddScoped<IEventsRepository, EventsRepository>();
            services.AddScoped<KnownUser>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// <summary>
        /// Configures the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The env.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
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
    }
}
