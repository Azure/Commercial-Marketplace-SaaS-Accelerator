using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System;
using System.Collections.Generic;


namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    public interface IOfferAttributesRepository : IDisposable
    {
        int? Add(OfferAttributes offerAttributes);
        IEnumerable<OfferAttributes> Get();
        IEnumerable<OfferAttributes> GetOfferAttributeDetailByOfferId(Guid offerGuId);

        //IEnumerable<Offers> GetOffersByUser(int userId);
        int? AddDeploymentAttributes(Guid offerId, int curretnUserId, List<DeploymentAttributes> deploymentAttributes);
        OfferAttributes Get(int Id);
        IEnumerable<DeploymentAttributes> GetDeploymentParameters();
        IEnumerable<OfferAttributes> GetAllOfferAttributeDetailByOfferId(Guid offerGuId);
    }
}
