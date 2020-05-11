namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

    /// <summary>
    /// Repository to access ARM Templates.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.IArmTemplateRepository" />
    public class ArmTemplateRepository : IArmTemplateRepository
    {
        /// <summary>
        /// The this.context.
        /// </summary>
        private readonly SaasKitContext context;

        /// <summary>
        /// The disposed.
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArmTemplateRepository"/> class.
        /// </summary>
        /// <param name="context">The this.context.</param>
        public ArmTemplateRepository(SaasKitContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns> Armtemplates.</returns>
        public IEnumerable<Armtemplates> GetAll()
        {
            return this.context.Armtemplates;
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="armTemplateId">The arm template identifier.</param>
        /// <returns>
        /// Armtemplates.
        /// </returns>
        public Armtemplates GetById(Guid armTemplateId)
        {
            return this.context.Armtemplates.Where(s => s.ArmtempalteId == armTemplateId).FirstOrDefault();
        }

        /// <summary>
        /// Adds the specified Offer details.
        /// </summary>
        /// <param name="templateDetails">The template details.</param>
        /// <returns>
        /// Identifier of the newly created template.
        /// </returns>
        public Guid? Save(Armtemplates templateDetails)
        {
            if (templateDetails != null && !string.IsNullOrEmpty(templateDetails.ArmtempalteName))
            {
                var existingTemplate = this.context.Armtemplates.Where(s => s.ArmtempalteName == templateDetails.ArmtempalteName).FirstOrDefault();
                if (existingTemplate != null)
                {
                    existingTemplate.TemplateLocation = templateDetails.TemplateLocation;
                    this.context.Armtemplates.Update(existingTemplate);
                    this.context.SaveChanges();
                }
                else
                {
                    this.context.Armtemplates.Add(templateDetails);
                    this.context.SaveChanges();
                    return templateDetails.ArmtempalteId;
                }
            }

            return null;
        }

        /// <summary>
        /// Adds the specified Offer details.
        /// </summary>
        /// <param name="templateParms">The template parms.</param>
        /// <returns> ParmId.</returns>
        public Guid? SaveParameters(ArmtemplateParameters templateParms)
        {
            if (templateParms != null && !string.IsNullOrEmpty(templateParms.Parameter))
            {
                this.context.ArmtemplateParameters.Add(templateParms);
                this.context.SaveChanges();
                return templateParms.ArmtemplateId;
            }

            return null;
        }

        /// <summary>
        /// Removes the specified plan details.
        /// </summary>
        /// <param name="templateDetails">The template details.</param>
        public void Remove(Armtemplates templateDetails)
        {
            var existingTemplate = this.context.Armtemplates.Where(s => s.ArmtempalteId == templateDetails.ArmtempalteId).FirstOrDefault();
            if (existingTemplate != null)
            {
                this.context.Armtemplates.Remove(existingTemplate);
                this.context.SaveChanges();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
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
                    this.context.Dispose();
                }
            }

            this.disposed = true;
        }
    }
}
