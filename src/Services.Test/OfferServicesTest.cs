using System;
using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
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

        var mockAttributesRepo = new Mock<IOfferAttributesRepository>();
        mockAttributesRepo.Setup(x => x.GetAllOfferAttributesByOfferId(It.IsAny<Guid>()))
            .Returns(CreateOfferAttributes());
        mockAttributesRepo.Setup(x => x.Add(It.IsAny<OfferAttributes>()))
            .Returns(1)
            .Verifiable();

        offersService = new OffersService(mockOfferRepo.Object, mockAttributesRepo.Object);
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
        var offerEntity = offersService.GetOfferById(Guid.NewGuid());
        
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

    private IEnumerable<OfferAttributes> CreateOfferAttributes()
    {
        var offerAttributes = new List<OfferAttributes>();

        int idCounter = 1;

        for (int i = 0; i < 3; i++)
        {
            var offerAttribute = CreateOfferAttribute(idCounter);

            offerAttributes.Add(offerAttribute);
        }

        return offerAttributes;
    }

    private OfferAttributes CreateOfferAttribute(int idCounter)
    {
        return new OfferAttributes()
        {
            CreateDate = DateTime.Now,
            Description = "Description",
            DisplayName = "DisplayName",
            DisplaySequence = idCounter,
            FromList = false,
            Id = idCounter,
            OfferId = Guid.NewGuid(),
            UserId = 1,
            IsDelete = false,
            Isactive = true,
            IsRequired = false,
            Max = 1,
            Min = 1,
            ParameterId = "ParameterId",
            Type = "Type",
            ValueTypeId = 1,
            ValuesList = "ValuesList"
        };
    }
}