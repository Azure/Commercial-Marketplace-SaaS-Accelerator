namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class OfferAttributesRepository : IOfferAttributesRepository
    {
        private readonly SaasKitContext Context;

        public OfferAttributesRepository(SaasKitContext context)
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
        public int? Add(OfferAttributes offerAttributes)
        {
            if (offerAttributes != null)
            {
                var existingOfferAttribute = Context.OfferAttributes.Where(s => s.Id ==
                offerAttributes.Id).FirstOrDefault();
                if (existingOfferAttribute != null)
                {
                    existingOfferAttribute.ParameterId = offerAttributes.ParameterId;
                    existingOfferAttribute.DisplayName = offerAttributes.DisplayName;
                    existingOfferAttribute.Description = offerAttributes.Description;
                    existingOfferAttribute.ValueTypeId = offerAttributes.ValueTypeId;
                    existingOfferAttribute.FromList = offerAttributes.FromList;
                    existingOfferAttribute.ValuesList = offerAttributes.ValuesList;
                    existingOfferAttribute.Max = offerAttributes.Max;
                    existingOfferAttribute.Min = offerAttributes.Min;
                    existingOfferAttribute.Type = offerAttributes.Type;
                    existingOfferAttribute.DisplaySequence = offerAttributes.DisplaySequence;
                    existingOfferAttribute.Isactive = offerAttributes.Isactive;
                    existingOfferAttribute.UserId = offerAttributes.UserId;
                    existingOfferAttribute.OfferId = offerAttributes.OfferId;

                    Context.OfferAttributes.Update(existingOfferAttribute);
                    Context.SaveChanges();
                    return existingOfferAttribute.Id;
                }
                else
                {
                    Context.OfferAttributes.Add(offerAttributes);
                    Context.SaveChanges();
                    return offerAttributes.Id;
                }
            }

            return null;
        }

        public IEnumerable<OfferAttributes> Get()
        {
            return Context.OfferAttributes;
        }
        public IEnumerable<OfferAttributes> GetOfferAttributeDetailByOfferId(Guid offerGuId)
        {
            return Context.OfferAttributes.Where(s => s.OfferId == offerGuId);
        }

        //IEnumerable<Offers> GetOffersByUser(int userId);

        public OfferAttributes Get(int Id)
        {
            return Context.OfferAttributes.Where(s => s.Id == Id).FirstOrDefault();
        }

        /// <summary>
        /// Removes the specified plan details.
        /// </summary>
        /// <param name="offerDetails">The offer details.</param>
        public void Remove(List<OfferAttributes> offerAttributes)
        {
            foreach (var offerAttribute in offerAttributes)
            {
                var existingOffersAttribute = Context.OfferAttributes.Where(s => s.Id == offerAttribute.Id).FirstOrDefault();
                if (existingOffersAttribute != null)
                {
                    Context.OfferAttributes.Remove(existingOffersAttribute);
                    Context.SaveChanges();
                }
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
