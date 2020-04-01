using SaaS.SDK.Client.DataAccess.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Marketplace.Saas.Web.Models
{
    public class TemplateModel
    {
        public int ArmtempalteId { get; set; }
        public string ArmtempalteName { get; set; }
        public string TemplateLocation { get; set; }
        public bool? Isactive { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? UserId { get; set; }
        public List<BulkUploadUsageStagingResult> BulkUploadUsageStagings { get; set; }
    }
}
