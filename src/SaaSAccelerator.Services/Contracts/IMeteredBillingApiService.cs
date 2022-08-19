// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.
namespace Microsoft.Marketplace.SaaS.SDK.Services.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;

    /// <summary>
    /// Metered ApiClient Interface.
    /// </summary>
    public interface IMeteredBillingApiService
    {
        /// <summary>
        /// Emits the usage event asynchronous.
        /// </summary>
        /// <param name="usageEventRequest">The usage event request.</param>
        /// <returns>Event usage.</returns>
        Task<MeteringUsageResult> EmitUsageEventAsync(MeteringUsageRequest usageEventRequest);

        /// <summary>
        /// Emits the batch usage event asynchronous.
        /// </summary>
        /// <param name="batchUsageEventRequest">The batch usage event request.</param>
        /// <returns> Batch Usage.</returns>
        Task<MeteringBatchUsageResult> EmitBatchUsageEventAsync(IEnumerable<MeteringUsageRequest> batchUsageEventRequest);
    }
}
