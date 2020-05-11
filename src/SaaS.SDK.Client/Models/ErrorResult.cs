namespace Microsoft.Marketplace.SaasKit.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// Binds Error Detail in the Response.
    /// </summary>
    public class ErrorResult
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
