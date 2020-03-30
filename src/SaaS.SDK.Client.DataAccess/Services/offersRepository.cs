namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class offersRepository: IOffersRepository
    {
        private readonly SaasKitContext Context;

        public offersRepository(SaasKitContext context)
        {
            Context = context;
        }

        public IEnumerable<Offers> GetOffers()
        {
            var getAllOffers = this.Context.Offers;
            return getAllOffers;
        }
    }
}
