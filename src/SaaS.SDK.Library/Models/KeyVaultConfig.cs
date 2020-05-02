using System;
using System.Collections.Generic;
using System.Text;

namespace SaaS.SDK.Provisioning.Webjob.Models
{
    public class KeyVaultConfig
    {
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string TenantID { get; set; }
        public string KeyVaultUrl { get; set; }
    }
}
