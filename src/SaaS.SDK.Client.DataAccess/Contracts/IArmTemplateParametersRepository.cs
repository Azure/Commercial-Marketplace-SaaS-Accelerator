namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

    /// <summary>
    /// Repository to access ARM Template and parameters.
    /// </summary>
    public interface IArmTemplateParametersRepository
    {
        /// <summary>
        /// Gets all ARM Templates with parameters.
        /// </summary>
        /// <returns> Armtemplate Parameters.</returns>
        IEnumerable<ArmtemplateParameters> GetAll();

        /// <summary>
        /// Gets the ARM Template and parameters by the template identifier.
        /// </summary>
        /// <param name="armTemplateID">The arm template identifier.</param>
        /// <returns>List of ARM Template parameters for a given template ID.</returns>
        IEnumerable<ArmtemplateParameters> GetById(Guid armTemplateID);
    }
}
