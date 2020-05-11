namespace Microsoft.Marketplace.SaaS.SDK.Services.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Deployment Parameter View Model.
    /// </summary>
    public class DeploymentParameterViewModel
    {
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the armtempalte identifier.
        /// </summary>
        /// <value>
        /// The armtempalte identifier.
        /// </value>
        public Guid? ArmtempalteId { get; set; }

        /// <summary>
        /// Gets or sets the depl parms.
        /// </summary>
        /// <value>
        /// The depl parms.
        /// </value>
        public List<ChindParameterViewModel> DeplParms { get; set; }

        /// <summary>
        /// Gets or sets the arm parms.
        /// </summary>
        /// <value>
        /// The arm parms.
        /// </value>
        public List<ARMTemplateViewModel> ARMParms { get; set; }
    }
}
