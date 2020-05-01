namespace Microsoft.Marketplace.SaasKit.Models
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// Purchaser Result
    /// </summary>
    public class PurchaserResult
    {
        /// <summary>
        /// Gets or sets the tenant identifier.
        /// </summary>
        /// <value>
        /// The tenant identifier.
        /// </value>
        [JsonProperty("tenantId")]
        public Guid TenantId { get; set; }

        [JsonProperty("emailId")]
        public string EmailId { get; set; }

        [JsonProperty("objectId")]
        public Guid ObjectId { get; set; }
    }
}