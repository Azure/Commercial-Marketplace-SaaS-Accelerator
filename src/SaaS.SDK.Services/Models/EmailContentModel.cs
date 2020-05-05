using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Marketplace.SaaS.SDK.Services.Models
{
    public class EmailContentModel
    {
        public string FromEmail { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Port { get; set; }
        public bool SSL { get; set; }
        public string Subject { get; set; }
        public string SMTPHost { get; set; }
        public string Body { get; set; }
        public string ToEmails { get; set; }
        public string CCEmails { get; set; }
        public string BCCEmails { get; set; }
        public string CustomerEmail { get; set; }
        public bool CopyToCustomer { get; set; }



    }
}
