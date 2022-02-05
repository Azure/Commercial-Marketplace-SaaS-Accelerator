using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Marketplace.Saas.Web.Controllers;
using Microsoft.Marketplace.SaaS.SDK.Services.Services;
using Microsoft.Marketplace.SaaS.SDK.Services.Utilities;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace SaaS.SDK.PublisherSolution.Controllers
{
    /// <summary>
    /// Offers Controller.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.Saas.Web.Controllers.BaseController" />
    [ServiceFilter(typeof(KnownUserAttribute))]
    public class ApplicationConfigController : BaseController
    {
        private readonly ILogger<ApplicationConfigController> logger;

        private ApplicationConfigService appConfigService;

        private readonly IApplicationConfigRepository appConfigRepository;

        public ApplicationConfigController(IApplicationConfigRepository applicationConfigRepository, ILogger<ApplicationConfigController> logger)
        {
            this.appConfigRepository = applicationConfigRepository;
            this.logger = logger;
            appConfigService = new ApplicationConfigService(this.appConfigRepository);
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>return All Application Config.</returns>
        public IActionResult Index()
        {
            this.logger.LogInformation("Application Config Controller / Index");
            try
            {
                IEnumerable<ApplicationConfiguration> getAllAppConfigData = new List<ApplicationConfiguration>();
                getAllAppConfigData = this.appConfigService.GetAllApplicationConfiguration();
                return this.View(getAllAppConfigData);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                return this.View("Error", ex);
            }
        }

        /// <summary>
        /// Get the apllication config item by Id.
        /// </summary>
        /// <param name="Id">The app config Id.</param>
        /// <returns>
        /// return an Application Config item.
        /// </returns>
        public IActionResult ApplicationConfigDetails(int Id)
        {
            this.logger.LogInformation("ApplicationConfig Controller / AppConfigDetails:  Id {0}", Id);
            try
            {
                ApplicationConfiguration applicationConfiguration = new ApplicationConfiguration();
                applicationConfiguration = this.appConfigService.GetById(Id);
                return this.PartialView(applicationConfiguration);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                return this.View("Error", ex);
            }
        }

        /// <summary>
        /// Saves the app config item changes.
        /// </summary>
        /// <param name="appConfig">The app config item.</param>
        /// <returns>
        /// return the changed app config item.
        /// </returns>
        [HttpPost]
        public IActionResult ApplicationConfigDetails(ApplicationConfiguration appConfig)
        {
            this.logger.LogInformation("ApplicationConfig Controller / ApplicationConfigDetails:  AppConfig {0}", JsonSerializer.Serialize(appConfig));
            try
            {
                if (appConfig != null)
                {
                    this.appConfigService.SaveAppConfig(appConfig);
                }

                this.ModelState.Clear();
                return this.RedirectToAction(nameof(this.ApplicationConfigDetails), new { Id = appConfig.Id });
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                return this.PartialView("Error", ex);
            }
        }
    }
}
