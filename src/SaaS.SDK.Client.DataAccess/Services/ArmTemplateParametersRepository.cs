using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    public class ArmTemplateParametersRepository: IArmTemplateParametersRepository
    {
        private readonly SaasKitContext Context;

        public ArmTemplateParametersRepository(SaasKitContext context)
        {
            Context = context;
        }
        public IEnumerable<ArmtemplateParameters> Get()
        {
            return Context.ArmtemplateParameters;
        }

        public IEnumerable<ArmtemplateParameters> GetArmtemplatesByID(Guid ArmTemplateID)
        {
            return Context.ArmtemplateParameters.Where(s => s.ArmtemplateId == ArmTemplateID);
        }


    }
}
