namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Repository to access ARM Templates
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.IArmTemplateRepository" />
    public class ArmTemplateRepository : IArmTemplateRepository
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly SaasKitContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArmTemplateRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public ArmTemplateRepository(SaasKitContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// The disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Armtemplates> GetAll()
        {
            return context.Armtemplates;
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Armtemplates GetById(Guid armTemplateId)
        {
            return context.Armtemplates.Where(s => s.ArmtempalteId == armTemplateId).FirstOrDefault();
        }

        /// <summary>
        /// Adds the specified Offer details.
        /// </summary>
        /// <param name="offerDetails">The Offers details.</param>
        /// <returns>Identifier of the newly created template</returns>
        public Guid? Save(Armtemplates templateDetails)
        {
            if (templateDetails != null && !string.IsNullOrEmpty(templateDetails.ArmtempalteName))
            {
                context.Armtemplates.Add(templateDetails);
                context.SaveChanges();
                return templateDetails.ArmtempalteId;
            }
            return null;
        }

        /// <summary>
        /// Adds the specified Offer details.
        /// </summary>
        /// <param name="offerDetails">The Offers details.</param>
        /// <returns></returns>
        public Guid? SaveParameters(ArmtemplateParameters templateParms)
        {
            if (templateParms != null && !string.IsNullOrEmpty(templateParms.Parameter))
            {
                context.ArmtemplateParameters.Add(templateParms);
                context.SaveChanges();
                return templateParms.ArmtemplateId;
            }
            return null;
        }

        /// <summary>
        /// Removes the specified plan details.
        /// </summary>
        /// <param name="offerDetails">The offer details.</param>
        public void Remove(Armtemplates templateDetails)
        {
            var existingTemplate = context.Armtemplates.Where(s => s.ArmtempalteId == templateDetails.ArmtempalteId).FirstOrDefault();
            if (existingTemplate != null)
            {
                context.Armtemplates.Remove(existingTemplate);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
