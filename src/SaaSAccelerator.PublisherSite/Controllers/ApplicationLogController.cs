using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Marketplace.SaaSAccelerator.DataAccess.Contracts;
using Microsoft.Marketplace.SaaSAccelerator.DataAccess.Entities;
using Microsoft.Marketplace.SaaSAccelerator.Services.Services;

namespace Microsoft.Marketplace.SaaSAccelerator.PublisherSite.Controllers
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
