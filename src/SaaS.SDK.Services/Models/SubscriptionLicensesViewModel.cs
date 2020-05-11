namespace Microsoft.Marketplace.SaaS.SDK.Services.Models
{
    using Microsoft.AspNetCore.Mvc.Rendering;

    /// <summary>
    /// SubscriptionLicenses ViewModel.
    /// </summary>
    public class SubscriptionLicensesViewModel
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the subscription identifier.
        /// </summary>
        /// <value>
        /// The subscription identifier.
        /// </value>
        public int SubScriptionID { get; set; }

        /// <summary>
        /// Gets or sets the ampsubscription identifier.
        /// </summary>
        /// <value>
        /// The ampsubscription identifier.
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

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SubscriptionLicensesViewModel" /> is status.
        /// </summary>
        /// <value>
        ///   <c>true</c> if status; otherwise, <c>false</c>.
        /// </value>
        public bool Status { get; set; }

        /// <summary>
        /// Gets or sets the subscription list.
        /// </summary>
        /// <value>
        /// The subscription list.
        /// </value>
        public SelectList SubscriptionList { get; set; }
    }
}
