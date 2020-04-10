using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    public interface IUsersRepository : IDisposable, IBaseRepository<Users>
    {
         Users GetPartnerDetailFromEmail(string emailAddress);
    }
}
