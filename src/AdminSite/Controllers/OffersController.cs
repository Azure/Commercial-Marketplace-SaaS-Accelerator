using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Web;
using Marketplace.SaaS.Accelerator.AdminSite.Models.Offers;
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
/// Offers Controller.
/// </summary>
/// <seealso cref="BaseController" />
[ServiceFilter(typeof(KnownUserAttribute))]
public class OffersController : BaseController
{
    private readonly IUsersRepository usersRepository;

    private readonly IValueTypesRepository valueTypesRepository;

    private readonly IOfferAttributesRepository offersAttributeRepository;

    private readonly SaaSClientLogger<OffersController> logger;

    private readonly OffersService offersService;
    
    private readonly IApplicationConfigRepository applicationConfigRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="OffersController"/> class.
    /// </summary>
    /// <param name="offersService">Concrete OffersService</param>
    /// <param name="applicationConfigRepository">The application configuration repository.</param>
    /// <param name="usersRepository">The users repository.</param>
    /// <param name="valueTypesRepository">The value types repository.</param>
    /// <param name="offersAttributeRepository">The offers attribute repository.</param>
    /// <param name="logger">The logger.</param>
    public OffersController(
        OffersService offersService,
        IApplicationConfigRepository applicationConfigRepository, 
        IUsersRepository usersRepository, 
        IValueTypesRepository valueTypesRepository, 
        IOfferAttributesRepository offersAttributeRepository, SaaSClientLogger<OffersController> logger):base(applicationConfigRepository)
    {
        this.applicationConfigRepository = applicationConfigRepository;
        this.usersRepository = usersRepository;
        this.valueTypesRepository = valueTypesRepository;
        this.offersService = offersService;
        this.applicationConfigRepository = applicationConfigRepository;
        this.offersAttributeRepository = offersAttributeRepository;
        this.logger = logger;
    }

    /// <summary>
    /// Indexes this instance.
    /// </summary>
    /// <returns>return All subscription.</returns>
    public IActionResult Index()
    {
        this.logger.Info("Offers Controller / Index");
        try
        {
            this.TempData["ShowWelcomeScreen"] = "True";
            
            var offersServiceModels = this.offersService.GetOffers();

            var viewModels = new List<OfferListItemViewModel>();

            foreach (var offersServiceModel in offersServiceModels)
            {
                var listItem = new OfferListItemViewModel()
                {
                    OfferId = offersServiceModel.OfferID,
                    OfferGuid = offersServiceModel.OfferGuId
                };

                viewModels.Add(listItem);
            }
            
            return this.View(viewModels);
        }
        catch (Exception ex)
        {
            this.logger.LogError($"Message:{ex.Message} :: {ex.InnerException}");
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
        this.logger.Info("Offers Controller / OfferDetails:  offerGuid {offerGuid}");

        try
        {
            this.TempData["ShowWelcomeScreen"] = "True";
            
            var currentUserDetail = this.usersRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);
            
            var offersViewModel = this.offersService.GetOfferById(offerGuid);
                offersViewModel.OfferAttributes = new List<OfferAttributesModel>();

            var valueTypes = this.valueTypesRepository.GetAll().ToList();
            ViewBag.ValueTypes = new SelectList(valueTypes, "ValueTypeId", "ValueType");

            var offerAttributesList = this.offersAttributeRepository.GetInputAttributesByOfferId(offerGuid);
            if (offerAttributesList != null)
            {
                foreach (var offerAttribute in offerAttributesList)
                {
                    var offerAttributes = MapOfferAttributesModel(offerAttribute, currentUserDetail, offersViewModel);

                    offersViewModel.OfferAttributes.Add(offerAttributes);
                }
            }

            return this.PartialView(offersViewModel);
        }
        catch (Exception ex)
        {
            this.logger.LogError($"Message:{ex.Message} :: {ex.InnerException}");
            return this.View("Error", ex);
        }
    }

    /// <summary>
    /// Indexes this instance.
    /// </summary>
    /// <param name="offersData">The offers data.</param>
    /// <returns>
    /// return All subscription.
    /// </returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult OfferDetails(OfferModel offersData)
    {
        this.logger.Info(HttpUtility.HtmlEncode($"Offers Controller / OfferDetails:  offerGuid {JsonSerializer.Serialize(offersData)}"));
        try
        {
            var currentUserDetail = this.usersRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);
            if (offersData != null && offersData.OfferAttributes != null)
            {
                // query
                var validItems = offersData.OfferAttributes.Where(i => i.IsRemove == false);

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
                        OfferId = offersData.OfferGuid,
                    };
                    this.offersAttributeRepository.Add(newOfferAttribute);
                }

                var valueTypes = this.valueTypesRepository.GetAll().ToList();
                this.ViewBag.ValueTypes = new SelectList(valueTypes, "ValueTypeId", "ValueType");
                this.TempData["ShowWelcomeScreen"] = "True";
            }

            this.ModelState.Clear();
            return this.RedirectToAction(nameof(this.OfferDetails), new { @offerGuId = offersData.OfferGuid });
        }
        catch (Exception ex)
        {
            this.logger.LogError($"Message:{ex.Message} :: {ex.InnerException}");
            return this.View("Error", ex);
        }
    }

    private static OfferAttributesModel MapOfferAttributesModel(OfferAttributes offerAttribute, Users currentUserDetail, OfferModel offerModel)
    {
        return new OfferAttributesModel()
        {
            AttributeID = offerAttribute.Id,
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
            IsRequired = offerAttribute.IsRequired ?? false,
            IsDelete = offerAttribute.IsDelete ?? false,
            CreateDate = DateTime.Now,
            UserId = currentUserDetail?.UserId ?? 0,
            OfferId = offerModel.OfferGuid,
        };
    }



}