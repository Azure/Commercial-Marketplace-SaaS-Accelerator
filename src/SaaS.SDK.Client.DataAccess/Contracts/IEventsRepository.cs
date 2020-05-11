namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    using System;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

    /// <summary>
    /// Repository to access plan events.
    /// </summary>
    public interface IEventsRepository
    {
        /// <summary>
        /// Gets the event identifier.
        /// </summary>
        /// <param name="name">The name of the event.</param>
        /// <returns>ID of the event by name.</returns>
        Events GetByName(string name);
    }
}
