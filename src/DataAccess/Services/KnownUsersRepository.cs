using System;
using System.Collections.Generic;
using System.Linq;
using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Services;

/// <summary>
/// The Known users data repository.
/// </summary>
/// <seealso cref="IKnownUsersRepository" />
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
    /// Gets all known users.
    /// </summary>
    /// <returns>
    /// All known users.
    /// </returns>
    public IEnumerable<KnownUsers> GetAllKnownUsers()
    {
        return this.context.KnownUsers;
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
    /// Adds the know users from application configuration.
    /// </summary>
    /// <param name="knownUsers">The known users.</param>
    public void AddKnowUsersFromAppConfig(string knownUsers)
    {
        var existingUsers = this.context.KnownUsers;
        if (existingUsers != null && existingUsers.ToList().Count() == 0)
        {
            List<string> knownUsersList = knownUsers.Split(',').ToList();
            foreach (var user in knownUsersList)
            {
                var users = new KnownUsers()
                {
                    UserEmail = user.Trim(),
                    RoleId = 1, // Publisher Admin
                };
                this.context.KnownUsers.Add(users);
                this.context.SaveChanges();
            }
        }
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
    /// Save all known users.
    /// </summary>
    /// <returns>The number of modified records.</returns>
    public int SaveAllKnownUsers(IEnumerable<KnownUsers> knownUsers)
    {
        var usersToAdd = knownUsers?.ExceptBy(context.KnownUsers?.ToList().Select(u1 => u1.UserEmail), u2 => u2.UserEmail);
        this.context.KnownUsers.AddRange(usersToAdd);
        var usersToRemove = context.KnownUsers?.ToList().ExceptBy(knownUsers?.Select(u1 => u1.UserEmail), u2 => u2.UserEmail);
        this.context.KnownUsers.RemoveRange(usersToRemove);

        return this.context.SaveChanges();
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