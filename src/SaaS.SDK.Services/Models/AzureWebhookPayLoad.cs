namespace Microsoft.Marketplace.SaaS.SDK.Services.Models
{
    using Newtonsoft.Json;
    using System;

    public class AzureWebHookPayLoad
    {
        [JsonProperty(PropertyName = "action")]
        public string Action { get; set; }

        [JsonProperty(PropertyName = "activityId")]
        public string ActivityId { get; set; }

        [JsonProperty(PropertyName = "offerId")]
        public string OfferId { get; set; }

        // Operation Id is presented as Id property on the json payload
        [JsonProperty(PropertyName = "id")] public string OperationId { get; set; }

        public string PlanId { get; set; }
        public string PublisherId { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }

        [JsonProperty(PropertyName = "subscriptionId")]
        public string SubscriptionId { get; set; }

        [JsonProperty(PropertyName = "timeStamp")]
        public DateTimeOffset TimeStamp { get; set; }
    }
}
