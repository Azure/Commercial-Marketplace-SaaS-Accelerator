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
            

        offerService = new OfferService(mockOfferRepo.Object, mockAttributesRepo.Object);
    }

    [TestMethod]
    public void CanGetOfferAttributesByOfferId()
    {
        var attributes = offerService.GetOfferAttributesById(Guid.NewGuid());
        Assert.IsNotNull(attributes);
    }

    [TestMethod]
    public void CanAddAttributes()
    {
        var attribute = CreateOfferAttribute(1);

        offerService.AddOfferAttributes(attribute);
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