using System;
using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Contracts;

/// <summary>
/// Repository to access Offers.
/// </summary>
/// <seealso cref="System.IDisposable" />
public interface IOffersRepository : IDisposable
{
    /// <summary>
    /// Adds the specified offer.
    /// </summary>
    /// <param name="offers">The offer to be added.</param>
    /// <returns>Id of the newly added offer.</returns>
    Guid Add(Offers offers);

    /// <summary>
    /// Gets all offers.
    /// </summary>
    /// <returns>List of offers.</returns>
    IEnumerable<Offers> GetAll();

    /// <summary>
    /// Gets the offer by identifier.
    /// </summary>
    /// <param name="offerId">The offer identifier.</param>
    /// <returns>The offer for the given identifier.</returns>
    Offers GetOfferById(Guid offerId);

    /// <summary>
    /// Gets the offers by user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>List of offers by user.</returns>
    IEnumerable<Offers> GetOffersByUser(int userId);

    /// <summary>
    /// Gets the offer by internal identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>Offer for the given identifier ( internal id ).</returns>
    Offers GetOfferByInternalId(int id);
}