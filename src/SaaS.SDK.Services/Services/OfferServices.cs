namespace Microsoft.Marketplace.SaaS.SDK.Services.Services
{
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Service to enable operations over offers
    /// </summary>
    public class OfferServices
    {
        /// <summary>
        /// The offer repository
        /// </summary>
        public IOffersRepository offerRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferServices"/> class.
        /// </summary>
        /// <param name="offerRepo">The offer repo.</param>
        public OfferServices(IOffersRepository offerRepo)
        {
            offerRepository = offerRepo;
        }

        /// <summary>
        /// Gets the offers.
        /// </summary>
        /// <returns></returns>
        public List<OffersModel> GetOffers()
        {
            List<OffersModel> offersList = new List<OffersModel>();
            var allOfferData = this.offerRepository.GetAll();
            foreach (var item in allOfferData)
            {
                OffersModel Offers = new OffersModel();
                Offers.Id = item.Id;
                Offers.offerID = item.OfferId;
                Offers.offerName = item.OfferName;
                Offers.CreateDate = item.CreateDate;
                Offers.UserID = item.UserId;
                Offers.offerGuId = item.OfferGuid;
                offersList.Add(Offers);
            }
            return offersList;
        }

        /// <summary>
        /// Gets the offer on identifier.
        /// </summary>
        /// <param name="offerGuId">The offer gu identifier.</param>
        /// <returns></returns>
        public OffersViewModel GetOfferOnId(Guid offerGuId)
        {
            var offer = this.offerRepository.GetOfferById(offerGuId);
            OffersViewModel offerModel = new OffersViewModel()
            {
                Id = offer.Id,
                OfferID = offer.OfferId,
                OfferName = offer.OfferName,
                OfferGuid = offer.OfferGuid
            };
            return offerModel;
        }
    }
}