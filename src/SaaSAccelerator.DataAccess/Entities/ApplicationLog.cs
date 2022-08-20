using System;

namespace Microsoft.Marketplace.SaaSAccelerator.DataAccess.Entities
{
    public partial class ApplicationLog
    {
        public int Id { get; set; }
        public DateTime? ActionTime { get; set; }
        public string LogDetail { get; set; }
    }
}
