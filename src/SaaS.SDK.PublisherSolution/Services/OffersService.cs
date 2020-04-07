using Microsoft.Marketplace.Saas.Web.Models;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Marketplace.SaasKit.Client.Services
{
    public class OffersService
    {
        public IOffersRepository offersRepository;


        public OffersService(IOffersRepository offersRepository)
        {
            this.offersRepository = offersRepository;
        }

        public List<OffersModel> GetOffers()
        {
            List<OffersModel> offersList = new List<OffersModel>();
            var allOfferData = this.offersRepository.Get();
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
            var offer = this.offersRepository.GetOfferDetailByOfferId(offerGuId);
            OffersViewModel offerModel = new OffersViewModel()
            {
                Id = offer.Id,
                OfferID = offer.OfferId,
                OfferName = offer.OfferName,
                OfferGuid = offer.OfferGuid
            };
            return offerModel;
        }

        //public OfferAttributesModel GetOfferattributesOnId(Guid offerGuId)
        //{
        //    var offer = this..GetOfferDetailByOfferId(offerGuId);
        //    OfferAttributesModel offerModel = new OfferAttributesModel()
        //    {
        //        Id = offer.Id,
        //        OfferID = offer.OfferId,
        //        OfferName = offer.OfferName,
        //        OfferGuid = offer.OfferGuid
        //    };
        //    return offerModel;
        //}

    }
}
