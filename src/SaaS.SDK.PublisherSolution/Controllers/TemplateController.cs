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
    using Microsoft.Marketplace.Saas.Web.Controllers;
    using Microsoft.Marketplace.SaaS.SDK.Services.Contracts;
    using Microsoft.Marketplace.SaaS.SDK.Services.Helpers;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using Microsoft.Marketplace.SaaS.SDK.Services.Services;
    using Microsoft.Marketplace.SaaS.SDK.Services.Utilities;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.DataModel;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

    using Microsoft.Marketplace.SaasKit.Models;
    using Newtonsoft.Json.Linq;

    [ServiceFilter(typeof(KnownUserAttribute))]
    public class TemplateController : BaseController
    {
        private readonly IUsersRepository usersRepository;

        private readonly IApplicationConfigRepository applicationConfigRepository;

        private readonly IKnownUsersRepository knownUsersRepository;

        private UserService userService;

        private readonly IUsersRepository userRepository;

        private readonly IArmTemplateRepository armTemplateRepository;

        private readonly IARMTemplateStorageService azureBlobFileClient;
        private ArmTemplateService armTemplateService;

        private readonly IArmTemplateParametersRepository armTemplateParametersRepository;

        public TemplateController(IUsersRepository usersRepository, IApplicationConfigRepository applicationConfigRepository, IKnownUsersRepository knownUsersRepository, IUsersRepository userRepository, IArmTemplateRepository armTemplateRepository, IArmTemplateParametersRepository armTemplateParametersRepository, IARMTemplateStorageService azureBlobFileClient)
        {
            this.usersRepository = usersRepository;
            this.applicationConfigRepository = applicationConfigRepository;
            this.knownUsersRepository = knownUsersRepository;
            this.userRepository = userRepository;
            this.userService = new UserService(this.userRepository);
            this.armTemplateRepository = armTemplateRepository;
            this.armTemplateService = new ArmTemplateService(this.armTemplateRepository);
            this.armTemplateParametersRepository = armTemplateParametersRepository;
            this.azureBlobFileClient = azureBlobFileClient;
        }

        public IActionResult Index()
        {
            ModelState.Clear();
            DeploymentParameterViewModel model = new DeploymentParameterViewModel();
            List<ARMTemplateViewModel> armTemplates = new List<ARMTemplateViewModel>();
            armTemplates = this.armTemplateService.GetARMTemplates();
            model.ARMParms = armTemplates;
            model.FileName = "";
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
                return View("Error", ex);
            }
        }

        [HttpPost("UploadBatchUsage")]
        public async Task<IActionResult> UploadBatchUsage(List<IFormFile> uploadfile)
        {
            BatchUsageUploadModel bulkUploadModel = new BatchUsageUploadModel();
            bulkUploadModel.BulkUploadUsageStagings = new List<BulkUploadUsageStagingResult>();
            bulkUploadModel.BatchLogId = 0;
            DeploymentParameterViewModel model = new DeploymentParameterViewModel();
            List<ChindParameterViewModel> childlist = new List<ChindParameterViewModel>();

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

                        string str = (new StreamReader(formFile.OpenReadStream())).ReadToEnd();
                        dynamic result = JObject.Parse(str);

                        ChindParameterViewModel childparms = new ChindParameterViewModel();
                        childparms.ParameterDataType = "string";
                        childparms.ParameterName = "ResourceGroup";
                        childparms.ParameterValue = "";
                        childparms.ParameterType = "input";
                        childlist.Add(childparms);
                        childparms = new ChindParameterViewModel();
                        childparms.ParameterDataType = "string";
                        childparms.ParameterName = "Location";
                        childparms.ParameterValue = "";
                        childparms.ParameterType = "input";
                        childlist.Add(childparms);
                        //if (result.parameters != null && result.parameters.Children() != null)
                        //{
                        foreach (JToken child in result.parameters.Children())
                        {
                            childparms = new ChindParameterViewModel();
                            childparms.ParameterType = "input";
                            var paramName = (child as JProperty).Name;
                            childparms.ParameterName = paramName;
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
                                                //childparms.ParameterValue = paramValue.ToString();
                                            }
                                        }
                                        else if (property.Value.Type == JTokenType.Integer && int.TryParse((string)property.Value, out propertyIntValue))
                                        {
                                            childparms.ParameterDataType = "int";
                                            paramValue = propertyIntValue;
                                            //childparms.ParameterValue = (string)property.Value;
                                        }
                                        else if (property.Value.Type == JTokenType.Boolean && bool.TryParse((string)property.Value, out propertyBoolValue))
                                        {
                                            childparms.ParameterDataType = "bool";
                                            paramValue = propertyBoolValue;
                                            //childparms.ParameterValue = (string)property.Value;
                                        }
                                        else
                                        {
                                            childparms.ParameterDataType = "string";
                                            paramValue = (string)property.Value;

                                        }
                                    }

                                }
                            }
                            childlist.Add(childparms);
                            //childparms.listparms= grandlist;
                            //childlist.Add(childparms);


                        }
                        //}
                        model.DeplParms = childlist;
                        //if (result.outputs != null && result.outputs.Children() != null)
                        //{
                        foreach (JToken child in result.outputs.Children())
                        {
                            childparms = new ChindParameterViewModel();
                            childparms.ParameterType = "output";
                            var paramName = (child as JProperty).Name;
                            childparms.ParameterName = paramName;
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
                                                //childparms.ParameterValue = paramValue.ToString();
                                            }
                                        }
                                        else if (property.Value.Type == JTokenType.Integer && int.TryParse((string)property.Value, out propertyIntValue))
                                        {
                                            childparms.ParameterDataType = "int";
                                            paramValue = propertyIntValue;
                                            //childparms.ParameterValue = (string)property.Value;
                                        }
                                        else if (property.Value.Type == JTokenType.Boolean && bool.TryParse((string)property.Value, out propertyBoolValue))
                                        {
                                            childparms.ParameterDataType = "bool";
                                            paramValue = propertyBoolValue;
                                            //childparms.ParameterValue = (string)property.Value;
                                        }
                                        else
                                        {
                                            childparms.ParameterDataType = "string";
                                            paramValue = (string)property.Value;

                                        }
                                    }

                                }
                            }
                            childlist.Add(childparms);
                        }
                        //}
                        model.DeplParms = childlist;
                        bulkUploadModel.DeploymentParameterViewModel = model;

                        await formFile.CopyToAsync(stream);
                        string fileuploadPath = this.azureBlobFileClient.SaveARMTemplate(formFile, filename, fileContantType, ArmtempalteId);
                        Armtemplates armTemplate = new Armtemplates()
                        {
                            ArmtempalteName = filename,
                            TemplateLocation = fileuploadPath,
                            Isactive = true,
                            CreateDate = DateTime.Now,
                            UserId = this.userService.GetUserIdFromEmailAddress(this.CurrentUserEmailAddress),
                            ArmtempalteId = ArmtempalteId
                        };
                        model.ArmtempalteId = this.armTemplateRepository.Save(armTemplate) ?? ArmtempalteId;
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

        [HttpPost]
        public IActionResult SaveTemplateParameters(DeploymentParameterViewModel armparameters)
        {
            var currentUserDetail = usersRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);
            ArmtemplateParameters armtemplateParameters = new ArmtemplateParameters();

            if (armparameters != null && armparameters.DeplParms != null && armparameters.DeplParms.Count > 0)
            {
                foreach (var parm in armparameters.DeplParms)
                {
                    armtemplateParameters = new ArmtemplateParameters()
                    {

                        ArmtemplateId = armparameters.ArmtempalteId ?? default,
                        Parameter = parm.ParameterName,
                        ParameterDataType = parm.ParameterDataType,
                        Value = parm.ParameterValue ?? "",
                        ParameterType = parm.ParameterType,
                        CreateDate = DateTime.Now,
                        UserId = currentUserDetail.UserId
                    };
                    this.armTemplateRepository.SaveParameters(armtemplateParameters);
                }

            }
            return PartialView("SuccessMessage");
        }

        public IActionResult ArmTemplateParmeters(Guid armtemplateId)
        {
            try
            {
                if (Convert.ToBoolean(applicationConfigRepository.GetValueByName(MainMenuStatusEnum.IsLicenseManagementEnabled.ToString())) == true)
                {
                    this.TempData["ShowLicensesMenu"] = true;
                }
                if (User.Identity.IsAuthenticated)
                {
                    List<ArmtemplateParameters> armTemplateParms = new List<ArmtemplateParameters>();
                    armTemplateParms = this.armTemplateParametersRepository.GetById(armtemplateId).ToList();
                    return this.View(armTemplateParms);
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }

    }
}

