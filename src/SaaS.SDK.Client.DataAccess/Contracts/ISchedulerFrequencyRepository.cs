namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

    /// <summary>
    /// Repository to access the possible data for Scheduler Frequency.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface ISchedulerFrequencyRepository : IDisposable
    {
        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>List of all Scheduler Frequency supported by the application.</returns>
        IEnumerable<SchedulerFrequency> GetAll();

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns> ValueTypes.</returns>
        SchedulerFrequency GetById(int id);

        
    }
}
