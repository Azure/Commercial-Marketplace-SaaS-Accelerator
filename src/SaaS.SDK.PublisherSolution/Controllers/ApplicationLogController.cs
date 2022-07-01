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
                getAllAppLogData = this.appLogService.GetAllLogs().ToList();
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
