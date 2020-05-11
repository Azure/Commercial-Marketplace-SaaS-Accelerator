namespace Microsoft.Marketplace.SaasKit.Models
{
    using Microsoft.Marketplace.SaasKit.Attributes;

    /// <summary>
    /// Get Fulfillment Result
    /// </summary>
    public class SaaSApiResult
    {
        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        /// <value>
        /// The request identifier.
        /// </value>
        [FromRequestHeader("x-ms-requestid")]
        public string RequestID { get; set; }
    }
}