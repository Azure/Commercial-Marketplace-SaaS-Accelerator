namespace Microsoft.Marketplace.SaaS.SDK.Services.Services
{
    using System;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Marketplace.SaaS.SDK.Services.Contracts;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    /// <summary>
    /// Implementation of IARMTemplateStorageService to store the template to Azure blob storage.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaaS.SDK.Services.Contracts.IARMTemplateStorageService" />
    public class AzureBlobStorageService : IARMTemplateStorageService
    {
        /// <summary>
        /// The azure BLOB configuration.
        /// </summary>
        private AzureBlobConfig azureBlobConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureBlobStorageService"/> class.
        /// </summary>
        /// <param name="azureBlobConfig">The azure BLOB configuration.</param>
        public AzureBlobStorageService(AzureBlobConfig azureBlobConfig)
        {
            this.azureBlobConfig = azureBlobConfig;
        }

        /// <summary>
        /// Saves the arm template.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileContantType">Type of the file contant.</param>
        /// <param name="referenceid">The referenceid.</param>
        /// <returns>
        /// Reference to the saved ARM template (eg:URL).
        /// </returns>
        public string SaveARMTemplate(IFormFile file, string fileName, string fileContantType, Guid referenceid)
        {
            CloudStorageAccount fileStorageAccount = CloudStorageAccount.Parse(this.azureBlobConfig.BlobConnectionString);
            CloudBlobClient blobClient = fileStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(this.azureBlobConfig.BlobContainer);
            bool result = container.CreateIfNotExistsAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            if (result)
            {
                container.SetPermissionsAsync(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob,
                });
            }

            fileName = fileName.Replace(" ", "-");
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
            blockBlob.Properties.ContentType = fileContantType;
            blockBlob.UploadFromStreamAsync(file.OpenReadStream(), file.Length);

            return blockBlob.Uri.ToString();
        }

        /// <summary>
        /// Gets the arm template content as string.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>
        /// Contents of ARM template as string.
        /// </returns>
        public string GetARMTemplateContentAsString(string fileName)
        {
            // Setup the connection to the storage account
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(this.azureBlobConfig.BlobConnectionString);

            // Connect to the blob storage
            CloudBlobClient serviceClient = storageAccount.CreateCloudBlobClient();

            // Connect to the blob container
            CloudBlobContainer container = serviceClient.GetContainerReference(this.azureBlobConfig.BlobContainer);

            // Connect to the blob file
            CloudBlockBlob blob = container.GetBlockBlobReference(fileName);

            // Get the blob file as text
            string contents = blob.DownloadTextAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            return contents;
        }
    }
}
