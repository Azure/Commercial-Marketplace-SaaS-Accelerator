using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using System;

namespace  Microsoft.Marketplace.SaaS.SDK.Library.Services
{
    public class ApplicationLogService
    {
        /// <summary>
        /// The application log repository
        /// </summary>
        private readonly IApplicationLogRepository ApplicationLogRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationLogService"/> class.
        /// </summary>
        /// <param name="applicationLogRepository">The application log repository.</param>
        public ApplicationLogService(IApplicationLogRepository applicationLogRepository)
        {
            ApplicationLogRepository = applicationLogRepository;
        }

        /// <summary>
        /// Adds the application log.
        /// </summary>
        /// <param name="logMessage">The log message.</param>
        public void AddApplicationLog(string logMessage)
        {
            ApplicationLog newLog = new ApplicationLog()
            {
                ActionTime = DateTime.Now,
                LogDetail = logMessage
            };

            ApplicationLogRepository.AddApplicationLogs(newLog);
        }
    }
}
