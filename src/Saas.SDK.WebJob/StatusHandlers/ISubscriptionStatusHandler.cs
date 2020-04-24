using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.WebJob.StatusHandlers
{
    interface ISubscriptionStatusHandler
    {
        void Process(Guid subscriptionID);
    }
}
