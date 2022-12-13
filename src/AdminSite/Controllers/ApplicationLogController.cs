using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Marketplace.Saas.Web.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.Services.Services;

namespace SaaS.SDK.PublisherSolution.Controllers
{
    public class ApplicationLogController : BaseController
    {
        private readonly ILogger<ApplicationLogController> logger;

        private ApplicationLogService appLogService;

        private readonly IApplicationLogRepository appLogRepository;

        public ApplicationLogController(IApplicationLogRepository applicationLogRepository, ILogger<ApplicationLogController> logger)
        {
            this.appLogRepository = applicationLogRepository;
            this.logger = logger;
            appLogService = new ApplicationLogService(this.appLogRepository);
        }
        public IActionResult Index()
        {
            this.logger.LogInformation("Application Log Controller / Index");
            try
            {
                IEnumerable<ApplicationLog> getAllAppLogData = new List<ApplicationLog>();
                getAllAppLogData = this.appLogService.GetAllLogs().OrderByDescending(d => d.ActionTime).ToList();
                return this.View(getAllAppLogData);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                return this.View("Error", ex);
            }
        }
    }
}
