namespace Microsoft.Marketplace.SaaS.SDK.Services.Models
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.DataModel;

    /// <summary>
    /// Tenamts Model.
    /// </summary>
    public class TemplateModel
    {
        /// <summary>
        /// Gets or sets the armtempalte identifier.
        /// </summary>
        /// <value>
        /// The armtempalte identifier.
        /// </value>
        public int ArmtempalteId { get; set; }

        /// <summary>
        /// Gets or sets the name of the armtempalte.
        /// </summary>
        /// <value>
        /// The name of the armtempalte.
        /// </value>
        public string ArmtempalteName { get; set; }

        /// <summary>
        /// Gets or sets the template location.
        /// </summary>
        /// <value>
        /// The template location.
        /// </value>
        public string TemplateLocation { get; set; }

        /// <summary>
        /// Gets or sets the isactive.
        /// </summary>
        /// <value>
        /// The isactive.
        /// </value>
        public bool? Isactive { get; set; }

        /// <summary>
        /// Gets or sets the create date.
        /// </summary>
        /// <value>
        /// The create date.
        /// </value>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public int? UserId { get; set; }

        /// <summary>
        /// Gets or sets the bulk upload usage stagings.
        /// </summary>
        /// <value>
        /// The bulk upload usage stagings.
        /// </value>
        public List<BulkUploadUsageStagingResult> BulkUploadUsageStagings { get; set; }

        /// <summary>
        /// Gets or sets the deployment parameter view model.
        /// </summary>
        /// <value>
        /// The deployment parameter view model.
        /// </value>
        public DeploymentParameterViewModel DeploymentParameterViewModel { get; set; }
    }
}
