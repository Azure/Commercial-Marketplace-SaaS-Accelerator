## Install the Azure Marketplace SaaS Accelerator using Azure Cloud Shell

You can install the SaaS Accelerator code using a __single command__ line within the Azure Portal ([video tutorial](https://youtu.be/BVZTj6fssQ8)).

   1. Copy the following section to an editor and update it to match your company preference. 
      - Replace `SOME-UNIQUE-STRING` with your Team name or some other meaningful name for your depth. (Ensure that the final name does not exceed 24 characters)
      - Replace `user@emai.com` with a valid email from your org that will use the portal for the first time. Once deployed, this account will be able to login to the administration panel and give access to more users.
	  - [optional] Replace `MarketplaceSaasGitHub` under `ResourceGroupForDeployment` with a value that matches your comany's naming conventions for resource groups
	  - [optional] Replace `East US` with a region closest to you.

``` powershell
git clone https://github.com/Azure/Commercial-Marketplace-SaaS-Accelerator.git -b 6.0.1 --depth 1; `
cd ./Commercial-Marketplace-SaaS-Accelerator/deployment; `
Connect-AzureAD -Confirm; `
.\Deploy.ps1 `
 -WebAppNamePrefix "marketplace-SOME-UNIQUE-STRING" `
 -SQLServerName "marketplace-SOME-UNIQUE-STRING" `
 -SQLAdminLogin "adminlogin" `
 -SQLAdminLoginPassword "" `
 -PublisherAdminUsers "user@email.com" `
 -ResourceGroupForDeployment "MarketplaceSaasGitHub" `
 -Location "East US" ;
.\Deploy-Code.ps1 `
 -WebAppNamePrefix "marketplace-SOME-UNIQUE-STRING" `
 -ResourceGroupForDeployment "MarketplaceSaasGitHub";
 ```

  The following lines are optional:
 ``` powershell
 -TenantID "xxxx-xxx-xxx-xxx-xxxx" `
 -AzureSubscriptionID "xxx-xx-xx-xx-xxxx" `
 -ADApplicationID "xxxx-xxx-xxx-xxx-xxxx" `
 -ADApplicationSecret "xxxx-xxx-xxx-xxx-xxxx" `
 -ADMTApplicationID "xxxx-xxx-xxx-xxx-xxxx" `
 -LogoURLpng "https://company_com/company_logo.png" `
 -LogoURLico "https://company_com/company_logo.ico"
 ```
The scripts above will perform these actions:
   - Create required App Registration for SaaS Marketplace API authentication
   - Create APP registration for SSO on your Landing Page
   - Deploy required infrastructure in Azure for 

## Update to a newer version of the SaaS Accelerator

If you are already have deployed the SaaS Accelerator, but you want to update it so that you take advantage of new features developed, you can run the following command.

*you need to ensure that you use the same parameters you used in the initial deployment 

``` powershell
git clone https://github.com/Azure/Commercial-Marketplace-SaaS-Accelerator.git -b <branch-to-deploy> --depth 1; `
cd ./Commercial-Marketplace-SaaS-Accelerator/deployment; `
.\Deploy-Code.ps1 `
 -WebAppNamePrefix "marketplace-SOME-UNIQUE-STRING" `
 -ResourceGroupForDeployment "MarketplaceSaasGitHub";
 ```

## Parameters

| Parameter | Description |
|-----------| -------------|
| WebAppNamePrefix | A unique prefix used for creating web applications. Example: contoso |
| TenantID | The value should match the value provided for Active Directory TenantID in the Technical Configuration of the Transactable Offer in Partner Center |
| ADApplicationID | The value should match the value provided for Active Directory Single-Tenant Application ID in the Technical Configuration of the Transactable Offer in Partner Center |
| ADApplicationSecret | Secret key of the AD Application |
| ADMTApplicationID | The value should match the value provided for Active Directory Multi-Tenant Application ID in the Technical Configuration of the Transactable Offer in Partner Center |
| SQLServerName | A unique name of the database server (without database.windows.net) |
| SQLAdminLogin | SQL Admin login |
| SQLAdminLoginPassword | SQL Admin password |
| PublisherAdminUsers | Provide a list of email addresses (as comma-separated-values) that should be granted access to the Publisher Portal |
| ResourceGroupForDeployment | Name of the resource group to deploy the resources |
| Location | Location of the resource group |
| AzureSubscriptionID | Subscription where the resources be deployed |
| LogoURLpng | The url of the company logo image in .png format with a size of 96x96 to be used on the website |
| LogoURLico | The url of the company logo image in .ico format |
| MeteredSchedulerSupport | Enable the metered scheduler. This is deployed by default. Use **true** to enable the feature. More information [here](https://github.com/Azure/Commercial-Marketplace-SaaS-Accelerator/blob/main/docs/Metered-Scheduler-Manager-Instruction.md).

## Alternative Deployments
There are other ways to deploy the SaaS Accelerator environment (e.g. development, maual deployment, etc).  Additional instruction can be found [here](Advanced-Instructions.md).
