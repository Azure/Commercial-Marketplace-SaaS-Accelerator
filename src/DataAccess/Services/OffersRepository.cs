using System;
using System.Collections.Generic;
using System.Linq;
using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Services;

/// <summary>
/// Repository to access offers.
/// </summary>
/// <seealso cref="IOffersRepository" />
public class OffersRepository : IOffersRepository
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
    /// Initializes a new instance of the <see cref="OffersRepository"/> class.
    /// </summary>
    /// <param name="context">The this.context.</param>
    public OffersRepository(SaasKitContext context)
    {
        this.context = context;
    }

    /// <summary>
    /// Gets all offers.
    /// </summary>
    /// <returns>
    /// List of offers.
    /// </returns>
    public IEnumerable<Offers> GetAll()
    {
        return this.context.Offers;
    }

    /// <summary>
    /// Gets the specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns> Offers.</returns>
    public Offers Get(int id)
    {
        return this.context.Offers.Where(s => s.Id == id).FirstOrDefault();
    }

    /// <summary>
    /// Adds the specified offer details.
    /// </summary>
    /// <param name="offerDetails">The offer details.</param>
    /// <returns> Offer Id.</returns>
    public Guid Add(Offers offerDetails)
    {
        if (offerDetails != null)
        {
            var existingOffer = this.context.Offers.Where(s => s.OfferId == offerDetails.OfferId).FirstOrDefault();
            if (existingOffer != null)
            {
                existingOffer.OfferId = offerDetails.OfferId;
                existingOffer.OfferName = offerDetails.OfferName;
                this.context.Offers.Update(existingOffer);
                this.context.SaveChanges();
                return existingOffer.OfferGuid;
            }
            else
            {
                this.context.Offers.Add(offerDetails);
                this.context.SaveChanges();
                return offerDetails.OfferGuid;
            }
        }

        return default;
    }

    /// <summary>
    /// Gets the offer by identifier.
    /// </summary>
    /// <param name="offerGuId">The offer identifier.</param>
    /// <returns>Offer by the given identifier.</returns>
    public Offers GetOfferById(Guid offerGuId)
    {
        return this.context.Offers.Where(s => s.OfferGuid == offerGuId).FirstOrDefault();
    }

    /// <summary>
    /// Gets the offers by user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>
    /// List of offers by user.
    /// </returns>
    public IEnumerable<Offers> GetOffersByUser(int userId)
    {
        var getAllOffers = this.context.Offers.Where(s => s.UserId == userId);
        return getAllOffers;
    }

    /// <summary>
    /// Gets the plan detail by plan identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>
    /// Offer model.
    /// </returns>
    public Offers GetOfferByInternalId(int id)
    {
        return this.context.Offers.Where(s => s.Id == id).FirstOrDefault();
    }

    /// <summary>
    /// Removes the specified plan details.
    /// </summary>
    /// <param name="offerDetails">The offer details.</param>
    public void Remove(Offers offerDetails)
    {
        var existingOffers = this.context.Offers.Where(s => s.Id == offerDetails.Id).FirstOrDefault();
        if (existingOffers != null)
        {
            this.context.Offers.Remove(existingOffers);
            this.context.SaveChanges();
        }
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