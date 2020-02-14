namespace Microsoft.Marketplace.SaasKit.Models
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Newtonsoft.Json;

    /// <summary>
    /// Sets Plan
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Models.SaaSApiResult" />
    public class PlanResult : SaaSApiResult
    {
        /// <summary>
        /// Gets or sets the plans.
        /// </summary>
        /// <value>
        /// The plans.
        /// </value>
        [JsonProperty("plans")]
        [DisplayName("plans")]
        public List<PlanDetailResult> Plans { get; set; }
    }
}
