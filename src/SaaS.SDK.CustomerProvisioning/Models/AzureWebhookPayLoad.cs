namespace Microsoft.Marketplace.SaasKit.Client.Models
{
    //using Newtonsoft.Json;
    using System.Text.Json;
    using System;
    using System.Text.Json.Serialization;

    public class AzureWebHookPayLoad
    {
        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("activityId")]
        public string ActivityId { get; set; }

        [JsonPropertyName("offerId")]
        public string OfferId { get; set; }

        // Operation Id is presented as Id property on the json payload
        [JsonPropertyName("id")]
        public string OperationId { get; set; }

        public string PlanId { get; set; }
        public string PublisherId { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }

        [JsonPropertyName("subscriptionId")]
        public string SubscriptionId { get; set; }

        [JsonPropertyName("timeStamp")]
        public DateTimeOffset TimeStamp { get; set; }
    }
}
