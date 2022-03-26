using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Marketplace.Saas.Web.Controllers;
using Microsoft.Marketplace.SaaS.SDK.Services.Services;
using Microsoft.Marketplace.SaaS.SDK.Services.Utilities;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        /// <summary>
        /// Upload file(s) to database
        /// </summary>
        /// <param name="files">The files</param>
        /// <returns>RedirectToAction.</returns>
        [HttpPost("FileUpload")]
        public IActionResult PostUpload(List<IFormFile> files)
        {
            this.logger.LogInformation("Application Config Controller / PostUpload ");
            try
            {
                if (files == null || files.Count == 0)
                {
                    TempData["Upload"] = "No files to upload";
                    return RedirectToAction("Index");
                }
                else
                {
                    if (files.Count > 2)
                    {
                        TempData["Upload"] = "No more than two files can be uploaded";
                        return RedirectToAction("Index");
                    }       
                    foreach (var file in files)
                    {
                        int maxLength = 1024 * 1024 * 5; //5 MB

                        if (file.Length > maxLength)
                        {
                            TempData["Upload"] = "File is too large, max size of file for upload is 5 MB";
                            return RedirectToAction("Index");
                        }
                        if (file.Length == 0)
                        {
                            TempData["Upload"] = "File is empty";
                            return RedirectToAction("Index");
                        }

                        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

                        if (fileExtension != ".png" && fileExtension != ".ico")
                        {
                            TempData["Upload"] = "Only .png or .ico files can be uploaded";
                            return RedirectToAction("Index");
                        }

                        var appConfigNames = this.appConfigService.GetAllApplicationConfiguration().Select(a => a.Name);
                       
                        if (!appConfigNames.Contains("LogoFile") || !appConfigNames.Contains("FaviconFile"))
                        {
                            TempData["Upload"] = "LogoFile or FaviconFile application config settings are missing in the database";
                            return RedirectToAction("Index");
                        }

                        if (this.appConfigService.UploadFileToDatabase(file, fileExtension) == false)
                        {
                            TempData["Upload"] = "File Upload failed!";
                            return RedirectToAction("Index");
                        }
                    }

                    if (files.Count == 1)
                    {
                        TempData["Upload"] = files.FirstOrDefault().FileName + "  uploaded successfully";
                    }
                    else
                    {
                        TempData["Upload"] = files[0].FileName + " and " + files[1].FileName + " uploaded successfully";
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                TempData["Upload"] = "File Upload failed!";
                return this.PartialView("Error", ex);
            }
        }

    }
}
