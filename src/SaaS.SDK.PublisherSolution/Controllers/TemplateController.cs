namespace Microsoft.Marketplace.Saas.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Marketplace.SaaS.SDK.Services.Contracts;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using Microsoft.Marketplace.SaaS.SDK.Services.Services;
    using Microsoft.Marketplace.SaaS.SDK.Services.Utilities;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.DataModel;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Template Controller to manage ARM templates.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.Saas.Web.Controllers.BaseController" />
    [ServiceFilter(typeof(KnownUserAttribute))]
    public class TemplateController : BaseController
    {
        private readonly IUsersRepository usersRepository;

        private readonly IApplicationConfigRepository applicationConfigRepository;

        private readonly IKnownUsersRepository knownUsersRepository;

        private readonly IUsersRepository userRepository;

        private readonly IArmTemplateRepository armTemplateRepository;

        private readonly IARMTemplateStorageService azureBlobFileClient;

        private readonly IArmTemplateParametersRepository armTemplateParametersRepository;

        private ArmTemplateService armTemplateService;

        private UserService userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateController"/> class.
        /// </summary>
        /// <param name="usersRepository">The users repository.</param>
        /// <param name="applicationConfigRepository">The application configuration repository.</param>
        /// <param name="knownUsersRepository">The known users repository.</param>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="armTemplateRepository">The arm template repository.</param>
        /// <param name="armTemplateParametersRepository">The arm template parameters repository.</param>
        /// <param name="azureBlobFileClient">The azure BLOB file client.</param>
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

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns> Return All ARM templates.</returns>
        public IActionResult Index()
        {
            this.ModelState.Clear();
            DeploymentParameterViewModel model = new DeploymentParameterViewModel();
            List<ARMTemplateViewModel> armTemplates = new List<ARMTemplateViewModel>();
            armTemplates = this.armTemplateService.GetARMTemplates();
            model.ARMParms = armTemplates;
            model.FileName = string.Empty;
            try
            {
                bool isKnownUser = this.knownUsersRepository.GetKnownUserDetail(this.CurrentUserEmailAddress, 1)?.Id > 0;
                if (this.User.Identity.IsAuthenticated && isKnownUser)
                {
                    var newBatchModel = new TemplateModel();
                    newBatchModel.BulkUploadUsageStagings = new List<BulkUploadUsageStagingResult>();
                    newBatchModel.DeploymentParameterViewModel = model;
                    return this.View(model);
                }
                else
                {
                    return this.View("Error", new ErrorViewModel { IsKnownUser = isKnownUser });
                }
            }
            catch (Exception ex)
            {
                return this.View("Error", ex);
            }
        }

        /// <summary>
        /// Uploads the batch usage.
        /// </summary>
        /// <param name="uploadfile">The uploadfile.</param>
        /// <returns> Parameters after file update.</returns>
        [HttpPost("UploadBatchUsage")]
        public async Task<IActionResult> UploadBatchUsage(List<IFormFile> uploadfile)
        {
            BatchUsageUploadModel bulkUploadModel = new BatchUsageUploadModel();
            bulkUploadModel.BulkUploadUsageStagings = new List<BulkUploadUsageStagingResult>();
            bulkUploadModel.BatchLogId = 0;
            DeploymentParameterViewModel model = new DeploymentParameterViewModel();
            List<ChindParameterViewModel> childlist = new List<ChindParameterViewModel>();

            ResponseModel response = new ResponseModel();
            var currentUserDetail = this.usersRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);
            var filename = string.Empty;
            var filePath = string.Empty;
            var fileContantType = string.Empty;
            var formFile = uploadfile.FirstOrDefault();
            var armtempalteId = Guid.NewGuid();
            if (formFile.Length > 0)
            {
                filename = formFile.FileName;

                fileContantType = formFile.ContentType;
                string fileExtension = Path.GetExtension(formFile.FileName);
                if (fileExtension == ".json")
                {
                    filePath = Path.GetTempFileName();
                    model.FileName = filename;

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        string str = new StreamReader(formFile.OpenReadStream()).ReadToEnd();
                        dynamic result = JObject.Parse(str);

                        ChindParameterViewModel childparms = new ChindParameterViewModel();
                        childparms.ParameterDataType = "string";
                        childparms.ParameterName = "ResourceGroup";
                        childparms.ParameterValue = string.Empty;
                        childparms.ParameterType = "input";
                        childlist.Add(childparms);
                        childparms = new ChindParameterViewModel();
                        childparms.ParameterDataType = "string";
                        childparms.ParameterName = "Location";
                        childparms.ParameterValue = string.Empty;
                        childparms.ParameterType = "input";
                        childlist.Add(childparms);

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
                                        }
                                        else if (property.Value.Type == JTokenType.Integer && int.TryParse((string)property.Value, out propertyIntValue))
                                        {
                                            childparms.ParameterDataType = "int";
                                            paramValue = propertyIntValue;
                                        }
                                        else if (property.Value.Type == JTokenType.Boolean && bool.TryParse((string)property.Value, out propertyBoolValue))
                                        {
                                            childparms.ParameterDataType = "bool";
                                            paramValue = propertyBoolValue;
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

                        model.DeplParms = childlist;

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
                                        }
                                        else if (property.Value.Type == JTokenType.Integer && int.TryParse((string)property.Value, out propertyIntValue))
                                        {
                                            childparms.ParameterDataType = "int";
                                            paramValue = propertyIntValue;
                                        }
                                        else if (property.Value.Type == JTokenType.Boolean && bool.TryParse((string)property.Value, out propertyBoolValue))
                                        {
                                            childparms.ParameterDataType = "bool";
                                            paramValue = propertyBoolValue;
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

                        model.DeplParms = childlist;
                        bulkUploadModel.DeploymentParameterViewModel = model;

                        await formFile.CopyToAsync(stream);
                        string fileuploadPath = this.azureBlobFileClient.SaveARMTemplate(formFile, filename, fileContantType, armtempalteId);
                        Armtemplates armTemplate = new Armtemplates()
                        {
                            ArmtempalteName = filename,
                            TemplateLocation = fileuploadPath,
                            Isactive = true,
                            CreateDate = DateTime.Now,
                            UserId = this.userService.GetUserIdFromEmailAddress(this.CurrentUserEmailAddress),
                            ArmtempalteId = armtempalteId,
                        };
                        model.ArmtempalteId = this.armTemplateRepository.Save(armTemplate) ?? armtempalteId;
                    }
                }
                else
                {
                    response.Message = "Please select JSON file.";
                    response.IsSuccess = false;

                    return this.View("RecordBatchUsage", response);
                }
            }
            else
            {
                response.Message = "Please select File!";
                response.IsSuccess = false;
            }

            bulkUploadModel.Response = response;
            return this.View("Index", model);
        }

        /// <summary>
        /// Saves the template parameters.
        /// </summary>
        /// <param name="armparameters">The armparameters.</param>
        /// <returns> Saved Parameters.</returns>
        [HttpPost]
        public IActionResult SaveTemplateParameters(DeploymentParameterViewModel armparameters)
        {
            var currentUserDetail = this.usersRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);
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
                        Value = parm.ParameterValue ?? string.Empty,
                        ParameterType = parm.ParameterType,
                        CreateDate = DateTime.Now,
                        UserId = currentUserDetail.UserId,
                    };
                    this.armTemplateRepository.SaveParameters(armtemplateParameters);
                }
            }

            return this.PartialView("SuccessMessage");
        }

        /// <summary>
        /// Arms the template parmeters.
        /// </summary>
        /// <param name="armtemplateId">The armtemplate identifier.</param>
        /// <returns> IActionResult.</returns>
        public IActionResult ArmTemplateParmeters(Guid armtemplateId)
        {
            try
            {
                if (Convert.ToBoolean(this.applicationConfigRepository.GetValueByName(MainMenuStatusEnum.IsLicenseManagementEnabled.ToString())) == true)
                {
                    this.TempData["ShowLicensesMenu"] = true;
                }

                if (this.User.Identity.IsAuthenticated)
                {
                    List<ArmtemplateParameters> armTemplateParms = new List<ArmtemplateParameters>();
                    armTemplateParms = this.armTemplateParametersRepository.GetById(armtemplateId).ToList();
                    return this.View(armTemplateParms);
                }
                else
                {
                    return this.RedirectToAction(nameof(this.Index));
                }
            }
            catch (Exception ex)
            {
                return this.View("Error", ex);
            }
        }
    }
}