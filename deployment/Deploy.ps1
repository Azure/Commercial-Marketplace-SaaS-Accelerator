# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See LICENSE file in the project root for license information.

#
# Powershell script to deploy the resources - Customer portal, Publisher portal and the Azure SQL Database
#

#.\Deploy.ps1 `
# -WebAppNamePrefix "amp_saas_accelerator_<unique>" `
# -Location "<region>" `
# -PublisherAdminUsers "<your@email.address>"

Param(  
   [string][Parameter(Mandatory)]$WebAppNamePrefix, # Prefix used for creating web applications
   [string][Parameter()]$ResourceGroupForDeployment, # Name of the resource group to deploy the resources
   [string][Parameter(Mandatory)]$Location, # Location of the resource group
   [string][Parameter(Mandatory)]$PublisherAdminUsers, # Provide a list of email addresses (as comma-separated-values) that should be granted access to the Publisher Portal
   [string][Parameter()]$TenantID, # The value should match the value provided for Active Directory TenantID in the Technical Configuration of the Transactable Offer in Partner Center
   [string][Parameter()]$AzureSubscriptionID, # Subscription where the resources be deployed
   [string][Parameter()]$ADApplicationID, # The value should match the value provided for Active Directory Application ID in the Technical Configuration of the Transactable Offer in Partner Center
   [string][Parameter()]$ADApplicationSecret, # Secret key of the AD Application
   [string][Parameter()]$ADMTApplicationID, # Multi-Tenant Active Directory Application ID
   [string][Parameter()]$SQLDatabaseName, # Name of the database (Defaults to AMPSaaSDB)
   [string][Parameter()]$SQLServerName, # Name of the database server (without database.windows.net)
   [string][Parameter()]$SQLAdminLogin, # SQL Admin login
   [string][Parameter()][ValidatePattern('^[^\s$@]{1,128}$')]$SQLAdminLoginPassword, # SQL Admin password  
   [string][Parameter()]$LogoURLpng,  # URL for Publisher .png logo
   [string][Parameter()]$LogoURLico,  # URL for Publisher .ico logo
   [string][Parameter()]$KeyVault, # Name of KeyVault
   [switch][Parameter()]$MeteredSchedulerSupport, # Add this key like -MeteredSchedulerSupport to the command to enable Metered Scheduler Support
   [switch][Parameter()]$Quiet #if set, only show error / warning output from script commands
)

# Make sure to install Az Module before running this script
# Install-Module Az
# Install-Module -Name AzureAD

$ErrorActionPreference = "Stop"
$startTime = Get-Date
#region Set up Variables and Default Parameters

if ($ResourceGroupForDeployment -eq "") {
    $ResourceGroupForDeployment = $WebAppNamePrefix 
}
if ($SQLServerName -eq "") {
    $SQLServerName = $WebAppNamePrefix + "-sql"
}
if ($SQLAdminLogin -eq "") {
    $SQLAdminLogin = "saasdbadmin" + $(Get-Random -Minimum 1 -Maximum 1000)
}
if ($SQLAdminLoginPassword -eq "") {
    $SQLAdminLoginPassword = ([System.Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes((New-Guid))))+"="
}
if ($SQLDatabaseName -eq "") {
    $SQLDatabaseName = "AMPSaaSDB"
}

if($KeyVault -eq "")
{
   $KeyVault=$WebAppNamePrefix+"-kv"
}

$SaaSApiConfiguration_CodeHash= git log --format='%H' -1
$azCliOutput = if($Quiet){'none'} else {'json'}

#endregion

#region Validate Parameters

if($SQLAdminLogin.ToLower() -eq "admin") {
    Throw "🛑 SQLAdminLogin may not be 'admin'."
    Exit
}
if($SQLAdminLoginPassword.Length -lt 8) {
    Throw "🛑 SQLAdminLoginPassword must be at least 8 characters."
    Exit
}
if($WebAppNamePrefix.Length -gt 21) {
    Throw "🛑 Web name prefix must be less than 21 characters."
    Exit
}


if(!($KeyVault -match "^[a-zA-Z][a-z0-9-]+$")) {
    Throw "🛑 KeyVault name only allows alphanumeric and hyphens, but cannot start with a number or special character."
    Exit
}

#endregion 

Write-Host "Starting SaaS Accelerator Deployment..."

#region Select Tenant / Subscription for deployment

$currentContext = az account show | ConvertFrom-Json
$currentTenant = $currentContext.tenantId
$currentSubscription = $currentContext.id

#Get TenantID if not set as argument
if(!($TenantID)) {    
    Get-AzTenant | Format-Table
    if (!($TenantID = Read-Host "⌨  Type your TenantID or press Enter to accept your current one [$currentTenant]")) { $TenantID = $currentTenant }    
}
else {
    Write-Host "🔑 Tenant provided: $TenantID"
}

#Get Azure Subscription if not set as argument
if(!($AzureSubscriptionID)) {    
    Get-AzSubscription -TenantId $TenantID | Format-Table
    if (!($AzureSubscriptionID = Read-Host "⌨  Type your SubscriptionID or press Enter to accept your current one [$currentSubscription]")) { $AzureSubscriptionID = $currentSubscription }
}
else {
    Write-Host "🔑 Azure Subscription provided: $AzureSubscriptionID"
}

#Set the AZ Cli context
az account set -s $AzureSubscriptionID
Write-Host "🔑 Azure Subscription '$AzureSubscriptionID' selected."

#endregion

#region Dowloading assets if provided

# Download Publisher's PNG logo
if($LogoURLpng) { 
    Write-Host "📷 Logo image provided"
	Write-Host "   🔵 Downloading Logo image file"
    Invoke-WebRequest -Uri $LogoURLpng -OutFile "../src/CustomerSite/wwwroot/contoso-sales.png"
    Invoke-WebRequest -Uri $LogoURLpng -OutFile "../src/AdminSite/wwwroot/contoso-sales.png"
    Write-Host "   🔵 Logo image downloaded"
}

# Download Publisher's FAVICON logo
if($LogoURLico) { 
    Write-Host "📷 Logo icon provided"
	Write-Host "   🔵 Downloading Logo icon file"
    Invoke-WebRequest -Uri $LogoURLico -OutFile "../src/CustomerSite/wwwroot/favicon.ico"
    Invoke-WebRequest -Uri $LogoURLico -OutFile "../src/AdminSite/wwwroot/favicon.ico"
    Write-Host "   🔵 Logo icon downloaded"
}

#endregion
 
#region Create AAD App Registrations

#Record the current ADApps to reduce deployment instructions at the end
$ISADMTApplicationIDProvided = $ADMTApplicationID

#Create App Registration for authenticating calls to the Marketplace API
if (!($ADApplicationID)) {   
    Write-Host "🔑 Creating Fulfilment API App Registration"
    try {   
        $ADApplication = az ad app create --only-show-errors --display-name "$WebAppNamePrefix-FulfillmentAppReg" | ConvertFrom-Json
		$ADObjectID = $ADApplication.id
        $ADApplicationID = $ADApplication.appId
        sleep 5 #this is to give time to AAD to register
        $ADApplicationSecret = az ad app credential reset --id $ADObjectID --append --display-name 'SaaSAPI' --years 2 --query password --only-show-errors --output tsv
				
        Write-Host "   🔵 FulfilmentAPI App Registration created."
		Write-Host "      ➡️ Application ID:" $ADApplicationID  
        Write-Host "      ➡️ App Secret:" $ADApplicationSecret
    }
    catch [System.Net.WebException],[System.IO.IOException] {
        Write-Host "🚨🚨   $PSItem.Exception"
        break;
    }
}

#Create Multi-Tenant App Registration for Landing Page User Login
if (!($ADMTApplicationID)) {  
    Write-Host "🔑 Creating Landing Page SSO App Registration"
    try {
	
		$appCreateRequestBodyJson = @"
{
	"displayName" : "$WebAppNamePrefix-LandingpageAppReg",
	"api": 
	{
		"requestedAccessTokenVersion" : 2
	},
	"signInAudience" : "AzureADandPersonalMicrosoftAccount",
	"web":
	{ 
		"redirectUris": 
		[
			"https://$WebAppNamePrefix-portal.azurewebsites.net",
			"https://$WebAppNamePrefix-portal.azurewebsites.net/",
			"https://$WebAppNamePrefix-portal.azurewebsites.net/Home/Index",
			"https://$WebAppNamePrefix-portal.azurewebsites.net/Home/Index/",
			"https://$WebAppNamePrefix-admin.azurewebsites.net",
			"https://$WebAppNamePrefix-admin.azurewebsites.net/",
			"https://$WebAppNamePrefix-admin.azurewebsites.net/Home/Index",
			"https://$WebAppNamePrefix-admin.azurewebsites.net/Home/Index/"
		],
		"logoutUrl": "https://$WebAppNamePrefix-portal.azurewebsites.net/logout",
		"implicitGrantSettings": 
			{ "enableIdTokenIssuance" : true }
	},
	"requiredResourceAccess":
	[{
		"resourceAppId": "00000003-0000-0000-c000-000000000000",
		"resourceAccess":
			[{ 
				"id": "e1fe6dd8-ba31-4d61-89e7-88639da4683d",
				"type": "Scope" 
			}]
	}]
}
"@	
		if ($PsVersionTable.Platform -ne 'Unix') {
			#On Windows, we need to escape quotes and remove new lines before sending the payload to az rest. 
			# See: https://github.com/Azure/azure-cli/blob/dev/doc/quoting-issues-with-powershell.md#double-quotes--are-lost
			$appCreateRequestBodyJson = $appCreateRequestBodyJson.replace('"','\"').replace("`r`n","")
		}

		$landingpageLoginAppReg = $(az rest --method POST --headers "Content-Type=application/json" --uri https://graph.microsoft.com/v1.0/applications --body $appCreateRequestBodyJson  ) | ConvertFrom-Json
	
		$ADMTApplicationID = $landingpageLoginAppReg.appId
		$ADMTObjectID = $landingpageLoginAppReg.id
	
        Write-Host "   🔵 Landing Page SSO App Registration created."
		Write-Host "      ➡️ Application Id: $ADMTApplicationID"
	
		# Download Publisher's AppRegistration logo
        if($LogoURLpng) { 
			Write-Host "   🔵 Logo image provided. Setting the Application branding logo"
			Write-Host "      ➡️ Setting the Application branding logo"
			$token=(az account get-access-token --resource "https://graph.microsoft.com" --query accessToken --output tsv)
			$logoWeb = Invoke-WebRequest $LogoURLpng
			$logoContentType = $logoWeb.Headers["Content-Type"]
			$logoContent = $logoWeb.Content
			
			$uploaded = Invoke-WebRequest `
			  -Uri "https://graph.microsoft.com/v1.0/applications/$ADMTObjectID/logo" `
			  -Method "PUT" `
			  -Header @{"Authorization"="Bearer $token";"Content-Type"="$logoContentType";} `
			  -Body $logoContent
		    
			Write-Host "      ➡️ Application branding logo set."
        }

    }
    catch [System.Net.WebException],[System.IO.IOException] {
        Write-Host "🚨🚨   $PSItem.Exception"
        break;
    }
}

#endregion

#region Prepare Code Packages
Write-host "📜 Prepare publish files for the application"
if (!(Test-Path '../Publish')) {		
	Write-host "   🔵 Preparing Admin Site"  
	dotnet publish ../src/AdminSite/AdminSite.csproj -c debug -o ../Publish/AdminSite/ -v q

	Write-host "   🔵 Preparing Metered Scheduler"
	dotnet publish ../src/MeteredTriggerJob/MeteredTriggerJob.csproj -c debug -o ../Publish/AdminSite/app_data/jobs/triggered/MeteredTriggerJob/ -v q --runtime win-x64 --self-contained true 

	Write-host "   🔵 Preparing Customer Site"
	dotnet publish ../src/CustomerSite/CustomerSite.csproj -c debug -o ../Publish/CustomerSite/ -v q

	Write-host "   🔵 Zipping packages"
	Compress-Archive -Path ../Publish/AdminSite/* -DestinationPath ../Publish/AdminSite.zip -Force
	Compress-Archive -Path ../Publish/CustomerSite/* -DestinationPath ../Publish/CustomerSite.zip -Force
}
#endregion

#region Deploy Azure Resources Infrastructure
Write-host "☁ Deploy Azure Resources"

#Set-up resource name variables
$WebAppNameService=$WebAppNamePrefix+"-asp"
$WebAppNameAdmin=$WebAppNamePrefix+"-admin"
$WebAppNamePortal=$WebAppNamePrefix+"-portal"

#keep the space at the end of the string - bug in az cli running on windows powershell truncates last char https://github.com/Azure/azure-cli/issues/10066
$ADApplicationSecretKeyVault="@Microsoft.KeyVault(VaultName=$KeyVault;SecretName=ADApplicationSecret) "
$DefaultConnectionKeyVault="@Microsoft.KeyVault(VaultName=$KeyVault;SecretName=DefaultConnection) "
$ServerUri = $SQLServerName+".database.windows.net"
$Connection="Data Source=tcp:"+$ServerUri+",1433;Initial Catalog=AMPSaaSDB;User Id="+$SQLAdminLogin+"@"+$SQLServerName+".database.windows.net;Password="+$SQLAdminLoginPassword+";"


Write-host "   🔵 Resource Group"
Write-host "      ➡️ Create Resource Group"
az group create --location $Location --name $ResourceGroupForDeployment --output $azCliOutput

Write-host "   🔵 SQL Server"
Write-host "      ➡️ Create Sql Server"
az sql server create --name $SQLServerName --resource-group $ResourceGroupForDeployment --location $Location --admin-user $SQLAdminLogin --admin-password $SQLAdminLoginPassword --output $azCliOutput
Write-host "      ➡️ Add SQL Server Firewall rules"
az sql server firewall-rule create --resource-group $ResourceGroupForDeployment --server $SQLServerName -n AllowAzureIP --start-ip-address "0.0.0.0" --end-ip-address "0.0.0.0" --output $azCliOutput
if ($env:ACC_CLOUD -eq $null){
    Write-host "      ➡️ Running in local environment - Add current IP to firewall"
	$publicIp = (Invoke-WebRequest -uri "http://ifconfig.me/ip").Content
    az sql server firewall-rule create --resource-group $ResourceGroupForDeployment --server $SQLServerName -n AllowIP --start-ip-address "$publicIp" --end-ip-address "$publicIp" --output $azCliOutput
}
Write-host "      ➡️ Create SQL DB"
az sql db create --resource-group $ResourceGroupForDeployment --server $SQLServerName --name $SQLDatabaseName  --edition Standard  --capacity 10 --zone-redundant false --output $azCliOutput

Write-host "   🔵 KeyVault"
Write-host "      ➡️ Create KeyVault"
az keyvault create --name $KeyVault --resource-group $ResourceGroupForDeployment --output $azCliOutput
Write-host "      ➡️ Add Secrets"
az keyvault secret set --vault-name $KeyVault  --name ADApplicationSecret --value $ADApplicationSecret --output $azCliOutput
az keyvault secret set --vault-name $KeyVault  --name DefaultConnection --value $Connection --output $azCliOutput

Write-host "   🔵 App Service Plan"
Write-host "      ➡️ Create App Service Plan"
az appservice plan create -g $ResourceGroupForDeployment -n $WebAppNameService --sku B1 --output $azCliOutput

Write-host "   🔵 Admin Portal WebApp"
Write-host "      ➡️ Create Web App"
az webapp create -g $ResourceGroupForDeployment -p $WebAppNameService -n $WebAppNameAdmin  --runtime dotnet:6 --output $azCliOutput
Write-host "      ➡️ Assign Identity"
$WebAppNameAdminId = az webapp identity assign -g $ResourceGroupForDeployment  -n $WebAppNameAdmin --identities [system] --query principalId -o tsv
Write-host "      ➡️ Setup access to KeyVault"
az keyvault set-policy --name $KeyVault  --object-id $WebAppNameAdminId --secret-permissions get list --key-permissions get list --output $azCliOutput
Write-host "      ➡️ Set Configuration"
az webapp config connection-string set -g $ResourceGroupForDeployment -n $WebAppNameAdmin -t SQLAzure --output $azCliOutput --settings DefaultConnection=$DefaultConnectionKeyVault 
az webapp config appsettings set -g $ResourceGroupForDeployment  -n $WebAppNameAdmin --output $azCliOutput --settings KnownUsers=$PublisherAdminUsers SaaSApiConfiguration__AdAuthenticationEndPoint=https://login.microsoftonline.com SaaSApiConfiguration__ClientId=$ADApplicationID SaaSApiConfiguration__ClientSecret=$ADApplicationSecretKeyVault SaaSApiConfiguration__FulFillmentAPIBaseURL=https://marketplaceapi.microsoft.com/api SaaSApiConfiguration__FulFillmentAPIVersion=2018-08-31 SaaSApiConfiguration__GrantType=client_credentials SaaSApiConfiguration__MTClientId=$ADMTApplicationID SaaSApiConfiguration__Resource=20e940b3-4c77-4b0b-9a53-9e16a1b010a7 SaaSApiConfiguration__TenantId=$TenantID SaaSApiConfiguration__SignedOutRedirectUri=https://$WebAppNamePrefix-portal.azurewebsites.net/Home/Index/ SaaSApiConfiguration__SupportmeteredBilling=$MeteredSchedulerSupport SaaSApiConfiguration_CodeHash=$SaaSApiConfiguration_CodeHash
az webapp config set -g $ResourceGroupForDeployment -n $WebAppNameAdmin --always-on true  --output $azCliOutput

Write-host "   🔵 Customer Portal WebApp"
Write-host "      ➡️ Create Web App"
az webapp create -g $ResourceGroupForDeployment -p $WebAppNameService -n $WebAppNamePortal --runtime dotnet:6 --output $azCliOutput
Write-host "      ➡️ Assign Identity"
$WebAppNamePortalId= az webapp identity assign -g $ResourceGroupForDeployment  -n $WebAppNamePortal --identities [system] --query principalId -o tsv 
Write-host "      ➡️ Setup access to KeyVault"
az keyvault set-policy --name $KeyVault  --object-id $WebAppNamePortalId --secret-permissions get list --key-permissions get list --output $azCliOutput
Write-host "      ➡️ Set Configuration"
az webapp config connection-string set -g $ResourceGroupForDeployment -n $WebAppNamePortal -t SQLAzure --output $azCliOutput --settings DefaultConnection=$DefaultConnectionKeyVault
az webapp config appsettings set -g $ResourceGroupForDeployment  -n $WebAppNamePortal --output $azCliOutput --settings SaaSApiConfiguration__AdAuthenticationEndPoint=https://login.microsoftonline.com SaaSApiConfiguration__ClientId=$ADApplicationID SaaSApiConfiguration__ClientSecret=$ADApplicationSecretKeyVault SaaSApiConfiguration__FulFillmentAPIBaseURL=https://marketplaceapi.microsoft.com/api SaaSApiConfiguration__FulFillmentAPIVersion=2018-08-31 SaaSApiConfiguration__GrantType=client_credentials SaaSApiConfiguration__MTClientId=$ADMTApplicationID SaaSApiConfiguration__Resource=20e940b3-4c77-4b0b-9a53-9e16a1b010a7 SaaSApiConfiguration__TenantId=$TenantID SaaSApiConfiguration__SignedOutRedirectUri=https://$WebAppNamePrefix-portal.azurewebsites.net/Home/Index/ SaaSApiConfiguration_CodeHash=$SaaSApiConfiguration_CodeHash
az webapp config set -g $ResourceGroupForDeployment -n $WebAppNamePortal --always-on true --output $azCliOutput

#endregion

#region Deploy Code
Write-host "📜 Deploy Code"

Write-host "   🔵 Deploy Database"
Write-host "      ➡️ Generate SQL schema/data script"
Set-Content -Path ../src/AdminSite/appsettings.Development.json -value "{`"ConnectionStrings`": {`"DefaultConnection`":`"$Connection`"}}"
dotnet-ef migrations script  --output script.sql --idempotent --context SaaSKitContext --project ../src/DataAccess/DataAccess.csproj --startup-project ../src/AdminSite/AdminSite.csproj
Write-host "      ➡️ Execute SQL schema/data script"
Invoke-Sqlcmd -InputFile ./script.sql -ServerInstance $ServerUri -database $SQLDatabaseName -Username $SQLAdminLogin -Password $SQLAdminLoginPassword 

Write-host "   🔵 Deploy Code to Admin Portal"
az webapp deploy --resource-group $ResourceGroupForDeployment --name $WebAppNameAdmin --src-path "../Publish/AdminSite.zip" --type zip --output $azCliOutput

Write-host "   🔵 Deploy Code to Customer Portal"
az webapp deploy --resource-group $ResourceGroupForDeployment --name $WebAppNamePortal --src-path "../Publish/CustomerSite.zip" --type zip --output $azCliOutput

Write-host "   🔵 Clean up"
Remove-Item -Path ../src/AdminSite/appsettings.Development.json
Remove-Item -Path script.sql
#Remove-Item -Path ../Publish -recurse -Force

#endregion

#region Present Output

Write-host "✅ If the intallation completed without error complete the folllowing checklist:"
if ($ISADMTApplicationIDProvided) {  #If provided then show the user where to add the landing page in AAD, otherwise script did this already for the user.
	Write-host "   🔵 Add The following URLs to the multi-tenant AAD App Registration in Azure Portal:"
	Write-host "      ➡️ https://$WebAppNamePrefix-portal.azurewebsites.net"
	Write-host "      ➡️ https://$WebAppNamePrefix-portal.azurewebsites.net/"
	Write-host "      ➡️ https://$WebAppNamePrefix-portal.azurewebsites.net/Home/Index"
	Write-host "      ➡️ https://$WebAppNamePrefix-portal.azurewebsites.net/Home/Index/"
	Write-host "      ➡️ https://$WebAppNamePrefix-admin.azurewebsites.net"
	Write-host "      ➡️ https://$WebAppNamePrefix-admin.azurewebsites.net/"
	Write-host "      ➡️ https://$WebAppNamePrefix-admin.azurewebsites.net/Home/Index"
	Write-host "      ➡️ https://$WebAppNamePrefix-admin.azurewebsites.net/Home/Index/"
	Write-host "   🔵 Verify ID Tokens checkbox has been checked-out ?"
}

Write-host "   🔵 Add The following URL in PartnerCenter SaaS Technical Configuration"
Write-host "      ➡️ Landing Page section:       https://$WebAppNamePrefix-portal.azurewebsites.net/"
Write-host "      ➡️ Connection Webhook section: https://$WebAppNamePrefix-portal.azurewebsites.net/api/AzureWebhook"
Write-host "      ➡️ Tenant ID:                  $TenantID"
Write-host "      ➡️ AAD Application ID section: $ADApplicationID"
$duration = (Get-Date) - $startTime
Write-Host "Deployment Complete in $($duration.Minutes)m:$($duration.Seconds)s"
Write-Host "DO NOT CLOSE THIS SCREEN.  Please make sure you copy or perform the actions above before closing."
#endregion
