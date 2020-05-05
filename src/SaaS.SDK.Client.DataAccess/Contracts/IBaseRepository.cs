namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    using System.Collections.Generic;

    /// <summary>
    /// Base repository for managing entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of the ent.</typeparam>
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Gets all the records 
        /// </summary>
        /// <returns></returns>
        IEnumerable<TEntity> Get();

        /// <summary>
        /// Gets the record for the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Entity for the given identifier</returns>
        TEntity Get(int id);

        /// <summary>
        /// Adds the specified entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        /// <returns>Internal identifier after saving the entity</returns>
        int Add(TEntity entities);

        /// <summary>
        /// Removes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Remove(TEntity entity);
    }
}
