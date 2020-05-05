namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Repository to access ApplicationConfiguration
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.IApplicationConfigRepository" />
    public class ApplicationConfigRepository : IApplicationConfigRepository
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly SaasKitContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationConfigRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public ApplicationConfigRepository(SaasKitContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Gets the name of the value by.
        /// </summary>
        /// <param name="Name">The name.</param>
        /// <returns>Value of application configuration entry by name</returns>
        public string GetValueByName(string Name)
        {
            return context.ApplicationConfiguration.Where(s => s.Name == Name).FirstOrDefault().Value;
        }

        /// <summary>
        /// Gets the value from application configuration.
        /// </summary>
        /// <returns>List of all application configuration items</returns>
        public IEnumerable<ApplicationConfiguration> GetAll()
        {
            return context.ApplicationConfiguration;
        }
    }
}
