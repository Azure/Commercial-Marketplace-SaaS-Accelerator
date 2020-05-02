using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Marketplace.SaaS.SDK.Library.Models
{
    public class DeploymentParameterViewModel
    {
        public string FileName { get; set; }
        public Guid? ArmtempalteId { get; set; }

        public List<ChindParameterViewModel> DeplParms { get; set; }

    }

    public class ChindParameterViewModel
    {
        public string ParameterName { get; set; }
        public string ParameterValue { get; set; }
        public string ParameterDataType { get; set; }
        public string ParameterType { get; set; }
    }
}
