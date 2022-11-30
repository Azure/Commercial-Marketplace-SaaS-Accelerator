using System;
using System.Collections.Generic;

namespace Marketplace.SaaS.Accelerator.DataAccess.Entities;

public partial class Plans
{
    public Plans()
    {
        MeteredDimensions = new HashSet<MeteredDimensions>();
        MeteredPlanSchedulerManagements = new HashSet<MeteredPlanSchedulerManagement>();
    }

    public int Id { get; set; }
    public string PlanId { get; set; }
    public string Description { get; set; }
    public string DisplayName { get; set; }
    public bool? IsmeteringSupported { get; set; }
    public bool? IsPerUser { get; set; }
    public Guid PlanGuid { get; set; }
    public Guid OfferId { get; set; }

    public virtual ICollection<MeteredDimensions> MeteredDimensions { get; set; }
    public virtual ICollection<MeteredPlanSchedulerManagement> MeteredPlanSchedulerManagements { get; set; }
}