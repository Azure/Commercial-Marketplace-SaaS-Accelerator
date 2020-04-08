using System;
using System.Collections.Generic;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities
{
    public partial class SubscriptionParametersOutput
    {
        public int RowNumber { get; set; }
        public int Id { get; set; }
        public int PlanAttributeId { get; set; }
        public int OfferAttributeId { get; set; }
        public string DisplayName { get; set; }
        public string Type { get; set; }
        public string ValueType { get; set; }
        public int DisplaySequence { get; set; }
        public bool IsEnabled { get; set; }
        public string Value { get; set; }
        public Guid SubscriptionId { get; set; }
        public Guid OfferId { get; set; }
        public Guid PlanId { get; set; }
    }
}
