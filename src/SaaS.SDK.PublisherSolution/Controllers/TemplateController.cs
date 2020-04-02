namespace Microsoft.Marketplace.Saas.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Marketplace.Saas.Client.Services;
    using Microsoft.Marketplace.Saas.Web.Controllers;
    using Microsoft.Marketplace.Saas.Web.Helpers;
    using Microsoft.Marketplace.Saas.Web.Models;
    using Microsoft.Marketplace.SaaS.SDK.PublisherSolution.Utilities;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.DataModel;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using Microsoft.Marketplace.SaasKit.Client.Services;
    using Microsoft.Marketplace.SaasKit.Models;

    [ServiceFilter(typeof(KnownUser))]
    public class TemplateController : BaseController
    {
        private readonly IUsersRepository usersRepository;

        private readonly IApplicationConfigRepository applicationConfigRepository;

        private readonly IKnownUsersRepository knownUsersRepository;

        private UserService userService;

        private readonly IUsersRepository userRepository;

        private readonly IArmTemplateRepository armTemplateRepository;


        public TemplateController(IUsersRepository usersRepository, IApplicationConfigRepository applicationConfigRepository, IKnownUsersRepository knownUsersRepository, IUsersRepository userRepository, IArmTemplateRepository armTemplateRepository)
        {
            this.usersRepository = usersRepository;
            this.applicationConfigRepository = applicationConfigRepository;
            this.knownUsersRepository = knownUsersRepository;
            this.userRepository = userRepository;
            this.userService = new UserService(this.userRepository);
            this.armTemplateRepository = armTemplateRepository;
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

        [HttpPost("UploadBatchUsage")]
        public async Task<IActionResult> UploadBatchUsage(List<IFormFile> uploadfile)
        {

            BatchUsageUploadModel bulkUploadModel = new BatchUsageUploadModel();
            bulkUploadModel.BulkUploadUsageStagings = new List<BulkUploadUsageStagingResult>();
            bulkUploadModel.BatchLogId = 0;

            ResponseModel response = new ResponseModel();
            var currentUserDetail = usersRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);
            var filename = string.Empty;
            var filePath = string.Empty;
            var fileContantType = string.Empty;
            var formFile = uploadfile.FirstOrDefault();
            var referenceid = Guid.NewGuid();
            if (formFile.Length > 0)
            {
                filename = formFile.FileName;

                fileContantType = formFile.ContentType;
                string fileExtension = Path.GetExtension(formFile.FileName);
                if (fileExtension == ".json")
                {
                    // full path to file in temp location
                    filePath = Path.GetTempFileName(); //we are using Temp file name just for the example. Add your own file path.
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {

                        await formFile.CopyToAsync(stream);
                        string fileuploadPath = BlobFileUploadHelper.UploadFile(formFile, filename, fileContantType, referenceid, applicationConfigRepository);
                        ARMTemplates armTemplate = new ARMTemplates()
                        {
                            ArmtempalteName = filename,
                            TemplateLocation = filePath,
                            Isactive = true,
                            CreateDate = DateTime.Now,
                            UserId = this.userService.GetUserIdFromEmailAddress(this.CurrentUserEmailAddress)
                        };
                        this.armTemplateRepository.Add(armTemplate);
                    }
                }
                else
                {
                    response.Message = "Please select JSON file.";
                    response.IsSuccess = false;

                    return View("RecordBatchUsage", response);
                }
            }
            else
            {
                response.Message = "Please select File!";
                response.IsSuccess = false;
            }
            bulkUploadModel.Response = response;
            return View("Index", bulkUploadModel);
        }

    }
}

