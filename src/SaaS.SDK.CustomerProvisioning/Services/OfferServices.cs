using Microsoft.Marketplace.SaaS.SDK.CustomerProvisioning.Models;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Marketplace.SaaS.SDK.CustomerProvisioning.Services
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

        //public int AddUpdatePartnerOffer(OffersViewModel offersViewModel)
        //{

        //    return OfferRepository.Add(offersViewModel);

        //}
    }
}
