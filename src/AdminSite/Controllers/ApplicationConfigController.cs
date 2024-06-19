using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.Services.Helpers;
using Marketplace.SaaS.Accelerator.Services.Services;
using Marketplace.SaaS.Accelerator.Services.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.SaaS.Accelerator.AdminSite.Controllers;

/// <summary>
/// ApplicationConfig Controller.
/// </summary>
/// <seealso cref="BaseController" />
[ServiceFilter(typeof(KnownUserAttribute))]
[ServiceFilter(typeof(RequestLoggerActionFilter))]
public class ApplicationConfigController : BaseController
{
    private readonly SaaSClientLogger<ApplicationConfigController> logger;
    private readonly ApplicationConfigService appConfigService;

    /// <summary>
    /// Move to a new controller?
    /// </summary>
    private readonly IEmailTemplateRepository emailTemplateRepository;

    public ApplicationConfigController(
            IApplicationConfigRepository applicationConfigRepository,
            IEmailTemplateRepository emailTemplateRepository,
            IAppVersionService appVersionService,
            SaaSClientLogger<ApplicationConfigController> logger) : base(applicationConfigRepository, appVersionService)
    {
        this.appConfigService = new ApplicationConfigService(applicationConfigRepository);
        this.emailTemplateRepository = emailTemplateRepository;
        this.logger = logger;
    }

    /// <summary>
    /// Main action for Application Config Page
    /// </summary>
    /// <returns>return All Application Config.</returns>
    [ServiceFilter(typeof(ExceptionHandlerAttribute))]
    public IActionResult Index()
    {
        var getAllAppConfigData = this.appConfigService.GetAllApplicationConfiguration();
        return this.View(getAllAppConfigData);
    }

    /// <summary>
    /// Indexes an EmailTemplate instance
    /// </summary>
    /// <returns>return a list of all EmailTemplates.</returns>
    [ServiceFilter(typeof(ExceptionHandlerAttribute))]
    public IActionResult EmailTemplates()
    {
        var getEmailTemplateData = this.emailTemplateRepository.GetAll();
        return this.View(getEmailTemplateData);
    }



    /// <summary>
    /// Get the fields of an EmailTemplate item by status
    /// </summary>
    /// <param name=status">The status that corresponds to the EmailTemplate.</param>
    /// <returns>
    /// return an EmailTemplate
    /// </returns>
    public IActionResult EmailTemplateDetails(string status)
    {
        var emailTemplate = this.emailTemplateRepository.GetTemplateForStatus(status);
        return this.PartialView(emailTemplate);
    }

    /// <summary>
    /// Saves changes to EmailTemplate
    /// </summary>
    /// <param name="emailTemplate">The modified EmailTemplate.</param>
    /// <returns>
    /// return the modified EmailTemplate.
    /// </returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EmailTemplateDetails(EmailTemplate emailTemplate)
    {
        this.emailTemplateRepository.SaveEmailTemplateByStatus(emailTemplate);
        this.ModelState.Clear();
        return new OkResult();
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
        var applicationConfiguration = this.appConfigService.GetById(Id);
        return this.PartialView(applicationConfiguration);
    }


    /// <summary>
    /// Saves the app config item changes.
    /// </summary>
    /// <param name="appConfig">The app config item.</param>
    /// <returns>
    /// return the changed app config item.
    /// </returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ApplicationConfigDetails(ApplicationConfiguration appConfig)
    {
        appConfig.Value = appConfig.Value ?? string.Empty;
        
        //check with the config is webhnotifcation url then validate if its proper url
        if (appConfig.Name == StringLiteralConstants.WebNotificationUrl && !UrlValidator.IsValidUrlHttps(appConfig.Value))
        {
            return this.BadRequest("Invalid URL, only https and port 443 are allowed.");
        }

        this.appConfigService.SaveAppConfig(appConfig);

        this.ModelState.Clear();
        return new OkResult();
    }


    /// <summary>
    /// Upload file(s) to database
    /// </summary>
    /// <param name="files">The files</param>
    /// <returns>RedirectToAction.</returns>
    [HttpPost("FileUpload")]
    [ServiceFilter(typeof(ExceptionHandlerAttribute))]
    [ValidateAntiForgeryToken]
    public IActionResult PostUpload(List<IFormFile> files)
    {
        if (!(files?.Any() == true))
        {
            TempData["Upload"] = "No files to upload";
            return RedirectToAction("Index");
        }
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

        return RedirectToAction("Index");
    }



}