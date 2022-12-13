using System;
using System.Collections.Generic;
using System.Linq;
using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Services;

/// <summary>
/// Repository to access users.
/// </summary>
/// <seealso cref="IUsersRepository" />
public class UsersRepository : IUsersRepository
{
    /// <summary>
    /// The context.
    /// </summary>
    private readonly SaasKitContext context;

    /// <summary>
    /// The disposed.
    /// </summary>
    private bool disposed = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="UsersRepository"/> class.
    /// </summary>
    /// <param name="context">The this.context.</param>
    public UsersRepository(SaasKitContext context)
    {
        this.context = context;
    }

    /// <summary>
    /// Gets this instance.
    /// </summary>
    /// <returns> List of Users.</returns>
    public IEnumerable<Users> Get()
    {
        return this.context.Users;
    }

    /// <summary>
    /// Gets the specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns> Users.</returns>
    public Users Get(int id)
    {
        return this.context.Users.Where(s => s.UserId == id).FirstOrDefault();
    }

    /// <summary>
    /// Removes the specified entity.
    /// </summary>
    /// <param name="userDetail">The entity.</param>
    public void Remove(Users userDetail)
    {
        this.context.Users.Remove(userDetail);
        this.context.SaveChanges();
    }

    /// <summary>
    /// Adds the specified user detail.
    /// </summary>
    /// <param name="userDetail">The user detail.</param>
    /// <returns> User Id.</returns>
    public int Save(Users userDetail)
    {
        var existingUser = this.context.Users.Where(s => s.EmailAddress == userDetail.EmailAddress).FirstOrDefault();
        if (existingUser != null)
        {
            existingUser.FullName = userDetail.FullName;
            this.context.Users.Update(existingUser);
            return existingUser.UserId;
        }
        else
        {
            this.context.Users.Add(userDetail);
        }

        this.context.SaveChanges();
        return userDetail.UserId;
    }

    /// <summary>
    /// Gets the partner detail from email.
    /// </summary>
    /// <param name="emailAddress">The email address.</param>
    /// <returns> user details.</returns>
    public Users GetPartnerDetailFromEmail(string emailAddress)
    {
        return this.context.Users.Where(s => s.EmailAddress == emailAddress).FirstOrDefault();
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