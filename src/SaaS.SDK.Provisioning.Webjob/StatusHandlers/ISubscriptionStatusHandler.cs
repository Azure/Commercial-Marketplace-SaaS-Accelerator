using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.Provisioning.Webjob.StatusHandlers
{
    interface ISubscriptionStatusHandler
    {
        void Process(Guid subscriptionID);
    }
}
