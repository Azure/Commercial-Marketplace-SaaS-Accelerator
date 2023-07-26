using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.Services.Services;

/// <summary>
/// Application Log Service.
/// </summary>
public class ApplicationLogService
{
    /// <summary>
    /// The application log repository.
    /// </summary>
    private readonly IApplicationLogRepository applicationLogRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationLogService"/> class.
    /// </summary>
    /// <param name="applicationLogRepository">The application log repository.</param>
    public ApplicationLogService(IApplicationLogRepository applicationLogRepository)
    {
        this.applicationLogRepository = applicationLogRepository;
    }

    /// <summary>
    /// Adds the application log.
    /// </summary>
    /// <param name="logMessage">The log message.</param>
    public async Task AddApplicationLog(string logMessage)
    {
        ApplicationLog newLog = new ApplicationLog()
        {
            ActionTime = DateTime.Now,
            LogDetail = HttpUtility.HtmlEncode(logMessage),
        };

        await this.applicationLogRepository.AddLog(newLog);
    }

    /// <summary>
    /// Updates the application log.
    /// </summary>
    /// <param name="logMessage">The log message.</param>
    public IEnumerable<ApplicationLog> GetAllLogs()
    {
        return this.applicationLogRepository.GetLogs();
    }
}