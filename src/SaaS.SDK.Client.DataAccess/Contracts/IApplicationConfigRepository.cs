using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    public interface IApplicationConfigRepository
    {
        string GetValuefromApplicationConfig(string name);

        /// <summary>
        /// Gets the valuefrom application configuration.
        /// </summary>
        /// <returns></returns>
        IEnumerable<ApplicationConfiguration> GetValuefromApplicationConfig();
    }
}
