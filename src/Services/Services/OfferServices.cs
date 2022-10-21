namespace Microsoft.Marketplace.SaaS.SDK.Services.Services
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;

    /// <summary>
    /// Service to enable operations over offers.
    /// </summary>
    public class OfferServices
    {
        /// <summary>
        /// The offer repository.
        /// </summary>
        private IOffersRepository offerRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferServices"/> class.
        /// </summary>
        /// <param name="offerRepo">The offer repo.</param>
        public OfferServices(IOffersRepository offerRepo)
        {
            this.offerRepository = offerRepo;
        }

        /// <summary>
        /// Gets the offers.
        /// </summary>
        /// <returns> Offers Model.</returns>
        public List<OffersModel> GetOffers()
        {
            List<OffersModel> offersList = new List<OffersModel>();
            var allOfferData = this.offerRepository.GetAll();
            foreach (var item in allOfferData)
            {
                OffersModel offers = new OffersModel();
                offers.Id = item.Id;
                offers.OfferID = item.OfferId;
                offers.OfferName = item.OfferName;
                offers.CreateDate = item.CreateDate;
                offers.UserID = item.UserId;
                offers.OfferGuId = item.OfferGuid;
                offersList.Add(offers);
            }

            return offersList;
        }

        /// <summary>
        /// Gets the offer on identifier.
        /// </summary>
        /// <param name="offerGuId">The offer gu identifier.</param>
        /// <returns> Offers View Model.</returns>
        public OffersViewModel GetOfferOnId(Guid offerGuId)
        {
            var offer = this.offerRepository.GetOfferById(offerGuId);
            OffersViewModel offerModel = new OffersViewModel()
            {
                Id = offer.Id,
                OfferID = offer.OfferId,
                OfferName = offer.OfferName,
                OfferGuid = offer.OfferGuid,
            };
            return offerModel;
        }
    }
}