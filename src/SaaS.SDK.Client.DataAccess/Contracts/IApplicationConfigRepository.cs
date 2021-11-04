namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    using System.Collections.Generic;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

    /// <summary>
    /// Repository to access application configuration.
    /// </summary>
    public interface IApplicationConfigRepository
    {
        /// <summary>
        /// Gets the value from application configuration.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>value corresponding to the application configuration key.</returns>
        string GetValueByName(string name);

        /// <summary>
        /// Gets the value from application configuration.
        /// </summary>
        /// <param Id="Id">The App Config Id.</param>
        /// <returns>value corresponding to the application configuration key.</returns>
        ApplicationConfiguration GetById(int Id);

        /// <summary>
        /// Gets all the  values from application configuration.
        /// </summary>
        /// <returns>List of key value pairs stored in application configuration.</returns>
        IEnumerable<ApplicationConfiguration> GetAll();

        /// <summary>
        /// Update an application configuration value.
        /// </summary>
        /// <returns>Success or Failure.</returns>
        int SaveById(ApplicationConfiguration applicationConfiguration);
    }
}
