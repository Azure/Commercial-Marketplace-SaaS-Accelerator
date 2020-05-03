using Microsoft.AspNetCore.Http;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Marketplace.SaaS.SDK.Services.Contracts
{
    public interface IFileUploadClient
    {
        string UploadFile(IFormFile file, string fileName, string fileContantType, Guid referenceid, IApplicationConfigRepository applicationConfigRepository, string StorageConnectionString);
    }
}
