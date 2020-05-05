namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Repository to access Offers
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IOffersRepository : IDisposable
    {
        /// <summary>
        /// Adds the specified offer
        /// </summary>
        /// <param name="Offers">The offer to be added.</param>
        /// <returns>Id of the newly added offer</returns>
        Guid Add(Offers Offers);

        /// <summary>
        /// Gets all offers
        /// </summary>
        /// <returns>List of offers</returns>
        IEnumerable<Offers> GetAll();

        /// <summary>
        /// Gets the offer by identifier.
        /// </summary>
        /// <param name="offerId">The offer identifier.</param>
        /// <returns>The offer for the given identifier</returns>
        Offers GetOfferById(Guid offerId);

        /// <summary>
        /// Gets the offers by user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>List of offers by user</returns>
        IEnumerable<Offers> GetOffersByUser(int userId);

        /// <summary>
        /// Gets the offer by internal identifier.
        /// </summary>
        /// <param name="Id">The identifier.</param>
        /// <returns>Offer for the given identifier ( internal id )</returns>
        Offers GetOfferByInternalId(int Id);
    }
}
