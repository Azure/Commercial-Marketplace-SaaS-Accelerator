using System.Collections.Generic;
using System.Linq;
using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Services;

/// <summary>
/// Repository to access ApplicationConfiguration.
/// </summary>
/// <seealso cref="IApplicationConfigRepository" />
public class ApplicationConfigRepository : IApplicationConfigRepository
{
    /// <summary>
    /// The context.
    /// </summary>
    private readonly SaasKitContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationConfigRepository"/> class.
    /// </summary>
    /// <param name="context">The this.context.</param>
    public ApplicationConfigRepository(SaasKitContext context)
    {
        this.context = context;
    }

    /// <summary>
    /// Gets the name of the value by.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>
    /// Value of application configuration entry by name.
    /// </returns>
    public string GetValueByName(string name)
    {
        return this.context.ApplicationConfiguration.Where(s => s.Name == name).FirstOrDefault()?.Value;
    }

    /// <summary>
    /// Gets the value from application configuration.
    /// </summary>
    /// <returns>List of all application configuration items.</returns>
    public IEnumerable<ApplicationConfiguration> GetAll()
    {
        return this.context.ApplicationConfiguration;
    }

    /// <summary>
    /// Sets the value from application configuration.
    /// </summary>
    /// <returns>Id of the application configuration.</returns>
    public int SaveById(ApplicationConfiguration applicationConfiguration)
    {
        var existingConfig = this.context.ApplicationConfiguration.Where(a => a.Id == applicationConfiguration.Id).FirstOrDefault();
        existingConfig.Value = applicationConfiguration.Value;
        existingConfig.Description = applicationConfiguration.Description;
        this.context.SaveChanges();
        return existingConfig.Id;
    }

    /// <summary>
    /// Get the Appconfig by Id.
    /// </summary>
    /// <returns>An application configuration item.</returns>
    public ApplicationConfiguration GetById(int Id)
    {
        return this.context.ApplicationConfiguration.Where(s => s.Id == Id).FirstOrDefault();
    }

    /// <summary>
    /// Update application configuration value.
    /// </summary>
    /// <param name="name">The app config name.</param>
    /// <param name="newValue">The new value.</param>
    /// <returns>True or false.</returns>
    public bool SaveValueByName(string name, string newValue)
    {
        var appConfig = this.context.ApplicationConfiguration.Where(a => a.Name == name).FirstOrDefault();
        if (appConfig != null)
        {
            appConfig.Value = newValue;
            this.context.SaveChanges();
            return true;
        }
        return false;
    }
}