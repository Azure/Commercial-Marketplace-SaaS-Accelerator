using Microsoft.AspNetCore.Http;
using Microsoft.Marketplace.SaaS.SDK.Services.Contracts;
using Microsoft.Marketplace.SaaS.SDK.Services.Models;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Marketplace.SaaS.SDK.Services.Services
{
    public class AzureBlobFileClient : IAzureBlobFileClient
    {
        private AzureBlobConfig azureBlobConfig = null;
        public AzureBlobFileClient(AzureBlobConfig azureBlobConfig)
        {
            this.azureBlobConfig = azureBlobConfig;
        }

        public string UploadARMTemplateToBlob(IFormFile file, string fileName, string fileContantType, Guid referenceid)
        {
            try
            {
                CloudStorageAccount fileStorageAccount = CloudStorageAccount.Parse(azureBlobConfig.BlobConnectionString);
                CloudBlobClient blobClient = fileStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(azureBlobConfig.BlobContainer);
                bool result = container.CreateIfNotExistsAsync().Result;
                if (result)
                {
                    container.SetPermissionsAsync(new BlobContainerPermissions
                    {
                        PublicAccess =
                      BlobContainerPublicAccessType.Blob
                    });

                }

                fileName = fileName.Replace(" ", "-");
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
                blockBlob.Properties.ContentType = fileContantType;
                blockBlob.UploadFromStreamAsync(file.OpenReadStream(), file.Length);

                return blockBlob.Uri.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ReadARMTemplateFromBlob(string fileName)
        {

            // Setup the connection to the storage account
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(azureBlobConfig.BlobConnectionString);

            // Connect to the blob storage
            CloudBlobClient serviceClient = storageAccount.CreateCloudBlobClient();
            // Connect to the blob container
            CloudBlobContainer container = serviceClient.GetContainerReference(azureBlobConfig.BlobContainer);
            // Connect to the blob file
            CloudBlockBlob blob = container.GetBlockBlobReference(fileName);

            // Get the blob file as text
            string contents = blob.DownloadTextAsync().Result;
            return contents;

        }
    }
}
