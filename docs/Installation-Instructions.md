## Install the Azure Marketplace SaaS Accelerator using Azure Cloud Shell

You can install the SaaS Accelerator code using a __single command__ line within the Azure Portal ([video tutorial](https://youtu.be/BVZTj6fssQ8)).

> Note: use the Azure Cloud Shell's PowerShell shell, not the default bash shell.

Copy the following section to an editor and update it to match your company preference. 
   - Replace `SOME-UNIQUE-STRING` with your Team name or some other meaningful name for your depth. (Ensure that the final name does not exceed 24 characters)
   - Replace `user@email.com` with a valid email from your org that will use the portal for the first time. Once deployed, this account will be able to login to the administration panel and give access to more users.
   - [optional] Replace `MarketplaceSaasGitHub` under `ResourceGroupForDeployment` with a value that matches your comany's naming conventions for resource groups
   - [optional] Replace `East US` with a region closest to you.

``` powershell
dotnet tool install --global dotnet-ef; `
git clone https://github.com/Azure/Commercial-Marketplace-SaaS-Accelerator.git -b 6.1.2 --depth 1; `
cd ./Commercial-Marketplace-SaaS-Accelerator/deployment; `
.\Deploy.ps1 `
 -WebAppNamePrefix "marketplace-SOME-UNIQUE-STRING" `
 -ResourceGroupForDeployment "marketplace-SOME-UNIQUE-STRING" `
 -PublisherAdminUsers "user@email.com" `
 -Location "East US" 
 ```

The script above will perform these actions:
   - Create required App Registration for SaaS Marketplace API authentication
   - Create required Aoo Registration for SSO on your Landing Page
   - Deploy required infrastructure in Azure for hosting the landing page, webhook and admin portal
   - Deploy the code and database on the infrastructure.


 The following are optional parameters that you can include in the deployment  (see parameter description below for details)
 
 ``` powershell
 -TenantID "xxxx-xxx-xxx-xxx-xxxx" `
 -AzureSubscriptionID "xxx-xx-xx-xx-xxxx" `
 -ADApplicationID "xxxx-xxx-xxx-xxx-xxxx" `
 -ADApplicationSecret "xxxx-xxx-xxx-xxx-xxxx" `
 -ADMTApplicationID "xxxx-xxx-xxx-xxx-xxxx" `
 -LogoURLpng "https://company_com/company_logo.png" `
 -LogoURLico "https://company_com/company_logo.ico" `
 -MeteredSchedulerSupport
 -Quiet
 ```

## Update to a newer version of the SaaS Accelerator

If you already have deployed the SaaS Accelerator, but you want to update it so that you take advantage of new features developed, you can run the following command:

*you need to ensure that you use the same parameters you used in the initial deployment 

``` powershell
git clone https://github.com/Azure/Commercial-Marketplace-SaaS-Accelerator.git -b <branch-to-deploy> --depth 1; `
cd ./Commercial-Marketplace-SaaS-Accelerator/deployment; `
.\Upgrade.ps1 `
 -WebAppNamePrefix "marketplace-SOME-UNIQUE-STRING" `
 -ResourceGroupForDeployment "marketplace-SOME-UNIQUE-STRING"" `
 ```

## Parameters

| Parameter | Description |
|-----------| -------------|
| WebAppNamePrefix | _[required]_ A unique prefix used for creating web applications. Example: `contoso` |
| ResourceGroupForDeployment | Name of the resource group to deploy the resources. Default: `WebAppNamePrefix` value |
| Location | _[required]_ Location of the resource group |
| PublisherAdminUsers | _[required]_ Provide a list of email addresses (as comma-separated-values) that should be granted access to the Publisher Portal |
| TenantID | The value should match the value provided for Active Directory TenantID in the Technical Configuration of the Transactable Offer in Partner Center. If value not provided, you will be asked to select the tenant during deployment |
| AzureSubscriptionID | Id of subscription where the resources will be deployed. Subscription must be part of the Tenant Provided. If value not provided, you will be asked to select the subscription during deployment. |
| ADApplicationID | The value should match the value provided for Active Directory Application ID in the Technical Configuration of the Transactable Offer in Partner Center. If value not provided, a new application will be created. |
| ADApplicationSecret | Valid secret for the ADApplication. Required if ADApplicationID is provided. If `ADApplicationID` is not provided, a secret will be generated. |
| ADMTApplicationID | A valid App Id for an Azure AD Application configured for SSO login. If value not provided, a new application will be created. |
| SQLServerName | A unique name of the database server (without database.windows.net). Default: `WebAppNamePrefix`-sql |
| SQLAdminLogin | SQL Admin login. Default: 'saasdbadminxxx' where xxx is a random number. |
| SQLAdminLoginPassword | SQL Admin password. Default: secure random password. |
| LogoURLpng | The url of the company logo image in .png format with a size of 96x96 to be used on the website |
| LogoURLico | The url of the company logo image in .ico format |
| MeteredSchedulerSupport | Enable the metered scheduler. This is deployed by default. Use **true** to enable the feature. More information [here](https://github.com/Azure/Commercial-Marketplace-SaaS-Accelerator/blob/main/docs/Metered-Scheduler-Manager-Instruction.md).
| Quiet | Disable verbose output when running the script

## Alternative Deployments
There are other ways to deploy the SaaS Accelerator environment (e.g. development, maual deployment, etc).  Additional instruction can be found [here](Advanced-Instructions.md).
