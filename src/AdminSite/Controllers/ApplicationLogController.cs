using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.Services.Services;
using Marketplace.SaaS.Accelerator.Services.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Marketplace.SaaS.Accelerator.AdminSite.Controllers;

[ServiceFilter(typeof(KnownUserAttribute))]
public class ApplicationLogController : BaseController
{
    private readonly SaaSClientLogger<ApplicationLogController> logger;

    private ApplicationLogService appLogService;

    private readonly IApplicationLogRepository appLogRepository;

    public ApplicationLogController(IApplicationLogRepository applicationLogRepository, SaaSClientLogger<ApplicationLogController> logger)
    {
        this.appLogRepository = applicationLogRepository;
        this.logger = new SaaSClientLogger<ApplicationLogController>();
        appLogService = new ApplicationLogService(this.appLogRepository);
    }
    public IActionResult Index()
    {
        logger.ProcessInformation("Application Log Controller / Index");
        try
        {
            IEnumerable<ApplicationLog> getAllAppLogData = new List<ApplicationLog>();
            getAllAppLogData = this.appLogService.GetAllLogs().OrderByDescending(d => d.ActionTime).ToList();
            return this.View(getAllAppLogData);
        }
        catch (Exception ex)
        {
            this.logger.LogError($"Message:{ex.Message} :: {ex.InnerException}");
            return this.View("Error", ex);
        }
    }

    private void ProcessInformation(string message)
    {
        string log = HttpUtility.HtmlEncode(message);
        this.logger.Info(log);
    }
}