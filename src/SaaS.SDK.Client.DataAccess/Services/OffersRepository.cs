namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class OffersRepository : IOffersRepository
    {
        private readonly SaasKitContext Context;

        public OffersRepository(SaasKitContext context)
        {
            Context = context;
        }

        /// <summary>
        /// The disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Offers> Get()
        {
            return Context.Offers;
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Offers Get(int id)
        {
            return Context.Offers.Where(s => s.Id == id).FirstOrDefault();
        }


        /// <summary>
        /// Adds the specified Offer details.
        /// </summary>
        /// <param name="offerDetails">The Offers details.</param>
        /// <returns></returns>
        public Guid Add(Offers offerDetails)
        {
            if (offerDetails != null)
            {
                var existingOffer = Context.Offers.Where(s => s.OfferId == offerDetails.OfferId).FirstOrDefault();
                if (existingOffer != null)
                {
                    existingOffer.OfferId = offerDetails.OfferId;
                    existingOffer.OfferName = offerDetails.OfferName;
                    Context.Offers.Update(existingOffer);
                    Context.SaveChanges();
                    return existingOffer.OfferGuid;
                }
                else
                {
                    Context.Offers.Add(offerDetails);
                    Context.SaveChanges();
                    return offerDetails.OfferGuid;
                }
            }
            return default;
        }

        /// <summary>
        /// Gets the plan detail by plan identifier.
        /// </summary>
        /// <param name="planId">The plan identifier.</param>
        /// <returns></returns>
        public Offers GetOfferDetailByOfferId(Guid offerGuId)
        {
            return Context.Offers.Where(s => s.OfferGuid == offerGuId).FirstOrDefault();
        }

        public IEnumerable<Offers> GetOffersByUser(int userId)
        {
            var getAllOffers = this.Context.Offers.Where(s => s.UserId == userId);
            return getAllOffers;
        }

        /// <summary>
        /// Gets the plan detail by plan identifier.
        /// </summary>
        /// <param name="planId">The plan identifier.</param>
        /// <returns></returns>
        public Offers GetOfferDetailById(int Id)
        {
            return Context.Offers.Where(s => s.Id == Id).FirstOrDefault();
        }

        /// <summary>
        /// Removes the specified plan details.
        /// </summary>
        /// <param name="offerDetails">The offer details.</param>
        public void Remove(Offers offerDetails)
        {
            var existingOffers = Context.Offers.Where(s => s.Id == offerDetails.Id).FirstOrDefault();
            if (existingOffers != null)
            {
                Context.Offers.Remove(existingOffers);
                Context.SaveChanges();
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
                    Context.Dispose();
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
