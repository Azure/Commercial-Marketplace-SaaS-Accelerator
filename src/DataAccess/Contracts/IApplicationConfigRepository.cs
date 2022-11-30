using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Contracts;

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
    /// Get application configuration by id.
    /// </summary>
    /// <param Id="Id">The App Config Id.</param>
    /// <returns>An application configuration with corresponding id.</returns>
    ApplicationConfiguration GetById(int Id);

    /// <summary>
    /// Get all the  values from application configuration.
    /// </summary>
    /// <returns>List of key value pairs stored in application configuration.</returns>
    IEnumerable<ApplicationConfiguration> GetAll();

    /// <summary>
    /// Update application configuration.
    /// </summary>
    /// <returns>Success or Failure.</returns>
    int SaveById(ApplicationConfiguration applicationConfiguration);

    /// <summary>
    /// Update application configuration value.
    /// </summary>
    /// <param name="name">The app config name.</param>
    /// <param name="newValue">The new value.</param>
    /// <returns>True or false.</returns>
    bool SaveValueByName(string name, string newValue);
}