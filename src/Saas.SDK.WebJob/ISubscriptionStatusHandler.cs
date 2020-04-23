using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.WebJob
{
    interface ISubscriptionStatusHandler
    {
        void Process(Guid subscriptionID);
    }
}
