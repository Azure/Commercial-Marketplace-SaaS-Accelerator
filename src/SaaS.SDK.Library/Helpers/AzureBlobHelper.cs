using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;


namespace Microsoft.Marketplace.SaaS.SDK.Library.Helpers
{
    public class AzureBlobHelper
    {
        public static string ReadARMTemplateFromBlob(string fileName)
        {
            string blobContainer = "armtemplateblob";
            string StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=ampsaasarmtemplates;AccountKey=0ljw7MwweuwnLYl45L2SrXYUpI7kLwlJVqtwg569ibZEnhJqtI/ps2pXhpnt8AxiCaTZAPaQNH9D3qIYUwbIdQ==;EndpointSuffix=core.windows.net";

            // Setup the connection to the storage account
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);

            // Connect to the blob storage
            CloudBlobClient serviceClient = storageAccount.CreateCloudBlobClient();
            // Connect to the blob container
            CloudBlobContainer container = serviceClient.GetContainerReference(blobContainer);
            // Connect to the blob file
            CloudBlockBlob blob = container.GetBlockBlobReference(fileName);

            // Get the blob file as text
            string contents = blob.DownloadTextAsync().Result;
            return contents;

        }
    }
}
