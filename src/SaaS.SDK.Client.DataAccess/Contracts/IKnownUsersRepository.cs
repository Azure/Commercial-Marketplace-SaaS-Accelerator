namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    using System;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

    /// <summary>
    /// Known User Repository.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.IBaseRepository{Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities.KnownUsers}" />
    public interface IKnownUsersRepository : IDisposable, IBaseRepository<KnownUsers>
    {
        /// <summary>
        /// Gets the known user detail.
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="roleId">The role identifier.</param>
        /// <returns>
        /// An instance of KnownUser.
        /// </returns>
        KnownUsers GetKnownUserDetail(string emailAddress, int roleId);
    }
}
