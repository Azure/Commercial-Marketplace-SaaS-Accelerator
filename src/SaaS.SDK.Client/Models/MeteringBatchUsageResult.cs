namespace Microsoft.Marketplace.SaasKit.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Subscription Batch Usage Result
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Models.SaaSApiResult" />
    public class MeteringBatchUsageResult : SaaSApiResult
    {
        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        [JsonProperty("count")]
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        [JsonProperty("result")]
        public IEnumerable<ResultBatchUsageResult> Result { get; set; }
    }

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
