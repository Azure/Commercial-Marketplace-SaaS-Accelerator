using System;
using System.Collections.Generic;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities
{
    public partial class SubscriptionAttributeValues
    {
        public int Id { get; set; }
        public int? PlanAttributeId { get; set; }
        public string Value { get; set; }
        public Guid? SubscriptionId { get; set; }
        public int? PlanId { get; set; }
        public int? OfferId { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? UserId { get; set; }
    }
}
