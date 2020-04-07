using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    public interface IOffersRepository : IDisposable //, IBaseRepository<Offers>
    {
        Guid Add(Offers Offers);
        IEnumerable<Offers> Get();
        Offers GetOfferDetailByOfferId(Guid offerGuId);

        IEnumerable<Offers> GetOffersByUser(int userId);

        Offers GetOfferDetailById(int Id);
    }
}
