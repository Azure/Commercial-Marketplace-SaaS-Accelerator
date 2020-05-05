namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Repository to access ARM template parameters
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.IArmTemplateParametersRepository" />
    public class ArmTemplateParametersRepository: IArmTemplateParametersRepository
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly SaasKitContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArmTemplateParametersRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public ArmTemplateParametersRepository(SaasKitContext context)
        {
            this.context = context;
        }
        /// <summary>
        /// Gets all ARM Templates with parameters
        /// </summary>
        /// <returns>List of ARM Template parameters</returns>
        public IEnumerable<ArmtemplateParameters> GetAll()
        {
            return context.ArmtemplateParameters;
        }

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <param name="ArmTemplateID">The arm template identifier.</param>
        /// <returns>List of ARM Template parameters for the given template ID</returns>
        public IEnumerable<ArmtemplateParameters> GetById(Guid ArmTemplateID)
        {
            return context.ArmtemplateParameters.Where(s => s.ArmtemplateId == ArmTemplateID);
        }
    }
}
