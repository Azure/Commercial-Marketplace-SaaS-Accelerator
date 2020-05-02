using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Marketplace.SaaS.SDK.Services.Helpers
{
    public static class HashTablelHelper
    {
        public static Hashtable MapCredentials(Dictionary<string, string> credenitals)
        {
            Hashtable hashTable = new Hashtable();
            foreach (var cred in credenitals)
            {
                hashTable.Add(cred.Key, cred.Value);
            }
            return hashTable;

        }
    }
}
