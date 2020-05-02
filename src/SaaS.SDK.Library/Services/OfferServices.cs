//using Microsoft.Marketplace.SaaS.SDK.CustomerProvisioning.Models;
using Microsoft.Marketplace.SaaS.SDK.Services.Models;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Marketplace.SaaS.SDK.Services.Services
{
    public class OfferServices
    {
        /// <summary>
        /// The plan repository
        /// </summary>
        public IOffersRepository OfferRepository;

        public OfferServices(IOffersRepository offerRepo)
        {
            OfferRepository = offerRepo;
        }

        public List<OffersModel> GetOffers()
        {
            List<OffersModel> offersList = new List<OffersModel>();
            var allOfferData = this.OfferRepository.Get();
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

        public OffersViewModel GetOfferOnId(Guid offerGuId)
        {
            var offer = this.OfferRepository.GetOfferDetailByOfferId(offerGuId);
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
