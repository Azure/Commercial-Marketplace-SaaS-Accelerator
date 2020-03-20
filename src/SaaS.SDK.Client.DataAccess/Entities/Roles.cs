using System;
using System.Collections.Generic;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities
{
    public partial class Roles
    {
        public Roles()
        {
            KnownUsers = new HashSet<KnownUsers>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<KnownUsers> KnownUsers { get; set; }
    }
}
