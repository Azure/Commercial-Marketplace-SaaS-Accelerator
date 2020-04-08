namespace Microsoft.Marketplace.Saas.Web.Controllers
{
    using Microsoft.Marketplace.Saas.Web.Controllers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Marketplace.SaasKit.Client.Services;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Marketplace.Saas.Web.Models;
    using Microsoft.Marketplace.SaaS.SDK.PublisherSolution.Utilities;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using Microsoft.AspNetCore.Mvc.Rendering;

    [ServiceFilter(typeof(KnownUser))]
    public class OffersController : BaseController
    {
        private readonly IUsersRepository usersRepository;

        private readonly IValueTypesRepository valueTypesRepository;

        private readonly IApplicationConfigRepository applicationConfigRepository;

        private readonly IOffersRepository offersRepository;

        private readonly IOfferAttributesRepository offersAttributeRepository;

        private OffersService offersService;

        public OffersController(IOffersRepository offersRepository, IApplicationConfigRepository applicationConfigRepository, IUsersRepository usersRepository, IValueTypesRepository valueTypesRepository, IOfferAttributesRepository offersAttributeRepository)
        {
            this.offersRepository = offersRepository;
            this.applicationConfigRepository = applicationConfigRepository;
            this.usersRepository = usersRepository;
            this.valueTypesRepository = valueTypesRepository;
            this.offersService = new OffersService(this.offersRepository);
            this.offersAttributeRepository = offersAttributeRepository;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>return All subscription</returns>
        public IActionResult Index()
        {
            List<OffersModel> getAllOffersData = new List<OffersModel>();
            this.TempData["ShowWelcomeScreen"] = "True";
            var currentUserDetail = usersRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);
            if (currentUserDetail != null)
            {
                getAllOffersData = this.offersService.GetOffers();
            }
            return this.View(getAllOffersData);
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>return All subscription</returns>
        public IActionResult OfferDetails(Guid offerGuId)
        {
            OffersViewModel OffersData = new OffersViewModel();
            this.TempData["ShowWelcomeScreen"] = "True";
            var currentUserDetail = usersRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);
            OffersData = this.offersService.GetOfferOnId(offerGuId);

            var offerAttributes = this.offersAttributeRepository.GetOfferAttributeDetailByOfferId(offerGuId);
            var valueTypes = valueTypesRepository.GetValueTypes().ToList();
            ViewBag.ValueTypes = new SelectList(valueTypes, "ValueTypeId", "ValueType");
            OffersData.OfferAttributes = new List<OfferAttributesModel>();
            if (offerAttributes != null)
            {
                //OffersData.OfferAttributes = offerAttributes.ToList();
                foreach (var offerAttribute in offerAttributes)
                {
                    var existingOfferAttribute = new OfferAttributesModel()
                    {
                        AttributeID = offerAttribute.Id,
                        ParameterId = offerAttribute.ParameterId,
                        DisplayName = offerAttribute.DisplayName,
                        Description = offerAttribute.Description,
                        ValueTypeId = offerAttribute.ValueTypeId,
                        //ValueType = valueTypes,
                        FromList = offerAttribute.FromList,
                        ValuesList = offerAttribute.ValuesList,
                        Max = offerAttribute.Max,
                        Min = offerAttribute.Min,
                        Type = offerAttribute.Type,
                        DisplaySequence = offerAttribute.DisplaySequence,
                        Isactive = offerAttribute.Isactive,
                        CreateDate = DateTime.Now,
                        UserId = currentUserDetail == null ? 0 : currentUserDetail.UserId,
                        OfferId = OffersData.OfferGuid
                    };
                    OffersData.OfferAttributes.Add(existingOfferAttribute);

                }
            }


            return this.PartialView(OffersData);
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>return All subscription</returns>
        [HttpPost]
        public IActionResult OfferDetails(OffersViewModel OffersData)
        {
            var currentUserDetail = usersRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);
            if (OffersData != null && OffersData.OfferAttributes != null)
            {
                // query
                var validItems = OffersData.OfferAttributes.Where(i => i.IsRemove == false);

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
                        CreateDate = DateTime.Now,
                        UserId = currentUserDetail == null ? 0 : currentUserDetail.UserId,
                        OfferId = OffersData.OfferGuid
                    };

                    this.offersAttributeRepository.Add(newOfferAttribute);
                }

                var deleteItems = OffersData.OfferAttributes.Where(i => i.IsRemove == true && i.AttributeID != 0);

                if (deleteItems != null && deleteItems.Count() > 0)
                {
                    //this.offersAttributeRepository.Remove(deleteItems);
                    /* Delete the Fields from existing Plans and subscriptios*/

                }

                var valueTypes = valueTypesRepository.GetValueTypes().ToList();
                ViewBag.ValueTypes = new SelectList(valueTypes, "ValueTypeId", "ValueType");
                this.TempData["ShowWelcomeScreen"] = "True";
            }
            ModelState.Clear();
            //return RedirectToAction(nameof(OfferDetails), new { @offerGuId = offerGuId });
            return RedirectToAction(nameof(Index));


            //return View(OffersData); 
        }



    }
}
