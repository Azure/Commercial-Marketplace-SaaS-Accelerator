namespace Microsoft.Marketplace.Saas.Web.Models
{
    using Microsoft.Marketplace.SaasKit.Models;
    using System.Collections.Generic;

    /// <summary>
    /// Subscription ViewModel
    /// </summary>
    public class SubscriptionViewModel
    {
        /// <summary>
        /// Gets or sets the subscriptions.
        /// </summary>
        /// <value>
        /// The subscriptions.
        /// </value>
        public List<SubscriptionResult> Subscriptions { get; set; } = new List<SubscriptionResult>();

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is success.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is success; otherwise, <c>false</c>.
        /// </value>
        public bool IsSuccess { get; set; }

        public bool IsAutomaticProvisioningSupported { get; set; }
    }
}
