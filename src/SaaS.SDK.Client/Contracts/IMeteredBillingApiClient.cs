namespace Microsoft.Marketplace.SaasKit.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Marketplace.SaasKit.Models;

    /// <summary>
    /// Metered ApiClient Interface.
    /// </summary>
    public interface IMeteredBillingApiClient
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
