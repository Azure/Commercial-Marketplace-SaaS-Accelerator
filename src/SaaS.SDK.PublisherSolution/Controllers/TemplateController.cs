namespace Microsoft.Marketplace.Saas.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using global::SaaS.SDK.Client.DataAccess.DataModel;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Marketplace.Saas.Web.Controllers;
    using Microsoft.Marketplace.Saas.Web.Models;
    using Microsoft.Marketplace.SaaS.SDK.PublisherSolution.Utilities;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using Microsoft.Marketplace.SaasKit.Client.Services;
    using Microsoft.Marketplace.SaasKit.Models;

    [ServiceFilter(typeof(KnownUser))]
    public class TemplateController : BaseController
    {
        private readonly IUsersRepository usersRepository;

        private readonly IApplicationConfigRepository applicationConfigRepository;

        private readonly IKnownUsersRepository knownUsersRepository;


        public TemplateController (IUsersRepository usersRepository, IApplicationConfigRepository applicationConfigRepository, IKnownUsersRepository knownUsersRepository)
        {
            this.usersRepository = usersRepository;
            this.applicationConfigRepository = applicationConfigRepository;
            this.knownUsersRepository = knownUsersRepository;
        }

        public IActionResult Index()
        {
            try
            {
                bool isKnownUser = knownUsersRepository.GetKnownUserDetail(base.CurrentUserEmailAddress, 1)?.Id > 0;
                if (User.Identity.IsAuthenticated && isKnownUser)
                {
                    var newBatchModel = new TemplateModel();
                    newBatchModel.BulkUploadUsageStagings = new List<BulkUploadUsageStagingResult>();
                    return View(newBatchModel);
                }
                else
                    return View("Error", new ErrorViewModel { IsKnownUser = isKnownUser });
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { IsKnownUser = true });
            }
        }
    }
}
