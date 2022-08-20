using Microsoft.Marketplace.SaaSAccelerator.DataAccess.Entities;

namespace Microsoft.Marketplace.SaaSAccelerator.DataAccess.Contracts
{
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
