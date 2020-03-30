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
            var allOfferData = this.offersRepository.GetOffers();
            foreach (var item in allOfferData)
            {
                OffersModel Offers = new OffersModel();
                Offers.offerID = item.OfferId;
                Offers.offerName = item.OfferName;
                Offers.CreateDate = item.CreateDate;
                Offers.UserID = item.UserId;
                offersList.Add(Offers);
            }
            return offersList;
        }

    }
}
