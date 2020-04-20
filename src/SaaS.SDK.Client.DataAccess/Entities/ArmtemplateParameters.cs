using System;
using System.Collections.Generic;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities
{
    public partial class ArmtemplateParameters
    {
        public int Id { get; set; }
        public Guid ArmtemplateId { get; set; }
        public string Parameter { get; set; }
        public string ParameterDataType { get; set; }
        public string Value { get; set; }
        public string ParameterType { get; set; }
        public DateTime CreateDate { get; set; }
        public int UserId { get; set; }
    }
}
