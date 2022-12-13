using System;
using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Contracts;

/// <summary>
/// Known User Repository.
/// </summary>
/// <seealso cref="System.IDisposable" />
/// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.IBaseRepository{Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities.KnownUsers}" />
public interface IKnownUsersRepository : IDisposable, IBaseRepository<KnownUsers>
{
    /// <summary>
    /// Gets all known users.
    /// </summary>
    /// <returns>
    /// All known users.
    /// </returns>
    public IEnumerable<KnownUsers> GetAllKnownUsers();

    /// <summary>
    /// Gets the known user detail.
    /// </summary>
    /// <param name="emailAddress">The email address.</param>
    /// <param name="roleId">The role identifier.</param>
    /// <returns>
    /// An instance of KnownUser.
    /// </returns>
    KnownUsers GetKnownUserDetail(string emailAddress, int roleId);

    /// <summary>
    /// Adds the know users from application configuration.
    /// </summary>
    /// <param name="knownUsers">The known users.</param>
    void AddKnowUsersFromAppConfig(string knownUsers);

    /// <summary>
    /// Saves all known users.
    /// </summary>
    /// <returns>The number of modified records.</returns>
    public int SaveAllKnownUsers(IEnumerable<KnownUsers> knownUsers);
}