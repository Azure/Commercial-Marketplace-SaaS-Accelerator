using System;
using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.Services.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Marketplace.SaaS.Accelerator.Services.Test.Services;

[TestClass]
public class OffersServiceTest
{
    private OfferService offerService = null;
    private int testOfferEntityCount = 1;
    
    [TestInitialize]
    public void Initialize()
    {
        var offerRepoMock = new Mock<IOffersRepository>();
            offerRepoMock
                .Setup(x => x.GetOfferById(It.IsAny<Guid>()))
                .Returns(CreateTestOfferEntity());

            offerRepoMock
                .Setup(x => x.GetAll())
                .Returns(CreateTestOfferEntities());

        offerService = new OfferService(offerRepoMock.Object);
    }

    [TestMethod]
    public void CanGetAllOffers()
    {
        var offers = offerService.GetAllOffers();

        Assert.IsNotNull(offers);
        Assert.IsTrue(2 < offers.Count);
    }

    [TestMethod]
    public void CanGetOfferById()
    {
        var offerModel = offerService.GetOfferById(Guid.NewGuid());
        
        Assert.IsNotNull(offerModel);
    }

    private Offer CreateTestOfferEntity()
    {
        var offerId = Guid.NewGuid();
        
        return new Offer()
        {
            OfferGuid = offerId,
            OfferId = offerId.ToString(),
            OfferName = "OfferName",
            CreateDate = DateTime.Now,
            Id = testOfferEntityCount,
            UserId = testOfferEntityCount++
        };
    }

    private IEnumerable<Offer> CreateTestOfferEntities()
    {
        List<Offer> offers = new List<Offer>
        {
            CreateTestOfferEntity(),
            CreateTestOfferEntity(),
            CreateTestOfferEntity()
        };

        return offers;
    }
}