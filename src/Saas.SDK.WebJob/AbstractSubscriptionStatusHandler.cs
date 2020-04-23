using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using System.Linq;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

namespace Microsoft.Marketplace.SaasKit.WebJob
{

    abstract class AbstractSubscriptionStatusHandler : ISubscriptionStatusHandler
    {
        private readonly SaasKitContext Context;
        protected Subscriptions GetSubscriptionById(Guid subscriptionId)
        {
            return Context.Subscriptions.Where(x => x.AmpsubscriptionId == subscriptionId).FirstOrDefault();
        }
        public abstract void Process(Guid subscriptionID);
    }
}

