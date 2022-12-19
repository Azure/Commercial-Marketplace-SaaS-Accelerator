using System;
using System.Collections.Generic;
using System.Linq;
using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Services;

/// <summary>
/// Repository to access offer.
/// </summary>
/// <seealso cref="IOffersRepository" />
public class OffersRepository : IOffersRepository
{
    /// <summary>
    /// The dbContext.
    /// </summary>
    private readonly SaasKitContext dbContext;

    /// <summary>
    /// The disposed.
    /// </summary>
    private bool disposed = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="OffersRepository"/> class.
    /// </summary>
    /// <param name="dbContext">dbContext from EF</param>
    public OffersRepository(SaasKitContext dbContext)
    {
        this.dbContext = dbContext;
    }

    /// <summary>
    /// Gets all offer.
    /// </summary>
    /// <returns>
    /// List of offer.
    /// </returns>
    public IEnumerable<Offer> GetAll()
    {
        return this.dbContext.Offers;
    }

    /// <summary>
    /// Gets the specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns> Offer.</returns>
    public Offer Get(int id)
    {
        return this.dbContext.Offers.FirstOrDefault(s => s.Id == id);
    }

    /// <summary>
    /// Adds the specified offer details.
    /// </summary>
    /// <param name="offerDetails">The offer details.</param>
    /// <returns> Offer Id.</returns>
    public Guid Add(Offer offerDetails)
    {
        if (offerDetails != null)
        {
            var existingOffer = this.dbContext.Offers.FirstOrDefault(s => s.OfferId == offerDetails.OfferId);

            if (existingOffer != null)
            {
                existingOffer.OfferId = offerDetails.OfferId;
                existingOffer.OfferName = offerDetails.OfferName;
                this.dbContext.Offers.Update(existingOffer);
                this.dbContext.SaveChanges();
                return existingOffer.OfferGuid;
            }
            else
            {
                this.dbContext.Offers.Add(offerDetails);
                this.dbContext.SaveChanges();
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
    public Offer GetOfferById(Guid offerGuId)
    {
        return this.dbContext.Offers.FirstOrDefault(s => s.OfferGuid == offerGuId);
    }

    /// <summary>
    /// Gets the offer by user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>
    /// List of offer by user.
    /// </returns>
    public IEnumerable<Offer> GetOffersByUser(int userId)
    {
        var getAllOffers = this.dbContext.Offers.Where(s => s.UserId == userId);
        return getAllOffers;
    }

    /// <summary>
    /// Gets the plan detail by plan identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>
    /// Offer model.
    /// </returns>
    public Offer GetOfferByInternalId(int id)
    {
        return this.dbContext.Offers.FirstOrDefault(s => s.Id == id);
    }

    /// <summary>
    /// Removes the specified plan details.
    /// </summary>
    /// <param name="offerDetails">The offer details.</param>
    public void Remove(Offer offerDetails)
    {
        var existingOffers = this.dbContext.Offers.FirstOrDefault(s => s.Id == offerDetails.Id);
        if (existingOffers != null)
        {
            this.dbContext.Offers.Remove(existingOffers);
            this.dbContext.SaveChanges();
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
                this.dbContext.Dispose();
            }
        }

        this.disposed = true;
    }
}