namespace Microsoft.Marketplace.SaasKit.Models
{
    using System.ComponentModel;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Plan Details.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Models.SaaSApiResult" />
    public class PlanDetailResult : SaaSApiResult
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the plan identifier.
        /// </summary>
        /// <value>
        /// The plan identifier.
        /// </value>
        [JsonPropertyName("planId")]
        [DisplayName("planId")]
        public string PlanId { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        [JsonPropertyName("displayName")]
        [DisplayName("displayName")]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is private.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is private; otherwise, <c>false</c>.
        /// </value>
        [JsonPropertyName("isPrivate")]
        [DisplayName("isPrivate")]
        public bool IsPrivate { get; set; }
    }
}
