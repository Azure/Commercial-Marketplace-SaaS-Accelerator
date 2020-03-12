using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;


namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    public class KnownUsersRepository : IKnownUsersRepository
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly SaasKitContext Context;

        /// <summary>
        /// Initializes a new instance of the <see cref="KnownUsersRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public KnownUsersRepository(SaasKitContext context)
        {
            Context = context;
        }

        /// <summary>
        /// The disposed
        /// </summary>
        private bool disposed = false;


        /// <summary>
        /// Gets the known user detail.
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="roleId">The role identifier.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public KnownUsers GetKnownUserDetail(string emailAddress, int roleId)
        {
            return Context.KnownUsers.Where(s => s.UserEmail == emailAddress && s.RoleId == roleId).FirstOrDefault();
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IEnumerable<KnownUsers> Get()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public KnownUsers Get(int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds the specified entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public int Add(KnownUsers entities)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void Remove(KnownUsers entity)
        {
            throw new NotImplementedException();
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
                    Context.Dispose();
                }
            }
            this.disposed = true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }


    }
}
