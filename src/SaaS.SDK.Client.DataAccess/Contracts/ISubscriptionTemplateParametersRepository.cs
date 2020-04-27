using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;


namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    public interface ISubscriptionTemplateParametersRepository
    {
        SubscriptionTemplateParameters GetSubscriptionTemplateParameters(Guid SubscriptionID);
    }
}
