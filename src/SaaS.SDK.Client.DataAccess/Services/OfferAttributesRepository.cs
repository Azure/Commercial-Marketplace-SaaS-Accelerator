namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class OfferAttributesRepository : IOfferAttributesRepository
    {
        private readonly SaasKitContext Context;

        public OfferAttributesRepository(SaasKitContext context)
        {
            Context = context;
        }

        /// <summary>
        /// The disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns></returns>
        public int? Add(OfferAttributes offerAttributes)
        {
            if (offerAttributes != null)
            {
                var existingOfferAttribute = Context.OfferAttributes.Where(s => s.Id ==
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

                    Context.OfferAttributes.Update(existingOfferAttribute);
                    Context.SaveChanges();
                    return existingOfferAttribute.Id;
                }
                else
                {
                    Context.OfferAttributes.Add(offerAttributes);
                    Context.SaveChanges();
                    return offerAttributes.Id;
                }
            }

            return null;
        }

        public IEnumerable<DeploymentAttributes> GetDeploymentParameters()
        {

            var deploymentAttributes = Context.DeploymentAttributes;
            return deploymentAttributes;
        }
        public int? AddDeploymentAttributes(Guid offerId, int curretnUserId, List<DeploymentAttributes> deploymentAttributes)
        {

            if (deploymentAttributes != null && deploymentAttributes.Count() > 0)
            {
                foreach (var attribute in deploymentAttributes)
                {
                    var existingOfferAttribute = Context.OfferAttributes.Where(s => s.ParameterId == attribute.ParameterId
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

                        Context.OfferAttributes.Update(existingOfferAttribute);
                        Context.SaveChanges();

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
                        Context.OfferAttributes.Add(newAttribute);
                        Context.SaveChanges();

                    }
                }

            }
            return null;
        }
        public IEnumerable<OfferAttributes> Get()
        {
            return Context.OfferAttributes.Where(s => s.IsDelete != true && s.Type.ToLower() == "input");
        }
        public IEnumerable<OfferAttributes> GetOfferAttributeDetailByOfferId(Guid offerGuId)
        {
            return Context.OfferAttributes.Where(s => s.OfferId == offerGuId && (s.IsDelete != true) && s.Type.ToLower() == "input");
        }

        public IEnumerable<OfferAttributes> GetAllOfferAttributeDetailByOfferId(Guid offerGuId)
        {
            return Context.OfferAttributes.Where(s => s.OfferId == offerGuId && (s.IsDelete != true));
        }

        //IEnumerable<Offers> GetOffersByUser(int userId);

        public OfferAttributes Get(int Id)
        {
            return Context.OfferAttributes.Where(s => s.Id == Id && s.IsDelete != true && s.Type.ToLower() == "input").FirstOrDefault();
        }

        ///// <summary>
        ///// Removes the specified plan details.
        ///// </summary>
        ///// <param name="offerDetails">The offer details.</param>
        //public void Remove(List<OfferAttributes> offerAttributes)
        //{
        //    foreach (var offerAttribute in offerAttributes)
        //    {
        //        var existingOffersAttribute = Context.OfferAttributes.Where(s => s.Id == offerAttribute.Id).FirstOrDefault();
        //        if (existingOffersAttribute != null)
        //        {
        //            Context.OfferAttributes.Remove(existingOffersAttribute);
        //            Context.SaveChanges();
        //        }
        //    }
        //}

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
                    Context.Dispose();
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
