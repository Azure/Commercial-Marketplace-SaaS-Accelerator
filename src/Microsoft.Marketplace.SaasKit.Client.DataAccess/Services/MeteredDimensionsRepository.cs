namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Metered Dimensions Repository
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.IMeteredDimensionsRepository" />
    public class MeteredDimensionsRepository : IMeteredDimensionsRepository
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly SaasKitContext Context;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlansRepository" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public MeteredDimensionsRepository(SaasKitContext context)
        {
            Context = context;
        }

        /// <summary>
        /// The disposed
        /// </summary>
        private bool disposed = false;

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

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MeteredDimensions> Get()
        {
            return Context.MeteredDimensions.Include(s => s.Plan);
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public MeteredDimensions Get(int id)
        {
            return Context.MeteredDimensions.Include(s => s.Plan).Where(s => s.Id == id).FirstOrDefault();
        }

        /// <summary>
        /// Adds the specified dimension details.
        /// </summary>
        /// <param name="dimensionDetails">The dimension details.</param>
        /// <returns></returns>
        public int Add(MeteredDimensions dimensionDetails)
        {
            if (dimensionDetails != null && !string.IsNullOrEmpty(dimensionDetails.Dimension))
            {
                var existingDimension = Context.MeteredDimensions.Where(s => s.Dimension == dimensionDetails.Dimension).FirstOrDefault();
                if (existingDimension != null)
                {
                    existingDimension.Description = dimensionDetails.Description;

                    Context.MeteredDimensions.Update(existingDimension);
                    Context.SaveChanges();
                    return existingDimension.Id;
                }
                else
                {
                    Context.MeteredDimensions.Add(dimensionDetails);
                    Context.SaveChanges();
                    return dimensionDetails.Id;
                }
            }
            return 0;
        }

        /// <summary>
        /// Removes the specified dimension details.
        /// </summary>
        /// <param name="dimensionDetails">The dimension details.</param>
        public void Remove(MeteredDimensions dimensionDetails)
        {
            Context.MeteredDimensions.Remove(dimensionDetails);
            Context.SaveChanges();
        }

        /// <summary>
        /// Gets the dimensions from plan identifier.
        /// </summary>
        /// <param name="planId">The plan identifier.</param>
        /// <returns></returns>
        public List<MeteredDimensions> GetDimensionsFromPlanId(string planId)
        {
            return Context.MeteredDimensions.Include(s => s.Plan).Where(s => s.Plan != null && s.Plan.PlanId == planId).ToList();
        }
    }
}