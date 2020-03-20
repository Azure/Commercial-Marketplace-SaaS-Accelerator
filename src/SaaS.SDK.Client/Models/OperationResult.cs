namespace Microsoft.Marketplace.SaasKit.Models
{
    using System;
    using Microsoft.Marketplace.SaasKit.Models;
    using Newtonsoft.Json;

    /// <summary>
    /// Subscription Operation Response
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Models.SaaSApiResult" />
    public class OperationResult : SaaSApiResult
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [JsonProperty("id")]
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        [JsonProperty("status")]
        public OperationStatusEnum Status { get; set; }

        /// <summary>
        /// Gets or sets the resource location.
        /// </summary>
        /// <value>
        /// The resource location.
        /// </value>
        [JsonProperty("resourceLocation")]
        public string ResourceLocation { get; set; }

        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>
        /// The created.
        /// </value>
        [JsonProperty("created")]
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the last modified.
        /// </summary>
        /// <value>
        /// The last modified.
        /// </value>
        [JsonProperty("lastModified")]
        public DateTime? LastModified { get; set; }
    }
}
