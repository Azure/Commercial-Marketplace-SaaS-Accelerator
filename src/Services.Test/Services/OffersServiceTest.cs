using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.DataAccess.Services;
using Marketplace.SaaS.Accelerator.Services.Models;
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
        var offerEntity = CreateTestOfferEntity();

        var offerRepoMock = new Mock<IOffersRepository>();
        offerRepoMock
            .Setup(x => x.GetOfferById(It.IsAny<Guid>()))
            .Returns(offerEntity);

        offerService = new OfferService(offerRepoMock.Object);
    }

    [TestMethod]
    public void CanGetOfferById()
    {
        var offerModel = offerService.GetOfferById(Guid.NewGuid());
        
        Assert.IsNotNull(offerModel);
        Assert.AreEqual(testOfferEntityCount, offerModel.Id);
    }

    private Offer CreateTestOfferEntity()
    {
        var offerId = Guid.NewGuid();
        
        ++testOfferEntityCount;
        
        return new Offer()
        {
            OfferGuid = offerId,
            OfferId = offerId.ToString(),
            OfferName = "OfferName",
            CreateDate = DateTime.Now,
            Id = testOfferEntityCount,
            UserId = testOfferEntityCount,

        };
    }
}