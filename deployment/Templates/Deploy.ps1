# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See LICENSE file in the project root for license information.

#
# Powershell script to deploy the resources - Customer portal, Publisher portal and the Azure SQL Database
#

Param(  
   [string][Parameter(Mandatory)]$WebAppNamePrefix, # Prefix used for creating web applications
   [string][Parameter()]$TenantID, # The value should match the value provided for Active Directory TenantID in the Technical Configuration of the Transactable Offer in Partner Center
   [string][Parameter()]$AzureSubscriptionID, # Subscription where the resources be deployed
   [string][Parameter()]$ADApplicationID, # The value should match the value provided for Active Directory Application ID in the Technical Configuration of the Transactable Offer in Partner Center
   [string][Parameter()]$ADApplicationSecret, # Secret key of the AD Application
   [string][Parameter()]$ADMTApplicationID, # The value should match the value provided for Multi-Tenant Active Directory Application ID in the Technical Configuration of the Transactable Offer in Partner Center
   [string][Parameter(Mandatory)]$SQLServerName, # Name of the database server (without database.windows.net)
   [string][Parameter(Mandatory)]$SQLAdminLogin, # SQL Admin login
   [string][Parameter(Mandatory)]$SQLAdminLoginPassword, # SQL Admin password
   [string][Parameter(Mandatory)]$PublisherAdminUsers, # Provide a list of email addresses (as comma-separated-values) that should be granted access to the Publisher Portal
   [string][Parameter(Mandatory)]$ResourceGroupForDeployment, # Name of the resource group to deploy the resources
   [string][Parameter(Mandatory)]$Location, # Location of the resource group
   [string][Parameter()]$LogoURLpng,  # URL for Publisher .png logo
   [string][Parameter()]$LogoURLico,  # URL for Publisher .ico logo
   [switch][Parameter()]$MeteredSchedulerSupport # set to true to enable Metered Support
)

$ErrorActionPreference = "Stop"
$SaaSApiConfiguration_CodeHash= git log --format='%H' -1
# Checking SQL username
if($SQLAdminLogin.ToLower() -eq "admin") {
    Throw "🛑 SQLAdminLogin may not be 'admin'."
    Exit
}

# Checking SQL password length
if($SQLAdminLoginPassword.Length -lt 8) {
    Throw "🛑 SQLAdminLoginPassword must be at least 8 characters."
    Exit
}
if($WebAppNamePrefix.Length -gt 21) {
    Throw "🛑 Web name prefix must be less than 21 characters."
    Exit
}


Write-Host "Starting SaaS Accelerator Deployment..."

# Record the current ADApps to reduce deployment instructions at the end
$IsADApplicationIDProvided = $ADApplicationIDProvided
$ISADMTApplicationIDProvided = $ADMTApplicationID
# Make sure to install Az Module before running this script
# Install-Module Az
# Install-Module -Name AzureAD

# Azure Login
if($env:ACC_CLOUD) {
    Write-Host "🔑  Authenticating using device..."
    #Connect-AzAccount -UseDeviceAuthentication
} else {
    Write-Host "🔑  Authenticating using AzAccount authentication..."
    Connect-AzAccount
}

Write-Host "🔑  Connecting to AzureAD..."
# Connect-AzureAD -Confirm   # TODO: Make this command works.  It fails when running from withing the script. 
Write-Host "🔑  All Authentications Connected."

$currentContext = Get-AzContext
$currentTenant = $currentContext.Tenant.Id
$currentSubscription = $currentContext.Subscription.Id
# Get TenantID if not set as argument
if(!($TenantID)) {    
    Get-AzTenant | Format-Table
    if (!($TenantID = Read-Host "⌨  Type your TenantID or press Enter to accept your current one [$currentTenant]")) { $TenantID = $currentTenant }    
}
else {
    Write-Host "🔑  TenantID provided: $TenantID"
}

# Get Azure Subscription
if(!($AzureSubscriptionID)) {    
    Get-AzSubscription -TenantId $TenantID | Format-Table
    if (!($AzureSubscriptionID = Read-Host "⌨  Type your SubscriptionID or press Enter to accept your current one [$currentSubscription]")) { $AzureSubscriptionID = $currentSubscription }
}
else {
    Write-Host "🔑  AzureSubscriptionID provided: $AzureSubscriptionID"
}

Write-Host "🔑  Selecting Azure Subscription..."
Select-AzSubscription -SubscriptionId $AzureSubscriptionID
Write-Host "🔑  Azure Subscription selected."

# Create AAD App Registration

# AAD App Registration - Create Multi-Tenant App Registration Requst
if (!($ADApplicationID)) {   # AAD App Registration - Create Single Tenant App Registration
    Write-Host "🔑  Creating ADApplicationID..."
    $Guid = New-Guid
    $startDate = Get-Date
    $endDate = $startDate.AddYears(2)
    $ADApplicationSecret = ([System.Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes(($Guid))))+"="

    try {    
        $ADApplication = New-AzureADApplication -DisplayName "$WebAppNamePrefix-FulfillmentApp"
        $ADObjectID = $ADApplication | %{ $_.ObjectId }
        $ADApplicationID = $ADApplication | %{ $_.AppId }
        Write-Host "🔑  AAD Single Tenant Object ID:" $ADObjectID    
        Write-Host "🔑  AAD Single Tenant Application ID:" $ADApplicationID  
        sleep 5 #this is to give time to AAD to register
        New-AzureADApplicationPasswordCredential -ObjectId $ADObjectID -StartDate $startDate -EndDate $endDate -Value $ADApplicationSecret -InformationVariable "SaaSAPI"
        Write-Host "🔑  ADApplicationID created."
        
    }
    catch [System.Net.WebException],[System.IO.IOException] {
        Write-Host "🚨🚨   $PSItem.Exception"
        break;
    }
}

$restbody = "" +`
"{ \`"displayName\`": \`"$WebAppNamePrefix-LandingpageAppReg\`"," +`
" \`"api\`":{\`"requestedAccessTokenVersion\`": 2}," +`
" \`"signInAudience\`" : \`"AzureADandPersonalMicrosoftAccount\`"," +`
" \`"web\`": " +`
"{ \`"redirectUris\`": " +`
"[" +`
"\`"https://$WebAppNamePrefix-portal.azurewebsites.net\`"," +`
"\`"https://$WebAppNamePrefix-portal.azurewebsites.net/\`"," +`
"\`"https://$WebAppNamePrefix-portal.azurewebsites.net/Home/Index\`"," +`
"\`"https://$WebAppNamePrefix-portal.azurewebsites.net/Home/Index/\`"," +`
"\`"https://$WebAppNamePrefix-admin.azurewebsites.net\`"," +`
"\`"https://$WebAppNamePrefix-admin.azurewebsites.net/\`"," +`
"\`"https://$WebAppNamePrefix-admin.azurewebsites.net/Home/Index\`"," +`
"\`"https://$WebAppNamePrefix-admin.azurewebsites.net/Home/Index/\`"" +`
"]," +`
" \`"logoutUrl\`": \`"https://$WebAppNamePrefix-portal.azurewebsites.net/logout\`"," +`
"\`"implicitGrantSettings\`": " +`
"{ \`"enableIdTokenIssuance\`": true }}," +`
" \`"requiredResourceAccess\`": " +`
" [{\`"resourceAppId\`": \`"00000003-0000-0000-c000-000000000000\`", " +`
" \`"resourceAccess\`": " +`
" [{ \`"id\`": \`"e1fe6dd8-ba31-4d61-89e7-88639da4683d\`"," +`
" \`"type\`": \`"Scope\`" }]}] }" 

Write-Host $restbody

if (!($ADMTApplicationID)) {   # AAD App Registration - Create Multi-Tenant App Registration Requst 
    Write-Host "🔑  Mapping Landing paged mapped to AppRegistration..."
    try {
        $landingpageLoginAppReg = $(az rest --method POST  --headers 'Content-Type=application/json' --uri https://graph.microsoft.com/v1.0/applications --body $restbody | jq '{lappID: .appId, publisherDomain: .publisherDomain, objectID: .id}')
        Write-Host "$landingpageLoginAppReg"
        $ADMTApplicationID = $landingpageLoginAppReg | jq .lappID | %{$_ -replace '"',''}
        Write-Host "🔑  Landing paged mapped to AppRegistration: $ADMTApplicationID"
        $ADMTObjectID = $landingpageLoginAppReg | jq .objectID | %{$_ -replace '"',''}
        Write-Host "🔑  Landing paged AppRegistration ObjectID: $ADMTObjectID"

        # Download Publisher's AppRegistration logo
        if($LogoURLpng) { 
            # Write-Host "📷  Downloading SSO AAD AppRegistration logo image..."
            # Invoke-WebRequest -Uri $LogoURLpng -OutFile "..\..\src\SaaS.SDK.CustomerProvisioning\wwwroot\applogo.png"
            # Write-Host "📷  SSO AAD AppRegistration logo image downloaded."    

            #Write-Host "🔑  Attaching Image to SSO AAD AppRegistration ObjectID: $ADMTObjectID ..."
            #$LogoURLpngPath = $(Resolve-Path "..\..\src\SaaS.SDK.CustomerProvisioning\wwwroot\applogo.png").Path

            #TODO: This is broken in PS CLI:  https://stackoverflow.microsoft.com/questions/276511
            # $LogoByteArray = [System.IO.File]::ReadAllBytes($LogoURLpngPath)
            # Set-AzureADApplicationLogo -ObjectId $ADMTObjectID -ImageByteArray $LogoByteArray 
            # Set-AzureADApplicationLogo -ObjectId $ADMTObjectID -FilePath $LogoURLpngPath
            #Write-Host "🔑  Image attached to SSO AAD AppRegistration."
        }
    }
    catch [System.Net.WebException],[System.IO.IOException] {
        Write-Host "🚨🚨   $PSItem.Exception"
        break;
    }
}

# Download Publisher's PNG logo
if($LogoURLpng) { 
    Write-Host "📷  Downloading PNG logo images..."
    Invoke-WebRequest -Uri $LogoURLpng -OutFile "..\..\src\SaaS.SDK.CustomerProvisioning\wwwroot\chef-progress-logo.png"
    Invoke-WebRequest -Uri $LogoURLpng -OutFile "..\..\src\SaaS.SDK.PublisherSolution\wwwroot\chef-progress-logo.png"
    Write-Host "📷  Logo images PNG downloaded."
}

# Download Publisher's FAVICON logo
if($LogoURLico) { 
    Write-Host "📷  Downloading ICO logo images..."
    Invoke-WebRequest -Uri $LogoURLico -OutFile "..\..\src\SaaS.SDK.CustomerProvisioning\wwwroot\favicon.ico"
    Invoke-WebRequest -Uri $LogoURLico -OutFile "..\..\src\SaaS.SDK.PublisherSolution\wwwroot\favicon.ico"
    Write-Host "📷  Logo images ICO downloaded."
}

Write-host "☁  Prepare publish files for the web application"



Write-host "☁  Preparing the publish files for PublisherPortal"  
dotnet publish ..\..\src\SaaS.SDK.PublisherSolution\SaaS.SDK.PublisherSolution.csproj -c debug -o ..\..\Publish\PublisherPortal

if ($MeteredSchedulerSupport -ne $true)
{ 
    Write-host "☁  Preparing the publish files for Metered Scheduler to PublisherPortal"
    mkdir -p ..\..\Publish\PublisherPortal\app_data\jobs\triggered\MeteredTriggerJob
    dotnet publish ..\..\src\SaaS.SDK.MeteredTriggerJob\SaaS.SDK.MeteredTriggerJob.csproj -c debug -o ..\..\Publish\PublisherPortal\app_data\jobs\triggered\MeteredTriggerJob  --runtime win-x64 --self-contained true 

}

Compress-Archive -Path ..\..\Publish\PublisherPortal\* -DestinationPath ..\..\Publish\PublisherPortal.zip -Force

Write-host "☁  Preparing the publish files for CustomerPortal"
dotnet publish ..\..\src\SaaS.SDK.CustomerProvisioning\SaaS.SDK.CustomerProvisioning.csproj -c debug -o ..\..\Publish\CustomerPortal
Compress-Archive -Path ..\..\Publish\CustomerPortal\* -DestinationPath ..\..\Publish\CustomerPortal.zip -Force


Write-host "☁ Path to web application packages $PathToWebApplicationPackages"

# Create RG if not exists
az group create --location $location --name $ResourceGroupForDeployment


Write-host "📜  Start Deploy resources"
$WebAppNameService=$WebAppNamePrefix+"AmpSvcPlan"
$WebAppNameAdmin=$WebAppNamePrefix+"-admin"
$WebAppNamePortal=$WebAppNamePrefix+"-portal"
$KeyVault=$WebAppNamePrefix+"-kv"
$KeyVault=$KeyVault -replace '_',''
$ADApplicationSecretKeyVault='"@Microsoft.KeyVault(VaultName={0};SecretName=ADApplicationSecret)"' -f $KeyVault
$DefaultConnectionKeyVault='"@Microsoft.KeyVault(VaultName={0};SecretName=DefaultConnection)"' -f $KeyVault
$Connection="Data Source=tcp:"+$SQLServerName+".database.windows.net,1433;Initial Catalog=AMPSaaSDB;User Id="+$SQLAdminLogin+"@"+$SQLServerName+".database.windows.net;Password="+$SQLAdminLoginPassword+";"


Write-host "Create SQL Server"
az sql server create --name $SQLServerName --resource-group $ResourceGroupForDeployment --location "$location" --admin-user $SQLAdminLogin --admin-password $SQLAdminLoginPassword

Write-host "Add SQL Server Firewall rules"
az sql server firewall-rule create --resource-group $ResourceGroupForDeployment --server $SQLServerName -n AllowAzureIP --start-ip-address "0.0.0.0" --end-ip-address "0.0.0.0"

Write-host "Create SQL DB"
az sql db create --resource-group $ResourceGroupForDeployment --server $SQLServerName --name "AMPSaaSDB"  --edition Standard  --capacity 10 --zone-redundant false 

## Prepare to deploy packages 
## This step to solve Linux/Windows relative path issue
if ($IsLinux) 
{ 
   $dbSqlFile=(get-item . ).parent.FullName+"/Database/AMP-DB.sql"  
   $publisherPackage=(get-item . ).parent.parent.FullName+"/Publish/PublisherPortal.zip"  
   $customerPackage=(get-item . ).parent.parent.FullName+"/Publish/CustomerPortal.zip"  
}
else {
    $dbSqlFile=(get-item . ).parent.FullName+"\Database\AMP-DB.sql"  
    $publisherPackage=(get-item . ).parent.parent.FullName+"\Publish\PublisherPortal.zip"  
    $customerPackage=(get-item . ).parent.parent.FullName+"\Publish\CustomerPortal.zip" 
}

# Deploy Code and database schema
Write-host "📜  Deploying the database schema"
$ServerUri = $SQLServerName+".database.windows.net"
Invoke-Sqlcmd -ServerInstance $ServerUri -database "AMPSaaSDB" -Username $SQLAdminLogin -Password $SQLAdminLoginPassword  -InputFile $dbSqlFile


Write-host "📜  Create Keyvault"
az keyvault create --name $KeyVault --resource-group $ResourceGroupForDeployment

Write-host "📜  Add Secrets"
az keyvault secret set --vault-name $KeyVault  --name ADApplicationSecret --value $ADApplicationSecret
az keyvault secret set --vault-name $KeyVault  --name DefaultConnection --value $Connection


Write-host "📜  Create WebApp Service Plan"
az appservice plan create -g $ResourceGroupForDeployment -n $WebAppNameService --sku B1

Write-host "📜  Create publisher Admin webapp"
az webapp create -g $ResourceGroupForDeployment -p $WebAppNameService -n $WebAppNameAdmin  --runtime dotnet:6
az webapp identity assign -g $ResourceGroupForDeployment  -n $WebAppNameAdmin --identities [system] 
$WebAppNameAdminId=$(az webapp identity show  -g $ResourceGroupForDeployment  -n $WebAppNameAdmin --query principalId -o tsv)

Write-host "📜  Add publisher Admin webapp Identity to KV"
az keyvault set-policy --name $KeyVault  --object-id $WebAppNameAdminId --secret-permissions get list --key-permissions get list

Write-host "📜  Add Admin Configuration"
az webapp config connection-string set -g $ResourceGroupForDeployment -n $WebAppNameAdmin -t SQLAzure --settings DefaultConnection=$DefaultConnectionKeyVault
az webapp config appsettings set -g $ResourceGroupForDeployment  -n $WebAppNameAdmin --settings KnownUsers=$PublisherAdminUsers SaaSApiConfiguration__AdAuthenticationEndPoint=https://login.microsoftonline.com SaaSApiConfiguration__ClientId=$ADApplicationID SaaSApiConfiguration__ClientSecret=$ADApplicationSecretKeyVault SaaSApiConfiguration__FulFillmentAPIBaseURL=https://marketplaceapi.microsoft.com/api SaaSApiConfiguration__FulFillmentAPIVersion=2018-08-31 SaaSApiConfiguration__GrantType=client_credentials SaaSApiConfiguration__MTClientId=$ADMTApplicationID SaaSApiConfiguration__Resource=20e940b3-4c77-4b0b-9a53-9e16a1b010a7 SaaSApiConfiguration__TenantId=$TenantID SaaSApiConfiguration__SignedOutRedirectUri=https://$WebAppNamePrefix-portal.azurewebsites.net/Home/Index/ SaaSApiConfiguration__SupportmeteredBilling=$MeteredSchedulerSupport SaaSApiConfiguration_CodeHash=$SaaSApiConfiguration_CodeHash

Write-host "📜  Create  customer portal webapp"
az webapp create -g $ResourceGroupForDeployment -p $WebAppNameService -n $WebAppNamePortal --runtime dotnet:6
az webapp identity assign -g $ResourceGroupForDeployment  -n $WebAppNamePortal --identities [system] 
$WebAppNamePortalId=$(az webapp identity show  -g $ResourceGroupForDeployment  -n $WebAppNamePortal --query principalId -o tsv)

Write-host "📜  Add publisher Admin webapp Identity to KV"
az keyvault set-policy --name $KeyVault  --object-id $WebAppNamePortalId --secret-permissions get list --key-permissions get list

Write-host "📜  Add Portal Configuration"
az webapp config connection-string set -g $ResourceGroupForDeployment -n $WebAppNamePortal -t SQLAzure --settings DefaultConnection=$DefaultConnectionKeyVault
az webapp config appsettings set -g $ResourceGroupForDeployment  -n $WebAppNamePortal --settings KnownUsers=$PublisherAdminUsers SaaSApiConfiguration__AdAuthenticationEndPoint=https://login.microsoftonline.com SaaSApiConfiguration__ClientId=$ADApplicationID SaaSApiConfiguration__ClientSecret=$ADApplicationSecretKeyVault SaaSApiConfiguration__FulFillmentAPIBaseURL=https://marketplaceapi.microsoft.com/api SaaSApiConfiguration__FulFillmentAPIVersion=2018-08-31 SaaSApiConfiguration__GrantType=client_credentials SaaSApiConfiguration__MTClientId=$ADMTApplicationID SaaSApiConfiguration__Resource=20e940b3-4c77-4b0b-9a53-9e16a1b010a7 SaaSApiConfiguration__TenantId=$TenantID SaaSApiConfiguration__SignedOutRedirectUri=https://$WebAppNamePrefix-portal.azurewebsites.net/Home/Index/ SaaSApiConfiguration__SupportmeteredBilling=$MeteredSchedulerSupport  SaaSApiConfiguration_CodeHash=$SaaSApiConfiguration_CodeHash

Write-host "📜  Deploying the Publisher Code to Admin portal"
az webapp deploy --resource-group "$ResourceGroupForDeployment" --name "$WebAppNameAdmin" --src-path $publisherPackage --type zip

Write-host "📜  Deploying the Customer Code to Customer portal"
az webapp deploy --resource-group "$ResourceGroupForDeployment" --name "$WebAppNamePortal" --src-path $customerPackage  --type zip

Write-host "🧹  Cleaning things up!"
# Cleanup : Delete the temporary storage account and the resource group created to host the bacpac file.
Remove-Item -path ["..\..\Publish"] -recurse -Force

Write-host "🏁  If the intallation completed without error complete the folllowing checklist:"
if ($ISADMTApplicationIDProvided) {  #If provided then show the user where to add the landing page in AAD, otherwise script did this already for the user.
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
