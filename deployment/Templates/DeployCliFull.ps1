# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See LICENSE file in the project root for license information.

#
# Powershell script to deploy the resources - Customer portal, Publisher portal and the Azure SQL Database
#

#.\DeployCli.ps1 -WebAppNamePrefix saasamp1 -Location canadacentral

Param(  
   [string][Parameter(Mandatory)]$WebAppNamePrefix, # Prefix used for creating web applications
   [string][Parameter(Mandatory)]$Location, # Location of the resource group
   [string][Parameter()]$ResourceGroupForDeployment, # Name of the resource group to deploy the resources - defaults to web app name prefix id not provided
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
   [string][Parameter()]$LogoURLpng,  # URL for Publisher .png logo
   [string][Parameter()]$LogoURLico  # URL for Publisher .ico logo
)

Write-Host "Starting SaaS Accelerator Deployment..."
Write-Host "🔑  Connecting to AzureAD..."

#Authenticating and Selecting the right subscription
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

Write-Host "🔑  All Authentications Connected."


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
$appServicePlanName = ($WebAppNamePrefix + "-asp")
$keyVaultName = ($WebAppNamePrefix + "-kv")
$customerPortalAppName = ($webAppNamePrefix + "-portal")
$publisherPortalAppName = ($webAppNamePrefix + "-admin")


# AAD App Registration - Create Single Tenant App Registration (Used for marketplace API integration)
if (!($ADApplicationID)) {   
    Write-Host "🔑  Creating Marketplace AD App Registration"
    $endDate = $(Get-Date).AddYears(2)
    $pass = ([System.Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes((New-Guid))))+"="

    try {    
        $ADApplication = (az ad app create --display-name "$WebAppNamePrefix-FulfillmentApp"  --password $pass --end-date $enddate --credential-description SaaSAPI) | ConvertFrom-Json
        $ADObjectID = $ADApplication.ObjectId
        $ADApplicationID = $ADApplication.AppId
        $ADApplicationSecret = $pass
        Write-Host "🔑  AAD Single Tenant Object ID:" $ADObjectID    
        Write-Host "🔑  AAD Single Tenant Application ID:" $ADApplicationID  
        Write-Host "🔑  ADApplicationID created!"
    }
    catch [System.Net.WebException],[System.IO.IOException] {
        Write-Host "🚨🚨  $PSItem.Exception"
        break;
    }
}

# AAD App Registration - Create Multi-Tenant App Registration Request (used for authenticating users to the landing page / publihser portal)
if (!($ADMTApplicationID)) {   
    Write-Host "🔑  Creating Landing page AppRegistration..."
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
        $ADMTObjectID = $landingpageLoginAppReg.ObjectId
        Write-Host "🔑  Landing page AppRegistration ObjectID: $ADMTObjectID"
        Write-Host "🔑  Landing page AppRegistration Application Id: $ADMTApplicationID"
        Write-Host "🔑  Landing page AppRegistration created!"
        

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
if (!(Test-Path -Path $localPathToPublisherPortalPackage)){
    dotnet publish ..\..\src\SaaS.SDK.PublisherSolution\SaaS.SDK.PublisherSolution.csproj -c debug -o ..\..\Publish\PublisherPortal -v q
    Compress-Archive -Path ..\..\Publish\PublisherPortal\* -DestinationPath $localPathToPublisherPortalPackage -Force
}

Write-host "☁  Preparing the publish files for CustomerPortal"
if (!(Test-Path -Path $localPathToCustomerPortalPackage)){
    dotnet publish ..\..\src\SaaS.SDK.CustomerProvisioning\SaaS.SDK.CustomerProvisioning.csproj -c debug -o ..\..\Publish\CustomerPortal -v q
    Compress-Archive -Path ..\..\Publish\CustomerPortal\* -DestinationPath $localPathToCustomerPortalPackage -Force
}

Write-host "📜Deploying resources..."

# Create Resrouce Group
az group create `
    --name $ResourceGroupForDeployment `
    --location $location `
    -o none
Write-host "📜  -Resource Group done!"

#Create the App Service Plan        
Write-host "📜  -Deploying App Service plan..."
az appservice plan create `
    --resource-group $ResourceGroupForDeployment `
    --name $appServicePlanName `
    --location $location `
    --sku B1 `
    --output none
    #--is-linux `

Write-host "📜  -App Service Plan Done!"


##Create and configure the Customer Portal Web App
Write-host "📜  -Deploying Customer Portal Web App..."

$clientSecretSetting = '"@Microsoft.KeyVault(VaultName={0};SecretName=SaaSApiConfiguration--ClientSecret)"' -f $keyVaultName
$sqlConnectionStringSetting = '"@Microsoft.KeyVault(VaultName={0};SecretName=SqlConnectionString)"' -f $keyVaultName

#Create the webapp
az webapp create `
    --resource-group $ResourceGroupForDeployment `
    --plan $appServicePlanName `
    --name $customerPortalAppName `
    --runtime 'DOTNETCORE:3.1' `
    --output none
#Set app settings
Write-host "📜  --Configuring Customer Portal Web App..."
az webapp config appsettings set `
    --resource-group $ResourceGroupForDeployment `
    --name $customerPortalAppName `
    --settings ASPNETCORE_ENVIRONMENT="Development" `
            SaaSApiConfiguration__AdAuthenticationEndPoint="https://login.microsoftonline.com" `
            SaaSApiConfiguration__ClientId=$ADApplicationID `
            SaaSApiConfiguration__MTClientId=$ADMTApplicationID `
            SaaSApiConfiguration__FulFillmentAPIBaseURL="https://marketplaceapi.microsoft.com/api" `
            SaaSApiConfiguration__FulFillmentAPIVersion="2018-08-31" `
            SaaSApiConfiguration__GrantType="client_credentials" `
            SaaSApiConfiguration__Resource="20e940b3-4c77-4b0b-9a53-9e16a1b010a7" `
            SaaSApiConfiguration__SaaSAppUrl="", `
            SaaSApiConfiguration__SignedOutRedirectUri="https://$customerPortalAppName.azurewebsites.net/Home/Index" `
            SaaSApiConfiguration__TenantId=$TenantID `
            SaaSApiConfiguration__ClientSecret=$clientSecretSetting `
            KnownUsers=$PublisherAdminUsers `
            WEBSITE_HTTPLOGGING_RETENTION_DAYS=1 `
            WEBSITE_RUN_FROM_PACKAGE=1

#Set app secrets
az webapp config connection-string set `
    --resource-group $ResourceGroupForDeployment `
    --name $customerPortalAppName `
    --connection-string-type SQLAzure `
    --settings DefaultConnection=$sqlConnectionStringSetting

Write-host "📜  --Creating Identity for Customer Portal Web App..."
#Assign identity
az webapp identity assign `
    --resource-group $ResourceGroupForDeployment `
    --name $customerPortalAppName `
    --output none
    
#Get identity
$customerPortalAppIdentity = (az webapp identity show `
        --resource-group $ResourceGroupForDeployment `
        --name $customerPortalAppName `
        | ConvertFrom-Json).principalId

Write-host "📜  -Customer Portal Web App done!"

##Create and configure the PublisherPortal Web App
Write-host "📜  -Deploying Publisher Portal Web App..."

#Create the webapp
az webapp create `
    --resource-group $ResourceGroupForDeployment `
    --plan $appServicePlanName `
    --name $publisherPortalAppName `
    --runtime "DOTNETCORE:3.1" `
    --output none
#Set app settings
Write-host "📜  --Configuring Publisher Portal Web App..."
az webapp config appsettings set `
    --resource-group $ResourceGroupForDeployment `
    --name $publisherPortalAppName `
    --settings ASPNETCORE_ENVIRONMENT="Development" `
            SaaSApiConfiguration__AdAuthenticationEndPoint="https://login.microsoftonline.com" `
            SaaSApiConfiguration__ClientId=$ADApplicationID `
            SaaSApiConfiguration__MTClientId=$ADMTApplicationID `
            SaaSApiConfiguration__FulFillmentAPIBaseURL="https://marketplaceapi.microsoft.com/api" `
            SaaSApiConfiguration__FulFillmentAPIVersion="2018-08-31" `
            SaaSApiConfiguration__GrantType="client_credentials" `
            SaaSApiConfiguration__Resource="20e940b3-4c77-4b0b-9a53-9e16a1b010a7" `
            SaaSApiConfiguration__SaaSAppUrl="", `
            SaaSApiConfiguration__SignedOutRedirectUri="https://$publisherPortalAppName.azurewebsites.net/Home/Index" `
            SaaSApiConfiguration__TenantId=$TenantID `
            SaaSApiConfiguration__ClientSecret=$clientSecretSetting `
            KnownUsers=$PublisherAdminUsers `
            WEBSITE_HTTPLOGGING_RETENTION_DAYS=1 `
            WEBSITE_RUN_FROM_PACKAGE=1

#Set app secrets
az webapp config connection-string set `
    --resource-group $ResourceGroupForDeployment `
    --name $publisherPortalAppName `
    --connection-string-type SQLAzure `
    --settings DefaultConnection=$sqlConnectionStringSetting

Write-host "📜  --Creating Identity for Publisher Portal Web App..."
#Assign identity
az webapp identity assign `
    --resource-group $ResourceGroupForDeployment `
    --name $publisherPortalAppName `
    --output none
#Get identity
$publisherPortalAppIdentity = 
    (az webapp identity show `
        --resource-group $ResourceGroupForDeployment `
        --name $publisherPortalAppName `
        | ConvertFrom-Json).principalId

Write-host "📜  Publisher Portal Web App done!"


##SQL Database
Write-host "📜  -Deploying SQL database..."

#Create the Database Server
az sql server create `
    --resource-group $ResourceGroupForDeployment `
    --name $SQLServerName `
    --location $location `
    --admin-user $SQLAdminLogin `
    --admin-password $SQLAdminLoginPassword `
    --output none

az sql server firewall-rule create `
    --resource-group $ResourceGroupForDeployment `
    --server $SQLServerName `
    --name "AzureInternal" `
    --start-ip-address 0.0.0.0 `
    --end-ip-address 0.0.0.0 `
    --output none

Write-host "📜  --SQL Server created!"
#Create the Database
az sql db create `
    --resource-group $ResourceGroupForDeployment `
    --name $SQLDatabaseName `
    --server $SQLServerName `
    --service-objective S0 `
    --catalog-collation "SQL_Latin1_General_CP1_CI_AS" `
    --max-size 268435456000 

Write-host "📜  --SQL Database created!"
#Get connection string
$SqlConnectionString = (az sql db show-connection-string `
    --client ado.net `
    --auth-type SQLPassword `
    --name $SQLDatabaseName `
    --server $SQLServerName)

$SqlConnectionString = $SqlConnectionString.replace("<password>",$SQLAdminLoginPassword).replace("<username>",$SQLAdminLogin)
Write-host "📜  -SQL database done!"


#KeyVault
Write-host "📜  -Deploying KeyVault..."

#Create the KeyVault
az keyvault create `
        --resource-group $ResourceGroupForDeployment `
        --location $location `
        --name $keyVaultName `
        --output none
Write-host "📜  -- KeyVault created"

#Give KeyVault permissions to the webapps
az keyvault set-policy `
    --resource-group $ResourceGroupForDeployment `
    --name $keyVaultName `
    --object-id $customerPortalAppIdentity `
    --secret-permissions get list
Write-host "📜  -- Customer portal App identity assigned"

az keyvault set-policy `
    --resource-group $ResourceGroupForDeployment `
    --name $keyVaultName `
    --object-id $publisherPortalAppIdentity `
    --secret-permissions get list
Write-host "📜  -- Publisher portal App identity assigned"

az keyvault secret set `
    --vault-name $keyVaultName `
    --name 'SqlConnectionString' `
    --value $SqlConnectionString
Write-host "📜  -- SQL Connection string secret added"
az keyvault secret set `
    --vault-name $keyVaultName `
    --name 'SaaSApiConfiguration--ClientSecret' `
    --value $ADApplicationSecret
Write-host "📜  -- Azure AD application Client secret added"

Write-host "📜  -KeyVault done!"

#Deploy the web apps
Write-host "📜  -Deploying Customer Portal App..."
az webapp deployment source config-zip `
    --resource-group  $ResourceGroupForDeployment `
    --name $customerPortalAppName `
    --src $localPathToCustomerPortalPackage
Write-host "📜  -Customer Portal App Deployed!"
Write-host "📜  -Deploying Publisher Portal App..."
az webapp deployment source config-zip `
    --resource-group  $ResourceGroupForDeployment `
    --name $publisherPortalAppName  `
    --src $localPathToPublisherPortalPackage
Write-host "📜  -Publisher Portal App Deployed!"


#Create a storage account for the SQL backup file and restore.
#Upload the SQL backup file, generate a shorttimes SAS token, and use it to restore the database.
Write-host "📜  -Restoring Database..."

az storage account create  `
    --resource-group $ResourceGroupForDeployment `
    --name $storageAccountName `
    --location $location `
    --sku Standard_LRS `
    -o none

az storage container create `
    --account-name $storageAccountName `
    --name $containerName `
    --public-access blob `
    -o none
Write-host "📜  --Created storage account"    

az storage blob upload `
    --account-name $storageAccountName `
    --container-name $containerName `
    --name $sqlBlobName `
    --file $LocalPathToBacpacFile
Write-host "📜  --Uploaded bacpac to storage"    

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
Write-host "📜  --Got temporary SAS key for blob"    

Write-host "📜  --Importing Bacpac"
az sql db import `
    --resource-group $ResourceGroupForDeployment `
    --server $SQLServerName `
    --admin-user $SQLAdminLogin `
    --admin-password $SQLAdminLoginPassword `
    --name $SQLDatabaseName `
    --storage-key $storageKey `
    --storage-key-type SharedAccessKey `
    --storage-uri $sqlBlobPath

Write-host "📜  -Database restored!"

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
