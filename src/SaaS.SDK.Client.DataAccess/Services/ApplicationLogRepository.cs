namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

    /// <summary>
    /// Repository to access application logs.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.IApplicationLogRepository" />
    public class ApplicationLogRepository : IApplicationLogRepository
    {
        /// <summary>
        /// The this.context.
        /// </summary>
        private readonly SaasKitContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationLogRepository"/> class.
        /// </summary>
        /// <param name="context">The this.context.</param>
        public ApplicationLogRepository(SaasKitContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Adds the application logs.
        /// </summary>
        /// <param name="logDetail">The log detail.</param>
        public Task<int> AddLog(ApplicationLog logDetail)
        {
            this.context.ApplicationLog.Add(logDetail);
            return this.context.SaveChangesAsync();
        }

    }
}