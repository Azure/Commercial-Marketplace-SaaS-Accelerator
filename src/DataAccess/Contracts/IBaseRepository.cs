using System.Collections.Generic;

namespace Marketplace.SaaS.Accelerator.DataAccess.Contracts;

/// <summary>
/// Base repository for managing entities.
/// </summary>
/// <typeparam name="TEntity">The type of the ent.</typeparam>
public interface IBaseRepository<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Gets all the records.
    /// </summary>
    /// <returns>Basic info.</returns>
    IEnumerable<TEntity> Get();

    /// <summary>
    /// Gets the record for the specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>Entity for the given identifier.</returns>
    TEntity Get(int id);

    /// <summary>
    /// Adds the specified entities.
    /// </summary>
    /// <param name="entities">The entities.</param>
    /// <returns>Internal identifier after saving the entity.</returns>
    int Save(TEntity entities);

    /// <summary>
    /// Removes the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    void Remove(TEntity entity);
}