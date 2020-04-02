using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Marketplace.SaasKit.Models;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Services;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
//using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Microsoft.Marketplace.Saas.Web.Helpers
{
    public class BlobFileUploadHelper
    {



        /// <summary>
        /// Uploads the image.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>
        /// File URL
        /// </returns>
        public static string UploadFile(IFormFile file, string fileName, string fileContantType, Guid referenceid, IApplicationConfigRepository applicationConfigRepository)
        {
            try
            {
                //this.LoggerService.Log("Inside of UploadImage() of UploadManager");
                string StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=ampsaasarmtemplates;AccountKey=0ljw7MwweuwnLYl45L2SrXYUpI7kLwlJVqtwg569ibZEnhJqtI/ps2pXhpnt8AxiCaTZAPaQNH9D3qIYUwbIdQ==;EndpointSuffix=core.windows.net";
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
                var guid = referenceid.ToString();
                fileName = guid.Substring(guid.Length - 6) + '-' + fileName;
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
                blockBlob.Properties.ContentType = fileContantType;
                blockBlob.UploadFromStreamAsync(file.OpenReadStream(), file.Length);

                return blockBlob.Uri.ToString();
            }
            catch (Exception ex)
            {
                //this.LoggerService.Log("UploadImage() -> Erroe while uploading the image", ex, LogSeverity.ERROR);
                //throw new CloudhubException(JsonConvert.SerializeObject(new { ErrorValue = C3ErrorConstants.FAILED_TO_UPLOAD_IMAGE, ErrorNotifyType = ErrorNotifyTypes.TRANSLATE_KEY }));
                throw ex;
            }
        }

        ///// <summary>
        ///// Deletes the image.
        ///// </summary>
        ///// <param name="url">The URL.</param>
        //public void DeleteImage(string url)
        //{
        //    //this.LoggerService.Log("Inside of DeleteImage() of UploadManager");

        //    CloudStorageAccount fileStorageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);

        //    CloudBlobClient blobClient = fileStorageAccount.CreateCloudBlobClient();

        //    CloudBlobContainer container = blobClient.GetContainerReference("profileimages");

        //    if (container.Exists())
        //    {
        //        var index = url.LastIndexOf('/');
        //        var fileName = url.Substring(index + 1);
        //        CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
        //        blockBlob.DeleteIfExists();
        //    }
        //}

    }
}
