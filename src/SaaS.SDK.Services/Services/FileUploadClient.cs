using Microsoft.AspNetCore.Http;
using Microsoft.Marketplace.SaaS.SDK.Services.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Marketplace.SaaS.SDK.Services.Services
{
    public class FileUploadClient : IFileUploadClient
    {
        private readonly IApplicationConfigRepository applicationConfigRepository;
        public FileUploadClient(IApplicationConfigRepository applicationConfigRepository)
        {
            this.applicationConfigRepository = applicationConfigRepository;
        }

        public string UploadFile(IFormFile file, string fileName, string fileContantType, Guid referenceid, IApplicationConfigRepository applicationConfigRepository, string StorageConnectionString)
        {
            try
            {
                CloudStorageAccount fileStorageAccount = CloudStorageAccount.Parse(StorageConnectionString);
                CloudBlobClient blobClient = fileStorageAccount.CreateCloudBlobClient();

                string blobContainer = "armtemplateblob";
                CloudBlobContainer container = blobClient.GetContainerReference(blobContainer);
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
    }
}
