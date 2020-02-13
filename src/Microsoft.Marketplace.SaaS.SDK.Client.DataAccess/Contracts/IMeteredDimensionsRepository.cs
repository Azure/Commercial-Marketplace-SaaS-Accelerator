﻿namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

    /// <summary>
    /// Metered Dimensions Repository Interface
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.IBaseRepository{Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities.MeteredDimensions}" />
    public interface IMeteredDimensionsRepository : IDisposable, IBaseRepository<MeteredDimensions>
    {
        /// <summary>
        /// Gets the dimensions from plan identifier.
        /// </summary>
        /// <param name="planId">The plan identifier.</param>
        /// <returns></returns>
        List<MeteredDimensions> GetDimensionsFromPlanId(string planId);
    }
}