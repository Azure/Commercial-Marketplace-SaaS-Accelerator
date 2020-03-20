using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    public class ApplicationConfigRepository : IApplicationConfigRepository
    {
        private readonly SaasKitContext Context;

        public ApplicationConfigRepository(SaasKitContext context)
        {
            Context = context;
        }

        public string GetValuefromApplicationConfig(string Name)
        {
            return Context.ApplicationConfiguration.Where(s => s.Name == Name).FirstOrDefault().Value;
        }

        /// <summary>
        /// Gets the value from application configuration.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ApplicationConfiguration> GetValuefromApplicationConfig()
        {
            return Context.ApplicationConfiguration;
        }
    }
}
