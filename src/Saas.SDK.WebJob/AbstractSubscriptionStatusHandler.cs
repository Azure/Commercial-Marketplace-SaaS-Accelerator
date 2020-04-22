using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.WebJob
{

    abstract class AbstractSubscriptionStatusHandler //: ISubscriptionStatusHandler
    {
        protected Subscription GetSubscriptionById(int subscriptionId)
        {
            return subscription.query(x => x.Id == subscriptionId).FirstOrDefault();
        }
    }
}

