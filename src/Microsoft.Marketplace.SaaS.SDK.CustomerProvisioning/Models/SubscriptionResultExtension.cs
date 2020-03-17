namespace Microsoft.Marketplace.SaasKit.Client.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Newtonsoft.Json;
    using Microsoft.Marketplace.SaasKit.Models;

    /// <summary>
    /// Subscription Response
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Models.SaaSApiResult" />
    public class SubscriptionResultExtension : SubscriptionResult
    {
        [JsonProperty("saasSubscriptionStatus")]
        [DisplayName("Subscription Status")]
        public new SubscriptionStatusEnumExtension SaasSubscriptionStatus { get; set; }

        public SubscriptionResultExtension()
        {
            base.SaasSubscriptionStatus = (SubscriptionStatusEnum)this.SaasSubscriptionStatus;
        }

    }
}
