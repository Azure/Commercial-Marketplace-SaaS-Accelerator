using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    public interface IArmTemplateRepository : IDisposable //, IBaseRepository<ARMTemplates>
    {
        Guid? Add(Armtemplates templateDetails);
        Guid? AddTemplateParameters(ArmtemplateParameters templateParms);
        IEnumerable<Armtemplates> Get();

    }
}
