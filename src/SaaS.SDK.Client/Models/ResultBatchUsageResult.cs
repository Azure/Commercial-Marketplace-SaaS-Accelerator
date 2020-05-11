namespace Microsoft.Marketplace.SaasKit.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// BatchUsage Result.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Models.MeteringUsageResult" />
    public class ResultBatchUsageResult : MeteringUsageResult
    {
        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>
        /// The Error.
        /// </value>
        [JsonProperty("error")]
        public object Error { get; set; }
    }
}
