namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

    /// <summary>
    /// Repository to access offer attributes.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.IOfferAttributesRepository" />
    public class OfferAttributesRepository : IOfferAttributesRepository
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
        /// Initializes a new instance of the <see cref="OfferAttributesRepository"/> class.
        /// </summary>
        /// <param name="context">The this.context.</param>
        public OfferAttributesRepository(SaasKitContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <param name="offerAttributes">The offer attributes.</param>
        /// <returns>
        /// Id of the newly added offer attribute.
        /// </returns>
        public int? Add(OfferAttributes offerAttributes)
        {
            if (offerAttributes != null)
            {
                var existingOfferAttribute = this.context.OfferAttributes.Where(s => s.Id ==
                offerAttributes.Id).FirstOrDefault();
                if (existingOfferAttribute != null)
                {
                    existingOfferAttribute.ParameterId = offerAttributes.ParameterId;
                    existingOfferAttribute.DisplayName = offerAttributes.DisplayName;
                    existingOfferAttribute.Description = offerAttributes.Description;
                    existingOfferAttribute.ValueTypeId = offerAttributes.ValueTypeId;
                    existingOfferAttribute.FromList = offerAttributes.FromList;
                    existingOfferAttribute.ValuesList = offerAttributes.ValuesList;
                    existingOfferAttribute.Max = offerAttributes.Max;
                    existingOfferAttribute.Min = offerAttributes.Min;
                    existingOfferAttribute.Type = offerAttributes.Type;
                    existingOfferAttribute.DisplaySequence = offerAttributes.DisplaySequence;
                    existingOfferAttribute.Isactive = offerAttributes.Isactive;
                    existingOfferAttribute.UserId = offerAttributes.UserId;
                    existingOfferAttribute.OfferId = offerAttributes.OfferId;
                    existingOfferAttribute.IsRequired = offerAttributes.IsRequired;
                    existingOfferAttribute.IsDelete = offerAttributes.IsDelete;

                    this.context.OfferAttributes.Update(existingOfferAttribute);
                    this.context.SaveChanges();
                    return existingOfferAttribute.Id;
                }
                else
                {
                    this.context.OfferAttributes.Add(offerAttributes);
                    this.context.SaveChanges();
                    return offerAttributes.Id;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the deployment parameters.
        /// </summary>
        /// <returns>
        /// List of deployment parameters across offers.
        /// </returns>
        public IEnumerable<DeploymentAttributes> GetDeploymentParameters()
        {
            var deploymentAttributes = this.context.DeploymentAttributes;
            return deploymentAttributes;
        }

        /// <summary>
        /// Adds the deployment attributes.
        /// </summary>
        /// <param name="offerId">The offer identifier.</param>
        /// <param name="curretnUserId">The curretn user identifier.</param>
        /// <param name="deploymentAttributes">The deployment attributes.</param>
        /// <returns> Deployment Attribute.</returns>
        public int? AddDeploymentAttributes(Guid offerId, int curretnUserId, List<DeploymentAttributes> deploymentAttributes)
        {
            if (deploymentAttributes != null && deploymentAttributes.Count() > 0)
            {
                foreach (var attribute in deploymentAttributes)
                {
                    var existingOfferAttribute = this.context.OfferAttributes.Where(s => s.ParameterId == attribute.ParameterId
                    && s.OfferId == offerId).FirstOrDefault();
                    if (existingOfferAttribute != null)
                    {
                        existingOfferAttribute.ParameterId = attribute.ParameterId;
                        existingOfferAttribute.DisplayName = attribute.DisplayName;
                        existingOfferAttribute.Description = attribute.Description;
                        existingOfferAttribute.ValueTypeId = attribute.ValueTypeId;
                        existingOfferAttribute.FromList = attribute.FromList;
                        existingOfferAttribute.ValuesList = attribute.ValuesList;
                        existingOfferAttribute.Max = attribute.Max;
                        existingOfferAttribute.Min = attribute.Min;
                        existingOfferAttribute.Type = attribute.Type;
                        existingOfferAttribute.DisplaySequence = attribute.DisplaySequence;
                        existingOfferAttribute.Isactive = attribute.Isactive;
                        existingOfferAttribute.UserId = attribute.UserId;
                        existingOfferAttribute.OfferId = offerId;
                        existingOfferAttribute.IsRequired = attribute.IsRequired;
                        existingOfferAttribute.IsDelete = attribute.IsDelete;

                        this.context.OfferAttributes.Update(existingOfferAttribute);
                        this.context.SaveChanges();
                    }
                    else
                    {
                        OfferAttributes newAttribute = new OfferAttributes();

                        newAttribute.ParameterId = attribute.ParameterId;
                        newAttribute.DisplayName = attribute.DisplayName;
                        newAttribute.Description = attribute.Description;
                        newAttribute.ValueTypeId = attribute.ValueTypeId;
                        newAttribute.FromList = attribute.FromList;
                        newAttribute.ValuesList = attribute.ValuesList;
                        newAttribute.Max = attribute.Max;
                        newAttribute.Min = attribute.Min;
                        newAttribute.Type = attribute.Type;
                        newAttribute.DisplaySequence = attribute.DisplaySequence;
                        newAttribute.Isactive = attribute.Isactive;
                        newAttribute.UserId = attribute.UserId;
                        newAttribute.OfferId = offerId;
                        newAttribute.IsRequired = attribute.IsRequired;
                        newAttribute.IsDelete = attribute.IsDelete;
                        newAttribute.CreateDate = DateTime.Now;
                        newAttribute.UserId = curretnUserId;
                        this.context.OfferAttributes.Add(newAttribute);
                        this.context.SaveChanges();
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets all offer attributes across offers.
        /// </summary>
        /// <returns>
        /// List of offer attributes across offers.
        /// </returns>
        public IEnumerable<OfferAttributes> GetAll()
        {
            return this.context.OfferAttributes.Where(s => s.IsDelete != true && s.Type.ToLower() == "input");
        }

        /// <summary>
        /// Gets the input attributes by offer identifier.
        /// </summary>
        /// <param name="offerGuId">The offer gu identifier.</param>
        /// <returns> list of OfferAttributes.</returns>
        public IEnumerable<OfferAttributes> GetInputAttributesByOfferId(Guid offerGuId)
        {
            return this.context.OfferAttributes.Where(s => s.OfferId == offerGuId && (s.IsDelete != true) && s.Type.ToLower() == "input");
        }

        /// <summary>
        /// Gets all offer attributes by offer identifier ( includes deployment and input attributes ).
        /// </summary>
        /// <param name="offerGuId">The offer identifier.</param>
        /// <returns>
        /// List of offer attributes.
        /// </returns>
        public IEnumerable<OfferAttributes> GetAllOfferAttributesByOfferId(Guid offerGuId)
        {
            return this.context.OfferAttributes.Where(s => s.OfferId == offerGuId && (s.IsDelete != true));
        }

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns> Offer Attributes.</returns>
        public OfferAttributes GetById(int id)
        {
            return this.context.OfferAttributes.Where(s => s.Id == id && s.IsDelete != true && s.Type.ToLower() == "input").FirstOrDefault();
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
