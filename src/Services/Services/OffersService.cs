using System;
using System.Collections.Generic;
using System.Linq;
using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Services;
using Marketplace.SaaS.Accelerator.Services.Models;

namespace Marketplace.SaaS.Accelerator.Services.Services;

/// <summary>
/// Service to enable operations over offers.
/// </summary>
public class OfferServices
{
    /// <summary>
    /// The offer repository.
    /// </summary>
    private readonly IOffersRepository offerRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="OfferServices"/> class.
    /// </summary>
    /// <param name="saasKitContext"></param>
    public OfferServices(SaasKitContext saasKitContext) : this(new OffersRepository(saasKitContext)) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="OfferServices"/> class.
    /// This constructor is for testing purposes only and should not be used for production.
    /// </summary>
    /// <param name="offerRepo">The offer repo.</param>
    public OfferServices(IOffersRepository offerRepo)
    {
        this.offerRepository = offerRepo;
    }

    /// <summary>
    /// Gets the offers.
    /// </summary>
    /// <returns> Offers Model.</returns>
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
    public OffersViewModel GetOfferOnId(Guid offerGuId)
    {
        var offer = this.offerRepository.GetOfferById(offerGuId);
        
        var offersViewModel = new OffersViewModel()
        {
            Id = offer.Id,
            OfferID = offer.OfferId,
            OfferName = offer.OfferName,
            OfferGuid = offer.OfferGuid,
        };
        return offersViewModel;
    }
}