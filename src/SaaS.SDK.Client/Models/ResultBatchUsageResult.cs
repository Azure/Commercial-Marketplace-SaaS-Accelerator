namespace Microsoft.Marketplace.SaasKit.Models
{
    using System.Text.Json.Serialization;

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
        [JsonPropertyName("error")]
        public object Error { get; set; }
    }
}
