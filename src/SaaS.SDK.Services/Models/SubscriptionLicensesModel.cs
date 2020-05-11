namespace Microsoft.Marketplace.SaaS.SDK.Services.Models
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc.Rendering;

    /// <summary>
    /// Subscription Licenses Model.
    /// </summary>
    public class SubscriptionLicensesModel
    {
        /// <summary>
        /// Gets or sets the sub scription identifier.
        /// </summary>
        /// <value>
        /// The sub scription identifier.
        /// </value>
        public int SubScriptionID { get; set; }

        /// <summary>
        /// Gets or sets the license key.
        /// </summary>
        /// <value>
        /// The license key.
        /// </value>
        public string LicenseKey { get; set; }

        /// <summary>
        /// Gets or sets the licenses.
        /// </summary>
        /// <value>
        /// The licenses.
        /// </value>
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
