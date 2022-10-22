using System;
using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Contracts;

/// <summary>
/// Metered Dimensions Repository Interface.
/// </summary>
/// <seealso cref="System.IDisposable" />
/// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.IBaseRepository{Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities.MeteredDimensions}" />
public interface IMeteredDimensionsRepository : IDisposable, IBaseRepository<MeteredDimensions>
{
    /// <summary>
    /// Gets the dimensions for the plan (by the identifier).
    /// </summary>
    /// <param name="planId">The plan identifier.</param>
    /// <returns>List of metered dimensions for the plan.</returns>
    List<MeteredDimensions> GetDimensionsByPlanId(string planId);
}