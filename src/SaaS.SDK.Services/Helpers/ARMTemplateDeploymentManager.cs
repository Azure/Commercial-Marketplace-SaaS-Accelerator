namespace Microsoft.Marketplace.SaaS.SDK.Services.Helpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Azure.Management.ResourceManager;
    using Microsoft.Azure.Management.ResourceManager.Models;
    using Microsoft.Extensions.Logging;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using Microsoft.Rest.Azure.Authentication;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Helper to deploy ARM templates to Azure / delete resource group.
    /// </summary>
    public class ARMTemplateDeploymentManager
    {
        private readonly ILogger<ARMTemplateDeploymentManager> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ARMTemplateDeploymentManager"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public ARMTemplateDeploymentManager(ILogger<ARMTemplateDeploymentManager> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Deploys the arm template.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="templateParameters">The template parameters.</param>
        /// <param name="credenitals">The credenitals.</param>
        /// <param name="armTemplateContent">Content of the arm template.</param>
        /// <returns> DeploymentExtended.</returns>
        public async Task<DeploymentExtended> DeployARMTemplate(Armtemplates template, List<SubscriptionTemplateParameters> templateParameters, CredentialsModel credenitals, string armTemplateContent)
        {
            this.logger.LogInformation($"Begin deployment of ARM Template - {template.ArmtempalteName}");

            try
            {
                string tenantId = credenitals.TenantID.Trim();
                string clientId = credenitals.ServicePrincipalID.Trim();
                string clientSecret = credenitals.ClientSecret.Trim();

                this.logger.LogInformation($"Obtain access token to deploy the template to the target tenant {tenantId} and subscription");

                var serviceCreds = await ApplicationTokenProvider.LoginSilentAsync(tenantId, clientId, clientSecret).ConfigureAwait(false);

                // Read the template and parameter file contents
                JObject templateFileContents = JObject.Parse(armTemplateContent);

                this.logger.LogInformation("Get resourceGroupName");
                var resourceGroupName = templateParameters.Where(s => s.Parameter.ToLower() == "resourcegroup").FirstOrDefault();
                this.logger.LogInformation("resourceGroupName: {0} ", resourceGroupName);

                this.logger.LogInformation("Get resourceGroupLocation");
                var resourceGroupLocation = templateParameters.Where(s => s.Parameter.ToLower() == "location").FirstOrDefault();
                this.logger.LogInformation("resourceGroupLocation: {0} ", resourceGroupLocation);

                var resourceManagementClient = new ResourceManagementClient(serviceCreds);
                resourceManagementClient.SubscriptionId = credenitals.SubscriptionID;
                this.logger.LogInformation("resourceManagementClient.SubscriptionId: {0}", resourceManagementClient.SubscriptionId);

                this.logger.LogInformation(" Create or check that resource group exists");
                this.EnsureResourceGroupExists(resourceManagementClient, resourceGroupName.Value, resourceGroupLocation.Value);

                this.logger.LogInformation("Remove resourceGroupName , resourceGroupLocation from parameters List (not deleting the resource)");
                templateParameters.Remove(resourceGroupName);
                templateParameters.Remove(resourceGroupLocation);

                this.logger.LogInformation("Prepare input parms list");

                Hashtable hashTable = new Hashtable();
                foreach (var cred in templateParameters)
                {
                    hashTable.Add(cred.Parameter, cred.Value);
                }

                string deploymentName = string.Format("{0}-deployment", resourceGroupName.Value);
                this.logger.LogInformation("Start a deployment {0}: DeployTemplate: {1}", deploymentName, template.ArmtempalteName);
                var result = this.DeployTemplate(resourceManagementClient, resourceGroupName.Value, deploymentName, templateFileContents, hashTable);
                this.logger.LogInformation("DeployTemplate Request Complete");
                return result;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error in Deployment {0}", ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Deletes the resoure group.
        /// </summary>
        /// <param name="templateParameters">The template parameters.</param>
        /// <param name="credenitals">The credenitals.</param>
        public void DeleteResoureGroup(List<SubscriptionTemplateParameters> templateParameters, CredentialsModel credenitals)
        {
            this.logger.LogInformation("Delete resource group");
            try
            {
                string tenantId = credenitals.TenantID.Trim();
                string clientId = credenitals.ServicePrincipalID.Trim();
                string clientSecret = credenitals.ClientSecret.Trim();

                this.logger.LogInformation("LoginSilentAsync");
                var serviceCreds = ApplicationTokenProvider.LoginSilentAsync(tenantId, clientId, clientSecret).ConfigureAwait(false).GetAwaiter().GetResult();
                this.logger.LogInformation("Get resourceGroupName");
                var resourceGroupName = templateParameters.Where(s => s.Parameter.ToLower() == "resourcegroup").FirstOrDefault();
                this.logger.LogInformation("resourceGroupName: {0} ", resourceGroupName);
                this.logger.LogInformation("Get resourceGroupLocation");
                var resourceGroupLocation = templateParameters.Where(s => s.Parameter.ToLower() == "location").FirstOrDefault();
                var resourceManagementClient = new ResourceManagementClient(serviceCreds);
                resourceManagementClient.SubscriptionId = credenitals.SubscriptionID;
                this.logger.LogInformation("resourceManagementClient.SubscriptionId: {0}", resourceManagementClient.SubscriptionId);
                this.logger.LogInformation(" Create or check that resource group exists");
                this.DeleteExistingResourceGroup(resourceManagementClient, resourceGroupName.Value, resourceGroupLocation.Value);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error in Deployment {0}", ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Ensures that a resource group with the specified name exists. If it does not, will attempt to create one.
        /// </summary>
        /// <param name="resourceManagementClient">The resource manager client.</param>
        /// <param name="resourceGroupName">The name of the resource group.</param>
        /// <param name="resourceGroupLocation">The resource group location. Required when creating a new resource group.</param>
        private void EnsureResourceGroupExists(ResourceManagementClient resourceManagementClient, string resourceGroupName, string resourceGroupLocation)
        {
            this.logger.LogInformation(string.Format("check if  resource group '{0}' in location '{1}' exists", resourceGroupName, resourceGroupLocation));
            if (resourceManagementClient.ResourceGroups.CheckExistence(resourceGroupName) != true)
            {
                this.logger.LogInformation(string.Format("Creating resource group '{0}' in location '{1}'", resourceGroupName, resourceGroupLocation));
                var resourceGroup = new ResourceGroup();
                resourceGroup.Location = resourceGroupLocation;
                resourceManagementClient.ResourceGroups.CreateOrUpdate(resourceGroupName, resourceGroup);
            }
            else
            {
                this.logger.LogInformation(string.Format("Using existing resource group '{0}'", resourceGroupName));
            }
        }

        private void DeleteExistingResourceGroup(ResourceManagementClient resourceManagementClient, string resourceGroupName, string resourceGroupLocation)
        {
            this.logger.LogInformation(string.Format("check if  resource group '{0}' in location '{1}' exists", resourceGroupName, resourceGroupLocation));
            if (resourceManagementClient.ResourceGroups.CheckExistence(resourceGroupName) == true)
            {
                this.logger.LogInformation(string.Format("Delete resource group '{0}' in location '{1}'", resourceGroupName, resourceGroupLocation));
                var resourceGroup = new ResourceGroup();
                resourceGroup.Location = resourceGroupLocation;
                resourceManagementClient.ResourceGroups.Delete(resourceGroupName);
                this.logger.LogInformation(string.Format("Resource group '{0}' in location '{1}' Deleted", resourceGroupName, resourceGroupLocation));
            }
            else
            {
                this.logger.LogInformation(string.Format("Resource Group not found existing resource group '{0}'", resourceGroupName));
            }
        }

        /// <summary>
        /// Starts a template deployment.
        /// </summary>
        /// <param name="resourceManagementClient">The resource manager client.</param>
        /// <param name="resourceGroupName">The name of the resource group.</param>
        /// <param name="deploymentName">The name of the deployment.</param>
        /// <param name="templateFileContents">The template file contents.</param>
        /// <param name="hashTable">The hash table.</param>
        /// <returns> DeploymentExtended.</returns>
        private DeploymentExtended DeployTemplate(ResourceManagementClient resourceManagementClient, string resourceGroupName, string deploymentName, JObject templateFileContents, Hashtable hashTable)
        {
            try
            {
                this.logger.LogInformation(string.Format("Starting template deployment '{0}' in resource group '{1}'", deploymentName, resourceGroupName));
                var deployment = new Deployment();

                string parameters = JsonConvert.SerializeObject(hashTable, Formatting.Indented, new JsonARMPropertiesConverter(typeof(Hashtable)));

                deployment.Properties = new DeploymentProperties
                {
                    Mode = DeploymentMode.Incremental,
                    Template = templateFileContents,
                    Parameters = parameters,
                };

                var deploymentResult = resourceManagementClient.Deployments.CreateOrUpdate(resourceGroupName, deploymentName, deployment);
                var outputs = deploymentResult.Properties.Outputs;

                this.logger.LogInformation(string.Format("Deployment status: {0}", deploymentResult.Properties.ProvisioningState));

                return deploymentResult;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error in Deployment {0}", ex.Message);
                throw ex;
            }
        }
    }
}
