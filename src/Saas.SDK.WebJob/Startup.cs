using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Saas.SDK.WebJob
{
    public class Startup
    {
        IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddDbContext<SaasKitContext>(options =>
              options.UseSqlServer("Data source=INFIHYD-WS002\\MSSQLSERVER17;initial catalog=AMP-SaasKit;user id=sa;password=Sa1"));

            //Configuration.GetConnectionString("DefaultConnection"))

            //services.AddSingleton<IConfigurationRoot>(Configuration);
            //services.AddSingleton<IMyService, MyService>();
        }
    }
}
