using System.Collections.Generic;

namespace Marketplace.SaaS.Accelerator.DataAccess.Entities;

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