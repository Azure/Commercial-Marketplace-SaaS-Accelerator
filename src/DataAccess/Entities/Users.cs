using System;
using System.Collections.Generic;

namespace Marketplace.SaaS.Accelerator.DataAccess.Entities;

public partial class Users
{
    public Users()
    {
        Subscriptions = new HashSet<Subscriptions>();
    }

    public int UserId { get; set; }
    public string EmailAddress { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string FullName { get; set; }

    public virtual ICollection<Subscriptions> Subscriptions { get; set; }
}