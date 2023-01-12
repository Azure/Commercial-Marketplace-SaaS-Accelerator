using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Services;
using Marketplace.SaaS.Accelerator.Services.Models;

namespace Marketplace.SaaS.Accelerator.Services.Services;

/// <summary>
/// Service to enable operations over offers.
/// </summary>
public class OffersService
{
    /// <summary>
    /// The offer repository.
    /// </summary>
    private readonly IOffersRepository offerRepository;

    private readonly IOfferAttributesRepository offerAttributesRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="OffersService"/> class.
    /// This constructor is for testing purposes only and should not be used for production.
    /// </summary>
    /// <param name="offerRepo">The offer repo.</param>
    /// <param name="offerAttributesRepository"></param>
    public OffersService(IOffersRepository offerRepo, IOfferAttributesRepository offerAttributesRepository)
    {
        this.offerRepository = offerRepo;
        this.offerAttributesRepository = offerAttributesRepository;
    }

    /// <summary>
    /// Gets the offers.
    /// </summary>
    /// <returns> OffersModel.</returns>
    public List<OffersModel> GetOffers()
    {
        var offers = this.offerRepository.GetAll();

        return offers.Select(item => new OffersModel()
            {
                Id = item.Id,
                OfferID = item.OfferId,
                OfferName = item.OfferName,
                CreateDate = item.CreateDate,
                UserID = item.UserId,
                OfferGuId = item.OfferGuid
            })
            .ToList();
    }

    /// <summary>
    /// Gets the offer on identifier.
    /// </summary>
    /// <param name="offerGuId">The offer gu identifier.</param>
    /// <returns> Offers View Model.</returns>
    public OfferModel GetOfferById(Guid offerGuId)
    {
        var offer = this.offerRepository.GetOfferById(offerGuId);
        
        var offersViewModel = new OfferModel()
        {
            Id = offer.Id,
            OfferID = offer.OfferId,
            OfferName = offer.OfferName,
            OfferGuid = offer.OfferGuid,
        };

        return offersViewModel;
    }
}