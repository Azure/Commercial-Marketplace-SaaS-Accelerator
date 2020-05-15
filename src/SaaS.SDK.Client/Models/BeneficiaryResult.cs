namespace Microsoft.Marketplace.SaasKit.Models
{
    using System;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Beneficiary Result.
    /// </summary>
    public class BeneficiaryResult
    {
        /// <summary>
        /// Gets or sets the tenant identifier.
        /// </summary>
        /// <value>
        /// The tenant identifier.
        /// </value>
        [JsonPropertyName("tenantId")]
        public Guid TenantId { get; set; }
    }
}