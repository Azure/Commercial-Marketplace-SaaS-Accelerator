using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Marketplace.SaaS.Accelerator.AdminSite.Models;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.Services.Models;
using Marketplace.SaaS.Accelerator.Services.Services;
using Marketplace.SaaS.Accelerator.Services.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace Marketplace.SaaS.Accelerator.AdminSite.Controllers;

/// <summary>
/// Offer Controller.
/// </summary>
/// <seealso cref="BaseController" />
[ServiceFilter(typeof(KnownUserAttribute))]
public class OffersController : BaseController
{
    private readonly IUsersRepository usersRepository;

    private readonly IValueTypesRepository valueTypesRepository;

    private readonly IApplicationConfigRepository applicationConfigRepository;

    private readonly IOffersRepository offersRepository;

    private readonly IOfferAttributesRepository offersAttributeRepository;

    private readonly ILogger<OffersController> logger;

    private OfferService offersService;

    /// <summary>
    /// Initializes a new instance of the <see cref="OffersController"/> class.
    /// </summary>
    /// <param name="offersRepository">The offers repository.</param>
    /// <param name="applicationConfigRepository">The application configuration repository.</param>
    /// <param name="usersRepository">The users repository.</param>
    /// <param name="valueTypesRepository">The value types repository.</param>
    /// <param name="offersAttributeRepository">The offers attribute repository.</param>
    /// <param name="logger">The logger.</param>
    public OffersController(
        IOffersRepository offersRepository, 
        IApplicationConfigRepository applicationConfigRepository, 
        IUsersRepository usersRepository, 
        IValueTypesRepository valueTypesRepository, 
        IOfferAttributesRepository offersAttributeRepository, 
        ILogger<OffersController> logger)
    {
        this.offersRepository = offersRepository;
        this.applicationConfigRepository = applicationConfigRepository;
        this.usersRepository = usersRepository;
        this.valueTypesRepository = valueTypesRepository;
        this.offersService = new OfferService(this.offersRepository);
        this.offersAttributeRepository = offersAttributeRepository;
        this.logger = logger;
    }

    /// <summary>
    /// Indexes this instance.
    /// </summary>
    /// <returns>return All subscription.</returns>
    public IActionResult Index()
    {
        this.logger.LogInformation("Offer Controller / Index");
        try
        {
            List<OfferModel> getAllOffersData = new List<OfferModel>();
            this.TempData["ShowWelcomeScreen"] = "True";
            var currentUserDetail = this.usersRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);

            getAllOffersData = this.offersService.GetAllOffers();

            return this.View(getAllOffersData);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, ex.Message);
            return this.View("Error", ex);
        }
    }

    /// <summary>
    /// Indexes this instance.
    /// </summary>
    /// <param name="offerGuid">The offer gu identifier.</param>
    /// <returns>
    /// return All subscription.
    /// </returns>
    public IActionResult OfferDetails(Guid offerGuid)
    {
        this.logger.LogInformation("Offers Controller / OfferDetails:  offerGuid {0}", offerGuid);

        try
        {
            var offerModel = this.offersService.GetOfferById(offerGuid);

            var offerAttributesViewModels = new List<OfferAttributesViewModel>();
            var offerAttributeEntities = this.offersAttributeRepository.GetInputAttributesByOfferId(offerGuid);
            var currentUserDetail = this.usersRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);
            
            foreach (var offerAttributeEntity in offerAttributeEntities)
            {
                offerAttributesViewModels.Add(new OfferAttributesViewModel()
                {
                    AttributeID = offerAttributeEntity.Id,
                    ParameterId = offerAttributeEntity.ParameterId,
                    DisplayName = offerAttributeEntity.DisplayName,
                    Description = offerAttributeEntity.Description,
                    ValueTypeId = offerAttributeEntity.ValueTypeId,
                    FromList = offerAttributeEntity.FromList,
                    ValuesList = offerAttributeEntity.ValuesList,
                    Max = offerAttributeEntity.Max,
                    Min = offerAttributeEntity.Min,
                    Type = offerAttributeEntity.Type,
                    DisplaySequence = offerAttributeEntity.DisplaySequence,
                    Isactive = offerAttributeEntity.Isactive,
                    IsRequired = offerAttributeEntity.IsRequired ?? false,
                    IsDelete = offerAttributeEntity.IsDelete ?? false,
                    CreateDate = DateTime.Now,
                    UserId = currentUserDetail?.UserId ?? 0,
                    OfferId = offerAttributeEntity.OfferId
                });
            }

            var offerDetailsViewModel = new OfferDetailsViewModel()
            {
                OfferID = offerModel.OfferID,
                Id = offerModel.Id,
                OfferGuid = offerModel.OfferGuid.GetValueOrDefault(),
                OfferName = offerModel.OfferName,
                OfferAttributes = offerAttributesViewModels
            };
            
            this.TempData["ShowWelcomeScreen"] = "True";

            var valueTypes = this.valueTypesRepository.GetAll().ToList();

            this.ViewBag.ValueTypes = new SelectList(valueTypes, "ValueTypeId", "ValueType");

            return this.PartialView(offerDetailsViewModel);
        }
        catch (Exception ex)
        {
            this.logger.LogError("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
            return this.View("Error", ex);
        }
    }

    /// <summary>
    /// Indexes this instance.
    /// </summary>
    /// <param name="offerDetailsData">The offers data.</param>
    /// <returns>
    /// return All subscription.
    /// </returns>
    [HttpPost]
    public IActionResult OfferDetails(OfferDetailsViewModel offerDetailsData)
    {
        this.logger.LogInformation("Offers Controller / OfferDetails:  offerGuid {0}", offerDetailsData.OfferGuid);
        
        try
        {
            var currentUserDetail = this.usersRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);
            if (offerDetailsData.OfferAttributes != null)
            {
                // query
                var validItems = offerDetailsData.OfferAttributes.Where(i => i.IsRemove == false);

                foreach (var offerAttribute in validItems)
                {
                    var newOfferAttribute = new OfferAttributes()
                    {
                        Id = offerAttribute.AttributeID,
                        ParameterId = offerAttribute.ParameterId,
                        DisplayName = offerAttribute.DisplayName,
                        Description = offerAttribute.Description,
                        ValueTypeId = offerAttribute.ValueTypeId,
                        FromList = offerAttribute.FromList,
                        ValuesList = offerAttribute.ValuesList,
                        Max = offerAttribute.Max,
                        Min = offerAttribute.Min,
                        Type = offerAttribute.Type,
                        DisplaySequence = offerAttribute.DisplaySequence,
                        Isactive = offerAttribute.Isactive,
                        IsRequired = offerAttribute.IsRequired,
                        IsDelete = offerAttribute.IsDelete,
                        CreateDate = DateTime.Now,
                        UserId = currentUserDetail == null ? 0 : currentUserDetail.UserId,
                        OfferId = offerDetailsData.OfferGuid,
                    };
                    this.offersAttributeRepository.Add(newOfferAttribute);
                }

                var valueTypes = this.valueTypesRepository.GetAll().ToList();
                this.ViewBag.ValueTypes = new SelectList(valueTypes, "ValueTypeId", "ValueType");
                this.TempData["ShowWelcomeScreen"] = "True";
            }

            this.ModelState.Clear();

            return this.RedirectToAction(nameof(this.OfferDetails), new
            {
                @offerGuId = offerDetailsData.OfferGuid
            });
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, ex.Message);
            return this.View("Error", ex);
        }
    }
}