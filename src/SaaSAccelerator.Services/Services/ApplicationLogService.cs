using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Marketplace.SaaSAccelerator.DataAccess.Contracts;
using Microsoft.Marketplace.SaaSAccelerator.DataAccess.Entities;

namespace Microsoft.Marketplace.SaaSAccelerator.Services.Services
{
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
                LogDetail = logMessage,
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
}
