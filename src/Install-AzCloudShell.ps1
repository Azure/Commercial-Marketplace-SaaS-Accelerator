wget https://dotnet.microsoft.com/download/dotnet/scripts/v1/dotnet-install.sh; `
chmod +x dotnet-install.sh; `
./dotnet-install.sh -version 8.0.303; `
$ENV:PATH="$HOME/.dotnet:$ENV:PATH"; `
dotnet tool install --global dotnet-ef --version 8.0.0; `
git clone https://github.com/officemikedev/Commercial-Marketplace-SaaS-Accelerator.git; `
cd ./Commercial-Marketplace-SaaS-Accelerator/deployment; `
.\Deploy.ps1 `
 -WebAppNamePrefix "SaasAcceleratorTest7" `
 -ResourceGroupForDeployment "saas-accelerator7" `
 -PublisherAdminUsers "iowamiker@hotmail.com,mike@officemike.com" `
 -Location "Central US" 

 ✅ If the intallation completed without error complete the folllowing checklist:
   🔵 Add The following URL in PartnerCenter SaaS Technical Configuration
      ➡️ Landing Page section:       https://WflowSaasOffer2024-portal.azurewebsites.net/
      ➡️ Connection Webhook section: https://WflowSaasOffer2024-portal.azurewebsites.net/api/AzureWebhook
      ➡️ Tenant ID:                  97d1fb75-540a-436c-a547-6acd9bf286b2
      ➡️ AAD Application ID section: a42ff9ed-c271-4bcb-870b-c499211f8d31


      🔵 App Service Plan
      ➡️ Create App Service Plan
/usr/lib64/az/lib/python3.9/site-packages/paramiko/pkey.py:100: CryptographyDeprecationWarning: TripleDES has been moved to cryptography.hazmat.decrepit.ciphers.algorithms.TripleDES and will be removed from this module in 48.0.0.
  "cipher": algorithms.TripleDES,
/usr/lib64/az/lib/python3.9/site-packages/paramiko/transport.py:259: CryptographyDeprecationWarning: TripleDES has been moved to cryptography.hazmat.decrepit.ciphers.algorithms.TripleDES and will be removed from this module in 48.0.0.
  "class": algorithms.TripleDES,
Readonly attribute name will be ignored in class <class 'azure.mgmt.web.v2023_01_01.models._models_py3.AppServicePlan'>
{
  "elasticScaleEnabled": false,
  "extendedLocation": null,
  "freeOfferExpirationTime": "2025-01-19T07:16:21.096666",
  "geoRegion": "Central US",
  "hostingEnvironmentProfile": null,
  "hyperV": false,
  "id": "/subscriptions/22224308-0e0a-4f96-9b8d-63a22a64425d/resourceGroups/saas-accelerator5/providers/Microsoft.Web/serverfarms/SaasAcceleratorTest5-asp",
  "isSpot": false,
  "isXenon": false,
  "kind": "linux",
  "kubeEnvironmentProfile": null,
  "location": "centralus",
  "maximumElasticWorkerCount": 1,
  "maximumNumberOfWorkers": 0,
  "name": "SaasAcceleratorTest5-asp",
  "numberOfSites": 0,
  "numberOfWorkers": 1,
  "perSiteScaling": false,
  "provisioningState": "Succeeded",
  "reserved": true,
  "resourceGroup": "saas-accelerator5",
  "sku": {
    "capabilities": null,
    "capacity": 1,
    "family": "B",
    "locations": null,
    "name": "B1",
    "size": "B1",
    "skuCapacity": null,
    "tier": "Basic"
  },
  "spotExpirationTime": null,
  "status": "Ready",
  "subscription": "22224308-0e0a-4f96-9b8d-63a22a64425d",
  "tags": null,
  "targetWorkerCount": 0,
  "targetWorkerSizeId": 0,
  "type": "Microsoft.Web/serverfarms",
  "workerTierName": null,
  "zoneRedundant": false
}
   🔵 Admin Portal WebApp
      ➡️ Create Web App
/usr/lib64/az/lib/python3.9/site-packages/paramiko/pkey.py:100: CryptographyDeprecationWarning: TripleDES has been moved to cryptography.hazmat.decrepit.ciphers.algorithms.TripleDES and will be removed from this module in 48.0.0.
  "cipher": algorithms.TripleDES,
/usr/lib64/az/lib/python3.9/site-packages/paramiko/transport.py:259: CryptographyDeprecationWarning: TripleDES has been moved to cryptography.hazmat.decrepit.ciphers.algorithms.TripleDES and will be removed from this module in 48.0.0.
  "class": algorithms.TripleDES,
Linux Runtime 'dotnet|8' is not supported.Run 'az webapp list-runtimes --os-type linux' to cross check
      ➡️ Assign Identity
/usr/lib64/az/lib/python3.9/site-packages/paramiko/pkey.py:100: CryptographyDeprecationWarning: TripleDES has been moved to cryptography.hazmat.decrepit.ciphers.algorithms.TripleDES and will be removed from this module in 48.0.0.
  "cipher": algorithms.TripleDES,
/usr/lib64/az/lib/python3.9/site-packages/paramiko/transport.py:259: CryptographyDeprecationWarning: TripleDES has been moved to cryptography.hazmat.decrepit.ciphers.algorithms.TripleDES and will be removed from this module in 48.0.0.
  "class": algorithms.TripleDES,
ERROR: (ResourceNotFound) The Resource 'Microsoft.Web/sites/SaasAcceleratorTest5-admin' under resource group 'saas-accelerator5' was not found. For more details please go to https://aka.ms/ARMResourceNotFoundFix
Code: ResourceNotFound
Message: The Resource 'Microsoft.Web/sites/SaasAcceleratorTest5-admin' under resource group 'saas-accelerator5' was not found. For more details please go to https://aka.ms/ARMResourceNotFoundFix
      ➡️ Setup access to KeyVault
argument --object-id: expected one argument