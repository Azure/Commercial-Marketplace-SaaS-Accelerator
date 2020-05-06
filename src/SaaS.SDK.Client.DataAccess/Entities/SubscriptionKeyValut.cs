using System;
using System.Collections.Generic;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities
{
    public partial class SubscriptionKeyVault
    {
        public int Id { get; set; }
        public Guid? SubscriptionId { get; set; }
        public string SecureId { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? UserId { get; set; }
    }
}
