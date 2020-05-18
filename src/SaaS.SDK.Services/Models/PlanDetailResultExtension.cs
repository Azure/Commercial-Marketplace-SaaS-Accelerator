namespace Microsoft.Marketplace.SaaS.SDK.Services.Models
{
    using System;
    using Microsoft.Marketplace.SaasKit.Models;

    /// <summary>
    /// PlanDetail Result Extension.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Models.PlanDetailResult" />
    public class PlanDetailResultExtension : PlanDetailResult
    {
        /// <summary>
        /// Gets or sets the offer identifier.
        /// </summary>
        /// <value>
        /// The offer identifier.
        /// </value>
        public Guid OfferId { get; set; }

        /// <summary>
        /// Gets or sets the plan unique identifier.
        /// </summary>
        /// <value>
        /// The plan unique identifier.
        /// </value>
        public Guid PlanGUID { get; set; }
    }
}
