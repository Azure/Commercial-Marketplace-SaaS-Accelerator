using System.Linq;
using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Services;

/// <summary>
/// Repository to access events.
/// </summary>
/// <seealso cref="IEventsRepository" />
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