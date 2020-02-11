namespace Microsoft.Marketplace.SaasKit.Models
{
    using System;

    /// <summary>
    /// Subscription Usage Request
    /// </summary>
    public class MeteringUsageRequest
    {
        /// <summary>Gets or sets the quantity.</summary>
        /// <value>The quantity.</value>
        public double Quantity { get; set; }

        /// <summary>Gets or sets the resource identifier.</summary>
        /// <value>The resource identifier.</value>
        public Guid ResourceId{ get; set; }

        /// <summary>Gets or sets the dimension.</summary>
        /// <value>The dimension.</value>
        public string Dimension { get; set; }

        /// <summary>Gets or sets the effective start time.</summary>
        /// <value>The effective start time.</value>
        public DateTime EffectiveStartTime { get; set; }

        /// <summary>Gets or sets the plan identifier.</summary>
        /// <value>The plan identifier.</value>
        public string PlanId { get; set; }
    }
}