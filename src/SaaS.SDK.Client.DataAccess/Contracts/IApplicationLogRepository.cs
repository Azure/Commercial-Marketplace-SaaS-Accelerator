using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    public interface IApplicationLogRepository : IDisposable
    {
        void AddApplicationLogs(ApplicationLog logDetail);
    }
}
