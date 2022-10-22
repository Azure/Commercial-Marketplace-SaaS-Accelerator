using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

/// <summary>
/// Gets or sets the Scheduler Frequency
/// </summary>
public partial class SchedulerFrequencyModel
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    public int Id { get; set; }
    /// <summary>
    /// Gets or sets the frequency.
    /// </summary>
    /// <value>
    /// The frequency.
    /// </value>
    public string Frequency { get; set; }

}
