namespace Microsoft.Marketplace.SaaS.SDK.Services.Models
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

    /// <summary>
    /// Subscription Usage ViewModel.
    /// </summary>
    public class SubscriptionUsageViewModel
    {
        /// <summary>
        /// Gets or sets the subscription detail.
        /// </summary>
        /// <value>
        /// The subscription detail.
        /// </value>
        public Subscriptions SubscriptionDetail { get; set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        /// <value>
        /// The quantity.
        /// </value>
        public string Quantity { get; set; }

        /// <summary>
        /// Gets or sets the dimensions list.
        /// </summary>
        /// <value>
        /// The dimensions list.
        /// </value>
        public SelectList DimensionsList { get; set; }

        /// <summary>Gets or sets the selected dimension.</summary>
        /// <value>The selected dimension.</value>
        public string SelectedDimension { get; set; }

        /// <summary>
        /// Gets or sets the metered audit logs.
        /// </summary>
        /// <value>
        /// The metered audit logs.
        /// </value>
        public List<MeteredAuditLogs> MeteredAuditLogs { get; set; }
    }
}