namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Repository to access the possible data types for the input attributes for a SaaS subscription
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IValueTypesRepository : IDisposable
    {
        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>List of all value types supported by the application</returns>
        IEnumerable<ValueTypes> GetAll();

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <param name="Id">The identifier.</param>
        /// <returns></returns>
        ValueTypes GetById(int Id);

        /// <summary>
        /// Gets the value type values.
        /// </summary>
        /// <returns>Returns all the values for value types</returns>
        IEnumerable<string> GetValueTypeValues();
    }
}
