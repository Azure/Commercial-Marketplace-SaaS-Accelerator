namespace Microsoft.Marketplace.SaaS.SDK.Services.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

    /// <summary>
    /// Subscription Usage ViewModel.
    /// </summary>
    public class SchedulerUsageViewModel
    {
        public SelectList SubscriptionList { get; set; }
        public string SelectedSubscription { get; set; }


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


        public SelectList SchedulerFrequencyList { get; set; }
        public string SelectedSchedulerFrequency { get; set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        /// <value>
        /// The quantity.
        /// </value>
        public string Quantity { get; set; }

        /// <summary>
        /// Get or set First Run Time
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FirstRunDate { get; set; }


        /// <summary>
        /// Get or set Next Run Time
        /// </summary>
        public DateTime NextRunDate { get; set; }

        /// <summary>
        /// Gets or sets the metered audit logs.
        /// </summary>
        /// <value>
        /// The metered audit logs.
        /// </value>
        /// 
        public List<MeteredAuditLogs> MeteredAuditLogs { get; set; }

    }
}