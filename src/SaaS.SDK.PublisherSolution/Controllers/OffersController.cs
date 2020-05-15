namespace Microsoft.Marketplace.Saas.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Logging;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using Microsoft.Marketplace.SaaS.SDK.Services.Services;
    using Microsoft.Marketplace.SaaS.SDK.Services.Utilities;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    
    /// <summary>
    /// Offers Controller.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.Saas.Web.Controllers.BaseController" />
    [ServiceFilter(typeof(KnownUserAttribute))]
    public class OffersController : BaseController
    {
        private readonly IUsersRepository usersRepository;

        private readonly IValueTypesRepository valueTypesRepository;

        private readonly IApplicationConfigRepository applicationConfigRepository;

        private readonly IOffersRepository offersRepository;

        private readonly IOfferAttributesRepository offersAttributeRepository;

        private readonly ILogger<OffersController> logger;

        private OfferServices offersService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OffersController"/> class.
        /// </summary>
        /// <param name="offersRepository">The offers repository.</param>
        /// <param name="applicationConfigRepository">The application configuration repository.</param>
        /// <param name="usersRepository">The users repository.</param>
        /// <param name="valueTypesRepository">The value types repository.</param>
        /// <param name="offersAttributeRepository">The offers attribute repository.</param>
        /// <param name="logger">The logger.</param>
        public OffersController(IOffersRepository offersRepository, IApplicationConfigRepository applicationConfigRepository, IUsersRepository usersRepository, IValueTypesRepository valueTypesRepository, IOfferAttributesRepository offersAttributeRepository, ILogger<OffersController> logger)
        {
            this.offersRepository = offersRepository;
            this.applicationConfigRepository = applicationConfigRepository;
            this.usersRepository = usersRepository;
            this.valueTypesRepository = valueTypesRepository;
            this.offersService = new OfferServices(this.offersRepository);
            this.offersAttributeRepository = offersAttributeRepository;
            this.logger = logger;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>return All subscription.</returns>
        public IActionResult Index()
        {
            this.logger.LogInformation("Offers Controller / Index");
            try
            {
                List<OffersModel> getAllOffersData = new List<OffersModel>();
                this.TempData["ShowWelcomeScreen"] = "True";
                var currentUserDetail = this.usersRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);

                getAllOffersData = this.offersService.GetOffers();

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
        /// <param name="offerGuId">The offer gu identifier.</param>
        /// <returns>
        /// return All subscription.
        /// </returns>
        public IActionResult OfferDetails(Guid offerGuId)
        {
            this.logger.LogInformation("Offers Controller / OfferDetails:  offerGuId {0}", offerGuId);
            try
            {
                OffersViewModel offersData = new OffersViewModel();
                this.TempData["ShowWelcomeScreen"] = "True";
                var currentUserDetail = this.usersRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);
                offersData = this.offersService.GetOfferOnId(offerGuId);

                var offerAttributes = this.offersAttributeRepository.GetInputAttributesByOfferId(offerGuId);
                var valueTypes = this.valueTypesRepository.GetAll().ToList();
                this.ViewBag.ValueTypes = new SelectList(valueTypes, "ValueTypeId", "ValueType");
                offersData.OfferAttributes = new List<OfferAttributesModel>();
                if (offerAttributes != null)
                {
                    foreach (var offerAttribute in offerAttributes)
                    {
                        var existingOfferAttribute = new OfferAttributesModel()
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
                            UserId = currentUserDetail == null ? 0 : currentUserDetail.UserId,
                            OfferId = offersData.OfferGuid,
                        };
                        offersData.OfferAttributes.Add(existingOfferAttribute);
                    }
                }

                return this.PartialView(offersData);
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
        /// <param name="offersData">The offers data.</param>
        /// <returns>
        /// return All subscription.
        /// </returns>
        [HttpPost]
        public IActionResult OfferDetails(OffersViewModel offersData)
        {
            this.logger.LogInformation("Offers Controller / OfferDetails:  offerGuId {0}",  JsonSerializer.Serialize(offersData));
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
                this.logger.LogError(ex, ex.Message);
                return this.View("Error", ex);
            }
        }
    }
}
