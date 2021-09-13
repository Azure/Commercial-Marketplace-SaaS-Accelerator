### Using Azure Cloud Shell
   
   1. Open Powershell in the Azure Cloud (PowerShell) and run the following commands to install Azure modules:
> Note: Make sure that you are using the latest Powershell to avoid issues in Compress-Archive in 5.1 that got resolved in latest version.
```powershell
Install-Module -Name Az -AllowClobber
```
   2. Clone the repository
   3. Navigate to the folder **.\deployment\Templates**
   4. Run the command below with all the values in quotes updated.
```
git clone https://github.com/code4clouds/Microsoft-commercial-marketplace-SaaS-offer-billing-SDK.git -b app-registration --depth 1; `
 cd ./Microsoft-commercial-marketplace-SaaS-offer-billing-SDK/deployment/Templates; `
 Connect-AzureAD -Confirm; .\Deploy.ps1 `
 -WebAppNamePrefix "marketplacesaasgithub" `
 -SQLServerName "marketplacesaasgithub" `
 -SQLAdminLogin "adminlogin" `
 -SQLAdminLoginPassword "a_very_PASSWORD_2_$ymB0L$" `
 -PublisherAdminUsers "user@email.com" `
 -BacpacUrl "https://raw.githubusercontent.com/Azure/Microsoft-commercial-marketplace-transactable-SaaS-offer-SDK/master/deployment/Database/AMPSaaSDB.bacpac" `
 -ResourceGroupForDeployment "MarketplaceSaasGitHub" `
 -Location "East US" `
 -PathToARMTemplate ".\deploy.json"
```