using System;
using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Contracts;

/// <summary>
/// Scheduler Manager View for Metered Plan.
/// </summary>
/// <seealso cref="System.IDisposable" />
public interface ISchedulerManagerViewRepository : IDisposable
{
    /// <summary>
    /// Get all scheduler Manager data
    /// </summary>
    /// <returns></returns>
    IEnumerable<SchedulerManagerView> GetAll();
    /// <summary>
    /// Get  scheduler Manager data by Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    SchedulerManagerView GetById(int id);
}