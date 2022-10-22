using System;
using System.Collections.Generic;
using System.Linq;
using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.SaaS.Accelerator.DataAccess.Services;

/// <summary>
/// Metered Dimensions Repository.
/// </summary>
/// <seealso cref="IMeteredDimensionsRepository" />
public class MeteredDimensionsRepository : IMeteredDimensionsRepository
{
    /// <summary>
    /// The context.
    /// </summary>
    private readonly SaasKitContext context;

    /// <summary>
    /// The disposed.
    /// </summary>
    private bool disposed = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="MeteredDimensionsRepository"/> class.
    /// </summary>
    /// <param name="context">The this.context.</param>
    public MeteredDimensionsRepository(SaasKitContext context)
    {
        this.context = context;
    }

    /// <summary>
    /// Gets this instance.
    /// </summary>
    /// <returns>
    /// List of Metered Dimensions.
    /// </returns>
    public IEnumerable<MeteredDimensions> Get()
    {
        return this.context.MeteredDimensions.Include(s => s.Plan);
    }

    /// <summary>
    /// Gets the specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns> Metered Dimensions.</returns>
    public MeteredDimensions Get(int id)
    {
        return this.context.MeteredDimensions.Include(s => s.Plan).Where(s => s.Id == id).FirstOrDefault();
    }

    /// <summary>
    /// Adds the specified dimension details.
    /// </summary>
    /// <param name="dimensionDetails">The dimension details.</param>
    /// <returns> dimension id.</returns>
    public int Save(MeteredDimensions dimensionDetails)
    {
        if (dimensionDetails != null && !string.IsNullOrEmpty(dimensionDetails.Dimension))
        {
            var existingDimension = this.context.MeteredDimensions.Where(s => s.Dimension == dimensionDetails.Dimension).FirstOrDefault();
            if (existingDimension != null)
            {
                existingDimension.Description = dimensionDetails.Description;

                this.context.MeteredDimensions.Update(existingDimension);
                this.context.SaveChanges();
                return existingDimension.Id;
            }
            else
            {
                this.context.MeteredDimensions.Add(dimensionDetails);
                this.context.SaveChanges();
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
        this.context.MeteredDimensions.Remove(dimensionDetails);
        this.context.SaveChanges();
    }

    /// <summary>
    /// Gets the dimensions from plan identifier.
    /// </summary>
    /// <param name="planId">The plan identifier.</param>
    /// <returns>
    /// List of MeteredDimensions.
    /// </returns>
    public List<MeteredDimensions> GetDimensionsByPlanId(string planId)
    {
        return this.context.MeteredDimensions.Include(s => s.Plan).Where(s => s.Plan != null && s.Plan.PlanId == planId).ToList();
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