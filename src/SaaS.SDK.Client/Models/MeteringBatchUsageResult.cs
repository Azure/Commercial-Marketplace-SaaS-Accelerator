namespace Microsoft.Marketplace.SaasKit.Models
{
    using System.Text.Json.Serialization;
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
        [JsonPropertyName("count")]
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        [JsonPropertyName("result")]
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
        [JsonPropertyName("error")]
        public object Error { get; set; }
    }
}
