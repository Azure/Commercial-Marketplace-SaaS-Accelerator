namespace Marketplace.SaaS.Accelerator.DataAccess.Entities;

public partial class KnownUsers
{
    public int Id { get; set; }
    public string UserEmail { get; set; }
    public int RoleId { get; set; }

    public virtual Roles Role { get; set; }
}