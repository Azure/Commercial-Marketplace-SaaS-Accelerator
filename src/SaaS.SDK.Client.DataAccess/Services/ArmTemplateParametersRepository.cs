namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

    /// <summary>
    /// Repository to access ARM template parameters.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.IArmTemplateParametersRepository" />
    public class ArmTemplateParametersRepository : IArmTemplateParametersRepository
    {
        /// <summary>
        /// The this.context.
        /// </summary>
        private readonly SaasKitContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArmTemplateParametersRepository"/> class.
        /// </summary>
        /// <param name="context">The this.context.</param>
        public ArmTemplateParametersRepository(SaasKitContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Gets all ARM Templates with parameters.
        /// </summary>
        /// <returns>List of ARM Template parameters.</returns>
        public IEnumerable<ArmtemplateParameters> GetAll()
        {
            return this.context.ArmtemplateParameters;
        }

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <param name="armTemplateID">The arm template identifier.</param>
        /// <returns>
        /// List of ARM Template parameters for the given template ID.
        /// </returns>
        public IEnumerable<ArmtemplateParameters> GetById(Guid armTemplateID)
        {
            return this.context.ArmtemplateParameters.Where(s => s.ArmtemplateId == armTemplateID);
        }
    }
}
