namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

    /// <summary>
    /// Repository to access ARM template parameters associated with SaaS subscription.
    /// </summary>
    public interface ISubscriptionTemplateParametersRepository
    {
        /// <summary>
        /// Gets the template parameters by subscription identifier.
        /// </summary>
        /// <param name="subscriptionID">The subscription identifier.</param>
        /// <returns>
        /// List of ARM template parameters associated with the SaaS subscription.
        /// </returns>
        IEnumerable<SubscriptionTemplateParameters> GetTemplateParametersBySubscriptionId(Guid subscriptionID);

        /// <summary>
        /// Saves the specified subscription template parameters.
        /// </summary>
        /// <param name="subscriptionTemplateParameters">The subscription template parameters.</param>
        /// <returns>Template Id.</returns>
        int Save(SubscriptionTemplateParameters subscriptionTemplateParameters);

        /// <summary>
        /// Gets the subscription template parameter by identifier.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="planId">The plan identifier.</param>
        /// <returns>List of resource deployment parameters related to the subscription.</returns>
        List<SubscriptionTemplateParameters> GetById(Guid subscriptionId, Guid planId);

        /// <summary>
        /// Update subscription template parameter by identifier.
        /// </summary>
        /// <param name="parms">The parms.</param>
        /// <param name="subscriptionID">The subscription identifier.</param>
        void Update(List<SubscriptionTemplateParameters> parms, Guid subscriptionID);
    }
}
