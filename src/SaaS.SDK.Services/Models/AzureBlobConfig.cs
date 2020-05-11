namespace Microsoft.Marketplace.SaaS.SDK.Services.Models
{
    /// <summary>
    /// Azure Blob Config.
    /// </summary>
    public class AzureBlobConfig
    {
        /// <summary>
        /// Gets or sets the BLOB container.
        /// </summary>
        /// <value>
        /// The BLOB container.
        /// </value>
        public string BlobContainer { get; set; }

        /// <summary>
        /// Gets or sets the BLOB connection string.
        /// </summary>
        /// <value>
        /// The BLOB connection string.
        /// </value>
        public string BlobConnectionString { get; set; }
    }
}
