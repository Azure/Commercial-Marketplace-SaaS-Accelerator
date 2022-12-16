using System;
using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Contracts;

/// <summary>
/// Repository to access Offer.
/// </summary>
/// <seealso cref="System.IDisposable" />
public interface IOffersRepository : IDisposable
{
    /// <summary>
    /// Adds the specified offer.
    /// </summary>
    /// <param name="offer">The offer to be added.</param>
    /// <returns>Id of the newly added offer.</returns>
    Guid Add(Offer offer);

    /// <summary>
    /// Gets all offer.
    /// </summary>
    /// <returns>List of offer.</returns>
    IEnumerable<Offer> GetAll();

    /// <summary>
    /// Gets the offer by identifier.
    /// </summary>
    /// <param name="offerId">The offer identifier.</param>
    /// <returns>The offer for the given identifier.</returns>
    Offer GetOfferById(Guid offerId);

    /// <summary>
    /// Gets the offer by user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>List of offer by user.</returns>
    IEnumerable<Offer> GetOffersByUser(int userId);

    /// <summary>
    /// Gets the offer by internal identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>Offer for the given identifier ( internal id ).</returns>
    Offer GetOfferByInternalId(int id);
}