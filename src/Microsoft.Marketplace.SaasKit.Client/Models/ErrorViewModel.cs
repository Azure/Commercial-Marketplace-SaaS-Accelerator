namespace Microsoft.Marketplace.SaasKit.Client.Models
{
    /// <summary>
    /// Defines the <see cref="ErrorViewModel" />
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Gets or sets the RequestId
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// Gets a value indicating whether ShowRequestId
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
