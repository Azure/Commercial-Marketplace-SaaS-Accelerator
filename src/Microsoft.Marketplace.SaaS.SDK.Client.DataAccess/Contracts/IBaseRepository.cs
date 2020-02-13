using System.Collections.Generic;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    public interface IBaseRepository<TEnt> where TEnt : class
    {
        IEnumerable<TEnt> Get();
        TEnt Get(int id);
        int Add(TEnt entities);
        void Remove(TEnt entity);
    }
}
