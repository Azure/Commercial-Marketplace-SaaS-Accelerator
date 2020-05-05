namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using System.Collections.Generic;

    /// <summary>
    /// Repository to access application configuration 
    /// </summary>
    public interface IApplicationConfigRepository
    {
        /// <summary>
        /// Gets the value from application configuration.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>value corresponding to the application configuration key</returns>
        string GetValueByName(string name);

        /// <summary>
        /// Gets all the  values from application configuration.
        /// </summary>
        /// <returns>List of key value pairs stored in application configuration</returns>
        IEnumerable<ApplicationConfiguration> GetAll();
    }
}
