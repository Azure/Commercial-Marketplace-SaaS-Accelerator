using System;
using System.Collections.Generic;
using System.Linq;
using Marketplace.SaaS.Accelerator.AdminSite.Models.Offer;
using Marketplace.SaaS.Accelerator.DataAccess.Context;
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

    private readonly ILogger<OffersController> logger;

    private readonly OfferService offersService;

    /// <summary>
    /// Initializes a new instance of the <see cref="OffersController"/> class.
    /// </summary>
    /// <param name="usersRepository">The users repository.</param>
    /// <param name="valueTypesRepository">The value types repository.</param>
    /// <param name="saasKitContext">Context for DB connectivity</param>
    /// <param name="logger">The logger.</param>
    public OffersController(
        IUsersRepository usersRepository, 
        IValueTypesRepository valueTypesRepository, 
        SaasKitContext saasKitContext,
        ILogger<OffersController> logger)
    {
        this.usersRepository = usersRepository;
        this.valueTypesRepository = valueTypesRepository;
        this.offersService = new OfferService(saasKitContext);
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
            this.TempData["ShowWelcomeScreen"] = "True";

            OffersListViewModel viewModel = new OffersListViewModel()
            {
                LineItems = new List<OffersListViewModel.OfferListItem>()
            };

            var currentUserDetail = this.usersRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);

            List<OfferModel> offers = this.offersService.GetAllOffers();

            foreach (var offer in offers)
            {
                var listItem = new OffersListViewModel.OfferListItem()
                {
                    OfferName = offer.OfferName,
                    OfferId = offer.OfferID,
                    OfferGuid = offer.OfferGuid.GetValueOrDefault()
                };

                viewModel.LineItems.Add(listItem);
            }

            return this.View(viewModel);
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
            var offerAttributesViewModels = new List<OfferAttributesViewModel>();
            
            var offerModel = this.offersService.GetOfferById(offerGuid);
            var offerAttributesModels = this.offersService.GetOfferAttributesById(offerGuid);
            var currentUserDetail = this.usersRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);
            
            foreach (OfferAttributesModel offerAttributesModel in offerAttributesModels)
            {
                var offerAttributesViewModel = MapOfferAttributesViewModel(offerAttributesModel, currentUserDetail);

                offerAttributesViewModels.Add(offerAttributesViewModel);
            }

            var offerDetailsViewModel = new OfferDetailsViewModel()
            {
                OfferId = offerModel.OfferID,
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
                var offerAttributesViewModels = offerDetailsData.OfferAttributes.Where(i => i.IsRemove == false);

                foreach (var offerAttributeViewModel in offerAttributesViewModels)
                {
                    var newOfferAttribute = MapOfferAttributes(offerDetailsData, offerAttributeViewModel, currentUserDetail);

                    this.offersService.AddOfferAttributes(newOfferAttribute);
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

    private OfferAttributes MapOfferAttributes(OfferDetailsViewModel offerDetailsData, OfferAttributesViewModel offerAttributeViewModel, Users currentUserDetail)
    {
        return new OfferAttributes()
        {
            Id = offerAttributeViewModel.AttributeId,
            ParameterId = offerAttributeViewModel.ParameterId,
            DisplayName = offerAttributeViewModel.DisplayName,
            Description = offerAttributeViewModel.Description,
            ValueTypeId = offerAttributeViewModel.ValueTypeId,
            FromList = offerAttributeViewModel.FromList,
            ValuesList = offerAttributeViewModel.ValuesList,
            Max = offerAttributeViewModel.Max,
            Min = offerAttributeViewModel.Min,
            Type = offerAttributeViewModel.Type,
            DisplaySequence = offerAttributeViewModel.DisplaySequence,
            Isactive = offerAttributeViewModel.IsActive,
            IsRequired = offerAttributeViewModel.IsRequired,
            IsDelete = offerAttributeViewModel.IsDelete,
            CreateDate = DateTime.Now,
            UserId = currentUserDetail == null ? 0 : currentUserDetail.UserId,
            OfferId = offerDetailsData.OfferGuid,
        };
    }

    private OfferAttributesViewModel MapOfferAttributesViewModel(OfferAttributesModel offerAttributesModel, Users currentUserDetail)
    {
        return new OfferAttributesViewModel()
        {
            AttributeId = offerAttributesModel.Id,
            ParameterId = offerAttributesModel.ParameterId,
            DisplayName = offerAttributesModel.DisplayName,
            Description = offerAttributesModel.Description,
            ValueTypeId = offerAttributesModel.ValueTypeId,
            FromList = offerAttributesModel.FromList,
            ValuesList = offerAttributesModel.ValuesList,
            Max = offerAttributesModel.Max,
            Min = offerAttributesModel.Min,
            Type = offerAttributesModel.Type,
            DisplaySequence = offerAttributesModel.DisplaySequence,
            IsActive = offerAttributesModel.IsActive,
            IsRequired = offerAttributesModel.IsRequired ?? false,
            IsDelete = offerAttributesModel.IsDelete ?? false,
            CreateDate = DateTime.Now,
            UserId = currentUserDetail?.UserId ?? 0,
            OfferId = offerAttributesModel.OfferId
        };
    }
}