using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Marketplace.SaaS.SDK.Services.Models
{
    public class EmailSMTPConfigs
    {
        public string FromEmail { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Port { get; set; }
        public bool SSL { get; set; }
        public string Subject { get; set; }
        public string SMTPHost { get; set; }
    }
}
