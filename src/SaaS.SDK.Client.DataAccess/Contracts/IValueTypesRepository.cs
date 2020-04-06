using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System;
using System.Collections.Generic;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    public interface IValueTypesRepository : IDisposable
    {
        IEnumerable<ValueTypes> GetValueTypes();
        ValueTypes GetValuetypeOnId(int Id);
        IEnumerable<string> GetValueTypesValues();
    }
}
