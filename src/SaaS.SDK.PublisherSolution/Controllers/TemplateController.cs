namespace Microsoft.Marketplace.Saas.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    //using global::SaaS.SDK.PublisherSolution.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    //using Microsoft.Marketplace.Saas.Client.Services;
    using Microsoft.Marketplace.Saas.Web.Controllers;
    using Microsoft.Marketplace.Saas.Web.Helpers;
    using Microsoft.Marketplace.Saas.Web.Models;
    using Microsoft.Marketplace.Saas.Web.Services;
    using Microsoft.Marketplace.SaaS.SDK.PublisherSolution.Utilities;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.DataModel;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using Microsoft.Marketplace.SaasKit.Client.Services;
    using Microsoft.Marketplace.SaasKit.Models;
    using Newtonsoft.Json.Linq;

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
            DeploymentParameterViewModel model = new DeploymentParameterViewModel();
            try
            {
                bool isKnownUser = knownUsersRepository.GetKnownUserDetail(base.CurrentUserEmailAddress, 1)?.Id > 0;
                if (User.Identity.IsAuthenticated && isKnownUser)
                {
                    var newBatchModel = new TemplateModel();
                    newBatchModel.BulkUploadUsageStagings = new List<BulkUploadUsageStagingResult>();
                    newBatchModel.DeploymentParameterViewModel = model;
                    return View(model);
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
            DeploymentParameterViewModel model = new DeploymentParameterViewModel();
            ResponseModel response = new ResponseModel();
            var currentUserDetail = usersRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);
            var filename = string.Empty;
            var filePath = string.Empty;
            var fileContantType = string.Empty;
            var formFile = uploadfile.FirstOrDefault();
            var ArmtempalteId = Guid.NewGuid();
            if (formFile.Length > 0)
            {
                filename = formFile.FileName;

                fileContantType = formFile.ContentType;
                string fileExtension = Path.GetExtension(formFile.FileName);
                if (fileExtension == ".json")
                {
                    // full path to file in temp location
                    filePath = Path.GetTempFileName(); //we are using Temp file name just for the example. Add your own file path.
                   
                    model.FileName = filename;
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {

                        //string paramFileContext = new System.Net.WebClient().DownloadString(filePath);
                        //string paramFileContext = new WebClient().DownloadString(paramFilePath);
                        //dynamic result = JObject.Parse(stream.ToString());
                        string str = (new StreamReader(formFile.OpenReadStream())).ReadToEnd();
                        dynamic result = JObject.Parse(str);
                        foreach (JToken child in result.parameters.Children())
                        {

                            var paramName = (child as JProperty).Name;
                            model.ParameterName = paramName;
                            object paramValue = string.Empty;

                            foreach (JToken grandChild in child)
                            {
                                foreach (JToken grandGrandChild in grandChild)
                                {
                                    var property = grandGrandChild as JProperty;

                                    if (property != null /*&& property.Name == "value"*/)
                                    {
                                        int propertyIntValue = 0;
                                        bool propertyBoolValue = false;

                                        var type = property.Value.GetType();

                                        if (type == typeof(JArray) ||
                                        property.Value.Type == JTokenType.Object ||
                                        property.Value.Type == JTokenType.Date)
                                        {
                                            paramValue = property.Value;
                                            if (paramValue != null)
                                            {
                                                model.ParameterValue = paramValue.ToString();
                                            }
                                        }
                                        else if (property.Value.Type == JTokenType.Integer && int.TryParse((string)property.Value, out propertyIntValue))
                                        {
                                            model.ParameterType = "int";
                                            paramValue = propertyIntValue;
                                            model.ParameterValue = (string)property.Value;
                                        }
                                        else if (property.Value.Type == JTokenType.Boolean && bool.TryParse((string)property.Value, out propertyBoolValue))
                                        {
                                            model.ParameterType = "bool";
                                            paramValue = propertyBoolValue;
                                            model.ParameterValue = (string)property.Value;
                                        }
                                        else
                                        {
                                            model.ParameterType = "string";
                                            paramValue = (string)property.Value;

                                        }
                                    }
                                }
                            }


                        }


                        bulkUploadModel.DeploymentParameterViewModel = model;

                        await formFile.CopyToAsync(stream);
                        string fileuploadPath = BlobFileUploadHelper.UploadFile(formFile, filename, fileContantType, ArmtempalteId, applicationConfigRepository);
                        Armtemplates armTemplate = new Armtemplates()
                        {
                            ArmtempalteName = filename,
                            TemplateLocation = filePath,
                            Isactive = true,
                            CreateDate = DateTime.Now,
                            UserId = this.userService.GetUserIdFromEmailAddress(this.CurrentUserEmailAddress),
                            ArmtempalteId = ArmtempalteId
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
            return View("Index", model);
        }

    }
}

