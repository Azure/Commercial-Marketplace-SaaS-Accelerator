using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.WebJob.Helpers
{
    public static class HashTablelHelper
    {
        public static Hashtable MapCredentials(List<SubscriptionParametersOutput> credenitals)
        {
            Hashtable hashTable = new Hashtable();
            foreach (var cred in credenitals)
            {
                hashTable.Add(cred.DisplayName, cred.Value);
            }
            return hashTable;

        }
    }
}
