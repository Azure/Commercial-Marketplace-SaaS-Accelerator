namespace Microsoft.Marketplace.SaaS.SDK.Services.Services
{
    using System.Collections.Generic;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

    /// <summary>
    /// Application Config Service.
    /// </summary>
    public class ApplicationConfigService
    {

        /// <summary>
        /// The app config repository.
        /// </summary>
        private IApplicationConfigRepository appConfigRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationConfigurationService"/> class.
        /// </summary>
        /// <param name="ApplicationConfigRepository">The application config repository.</param>
        public ApplicationConfigService(IApplicationConfigRepository applicationConfigRepository)
        {
            this.appConfigRepository = applicationConfigRepository;
        }

        /// <summary>
        /// Get all Application Config items.
        /// </summary>
        public IEnumerable<ApplicationConfiguration> GetAllApplicationConfiguration()
        {
            return this.appConfigRepository.GetAll();
        }

        /// <summary>
        /// Save Application Config item.
        /// </summary>
        public int? SaveAppConfig(ApplicationConfiguration appConfig)
        {
            if (appConfig != null)
            {
                return this.appConfigRepository.SaveById(appConfig);
            }

            return null;
        }

        /// <summary>
        /// Get an Application Config item by Id.
        /// </summary>
        public ApplicationConfiguration GetById(int Id)
        {
            return this.appConfigRepository.GetById(Id);
        }
    }
}
