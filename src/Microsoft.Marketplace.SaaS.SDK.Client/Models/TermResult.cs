namespace Microsoft.Marketplace.SaasKit.Models
{
    using System;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Get TermResult
    /// </summary>
    public class TermResult
    {
        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>
        /// The end date.
        /// </value>
        [JsonProperty("endDate")]
        public DateTimeOffset EndDate { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>
        /// The start date.
        /// </value>
        [JsonProperty("startDate")]
        public DateTimeOffset StartDate { get; set; }

        /// <summary>
        /// Gets or sets the term unit.
        /// </summary>
        /// <value>
        /// The term unit.
        /// </value>
        [JsonProperty("termUnit")]
        [JsonConverter(typeof(StringEnumConverter))]
        public TermUnitEnum TermUnit { get; set; }
    }
}
