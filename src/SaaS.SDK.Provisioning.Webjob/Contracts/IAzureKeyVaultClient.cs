using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SaaS.SDK.Provisioning.Webjob.Contracts
{
    public interface IAzureKeyVaultClient
    {
        Task<string> WriteKeyAsync(string key, string val);
        Task<string> GetKeyAsync(string key);
        bool ValidateUserParameters(IDictionary<string, string> dictionary);
    }
}
