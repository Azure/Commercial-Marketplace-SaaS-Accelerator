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
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh; `
chmod +x dotnet-install.sh; `
./dotnet-install.sh -version 10.0.400; `
$ENV:PATH="$HOME/.dotnet:$ENV:PATH"; `
dotnet tool install --global dotnet-ef --version 10.0.11; `
$ENV:PATH += ":$HOME/.dotnet/tools"; `
git clone https://github.com/Azure/Commercial-Marketplace-SaaS-Accelerator.git -b 10.0.0 --depth 1; `
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
*if upgrading to an older release, use the .NET SDK and dotnet-ef versions required by that release.

``` powershell
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh; `
chmod +x dotnet-install.sh; `
./dotnet-install.sh -version 10.0.400; `
$ENV:PATH="$HOME/.dotnet:$ENV:PATH"; `
dotnet tool update --global dotnet-ef --version 10.0.11; `
$ENV:PATH += ":$HOME/.dotnet/tools"; `
git clone https://github.com/Azure/Commercial-Marketplace-SaaS-Accelerator.git -b <release-version-branch-to-deploy> --depth 1; `
cd ./Commercial-Marketplace-SaaS-Accelerator/deployment; `
.\Upgrade.ps1 `
 -WebAppNamePrefix "marketplace-SOME-UNIQUE-STRING" `
 -ResourceGroupForDeployment "marketplace-SOME-UNIQUE-STRING"
 ```

### Optional upgrade parameters

| Parameter | Description |
|-----------|-------------|
| AzureSubscriptionID | Subscription ID for the existing deployment. If omitted, the script prompts and offers the current Azure CLI subscription. The selected subscription and tenant are verified even when this parameter is supplied. |
| UsePrivateEndpoints | Pass `$true` for a private-endpoint deployment or `$false` for a public-endpoint deployment. If omitted, the existing Y/N prompt is retained. This describes the required execution environment; it does not change networking or authentication. |

For example, to supply both values explicitly:

```powershell
.\Upgrade.ps1 `
 -WebAppNamePrefix "SOME-UNIQUE-STRING" `
 -ResourceGroupForDeployment "SOME-RG-NAME" `
 -AzureSubscriptionID "<subscription-id>" `
 -UsePrivateEndpoints $false
```

Use `-UsePrivateEndpoints $true` only from a host with the required private connectivity described below. These parameters skip the subscription and private-endpoint prompts, but the consent and tenant prompts remain.

### Deployment and upgrade prerequisites and recovery

- Sign in with `az login` (Cloud Shell normally signs in automatically). Check `az account show` and select the intended subscription and tenant when prompted. The Az PowerShell context used to list tenants/subscriptions must also be signed in (`Connect-AzAccount` when running locally). The scripts verify the Azure CLI subscription and tenant before proceeding.
- Install the `SqlServer` PowerShell module, version 22 or later (`Install-Module SqlServer -MinimumVersion 22.0.0 -Scope CurrentUser`). The scripts check that `Invoke-Sqlcmd` supports access tokens, terminating SQL errors and encryption settings. Keep the SDK and `dotnet-ef` versions appropriate to the release being deployed. The PATH additions above are for Linux Cloud Shell and must be repeated in each session.
- The operator needs permission to read the `DefaultConnection` Key Vault secret, deploy to both web apps, and access the database. Database migrations require `db_owner` membership or `CONTROL DATABASE`; the scripts check this before migration. New installations also create database users for the web applications and require permission to create the documented Azure resources and app registrations.
- Upgrades support SQL credentials (`User ID`/`UID` and `Password`/`PWD`, with no `Authentication` setting or with `Authentication=Sql Password`). They also support `Authentication=Active Directory Managed Identity` (or `MSI`), `Active Directory Default`, and `Active Directory Interactive`. In those Entra modes, **the signed-in Azure CLI operator** obtains an Azure SQL token, not the web application's Managed Identity. Provision that operator as a database user with the required permissions. A managed-identity client ID in the application's connection string is not used as the operator's identity. Password-based Entra authentication, service-principal/workload-identity connection strings, integrated Windows authentication and ambiguous credential combinations are rejected.
- `Server`/`Data Source`/`Address`/`Addr`/`Network Address`, `Database`/`Initial Catalog`, and SQL credential aliases are parsed without substring assumptions. Conflicting aliases are rejected. Encryption defaults follow the installed SqlServer module; explicit `Encrypt` and `TrustServerCertificate` settings are honored. Prefer a correctly trusted SQL certificate rather than disabling validation.
- **Before upgrading, take a database backup or confirm Azure SQL point-in-time restore retention and test your recovery procedure.** Save customizations and record the currently deployed application version. Plan a maintenance window: migrations and application deployment are **not atomic**, and there is no automatic rollback.
- Both scripts restore the solution, build all application components, generate a fresh idempotent migration, and package the applications before database changes. Command failures stop the process; a build/restore/generation failure does not run SQL. A migration or later deployment failure can leave partial changes. Inspect the database and application versions before retrying or restoring a backup; do not assume that failure rolled anything back.
- Generated migration scripts and publish output are isolated in a unique temporary directory and removed in `finally`. Existing development settings, legacy `deployment/script.sql`, and existing `Publish` directories are not overwritten or deleted. Real connection strings are not written into temporary JSON; EF generates the script offline with a non-secret environment override, restored afterwards. Development settings are excluded from deployment packages.
- For upgrades, allow the execution host through your existing SQL/Key Vault network controls before starting. The upgrade script does not add firewall rules or enable public network access. New installations retain the existing provisioning topology and temporarily allow only the client's IPv4 address for SQL setup; their unique firewall rule is removed on success or failure. A cleanup failure is reported as a failure and may require manual removal of that rule.

### Private endpoint upgrades

Use a host with private network routing and private DNS resolution to SQL and Key Vault, plus access to App Service deployment endpoints (for example a suitably configured VM or VNet-isolated Cloud Shell). Ordinary public Cloud Shell cannot reach private-only resources. Selecting private endpoints does not grant connectivity, switch authentication, or open a public firewall. The same migration and authentication flow runs for both network topologies; inaccessible resources cause a terminating error.

The private network path and initial provisioning topology still require integration validation in your environment. These changes do not claim complete private endpoint support or alter public-network-access policies automatically.

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
