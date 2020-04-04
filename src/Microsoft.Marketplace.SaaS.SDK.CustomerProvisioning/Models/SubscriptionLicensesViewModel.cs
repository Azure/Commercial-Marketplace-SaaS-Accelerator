namespace Microsoft.Marketplace.SaasKit.Client.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// the SubscriptionLicensesViewModel
    /// </summary>
    public class SubscriptionLicensesViewModel
    {
        /// <summary>
        /// Gets or sets the subscription identifier.
        /// </summary>
        /// <value>
        /// The subscription identifier.
        /// </value>
        public string AmpsubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the name of the subscription.
        /// </summary>
        /// <value>
        /// The name of the subscription.
        /// </value>
        public string SubscriptionName { get; set; }

        /// <summary>
        /// Gets or sets the name of the plan.
        /// </summary>
        /// <value>
        /// The name of the plan.
        /// </value>
        public string PlanName { get; set; }

        /// <summary>
        /// Gets or sets the license key.
        /// </summary>
        /// <value>
        /// The license key.
        /// </value>
        public string LicenseKey { get; set; }
    }
}
