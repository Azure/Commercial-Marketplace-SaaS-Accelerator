namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;

using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

/// <summary>
/// Repository to access application log entries.
/// </summary>
public interface IApplicationLogRepository
{
    /// <summary>
    /// Adds the log.
    /// </summary>
    /// <param name="logDetail">The log detail.</param>
    Task<int> AddLog(ApplicationLog logDetail);

    /// <summary>
    /// Updates the log.
    /// </summary>
    /// <param name="logDetail">The log detail.</param>
    Task<int> UpdateLog(ApplicationLog logDetail);

    /// <summary>
    /// Retrieve the logs
    /// </summary>
     IEnumerable<ApplicationLog> GetLogs();

}
