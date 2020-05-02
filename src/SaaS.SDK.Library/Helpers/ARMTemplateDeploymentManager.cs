using System;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Management.ResourceManager.Models;
//using Microsoft.Rest.Azure.Authentication;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Microsoft.Rest.Azure.Authentication;
using Microsoft.Marketplace.SaaS.SDK.Services.Models;
using Microsoft.Marketplace.SaaS.SDK.Services.Helpers;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System.Collections.Generic;
using Microsoft.Marketplace.SaasKit.Helpers;
using System.Threading.Tasks;
using System.Collections;
using System.Linq;

namespace Microsoft.Marketplace.SaaS.SDK.Services.Helpers
{
    public class ARMTemplateDeploymentManager
    {   
        /// <summary>
        /// Reads a JSON file from the specified path
        /// </summary>
        /// <param name="pathToJson">The full path to the JSON file</param>
        /// <returns>The JSON file contents</returns>
        private JObject GetJsonFileContents(string pathToJson)
        {
            JObject templatefileContent = new JObject();
            using (StreamReader file = File.OpenText(pathToJson))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    templatefileContent = (JObject)JToken.ReadFrom(reader);
                    return templatefileContent;
                }
            }
        }

        public async Task<DeploymentExtended> DeployARMTemplate(Armtemplates template, List<SubscriptionTemplateParameters> templateParameters, CredentialsModel credenitals)
        {
            Console.WriteLine("DeployARMTemplate");
            try
            {
                string tenantId = credenitals.TenantID.Trim();
                string clientId = credenitals.ServicePrincipalID.Trim();
                string clientSecret = credenitals.ClientSecret.Trim();

                Console.WriteLine("LoginSilentAsync");
                var serviceCreds = await ApplicationTokenProvider.LoginSilentAsync(tenantId, clientId, clientSecret).ConfigureAwait(false);
                Console.WriteLine("ReadARMTemplateFromBlob template.ArmtempalteName {0}", template.ArmtempalteName);
                string armContent = AzureBlobHelper.ReadARMTemplateFromBlob(template.ArmtempalteName);

                // Read the template and parameter file contents
                JObject templateFileContents = JObject.Parse(armContent);

                Console.WriteLine("Get resourceGroupName");
                var resourceGroupName = templateParameters.Where(s => s.Parameter.ToLower() == "resourcegroup").FirstOrDefault();
                Console.WriteLine("resourceGroupName: {0} ", resourceGroupName);
                Console.WriteLine("Get resourceGroupLocation");
                var resourceGroupLocation = templateParameters.Where(s => s.Parameter.ToLower() == "location").FirstOrDefault();


                var resourceManagementClient = new ResourceManagementClient(serviceCreds);
                resourceManagementClient.SubscriptionId = credenitals.SubscriptionID;
                Console.WriteLine("resourceManagementClient.SubscriptionId: {0}", resourceManagementClient.SubscriptionId);

                Console.WriteLine(" Create or check that resource group exists");
                EnsureResourceGroupExists(resourceManagementClient, resourceGroupName.Value, resourceGroupLocation.Value);

                Console.WriteLine(" Remove resourceGroupName , resourceGroupLocation from parameters List (not deleting the resource)");
                templateParameters.Remove(resourceGroupName);
                templateParameters.Remove(resourceGroupLocation);

                Console.WriteLine("Prepare input parms list");

                Hashtable hashTable = new Hashtable();
                foreach (var cred in templateParameters)
                {
                    hashTable.Add(cred.Parameter, cred.Value);
                }


                string deploymentName = string.Format("{0}-deployment", resourceGroupName.Value);
                Console.WriteLine(" Start a deployment {0}: DeployTemplate: {1}", deploymentName, template.ArmtempalteName);
                var result = DeployTemplate(resourceManagementClient, resourceGroupName.Value, deploymentName, templateFileContents, hashTable);
                Console.WriteLine("DeployTemplate Request Complete");
                return result;
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in Deployment {0} : {1}", ex.Message, ex.InnerException);
                Console.WriteLine(ex.Message);
            }
            return new DeploymentExtended();
        }

        /// <summary>
        /// Ensures that a resource group with the specified name exists. If it does not, will attempt to create one.
        /// </summary>
        /// <param name="resourceManagementClient">The resource manager client.</param>
        /// <param name="resourceGroupName">The name of the resource group.</param>
        /// <param name="resourceGroupLocation">The resource group location. Required when creating a new resource group.</param>
        private static void EnsureResourceGroupExists(ResourceManagementClient resourceManagementClient, string resourceGroupName, string resourceGroupLocation)
        {
            Console.WriteLine(string.Format("check if  resource group '{0}' in location '{1}' exists", resourceGroupName, resourceGroupLocation));
            if (resourceManagementClient.ResourceGroups.CheckExistence(resourceGroupName) != true)
            {
                Console.WriteLine(string.Format("Creating resource group '{0}' in location '{1}'", resourceGroupName, resourceGroupLocation));
                var resourceGroup = new ResourceGroup();
                resourceGroup.Location = resourceGroupLocation;
                resourceManagementClient.ResourceGroups.CreateOrUpdate(resourceGroupName, resourceGroup);
            }
            else
            {
                Console.WriteLine(string.Format("Using existing resource group '{0}'", resourceGroupName));
            }
        }


        public void DeleteResoureGroup(List<SubscriptionTemplateParameters> templateParameters, CredentialsModel credenitals)
        {
            Console.WriteLine("DeployARMTemplate");
            try
            {
                string tenantId = credenitals.TenantID.Trim();
                string clientId = credenitals.ServicePrincipalID.Trim();
                string clientSecret = credenitals.ClientSecret.Trim();

                Console.WriteLine("LoginSilentAsync");
                var serviceCreds = ApplicationTokenProvider.LoginSilentAsync(tenantId, clientId, clientSecret).ConfigureAwait(false).GetAwaiter().GetResult();
                //Console.WriteLine("ReadARMTemplateFromBlob template.ArmtempalteName {0}", template.ArmtempalteName);
                //string armContent = AzureBlobHelper.ReadARMTemplateFromBlob(template.ArmtempalteName);

                //// Read the template and parameter file contents
                //JObject templateFileContents = JObject.Parse(armContent);

                Console.WriteLine("Get resourceGroupName");
                var resourceGroupName = templateParameters.Where(s => s.Parameter.ToLower() == "resourcegroup").FirstOrDefault();
                Console.WriteLine("resourceGroupName: {0} ", resourceGroupName);
                Console.WriteLine("Get resourceGroupLocation");
                var resourceGroupLocation = templateParameters.Where(s => s.Parameter.ToLower() == "location").FirstOrDefault();


                var resourceManagementClient = new ResourceManagementClient(serviceCreds);
                resourceManagementClient.SubscriptionId = credenitals.SubscriptionID;
                Console.WriteLine("resourceManagementClient.SubscriptionId: {0}", resourceManagementClient.SubscriptionId);

                Console.WriteLine(" Create or check that resource group exists");
                DeleteExistingResourceGroup(resourceManagementClient, resourceGroupName.Value, resourceGroupLocation.Value);

            }
            catch (Exception ex)
            {

                string errorDescriptin = string.Format("Exception: {0} :: Innser Exception:{1}", ex.Message, ex.InnerException);
                Console.WriteLine(errorDescriptin);
            }

        }

        private static void DeleteExistingResourceGroup(ResourceManagementClient resourceManagementClient, string resourceGroupName, string resourceGroupLocation)
        {
            Console.WriteLine(string.Format("check if  resource group '{0}' in location '{1}' exists", resourceGroupName, resourceGroupLocation));
            if (resourceManagementClient.ResourceGroups.CheckExistence(resourceGroupName) != true)
            {
                Console.WriteLine(string.Format("Delete resource group '{0}' in location '{1}'", resourceGroupName, resourceGroupLocation));
                var resourceGroup = new ResourceGroup();
                resourceGroup.Location = resourceGroupLocation;
                resourceManagementClient.ResourceGroups.Delete(resourceGroupName);
                Console.WriteLine(string.Format("Resource group '{0}' in location '{1}' Deleted", resourceGroupName, resourceGroupLocation));
            }
            else
            {
                Console.WriteLine(string.Format("Resource Group not found existing resource group '{0}'", resourceGroupName));
            }
        }

        /// <summary>
        /// Starts a template deployment.
        /// </summary>
        /// <param name="resourceManagementClient">The resource manager client.</param>
        /// <param name="resourceGroupName">The name of the resource group.</param>
        /// <param name="deploymentName">The name of the deployment.</param>
        /// <param name="templateFileContents">The template file contents.</param>
        /// <param name="parameterFileContents">The parameter file contents.</param>
        private static DeploymentExtended DeployTemplate(ResourceManagementClient resourceManagementClient, string resourceGroupName, string deploymentName, JObject templateFileContents, Hashtable hashTable)
        {
            try
            {
                Console.WriteLine(string.Format("Starting template deployment '{0}' in resource group '{1}'", deploymentName, resourceGroupName));
                var deployment = new Deployment();

                string parameters = JsonConvert.SerializeObject(hashTable, Formatting.Indented, new JsonARMPropertiesConverter(typeof(Hashtable)));

                deployment.Properties = new DeploymentProperties
                {
                    Mode = DeploymentMode.Incremental,
                    Template = templateFileContents,
                    Parameters = parameters
                };

                var deploymentResult = resourceManagementClient.Deployments.CreateOrUpdate(resourceGroupName, deploymentName, deployment);
                var outputs = deploymentResult.Properties.Outputs;

                Console.WriteLine(string.Format("Deployment status: {0}", deploymentResult.Properties.ProvisioningState));

                return deploymentResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return new DeploymentExtended();
        }



    }
}
