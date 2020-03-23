namespace Microsoft.Marketplace.SaasKit.Models
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// Beneficiary Result
    /// </summary>
    public class BeneficiaryResult
    {
        /// <summary>
        /// Gets or sets the tenant identifier.
        /// </summary>
        /// <value>
        /// The tenant identifier.
        /// </value>
        [JsonProperty("tenantId")]
        public Guid TenantId { get; set; }
    }
}