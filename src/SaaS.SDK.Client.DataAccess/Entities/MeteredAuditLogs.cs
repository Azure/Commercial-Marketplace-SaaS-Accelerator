using System;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities
{
    public partial class MeteredAuditLogs
    {
        public int Id { get; set; }
        public int? SubscriptionId { get; set; }
        public string RequestJson { get; set; }
        public string ResponseJson { get; set; }
        public string StatusCode { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? SubscriptionUsageDate { get; set; }

        public virtual Subscriptions Subscription { get; set; }
    }
}
