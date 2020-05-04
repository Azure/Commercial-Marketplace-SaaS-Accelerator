using Microsoft.Marketplace.Saas.Web.Models;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Marketplace.SaasKit.Client.Services
{
    public class ArmTemplateService
    {

        private readonly IArmTemplateRepository armTemplateRepository;

        public ArmTemplateService armTemplateService;

        public ArmTemplateService(IArmTemplateRepository armTemplateRepository)
        {
            this.armTemplateRepository = armTemplateRepository;
        }

        public List<ARMTemplateViewModel> GetARMTemplates()
        {
            List<ARMTemplateViewModel> armTemplateList = new List<ARMTemplateViewModel>();
            var allTemplates = this.armTemplateRepository.Get();
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
