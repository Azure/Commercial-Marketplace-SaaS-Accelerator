using Microsoft.AspNetCore.Http;
using Microsoft.Marketplace.SaaS.SDK.Services.Models;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Marketplace.SaaS.SDK.Services.Contracts
{
    public interface IEmailService
    {
        void SendEmail(EmailContentModel emailContent);
    }
}
