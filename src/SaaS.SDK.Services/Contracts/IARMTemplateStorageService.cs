namespace Microsoft.Marketplace.SaaS.SDK.Services.Contracts
{
    using System;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Contract to manage ARM templates.
    /// </summary>
    public interface IARMTemplateStorageService
    {
        /// <summary>
        /// Saves the arm template.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileContantType">Type of the file contant.</param>
        /// <param name="referenceid">The referenceid.</param>
        /// <returns>Reference to the saved ARM template (eg:URL). </returns>
        string SaveARMTemplate(IFormFile file, string fileName, string fileContantType, Guid referenceid);

        /// <summary>
        /// Gets the arm template content as string.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Contents of ARM template as string.</returns>
        string GetARMTemplateContentAsString(string fileName);
    }
}
