using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

public partial class SchedulerFrequency
{
    public SchedulerFrequency()
    {
        MeteredPlanSchedulerManagements = new HashSet<MeteredPlanSchedulerManagement>();
    }
    public int Id { get; set; }
    public string Frequency { get; set; }
    public virtual ICollection<MeteredPlanSchedulerManagement> MeteredPlanSchedulerManagements { get; set; }
}
