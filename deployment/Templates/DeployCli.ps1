# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See LICENSE file in the project root for license information.

#
# Powershell script to deploy the resources - Customer portal, Publisher portal and the Azure SQL Database
#

#.\DeployCli.ps1 -WebAppNamePrefix saasamp1 -Location canadacentral

#simplified parameter requirements by inferring some defaults
# - resource group name and sql server name would be same as app prefix
# - path to ARM template is the local file name
# - PublisherAdminUsers, if not provided is the user that's running this
# - SQLAdminLoginPassword, if not provided we'll generate a random one
# - SQLAdminLogin, if not provided we'll generate a random one


Param(  
   [string][Parameter(Mandatory)]$WebAppNamePrefix, # Prefix used for creating web applications
   [string][Parameter()]$TenantID, # The value should match the value provided for Active Directory TenantID in the Technical Configuration of the Transactable Offer in Partner Center
   [string][Parameter()]$SubscriptionID, # Subscription where the resources be deployed
   [string][Parameter()]$ADApplicationID, # The value should match the value provided for Active Directory Application ID in the Technical Configuration of the Transactable Offer in Partner Center
   [string][Parameter()]$ADApplicationSecret, # Secret key of the AD Application
   [string][Parameter()]$ADMTApplicationID, # The value should match the value provided for Multi-Tenant Active Directory Application ID in the Technical Configuration of the Transactable Offer in Partner Center
   [string][Parameter()]$SQLServerName, # Name of the database server (without database.windows.net)
   [string][Parameter()]$SQLDatabaseName, # Name of the database 
   [string][Parameter()]$SQLAdminLogin, # SQL Admin login
   [string][Parameter()]$SQLAdminLoginPassword, # SQL Admin password
   [string][Parameter()]$PublisherAdminUsers, # Provide a list of email addresses (as comma-separated-values) that should be granted access to the Publisher Portal
   [string][Parameter()]$ResourceGroupForDeployment, # Name of the resource group to deploy the resources - defaults to web app name prefix id not provided
   [string][Parameter(Mandatory)]$Location, # Location of the resource group
   [string][Parameter()]$PathToARMTemplate = "deployCli.json",  # Local Path to the ARM Template
   [string][Parameter()]$LogoURLpng,  # URL for Publisher .png logo
   [string][Parameter()]$LogoURLico  # URL for Publisher .ico logo
)

Write-Host "Starting SaaS Accelerator Deployment..."
Write-Host "🔑  Connecting to AzureAD..."
Write-Host "🔑  All Authentications Connected."

# Selecting the right Subscription
$account = (az account show | ConvertFrom-Json)

if ($SubscriptionID -ne "") {
    $subs = az account list --query '[].{subscriptionId: id}' -o table
    if (!($subs.Contains($SubscriptionID))) {
        Write-Host "🚨🚨  Unable to find subscription with Id ($SubscriptionID) under your tenant. Please review the subscription id, or use az login to authenticate to a different tenant."
        exit 1
    }
    az account set -s $SubscriptionID
    $account = (az account show | ConvertFrom-Json)  
}

Write-Host "Subscription selected: $($account.name)-($($account.id))"
Write-Host "Tenant selected: $($account.tenantId)"
$SubscriptionID = $account.id
$TenantID = $account.tenantId

#Handle defaults for parameters

if ($PublisherAdminUsers -eq "") {
    $PublisherAdminUsers = $account.user.name
}
if ($ResourceGroupForDeployment -eq "") {
    $ResourceGroupForDeployment = $WebAppNamePrefix
}
if ($SQLServerName -eq "") {
    $SQLServerName = "$WebAppNamePrefix-sql"
}
if ($SQLDatabaseName -eq "") {
    $SQLDatabaseName = "AMPSaaSDB"
}
if ($SQLAdminLoginPassword -eq "") {
    $SQLAdminLoginPassword = ([System.Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes((New-Guid))))+"="
}
if ($SQLAdminLogin -eq "") {
    $SQLAdminLogin = "saasdbadmin" + $(Get-Random -Minimum 1 -Maximum 1000)
}

$storageAccountName = ($WebAppNamePrefix + "storage").ToLower()
$containerName = "packagefiles" 

# AAD App Registration - Create Single Tenant App Registration (Used for marketplace API integration)
if (!($ADApplicationID)) {   
    Write-Host "🔑  Creating ADApplicationID..."
    $endDate = $(Get-Date).AddYears(2)
    $pass = ([System.Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes((New-Guid))))+"="

    try {    
        $ADApplication = (az ad app create --display-name "$WebAppNamePrefix-FulfillmentApp"  --password $pass --end-date $enddate --credential-description SaaSAPI) | ConvertFrom-Json
        $ADObjectID = $ADApplication.ObjectId
        $ADApplicationID = $ADApplication.AppId
        $ADApplicationSecret = $pass
        Write-Host "🔑  AAD Single Tenant Object ID:" $ADObjectID    
        Write-Host "🔑  AAD Single Tenant Application ID:" $ADApplicationID  
        Write-Host "🔑  ADApplicationID created."
    }
    catch [System.Net.WebException],[System.IO.IOException] {
        Write-Host "🚨🚨  $PSItem.Exception"
        break;
    }
}

# AAD App Registration - Create Multi-Tenant App Registration Request (used for authenticating users to the landing page / publihser portal)
if (!($ADMTApplicationID)) {   
    Write-Host "🔑  Mapping Landing paged mapped to AppRegistration..."
    try {
        $landingpageLoginAppReg = (az ad app create `
            --display-name "$WebAppNamePrefix-LandingpageAppReg" `
            --oauth2-allow-implicit-flow `
            --available-to-other-tenants `
            --optional-claims '{\"idToken\": [{ \"name\": \"auth_time\", \"source\": null,\"essential\": false }]}' `
            --required-resource-accesses '[{\"resourceAppId\": \"00000003-0000-0000-c000-000000000000\",\"resourceAccess\": [{\"id\": \"e1fe6dd8-ba31-4d61-89e7-88639da4683d\",\"type\": \"Scope\"}]}]' `
            --reply-urls "https://$WebAppNamePrefix-portal.azurewebsites.net" "https://$WebAppNamePrefix-portal.azurewebsites.net/" "https://$WebAppNamePrefix-portal.azurewebsites.net/Home/Index/" "https://$WebAppNamePrefix-portal.azurewebsites.net/Home/Index" "https://$WebAppNamePrefix-admin.azurewebsites.net" "https://$WebAppNamePrefix-admin.azurewebsites.net/" "https://$WebAppNamePrefix-admin.azurewebsites.net/Home/Index/" "https://$WebAppNamePrefix-admin.azurewebsites.net/Home/Index"  ) `
            | ConvertFrom-Json   
        
        az ad app update `
            --id $landingpageLoginAppReg.AppId `
            --set logoutUrl="https://$WebAppNamePrefix-portal.azurewebsites.net/Logout"

        $ADMTApplicationID = $landingpageLoginAppReg.AppId
        Write-Host "🔑  Landing paged mapped to AppRegistration: $ADMTApplicationID"
        $ADMTObjectID = $landingpageLoginAppReg.ObjectId
        Write-Host "🔑  Landing paged AppRegistration ObjectID: $ADMTObjectID"

        #ToDo -> setting logo with AZ CLI is not possible at the moment.
    }
    catch [System.Net.WebException],[System.IO.IOException] {
        Write-Host "🚨🚨   $PSItem.Exception"
        break;
    }
}

# Setup deployment artifacts -> Packages for web apps and bacpac for database
$localPathToPublisherPortalPackage = "..\..\Publish\publisherPortal.zip"
$localPathToCustomerPortalPackage = "..\..\Publish\customerPortal.zip"
$localPathToBacpacFile = '..\Database\AMPSaaSDB.bacpac' 
$sqlBlobName = "sqlbackup.bacpac"

#Compiling apps to produce packages
Write-host "☁  Preparing the publish files for PublisherPortal"  
dotnet publish ..\..\src\SaaS.SDK.PublisherSolution\SaaS.SDK.PublisherSolution.csproj -c debug -o ..\..\Publish\PublisherPortal -v q
Compress-Archive -Path ..\..\Publish\PublisherPortal\* -DestinationPath $localPathToPublisherPortalPackage -Force

Write-host "☁  Preparing the publish files for CustomerPortal"
dotnet publish ..\..\src\SaaS.SDK.CustomerProvisioning\SaaS.SDK.CustomerProvisioning.csproj -c debug -o ..\..\Publish\CustomerPortal -v q
Compress-Archive -Path ..\..\Publish\CustomerPortal\* -DestinationPath $localPathToCustomerPortalPackage -Force


# Create RG if not exists
az group create `
    --name $ResourceGroupForDeployment `
    --location $location
Write-host "📜  Deploying the ARM template to set up resources"

# Deploy resources using ARM template
az deployment group create `
    --resource-group $ResourceGroupForDeployment `
    --template-file $PathToARMTemplate `
    --parameters `
        webAppNamePrefix=$WebAppNamePrefix `
        TenantID=$TenantID `
        ADApplicationID=$ADApplicationID `
        ADApplicationSecret=$ADApplicationSecret `
        ADMTApplicationID=$ADMTApplicationID `
        SQLServerName=$SQLServerName `
        SQLDatabaseName=$SQLDatabaseName `
        SQLAdminLogin=$SQLAdminLogin `
        SQLAdminLoginPassword=$SQLAdminLoginPassword `
        PublisherAdminUsers=$PublisherAdminUsers 

#Create a storage account for the SQL backup file and restore.
#Upload the SQL backup file, generate a shorttimes SAS token, and use it to restore the database.
az storage account create  `
    --resource-group $ResourceGroupForDeployment `
    --name $storageAccountName `
    --location $location `
    --sku Standard_LRS 

az storage container create `
    --account-name $storageAccountName `
    --name $containerName `
    --public-access blob

az storage blob upload `
    --account-name $storageAccountName `
    --container-name $containerName `
    --name $sqlBlobName `
    --file $LocalPathToBacpacFile

$sqlBlobPath = (az storage blob url  `
    --account-name $storageAccountName `
    --container-name $containerName `
    --name $sqlBlobName)

$storageKey = (az storage blob generate-sas `
    --account-name $storageAccountName `
    --container $containerName `
    --name $sqlBlobName `
    --permissions r `
    --expiry $((Get-Date).AddDays(1)).ToString("yyyy-MM-ddTHH:mm:ssZ"))

az sql db import `
    --resource-group $ResourceGroupForDeployment `
    --server $SQLServerName `
    --admin-user $SQLAdminLogin `
    --admin-password $SQLAdminLoginPassword `
    --name $SQLDatabaseName `
    --storage-key $storageKey `
    --storage-key-type SharedAccessKey `
    --storage-uri $sqlBlobPath

#Deploy the web apps
az webapp deployment source config-zip `
    --resource-group  $ResourceGroupForDeployment `
    --name "$WebAppNamePrefix-portal" `
    --src $localPathToCustomerPortalPackage

az webapp deployment source config-zip `
    --resource-group  $ResourceGroupForDeployment `
    --name "$WebAppNamePrefix-admin" `
    --src $localPathToPublisherPortalPackage

Write-host "🏁  If the installation completed without error complete the folllowing checklist:"
if ($ADMTApplicationID) {  #If provided then show the user where to add the landing page in AAD, otherwise script did this already for the user.
Write-host "__ Add The following URLs to the multi-tenant AAD App Registration in Azure Portal:"
Write-host "   https://$WebAppNamePrefix-portal.azurewebsites.net"
Write-host "   https://$WebAppNamePrefix-portal.azurewebsites.net/"
Write-host "   https://$WebAppNamePrefix-portal.azurewebsites.net/Home/Index"
Write-host "   https://$WebAppNamePrefix-portal.azurewebsites.net/Home/Index/"
Write-host "   https://$WebAppNamePrefix-admin.azurewebsites.net"
Write-host "   https://$WebAppNamePrefix-admin.azurewebsites.net/"
Write-host "   https://$WebAppNamePrefix-admin.azurewebsites.net/Home/Index"
Write-host "   https://$WebAppNamePrefix-admin.azurewebsites.net/Home/Index/"
Write-host "__ Verify ID Tokens checkbox has been checked-out ✅"
}

Write-host "__ Add The following URL in PartnerCenter SaaS Technical Configuration->Landing Page section"
Write-host "   https://$WebAppNamePrefix-portal.azurewebsites.net/"
Write-host "__ Add The following URL in PartnerCenter SaaS Technical Configuration->Connection Webhook section"
Write-host "   https://$WebAppNamePrefix-portal.azurewebsites.net/api/AzureWebhook"
Write-host "__ Add The following TenantID in PartnerCenter SaaS Technical Configuration Tenant ID"
Write-host "   $TenantID"
Write-host "__ Add The following ApplicationID in PartnerCenter SaaS Technical Configuration->AAD Application ID section"
Write-host "   $ADApplicationID"
