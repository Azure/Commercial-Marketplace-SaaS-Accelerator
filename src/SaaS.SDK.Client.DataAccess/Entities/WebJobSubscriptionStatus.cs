using System;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities
{
    public partial class WebJobSubscriptionStatus
    {
        public int Id { get; set; }
        public Guid? SubscriptionId { get; set; }
        public string SubscriptionStatus { get; set; }
        public string Description { get; set; }
        public DateTime? InsertDate { get; set; }
    }
}
