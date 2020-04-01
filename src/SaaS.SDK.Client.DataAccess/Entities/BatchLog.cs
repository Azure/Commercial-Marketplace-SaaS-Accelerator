using System;
using System.Collections.Generic;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities
{
    public partial class BatchLog
    {
        public BatchLog()
        {
            BulkUploadUsageStaging = new HashSet<BulkUploadUsageStaging>();
        }

        public int Id { get; set; }
        public Guid? ReferenceId { get; set; }
        public string FileName { get; set; }
        public string UploadedBy { get; set; }
        public DateTime? UploadedOn { get; set; }
        public string BatchStatus { get; set; }

        public virtual ICollection<BulkUploadUsageStaging> BulkUploadUsageStaging { get; set; }
    }
}
