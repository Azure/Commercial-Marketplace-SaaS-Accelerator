namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    using System;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

    /// <summary>
    /// Repository to access users.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.IBaseRepository{Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities.Users}" />
    public interface IUsersRepository : IDisposable, IBaseRepository<Users>
    {
        /// <summary>
        /// Gets the partner detail from email.
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <returns> Users.</returns>
        Users GetPartnerDetailFromEmail(string emailAddress);
    }
}
