namespace Microsoft.Marketplace.SaaS.SDK.Services.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Contract for Vault service to store sensitive detail.
    /// </summary>
    public interface IVaultService
    {
        /// <summary>
        /// Writes the key asynchronous.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="val">The value.</param>
        /// <returns> Key vault url.</returns>
        Task<string> WriteKeyAsync(string key, string val);

        /// <summary>
        /// Gets the key asynchronous.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns> key vault sting.</returns>
        Task<string> GetKeyAsync(string key);

        /// <summary>
        /// Validates the user parameters.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns> Validate User Parameters.</returns>
        bool ValidateUserParameters(IDictionary<string, string> dictionary);
    }
}
