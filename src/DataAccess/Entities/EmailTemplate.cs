using System;

namespace Marketplace.SaaS.Accelerator.DataAccess.Entities;

public partial class EmailTemplate
{
    public int Id { get; set; }
    public string Status { get; set; }
    public string Description { get; set; }
    public DateTime? InsertDate { get; set; }
    public string TemplateBody { get; set; }
    public string Subject { get; set; }
    public string ToRecipients { get; set; }
    public string Cc { get; set; }
    public string Bcc { get; set; }
    public bool IsActive { get; set; }
}