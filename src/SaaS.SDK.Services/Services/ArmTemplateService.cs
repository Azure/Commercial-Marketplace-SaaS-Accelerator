namespace Microsoft.Marketplace.SaaS.SDK.Services.Services
{
    using System.Collections.Generic;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;

    /// <summary>
    /// Arm Template Service.
    /// </summary>
    public class ArmTemplateService
    {
        /// <summary>
        /// The arm template repository.
        /// </summary>
        private readonly IArmTemplateRepository armTemplateRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArmTemplateService"/> class.
        /// </summary>
        /// <param name="armTemplateRepository">The arm template repository.</param>
        public ArmTemplateService(IArmTemplateRepository armTemplateRepository)
        {
            this.armTemplateRepository = armTemplateRepository;
        }

        /// <summary>
        /// Gets the arm templates.
        /// </summary>
        /// <returns> ARMTemplateViewModel list.</returns>
        public List<ARMTemplateViewModel> GetARMTemplates()
        {
            List<ARMTemplateViewModel> armTemplateList = new List<ARMTemplateViewModel>();
            var allTemplates = this.armTemplateRepository.GetAll();
            foreach (var item in allTemplates)
            {
                ARMTemplateViewModel armTemplate = new ARMTemplateViewModel();
                armTemplate.Id = item.Id;
                armTemplate.ArmtempalteId = item.ArmtempalteId;
                armTemplate.ArmtempalteName = item.ArmtempalteName;
                armTemplate.TemplateLocation = item.TemplateLocation;
                armTemplate.Isactive = item.Isactive;
                armTemplate.CreateDate = item.CreateDate;
                armTemplate.UserId = item.UserId;
                armTemplateList.Add(armTemplate);
            }

            return armTemplateList;
        }
    }
}
