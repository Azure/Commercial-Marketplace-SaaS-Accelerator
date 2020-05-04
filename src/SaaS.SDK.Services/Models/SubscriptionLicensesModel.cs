using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Microsoft.Marketplace.SaaS.SDK.Services.Models
{
    public class SubscriptionLicensesModel
    {

        public int SubScriptionID { get; set; }
        public string LicenseKey { get; set; }
        public List<SubscriptionLicensesViewModel> Licenses { get; set; }
        /// <summary>
        /// Gets or sets the subscription list.
        /// </summary>
        /// <value>
        /// The subscription list.
        /// </value>
        public SelectList SubscriptionList { get; set; }
    }
}
