using System;
using System.Collections.Generic;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities
{
    public partial class Subscriptions
    {
        public Subscriptions()
        {
            MeteredAuditLogs = new HashSet<MeteredAuditLogs>();
            SubscriptionAuditLogs = new HashSet<SubscriptionAuditLogs>();
            SubscriptionLicenses = new HashSet<SubscriptionLicenses>();
        }

        public int Id { get; set; }
        public Guid AmpsubscriptionId { get; set; }
        public string SubscriptionStatus { get; set; }
        public string AmpplanId { get; set; }
        public bool? IsActive { get; set; }
        public int? CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public int? UserId { get; set; }
        public string Name { get; set; }
        public int Ampquantity { get; set; }
        public string PurchaserEmail { get; set; }
        public Guid? PurchaserTenantId { get; set; }

        public virtual Users User { get; set; }
        public virtual ICollection<MeteredAuditLogs> MeteredAuditLogs { get; set; }
        public virtual ICollection<SubscriptionAuditLogs> SubscriptionAuditLogs { get; set; }
        public virtual ICollection<SubscriptionLicenses> SubscriptionLicenses { get; set; }
    }
}
