namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using System;
    using System.Linq;

    /// <summary>
    /// Repository to access events
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.IEventsRepository" />
    public class EventsRepository : IEventsRepository
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly SaasKitContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public EventsRepository(SaasKitContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Gets the name of the by.
        /// </summary>
        /// <param name="Name">The name.</param>
        /// <returns>Event id by name</returns>
        public int GetByName(String Name)
        {
            var results = context.Events.Where(s => s.EventsName == Name);
            if (results.Count() == 0)
                return 0;
            else
                return context.Events.Where(s => s.EventsName == Name).FirstOrDefault().EventsId;
        }
    }
}
