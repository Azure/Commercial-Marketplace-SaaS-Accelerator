﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Marketplace.SaaS.SDK.CustomerProvisioning.Models
{
    public class SubscriptionParametersModel
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
        public bool IsRequired { get; set; }
        public string Value { get; set; }
        public Guid SubscriptionId { get; set; }
        public Guid OfferId { get; set; }
        public Guid PlanId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}