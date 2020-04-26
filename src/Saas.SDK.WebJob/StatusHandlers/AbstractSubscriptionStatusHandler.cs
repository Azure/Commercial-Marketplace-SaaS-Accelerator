using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using System.Linq;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Marketplace.SaasKit.WebJob.StatusHandlers
{

    abstract class AbstractSubscriptionStatusHandler : DbContext, ISubscriptionStatusHandler
    {
        protected SaasKitContext Context;


        public AbstractSubscriptionStatusHandler(SaasKitContext context)
        {
            Context = context;
        }
        protected Subscriptions GetSubscriptionById(Guid subscriptionId)
        {
            return Context.Subscriptions.Where(x => x.AmpsubscriptionId == subscriptionId).FirstOrDefault();
        }

        protected Plans GetPlanById(string planId)
        {
            return Context.Plans.Where(x => x.PlanId == planId).FirstOrDefault();
        }
        public abstract void Process(Guid subscriptionID);
    }
}

