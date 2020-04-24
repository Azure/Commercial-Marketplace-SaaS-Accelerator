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
using Microsoft.Marketplace.SaasKit.WebJob.Models;
using Microsoft.Marketplace.SaasKit.WebJob.Helpers;

namespace Microsoft.Marketplace.SaasKit.WebJob
{
    public class Deploy
    {
        string subscriptionId = "980a314e-1f55-416a-a85b-b97f3ff68d8e";
        string clientId = "28b1d793-eede-411a-a9fe-ba996808d4ea";
        string clientSecret = "sXJn9bGcp5cmhZ@Ns:?Z77Jb?Zp[?x3.";
        string resourceGroupName = "IndraTest";
        string deploymentName = "ISVs";
        string resourceGroupLocation = "Central US"; // must be specified for creating a new resource group
        string pathToTemplateFile = @"C:\Users\ibijjala\Desktop\deploy-amp-saaskit.json";
        string pathToParameterFile = @"C:\Users\ibijjala\Desktop\deploy-parameters.json";
        string tenantId = "6d7e0652-b03d-4ed2-bf86-f1999cecde17";
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

        public async void Run()
        {
            // Try to obtain the service credentials

            try
            {
                var serviceCreds = ApplicationTokenProvider.LoginSilentAsync(tenantId, clientId, clientSecret).ConfigureAwait(false).GetAwaiter().GetResult();

                // Read the template and parameter file contents
                JObject templateFileContents = GetJsonFileContents(pathToTemplateFile);
                JObject parameterFileContents = GetJsonFileContents(pathToParameterFile);
                //webAppNamePrefix parm = JsonConvert.DeserializeObject<webAppNamePrefix>(parameterFileContents.ToString());



                Parameters configuration = new Parameters()
                {
                    webAppNamePrefix = resourceGroupName
                };

                // Create the resource manager client
                var resourceManagementClient = new ResourceManagementClient(serviceCreds);
                resourceManagementClient.SubscriptionId = subscriptionId;

                // Create or check that resource group exists
                EnsureResourceGroupExists(resourceManagementClient, resourceGroupName, resourceGroupLocation);

                // Start a deployment
                DeployTemplate(resourceManagementClient, resourceGroupName, resourceGroupName, templateFileContents, parameterFileContents, configuration);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Ensures that a resource group with the specified name exists. If it does not, will attempt to create one.
        /// </summary>
        /// <param name="resourceManagementClient">The resource manager client.</param>
        /// <param name="resourceGroupName">The name of the resource group.</param>
        /// <param name="resourceGroupLocation">The resource group location. Required when creating a new resource group.</param>
        private static void EnsureResourceGroupExists(ResourceManagementClient resourceManagementClient, string resourceGroupName, string resourceGroupLocation)
        {
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

        /// <summary>
        /// Starts a template deployment.
        /// </summary>
        /// <param name="resourceManagementClient">The resource manager client.</param>
        /// <param name="resourceGroupName">The name of the resource group.</param>
        /// <param name="deploymentName">The name of the deployment.</param>
        /// <param name="templateFileContents">The template file contents.</param>
        /// <param name="parameterFileContents">The parameter file contents.</param>
        private static void DeployTemplate(ResourceManagementClient resourceManagementClient, string resourceGroupName, string deploymentName, JObject templateFileContents, JObject parameterFileContents, Parameters configuration)
        {
            Console.WriteLine(string.Format("Starting template deployment '{0}' in resource group '{1}'", deploymentName, resourceGroupName));
            var deployment = new Deployment();



            string parameters = JsonConvert.SerializeObject(configuration, Formatting.Indented, new JsonARMPropertiesConverter(typeof(Parameters)));

            deployment.Properties = new DeploymentProperties
            {
                Mode = DeploymentMode.Incremental,
                Template = templateFileContents,
                Parameters = parameters
            };

            var deploymentResult = resourceManagementClient.Deployments.CreateOrUpdate(resourceGroupName, deploymentName, deployment);
            Console.WriteLine(string.Format("Deployment status: {0}", deploymentResult.Properties.ProvisioningState));
        }



    }
}
