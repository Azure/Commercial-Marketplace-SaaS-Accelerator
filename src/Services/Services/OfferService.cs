using System;
using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.DataAccess.Services;
using Marketplace.SaaS.Accelerator.Services.Models;

namespace Marketplace.SaaS.Accelerator.Services.Services;

/// <summary>
/// Service to enable operations over offers.
/// </summary>
public class OfferService
{
    /// <summary>
    /// The offer repository.
    /// </summary>
    private readonly IOffersRepository offerRepository;

    private readonly IOfferAttributesRepository offerAttributesRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="OfferService"/> class.
    /// </summary>
    /// <param name="saasKitContext">DbContext for EF</param>
    public OfferService(SaasKitContext saasKitContext) : this(new OffersRepository(saasKitContext), new OfferAttributesRepository(saasKitContext)) {}

    /// <summary>
    /// Initializes a new instance of the <see cref="OfferService"/> class.
    /// This method provided for testing purposes. Use other constructor for production.
    /// </summary>
    /// <param name="offerRepo">The offer repo.</param>
    /// <param name="offerAttributesRepository">The offer attributes repo.</param>
    public OfferService(IOffersRepository offerRepo, IOfferAttributesRepository offerAttributesRepository)
    {
        this.offerRepository = offerRepo;
        this.offerAttributesRepository = offerAttributesRepository;
    }

    /// <summary>
    /// Gets the offers.
    /// </summary>
    /// <returns> A list of OfferModel.</returns>
    public List<OfferModel> GetAllOffers()
    {
        var offersList = new List<OfferModel>();
        var allOfferData = this.offerRepository.GetAll();

        foreach (var item in allOfferData)
        {
            var offerModel = new OfferModel()
            {
                Id = item.Id,
                OfferID = item.OfferId,
                OfferName = item.OfferName,
                CreateDate = item.CreateDate,
                UserID = item.UserId,
                OfferGuid = item.OfferGuid
            };
            
            offersList.Add(offerModel);
        }

        return offersList;
    }

    /// <summary>
    /// Gets the offer on identifier.
    /// </summary>
    /// <param name="offerGuId">The offer gu identifier.</param>
    /// <returns> Offer View Model.</returns>
    public OfferModel GetOfferById(Guid offerGuId)
    {
        var offer = this.offerRepository.GetOfferById(offerGuId);
        
        OfferModel offerModel = new OfferModel()
        {
            Id = offer.Id,
            OfferGuid = offerGuId,
            OfferID = offer.OfferId,
            OfferName = offer.OfferName,
            CreateDate = offer.CreateDate,
            UserID = offer.UserId
        };

        return offerModel;
    }

    public void AddOfferAttributes(OfferAttributes offerAttributes)
    {
        offerAttributesRepository.Add(offerAttributes);
    }

    public IList<OfferAttributesModel> GetOfferAttributesById(Guid id)
    {
        var offerAttributesModels = new List<OfferAttributesModel>();

        var listOfOfferAttributes = offerAttributesRepository.GetAllOfferAttributesByOfferId(id);

        foreach (var attributes in listOfOfferAttributes)
        {
            var attributesModel = MapOfferAttributesModel(attributes);

            offerAttributesModels.Add(attributesModel);
        }

        return offerAttributesModels;
    }

    private static OfferAttributesModel MapOfferAttributesModel(OfferAttributes attributes)
    {
        return new OfferAttributesModel()
        {
            CreateDate = attributes.CreateDate,
            Description = attributes.Description,
            DisplayName = attributes.DisplayName,
            DisplaySequence = attributes.DisplaySequence,
            FromList = attributes.FromList,
            Id = attributes.Id,
            IsActive = attributes.Isactive,
            IsDelete = attributes.IsDelete,
            IsRequired = attributes.IsRequired,
            Max = attributes.Max,
            Min = attributes.Min,
            OfferId = attributes.OfferId,
            ParameterId = attributes.ParameterId,
            Type = attributes.Type,
            UserId = attributes.UserId,
            ValuesList = attributes.ValuesList,
            ValueTypeId = attributes.ValueTypeId,
        };
    }
}