using Microsoft.AspNetCore.Http;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Marketplace.SaaS.SDK.Services.Contracts
{
    public interface IAzureBlobFileClient
    {
        string UploadARMTemplateToBlob(IFormFile file, string fileName, string fileContantType, Guid referenceid);
        string ReadARMTemplateFromBlob(string fileName);
    }
}
