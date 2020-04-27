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
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System.Collections.Generic;
using Saas.SDK.WebJob.Helpers;
using System.Threading.Tasks;
using System.Collections;
using System.Linq;

namespace Microsoft.Marketplace.SaasKit.WebJob
{
    public class Deploy
    {
        //string subscriptionId = "980a314e-1f55-416a-a85b-b97f3ff68d8e";


        //string resourceGroupName = "IndraTest";
        //string deploymentName = "ISVs";
        //string resourceGroupLocation = "Central US"; 
        // must be specified for creating a new resource group
        //string pathToTemplateFile = @"C:\Users\ibijjala\Desktop\deploy-amp-saaskit.json";
        //string pathToParameterFile = @"C:\Users\ibijjala\Desktop\deploy-parameters.json";

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
            // Try to obtain the service credentials

            try
            {
                string tenantId = credenitals.TenantID;            //"6d7e0652-b03d-4ed2-bf86-f1999cecde17";
                string clientId = credenitals.ServicePrincipalID;
                string clientSecret = credenitals.ClientSecret;               //"sXJn9bGcp5cmhZ@Ns:?Z77Jb?Zp[?x3.";



                var serviceCreds = ApplicationTokenProvider.LoginSilentAsync(tenantId, clientId, clientSecret).ConfigureAwait(false).GetAwaiter().GetResult();
                string armContent = AzureBlobHelper.ReadARMTemplateFromBlob(template.ArmtempalteName);

                // Read the template and parameter file contents
                JObject templateFileContents = JObject.Parse(armContent);
                //GetJsonFileContents(pathToTemplateFile);
                //JObject parameterFileContents = GetJsonFileContents(pathToParameterFile);
                //webAppNamePrefix parm = JsonConvert.DeserializeObject<webAppNamePrefix>(parameterFileContents.ToString());


                var resourceGroupName = templateParameters.Where(s => s.Parameter.ToLower() == "resourcegroup").FirstOrDefault();
                var resourceGroupLocation = templateParameters.Where(s => s.Parameter.ToLower() == "location").FirstOrDefault();

                // Create the resource manager client
                var resourceManagementClient = new ResourceManagementClient(serviceCreds);
                resourceManagementClient.SubscriptionId = credenitals.SubscriptionID;

                // Create or check that resource group exists
                EnsureResourceGroupExists(resourceManagementClient, resourceGroupName.Value, resourceGroupLocation.Value);

                templateParameters.Remove(resourceGroupName);
                templateParameters.Remove(resourceGroupLocation);
                Hashtable hashTable = new Hashtable();
                foreach (var cred in templateParameters)
                {
                    hashTable.Add(cred.Parameter, cred.Value);
                }

                // Start a deployment
                var result = DeployTemplate(resourceManagementClient, resourceGroupName.Value, "testdeployment", templateFileContents, hashTable);
                return result;
            }


            catch (Exception ex)
            {
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
