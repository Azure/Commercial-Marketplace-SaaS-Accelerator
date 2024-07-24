---

The SaaS Accelerator is offered under the MIT License as open source software and is <ins>not supported</ins> by Microsoft. <br>
If you need help with the accelerator or would like to report defects or feature requests, use the Issues feature on this GitHub repository.

---

# Install the Azure Marketplace SaaS Accelerator using Azure Cloud Shell

<!-- no toc -->
- [Video walkthrough of installation](#video-walkthrough-of-installation)
- [Basic installation script](#basic-installation-script)
  - [Optional install script parameters](#optional-install-script-parameters)
- [Update to a newer version of the SaaS Accelerator](#update-to-a-newer-version-of-the-saas-accelerator)
- [Install script parameter descriptions](#install-script-parameter-descriptions)
- [Setting up a development environment](#setting-up-a-development-environment)
- [Alternative deployments](#alternative-deployments)

## Video walkthrough of installation

⏯️ See this video for a complete walkthrough: [Installing the SaaS Accelerator with the Azure portal cloud shell](https://go.microsoft.com/fwlink/?linkid=2196326).

## Basic installation script

You can install the SaaS Accelerator code using a __single command__ line within the Azure Portal Cloud Shell.

> Note: use the [Azure Cloud Shell](https://shell.azure.com)'s PowerShell shell, not the default bash shell. You can select the shell via the drop-down in the top left corner.

Copy the following section to an editor and update it to match your company preference.

- Replace `SOME-UNIQUE-STRING` with your Team name or some other meaningful name for your depth. (Ensure that the final name does not exceed 21 characters)
- Replace `user@email.com` with a valid email from your org that will use the portal for the first time. Once deployed, this account will be able to login to the administration panel and give access to more users.
- Replace `SOME-RG-NAME` with a value that matches your company's naming conventions for resource groups
- [Optional] Replace `East US` with a region closest to you.

``` powershell
wget https://dotnet.microsoft.com/download/dotnet/scripts/v1/dotnet-install.sh; `
chmod +x dotnet-install.sh; `
./dotnet-install.sh -version 6.0.417; `
$ENV:PATH="$HOME/.dotnet:$ENV:PATH"; `
dotnet tool install --global dotnet-ef --version 6.0.1; `
git clone https://github.com/Azure/Commercial-Marketplace-SaaS-Accelerator.git -b 7.6.2 --depth 1; `
cd ./Commercial-Marketplace-SaaS-Accelerator/deployment; `
.\Deploy.ps1 `
 -WebAppNamePrefix "SOME-UNIQUE-STRING" `
 -ResourceGroupForDeployment "SOME-RG-NAME" `
 -PublisherAdminUsers "user1@email.com,user2@email" `
 -Location "East US" 
 ```

The script above will perform the following actions.

- Create required App Registration for SaaS Marketplace API authentication
- Create required App Registration for SSO on your Landing Page
- Deploy required infrastructure in Azure for hosting the landing page, webhook and admin portal
- Deploy the code and database on the infrastructure.

### Optional install script parameters

 The following are optional parameters that you can include in the deployment  (see parameter description below for details).
 
 ``` powershell
 -TenantID "xxxx-xxx-xxx-xxx-xxxx" `
 -AzureSubscriptionID "xxx-xx-xx-xx-xxxx" `
 -ADApplicationID "xxxx-xxx-xxx-xxx-xxxx" `
 -ADApplicationSecret "xxxx-xxx-xxx-xxx-xxxx" `
 -ADMTApplicationID "xxxx-xxx-xxx-xxx-xxxx" `
 -LogoURLpng "https://company_com/company_logo.png" `
 -LogoURLico "https://company_com/company_logo.ico" `
 -IsAdminPortalMultiTenant "true" `
 -Quiet
 ```

## Update to a newer version of the SaaS Accelerator

⚠️Caution: This will deploy/upgrade the Accelerator to the `<branch-to-deploy>` specified while running the upgrade command, which will replace the current version. If you have any custom changes, we recommend you implement a strategy to backup and replace them after the upgrade.

If you already have deployed the SaaS Accelerator, but you want to update it so that you take advantage of new features developed, you can run the following command:

*you need to ensure that you use the same parameters you used in the initial deployment 

``` powershell
wget https://dotnet.microsoft.com/download/dotnet/scripts/v1/dotnet-install.sh; `
chmod +x dotnet-install.sh; `
./dotnet-install.sh -version 6.0.417; `
$ENV:PATH="$HOME/.dotnet:$ENV:PATH"; `
dotnet tool install --global dotnet-ef --version 6.0.1; `
git clone https://github.com/Azure/Commercial-Marketplace-SaaS-Accelerator.git -b <release-version-branch-to-deploy> --depth 1; `
cd ./Commercial-Marketplace-SaaS-Accelerator/deployment; `
.\Upgrade.ps1 `
 -WebAppNamePrefix "marketplace-SOME-UNIQUE-STRING" `
 -ResourceGroupForDeployment "marketplace-SOME-UNIQUE-STRING" `
 ```

## Install script parameter descriptions

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
| LogoURLpng | The url of the company logo image in .png format with a size of 96x96 to be used on the website |
| LogoURLico | The url of the company logo image in .ico format |
| IsAdminPortalMultiTenant | Set to `true` if you want to enable multi-tenant support for the admin portal. Default: `false` |
| Quiet | Disable verbose output when running the script

## Setting up a development environment

You may be interested in customizing the look and feel or the sites or behavior of the code. There is a video module showing how to set up a development environment for the SaaS Accelerator.

The video is rather lengthy, so use the chapter links in the video description to skip to the exact section you want to see.

⏯️ [Setting up a development environment for the SaaS Accelerator](https://go.microsoft.com/fwlink/?linkid=2224222)

## Alternative deployments
There are other ways to deploy the SaaS Accelerator environment (e.g. development, maual deployment, etc).  Additional instruction can be found [here](Advanced-Instructions.md).

## Authentication between the WebApps and the Database
The Webapps uses Managed Identity to communicate with the database. The Managed Identity is created during the deployment of the WebApps. The Managed Identity is then used to create a user in the database and grant the user the necessary permissions. The connection string used by the WebApps to connect to the database is then updated to use the Managed Identity.
For more information on how App Service use Managed Identity to connect to the database, please refer to the following [link](https://learn.microsoft.com/en-us/azure/app-service/tutorial-connect-msi-sql-database).
