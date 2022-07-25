namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    using System.Linq;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

    /// <summary>
    /// Repository to access events.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.IEventsRepository" />
    public class EventsRepository : IEventsRepository
    {
        /// <summary>
        /// The context.
        /// </summary>
        private readonly SaasKitContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsRepository"/> class.
        /// </summary>
        /// <param name="context">The this.context.</param>
        public EventsRepository(SaasKitContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Gets the name of the by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// Event id by name.
        /// </returns>
        public Events GetByName(string name)
        {
            var results = this.context.Events.Where(s => s.EventsName == name);
            return this.context.Events.Where(s => s.EventsName == name).FirstOrDefault();
        }
    }
}
