﻿namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    using System;
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
        /// The disposed.
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationLogRepository"/> class.
        /// </summary>
        /// <param name="context">The this.context.</param>
        public ApplicationLogRepository(SaasKitContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Adds the application logs.
        /// </summary>
        /// <param name="logDetail">The log detail.</param>
        public void AddLog(ApplicationLog logDetail)
        {
            this.context.ApplicationLog.Add(logDetail);
            this.context.SaveChanges();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.context.Dispose();
                }
            }

            this.disposed = true;
        }
    }
}