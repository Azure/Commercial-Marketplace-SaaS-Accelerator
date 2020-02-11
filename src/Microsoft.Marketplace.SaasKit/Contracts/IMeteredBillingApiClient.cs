namespace Microsoft.Marketplace.SaasKit.Contracts
{
    using Microsoft.Marketplace.SaasKit.Models;
    using System.Threading.Tasks;

    /// <summary>
    /// Metered ApiClient Interface
    /// </summary>
    public interface IMeteredBillingApiClient
    {
        /// <summary>
        /// Emits the usage event asynchronous.
        /// </summary>
        /// <param name="usageEventRequest">The usage event request.</param>
        /// <returns></returns>
        Task<MeteringUsageResult> EmitUsageEventAsync(MeteringUsageRequest usageEventRequest);
    }
}
