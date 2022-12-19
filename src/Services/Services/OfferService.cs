using System;
using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
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

    /// <summary>
    /// Initializes a new instance of the <see cref="OfferService"/> class.
    /// </summary>
    /// <param name="saasKitContext">DbContext for EF</param>
    public OfferService(SaasKitContext saasKitContext) : this(new OffersRepository(saasKitContext)) {}

    /// <summary>
    /// Initializes a new instance of the <see cref="OfferService"/> class.
    /// </summary>
    /// <param name="offerRepo">The offer repo.</param>
    public OfferService(IOffersRepository offerRepo)
    {
        this.offerRepository = offerRepo;
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
}