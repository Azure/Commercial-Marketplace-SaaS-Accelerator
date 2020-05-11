namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

    /// <summary>
    /// The Known users data repository.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.IKnownUsersRepository" />
    public class KnownUsersRepository : IKnownUsersRepository
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
        public KnownUsersRepository(SaasKitContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Gets the known user detail.
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="roleId">The role identifier.</param>
        /// <returns>
        /// User detail by email and role.
        /// </returns>
        public KnownUsers GetKnownUserDetail(string emailAddress, int roleId)
        {
            return this.context.KnownUsers.Where(s => s.UserEmail == emailAddress && s.RoleId == roleId).FirstOrDefault();
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns> Exception.</returns>
        public IEnumerable<KnownUsers> Get()
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
        public KnownUsers Get(int id)
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
        public int Save(KnownUsers entities)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void Remove(KnownUsers entity)
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
}
