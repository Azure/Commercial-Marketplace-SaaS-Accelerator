using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    public class SubscriptionTemplateParametersRepository : ISubscriptionTemplateParametersRepository
    {
        private readonly SaasKitContext Context;

        public SubscriptionTemplateParametersRepository(SaasKitContext context)
        {
            Context = context;
        }
        public IEnumerable<SubscriptionTemplateParameters> GetSubscriptionTemplateParameters(Guid SubscriptionID)
        {
            var results = Context.SubscriptionTemplateParameters.Where(s => s.AmpsubscriptionId == SubscriptionID);
            if (results.Count() == 0)
                return null;
            else
                return Context.SubscriptionTemplateParameters.Where(s => s.AmpsubscriptionId == SubscriptionID);
        }
    }
}



