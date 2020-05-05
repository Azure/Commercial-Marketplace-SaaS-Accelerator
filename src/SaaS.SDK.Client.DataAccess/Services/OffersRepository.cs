namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Repository to access offers
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.IOffersRepository" />
    public class OffersRepository : IOffersRepository
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly SaasKitContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="OffersRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public OffersRepository(SaasKitContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// The disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Gets all offers
        /// </summary>
        /// <returns>
        /// List of offers
        /// </returns>
        public IEnumerable<Offers> GetAll()
        {
            return context.Offers;
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Offers Get(int id)
        {
            return context.Offers.Where(s => s.Id == id).FirstOrDefault();
        }

        /// <summary>
        /// Adds the specified offer details.
        /// </summary>
        /// <param name="offerDetails">The offer details.</param>
        /// <returns></returns>
        public Guid Add(Offers offerDetails)
        {
            if (offerDetails != null)
            {
                var existingOffer = context.Offers.Where(s => s.OfferId == offerDetails.OfferId).FirstOrDefault();
                if (existingOffer != null)
                {
                    existingOffer.OfferId = offerDetails.OfferId;
                    existingOffer.OfferName = offerDetails.OfferName;
                    context.Offers.Update(existingOffer);
                    context.SaveChanges();
                    return existingOffer.OfferGuid;
                }
                else
                {
                    context.Offers.Add(offerDetails);
                    context.SaveChanges();
                    return offerDetails.OfferGuid;
                }
            }
            return default;
        }

        /// <summary>
        /// Gets the offer by identifier.
        /// </summary>
        /// <param name="offerGuId">The offer identifier.</param>
        /// <returns>Offer by the given identifier</returns>
        public Offers GetOfferById(Guid offerGuId)
        {
            return context.Offers.Where(s => s.OfferGuid == offerGuId).FirstOrDefault();
        }

        /// <summary>
        /// Gets the offers by user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>
        /// List of offers by user
        /// </returns>
        public IEnumerable<Offers> GetOffersByUser(int userId)
        {
            var getAllOffers = this.context.Offers.Where(s => s.UserId == userId);
            return getAllOffers;
        }

        /// <summary>
        /// Gets the plan detail by plan identifier.
        /// </summary>
        /// <param name="planId">The plan identifier.</param>
        /// <returns></returns>
        public Offers GetOfferByInternalId(int Id)
        {
            return context.Offers.Where(s => s.Id == Id).FirstOrDefault();
        }

        /// <summary>
        /// Removes the specified plan details.
        /// </summary>
        /// <param name="offerDetails">The offer details.</param>
        public void Remove(Offers offerDetails)
        {
            var existingOffers = context.Offers.Where(s => s.Id == offerDetails.Id).FirstOrDefault();
            if (existingOffers != null)
            {
                context.Offers.Remove(existingOffers);
                context.SaveChanges();
            }
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
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
