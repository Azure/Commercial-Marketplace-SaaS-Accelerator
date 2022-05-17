﻿namespace Microsoft.Marketplace.Saas.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

    /// <summary>
    /// KnownUsers Controller.
    /// </summary>
    public class KnownUsersController : BaseController
    {
        private readonly IKnownUsersRepository knownUsersRepository;
        private readonly ILogger<OffersController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="KnownUsersController" /> class.
        /// </summary>
        /// <param name = "knownUsersRepository" > The known users repository.</param>
        /// <param name="logger">The logger.</param>

        public KnownUsersController(IKnownUsersRepository knownUsersRepository, ILogger<OffersController> logger)
        {
            this.knownUsersRepository = knownUsersRepository;
            this.logger = logger;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>All known users.</returns>
        public IActionResult Index()
        {
            this.logger.LogInformation("KnownUsers Controller / Index");
            try
            {
                var getAllKnownUsers = this.knownUsersRepository.GetAllKnownUsers();
                return this.View(getAllKnownUsers);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                return this.View("Error", ex);
            }
        }

        /// <summary>
        /// Save known users.
        /// </summary>
        /// <param name="knownUsers">The list of known users</param>
        /// <returns> Json.</returns>
        public JsonResult SaveKnownUsers([FromBody] IEnumerable<KnownUsers> knownUsers)
        {

            this.logger.LogInformation("KnownUsers Controller / SaveKnownUsers");
            try
            {
                return Json(this.knownUsersRepository.SaveAllKnownUsers(knownUsers));
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                return Json(string.Empty);
            }
        }
    }
}