namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities
{
    using System;

    /// <summary>
    /// Bulk Upload Usage Staging.
    /// </summary>
    public partial class BulkUploadUsageStaging
    {
        public int Id { get; set; }
        public int? BatchLogId { get; set; }
        public string SubscriptionId { get; set; }
        public string Apitype { get; set; }
        public string ConsumedUnits { get; set; }
        public bool? ValidationStatus { get; set; }
        public string ValidationErrorDetail { get; set; }
        public DateTime? StagedOn { get; set; }
        public DateTime? ProcessedOn { get; set; }

        public virtual BatchLog BatchLog { get; set; }
    }
}
