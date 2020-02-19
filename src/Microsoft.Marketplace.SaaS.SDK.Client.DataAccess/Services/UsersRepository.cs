﻿using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    public class UsersRepository : IUsersRepository
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly SaasKitContext Context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public UsersRepository(SaasKitContext context)
        {
            Context = context;
        }

        /// <summary>
        /// The disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IEnumerable<Users> Get()
        {
            return Context.Users;
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Users Get(int id)
        {
            return Context.Users.Where(s => s.UserId == id).FirstOrDefault();
        }

        /// <summary>
        /// Removes the specified entity.
        /// </summary>
        /// <param name="userDetail">The entity.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void Remove(Users userDetail)
        {
            Context.Users.Remove(userDetail);
            Context.SaveChanges();
        }

        /// <summary>
        /// Adds the specified user detail.
        /// </summary>
        /// <param name="userDetail">The user detail.</param>
        /// <returns></returns>
        public int Add(Users userDetail)
        {
            var existingUser = Context.Users.Where(s => s.EmailAddress == userDetail.EmailAddress).FirstOrDefault();
            if (existingUser != null)
            {
                existingUser.FullName = userDetail.FullName;
                Context.Users.Update(existingUser);
                return existingUser.UserId;
            }
            else
            {
                Context.Users.Add(userDetail);
            }
            Context.SaveChanges();
            return userDetail.UserId;
        }

        /// <summary>
        /// Gets the partner detail from email.
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <returns></returns>
        public Users GetPartnerDetailFromEmail(string emailAddress)
        {
            return Context.Users.Where(s => s.EmailAddress == emailAddress).FirstOrDefault();
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
