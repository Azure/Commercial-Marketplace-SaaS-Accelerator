using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.Services.Models;
using Marketplace.SaaS.Accelerator.Services.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Marketplace.SaaS.Accelerator.Services.Test;

[TestClass]
public class OfferServicesTest
{
    private OffersService offersService;
    private int testOfferModelCount = 0;

    [TestInitialize]
    public void Initialize()
    {
        var mockOfferRepo = new Mock<IOffersRepository>();
        mockOfferRepo
            .Setup(x => x.GetOfferById(It.IsAny<Guid>()))
            .Returns(CreateTestOfferEntity());

        mockOfferRepo
            .Setup(x => x.GetAll())
            .Returns(CreateTestOfferEntities());

        offersService = new OffersService(mockOfferRepo.Object);
    }

    [TestMethod]
    public void CanGetAllOffers()
    {
        var offerEntites = offersService.GetOffers();

        Assert.IsNotNull(offerEntites);
        Assert.AreEqual(3, offerEntites.Count);
    }

    [TestMethod]
    public void CanGetAnOfferById()
    {
        var offerEntity = offersService.GetOfferOnId(Guid.NewGuid());
        
        Assert.IsNotNull(offerEntity);
    }

    private Offers CreateTestOfferEntity()
    {
        var offerId = Guid.NewGuid();

        return new Offers()
        {
            CreateDate = DateTime.Now,
            Id = testOfferModelCount,
            OfferName = "OfferName",
            OfferGuid = offerId,
            OfferId = offerId.ToString(),
            UserId = 1
        };
    }

    private IEnumerable<Offers> CreateTestOfferEntities()
    {
        List<Offers> offers = new List<Offers>
        {
            CreateTestOfferEntity(),
            CreateTestOfferEntity(),
            CreateTestOfferEntity()
        };

        return offers;
    }

}