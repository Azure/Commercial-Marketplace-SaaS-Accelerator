using System;
using System.Collections.Generic;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities
{
    public partial class SubscriptionLicenses
    {
        public int Id { get; set; }
        public string LicenseKey { get; set; }
        public bool? IsActive { get; set; }
        public int? SubscriptionId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }

        public virtual Subscriptions Subscription { get; set; }
    }
}
