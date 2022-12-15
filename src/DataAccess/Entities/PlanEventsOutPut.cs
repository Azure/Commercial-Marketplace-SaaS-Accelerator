using System;

namespace Marketplace.SaaS.Accelerator.DataAccess.Entities;

public partial class PlanEventsOutPut
{
    public int RowNumber { get; set; }
    public int Id { get; set; }
    public Guid PlanId { get; set; }
    public bool Isactive { get; set; }
    public string SuccessStateEmails { get; set; }
    public string FailureStateEmails { get; set; }
    public int EventId { get; set; }
    public string EventsName { get; set; }
    public bool? CopyToCustomer { get; set; }
}