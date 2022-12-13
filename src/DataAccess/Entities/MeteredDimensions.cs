using System;
using System.Collections.Generic;

namespace Marketplace.SaaS.Accelerator.DataAccess.Entities;

public partial class MeteredDimensions
{
    public MeteredDimensions()
    {
        MeteredPlanSchedulerManagements = new HashSet<MeteredPlanSchedulerManagement>();
    }
    public int Id { get; set; }
    public string Dimension { get; set; }
    public int? PlanId { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string Description { get; set; }

    public virtual Plans Plan { get; set; }
    public virtual ICollection<MeteredPlanSchedulerManagement> MeteredPlanSchedulerManagements { get; set; }
}