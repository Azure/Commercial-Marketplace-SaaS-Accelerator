namespace Microsoft.Marketplace.SaasKit.Models
{
    using System.Text.Json.Serialization;

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
        [JsonPropertyName("code")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
