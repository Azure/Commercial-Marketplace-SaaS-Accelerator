namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SubscriptionTemplateParametersRepository : ISubscriptionTemplateParametersRepository
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly SaasKitContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionTemplateParametersRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public SubscriptionTemplateParametersRepository(SaasKitContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Gets the template parameters by subscription identifier.
        /// </summary>
        /// <param name="SubscriptionID">The subscription identifier.</param>
        /// <returns>
        /// List of ARM template parameters associated with the SaaS subscription
        /// </returns>
        public IEnumerable<SubscriptionTemplateParameters> GetTemplateParametersBySubscriptionId(Guid SubscriptionID)
        {
            var results = context.SubscriptionTemplateParameters.Where(s => s.AmpsubscriptionId == SubscriptionID);
            if (results.Count() == 0)
                return null;
            else
                return context.SubscriptionTemplateParameters.Where(s => s.AmpsubscriptionId == SubscriptionID);
        }
    }
}