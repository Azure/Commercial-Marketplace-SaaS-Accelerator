using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    /// <summary>
    ///   Scheduler Frequency Repository.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.ISchedulerFrequencyRepository" />
    public class SchedulerFrequencyRepository : ISchedulerFrequencyRepository
    {
        /// <summary>
        /// The context.
        /// </summary>
        private readonly SaasKitContext context;

        /// <summary>
        /// The disposed.
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulerFrequencyRepository"/> class.
        /// </summary>
        /// <param name="context">The this.context.</param>
        public SchedulerFrequencyRepository(SaasKitContext context)
        {
            this.context = context;
        }
        /// <summary>
        /// Get Frequency by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
          public SchedulerFrequency GetById(int id)
        {
            return this.context.SchedulerFrequency.Where(s => s.Id == id).FirstOrDefault();
        }
        /// <summary>
        /// Get All records
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SchedulerFrequency> GetAll()
        {
            return this.context.SchedulerFrequency;
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

