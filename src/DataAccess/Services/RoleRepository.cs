using System;
using System.Collections.Generic;
using System.Linq;
using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Services;
public class RoleRepository : IRoleRepository
{
    /// <summary>
    /// The this.context.
    /// </summary>
    private readonly SaasKitContext context;

    /// <summary>
    /// The disposed.
    /// </summary>
    private bool disposed = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="KnownUsersRepository"/> class.
    /// </summary>
    /// <param name="context">The this.context.</param>
    public RoleRepository(SaasKitContext context)
    {
        this.context = context;
    }

    /// <summary>
    /// Gets all known users.
    /// </summary>
    /// <returns>
    /// All known users.
    /// </returns>
    public IEnumerable<Roles> GetAllRoles()
    {
        return this.context.Roles;
    }

    /// <summary>
    /// Gets this instance.
    /// </summary>
    /// <returns> Exception.</returns>
    public IEnumerable<Roles> Get()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets the specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>
    /// Entity for the given identifier.
    /// </returns>
    public Roles Get(int id)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Adds the specified entities.
    /// </summary>
    /// <param name="entities">The entities.</param>
    /// <returns>
    /// Internal identifier after saving the entity.
    /// </returns>
    public int Save(Roles entities)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Removes the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    public void Remove(Roles entity)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        this.Dispose(true);

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                this.context.Dispose();
            }
        }

        this.disposed = true;
    }
}
