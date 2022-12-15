using System;

namespace Marketplace.SaaS.Accelerator.DataAccess.Entities;

public partial class DatabaseVersionHistory
{
    public int Id { get; set; }
    public decimal VersionNumber { get; set; }
    public string ChangeLog { get; set; }
    public DateTime CreateDate { get; set; }
    public string CreateBy { get; set; }
}