namespace Microsoft.Marketplace.SaaS.SDK.Services.Services
{
    using System;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

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
        public void AddApplicationLog(string logMessage)
        {
            ApplicationLog newLog = new ApplicationLog()
            {
                ActionTime = DateTime.Now,
                LogDetail = logMessage,
            };

            this.applicationLogRepository.AddLog(newLog);
        }
    }
}
