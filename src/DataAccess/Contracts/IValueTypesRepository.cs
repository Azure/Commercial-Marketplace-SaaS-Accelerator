using System;
using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Contracts;

/// <summary>
/// Repository to access the possible data types for the input attributes for a SaaS subscription.
/// </summary>
/// <seealso cref="System.IDisposable" />
public interface IValueTypesRepository : IDisposable
{
    /// <summary>
    /// Gets all.
    /// </summary>
    /// <returns>List of all value types supported by the application.</returns>
    IEnumerable<ValueTypes> GetAll();

    /// <summary>
    /// Gets the by identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns> ValueTypes.</returns>
    ValueTypes GetById(int id);

    /// <summary>
    /// Gets the value type values.
    /// </summary>
    /// <returns>Returns all the values for value types.</returns>
    IEnumerable<string> GetValueTypeValues();
}