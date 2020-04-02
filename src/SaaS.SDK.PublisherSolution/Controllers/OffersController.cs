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

    [ServiceFilter(typeof(KnownUser))]
    public class OffersController : BaseController
    {
        private readonly IUsersRepository usersRepository;

        private readonly IApplicationConfigRepository applicationConfigRepository;

        private readonly IOffersRepository offersRepository;

        private OffersService offersService;

        public OffersController(IOffersRepository offersRepository, IApplicationConfigRepository applicationConfigRepository, IUsersRepository usersRepository)
        {
            this.offersRepository = offersRepository;
            this.applicationConfigRepository = applicationConfigRepository;
            this.usersRepository = usersRepository;
            this.offersService = new OffersService(this.offersRepository);
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
        public IActionResult OfferDetails(int OfferId)
        {
            OffersViewModel OffersData = new OffersViewModel();
            this.TempData["ShowWelcomeScreen"] = "True";
            var currentUserDetail = usersRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);
            //if (currentUserDetail != null)
            //{
            OffersData = this.offersService.GetOfferOnId(OfferId);
            //}
            return this.PartialView(OffersData);
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>return All subscription</returns>
        [HttpPost]
        public IActionResult OfferDetails(OffersViewModel OffersData)
        {
            //this.TempData["ShowWelcomeScreen"] = "True";
            //var currentUserDetail = usersRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);
            ////if (currentUserDetail != null)
            ////{
            //OffersData = this.offersService.GetOfferOnId(OfferId);
            ////}
            //return this.View(OffersData);
            return null;
        }



    }
}
