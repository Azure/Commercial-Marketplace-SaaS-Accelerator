using System;
using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Contracts;

/// <summary>
/// Repository to access Offer attributes.
/// </summary>
/// <seealso cref="System.IDisposable" />
public interface IOfferAttributesRepository : IDisposable
{
    /// <summary>
    /// Adds the specified offer attributes.
    /// </summary>
    /// <param name="offerAttributes">The offer attributes.</param>
    /// <returns>Identifier of the newly added offer attribute.</returns>
    int? Add(OfferAttributes offerAttributes);

    /// <summary>
    /// Gets all offer attributes across offers.
    /// </summary>
    /// <returns>List of offer attributes across offers.</returns>
    IEnumerable<OfferAttributes> GetAll();

    /// <summary>
    /// Gets the input type offer attributes by offer identifier.
    /// </summary>
    /// <param name="offerId">The offer identifier.</param>
    /// <returns>List of attributes of an offer.</returns>
    IEnumerable<OfferAttributes> GetInputAttributesByOfferId(Guid offerId);

    /// <summary>
    /// Gets the offer attribute by identifier.
    /// </summary>
    /// <param name="offerAttributeId">The identifier of the offer attribute.</param>
    /// <returns>Offer attribute.</returns>
    OfferAttributes GetById(int offerAttributeId);

    /// <summary>
    /// Gets all offer attributes by offer identifier ( includes deployment and input attributes).
    /// </summary>
    /// <param name="offerGuId">The offer identifier.</param>
    /// <returns>List of offer attributes.</returns>
    IEnumerable<OfferAttributes> GetAllOfferAttributesByOfferId(Guid offerGuId);
}