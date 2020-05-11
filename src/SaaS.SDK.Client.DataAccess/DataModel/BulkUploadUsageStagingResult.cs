namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.DataModel
{
    using System;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

    /// <summary>
    /// Bulk Upload Usage Staging Result.
    /// </summary>
    public class BulkUploadUsageStagingResult
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the batch log identifier.
        /// </summary>
        /// <value>
        /// The batch log identifier.
        /// </value>
        public int? BatchLogId { get; set; }

        /// <summary>
        /// Gets or sets the subscription identifier.
        /// </summary>
        /// <value>
        /// The subscription identifier.
        /// </value>
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the apitype.
        /// </summary>
        /// <value>
        /// The apitype.
        /// </value>
        public string Apitype { get; set; }

        /// <summary>
        /// Gets or sets the consumed units.
        /// </summary>
        /// <value>
        /// The consumed units.
        /// </value>
        public string ConsumedUnits { get; set; }

        /// <summary>
        /// Gets or sets the validation status.
        /// </summary>
        /// <value>
        /// The validation status.
        /// </value>
        public bool? ValidationStatus { get; set; }

        /// <summary>
        /// Gets or sets the validation error detail.
        /// </summary>
        /// <value>
        /// The validation error detail.
        /// </value>
        public string ValidationErrorDetail { get; set; }

        /// <summary>
        /// Gets or sets the staged on.
        /// </summary>
        /// <value>
        /// The staged on.
        /// </value>
        public DateTime? StagedOn { get; set; }

        /// <summary>
        /// Gets or sets the processed on.
        /// </summary>
        /// <value>
        /// The processed on.
        /// </value>
        public DateTime? ProcessedOn { get; set; }

        /// <summary>
        /// Gets or sets the batch log.
        /// </summary>
        /// <value>
        /// The batch log.
        /// </value>
        public virtual BatchLog BatchLog { get; set; }
    }
}
