using System.Collections.Generic;
using System.Threading.Tasks;
using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Services;

/// <summary>
/// Repository to access application logs.
/// </summary>
/// <seealso cref="IApplicationLogRepository" />
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

    /// <summary>
    /// Updates the application logs.
    /// </summary>
    /// <param name="logDetail">The log detail.</param>
    public Task<int> UpdateLog(ApplicationLog logDetail)
    {
        this.context.ApplicationLog.Update(logDetail);
        return this.context.SaveChangesAsync();
    }

    /// <summary>
    /// Gets a list of application logs.
    /// </summary>
    public IEnumerable<ApplicationLog> GetLogs()
    {
        return this.context.ApplicationLog;
    }

}