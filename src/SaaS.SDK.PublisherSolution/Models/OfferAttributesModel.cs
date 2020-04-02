using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Marketplace.Saas.Web.Models
{
    public class OfferAttributesModel
    {
        public int AttributeID { get; set; }
        public string ParameterId { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public int ValueTypeId { get; set; }
        public bool FromList { get; set; }
        public string ValuesList { get; set; }
        public int Max { get; set; }
        public int Min { get; set; }
        public int OfferID { get; set; }
        public string Type { get; set; }
        public int DisplaySequence { get; set; }
        public string Isactive { get; set; }

        public bool IsRemove { get; set; }
        public string UserId { get; set; }
    }
}
