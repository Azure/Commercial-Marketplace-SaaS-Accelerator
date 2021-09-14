#
# Powershell script to deploy the resources - Customer portal, Publisher portal and the Azure SQL Database
#

Param(  
   [string][Parameter(Mandatory)]$WebAppNamePrefix, # Prefix used for creating web applications
   [string][Parameter(Mandatory)]$TenantID, # The value should match the value provided for Active Directory TenantID in the Technical Configuration of the Transactable Offer in Partner Center
   [string][Parameter(Mandatory)]$ADApplicationID, # The value should match the value provided for Active Directory Application ID in the Technical Configuration of the Transactable Offer in Partner Center
   [string][Parameter(Mandatory)]$ADApplicationSecret, # Secret key of the AD Application
   [string][Parameter(Mandatory)]$ADMTApplicationID, # The value should match the value provided for Multi-Tenant Active Directory Application ID in the Technical Configuration of the Transactable Offer in Partner Center
   [string][Parameter(Mandatory)]$SQLServerName, # Name of the database server (without database.windows.net)
   [string][Parameter(Mandatory)]$SQLAdminLogin, # SQL Admin login
   [string][Parameter(Mandatory)]$SQLAdminLoginPassword, # SQL Admin password
   [string][Parameter(Mandatory)]$PublisherAdminUsers, # Provide a list of email addresses (as comma-separated-values) that should be granted access to the Publisher Portal
   [string][Parameter(Mandatory)]$BacpacUrl, # The url to the blob storage where the SaaS DB bacpac is stored
   [string][Parameter(Mandatory)]$ResourceGroupForDeployment, # Name of the resource group to deploy the resources
   [string][Parameter(Mandatory)]$Location, # Location of the resource group
   [string][Parameter(Mandatory)]$AzureSubscriptionID, # Subscription where the resources be deployed
   [string][Parameter(Mandatory)]$PathToARMTemplate              # Local Path to the ARM Template
)

#   Make sure to install Az Module before running this script

#   Install-Module Az

$TempFolderToStoreBacpac = '.\AMPSaaSDatabase'
$BacpacFileName = "AMPSaaSDB.bacpac"
$LocalPathToBacpacFile = $TempFolderToStoreBacpac + "\" + $BacpacFileName  

# Create a temporary folder
New-Item -Path $TempFolderToStoreBacpac -ItemType Directory -Force

# Download Bacpac
Invoke-WebRequest -Uri $BacpacUrl -OutFile $LocalPathToBacpacFile

Connect-AzAccount
$storagepostfix = Get-Random -Minimum 1 -Maximum 1000

$StorageAccountName = "amptmpstorage" + $storagepostfix       #enter storage account name

$ContainerName = "packagefiles" #container name for uploading SQL DB file 
$BlobName = "blob"
$resourceGroupForStorageAccount = "amptmpstorage"   #resource group name for the storage account.
                                                      

Write-host "Select subscription : $AzureSubscriptionID" 
Select-AzSubscription -SubscriptionId $AzureSubscriptionID


Write-host "Creating a temporary resource group and storage account - $resourceGroupForStorageAccount"
New-AzResourceGroup -Name $resourceGroupForStorageAccount -Location $location -Force
New-AzStorageAccount -ResourceGroupName $resourceGroupForStorageAccount -Name $StorageAccountName -Location $location -SkuName Standard_LRS -Kind StorageV2
$StorageAccountKey = @((Get-AzStorageAccountKey -ResourceGroupName $resourceGroupForStorageAccount -Name $StorageAccountName).Value)
$key = $StorageAccountKey[0]

$ctx = New-AzstorageContext -StorageAccountName $StorageAccountName  -StorageAccountKey $key

New-AzStorageContainer -Name $ContainerName -Context $ctx -Permission Blob 
Set-AzStorageBlobContent -File $LocalPathToBacpacFile -Container $ContainerName -Blob $BlobName -Context $ctx -Force

$URLToBacpacFromStorage = (Get-AzStorageBlob -blob $BlobName -Container $ContainerName -Context $ctx).ICloudBlob.uri.AbsoluteUri

Write-host "Uploaded the bacpac file to $URLToBacpacFromStorage"    


Write-host "Prepare publish files for the web application"

Write-host "Preparing the publish files for PublisherPortal"  
dotnet publish ..\..\src\SaaS.SDK.PublisherSolution\SaaS.SDK.PublisherSolution.csproj -c debug -o ..\..\Publish\PublisherPortal
Compress-Archive -Path ..\..\Publish\PublisherPortal\* -DestinationPath ..\..\Publish\PublisherPortal.zip -Force

Write-host "Preparing the publish files for CustomerPortal"
dotnet publish ..\..\src\SaaS.SDK.CustomerProvisioning\SaaS.SDK.CustomerProvisioning.csproj -c debug -o ..\..\Publish\CustomerPortal
Compress-Archive -Path ..\..\Publish\CustomerPortal\* -DestinationPath ..\..\Publish\CustomerPortal.zip -Force

Write-host "Upload published files to storage account"
Set-AzStorageBlobContent -File "..\..\Publish\PublisherPortal.zip" -Container $ContainerName -Blob "PublisherPortal.zip" -Context $ctx -Force
Set-AzStorageBlobContent -File "..\..\Publish\CustomerPortal.zip" -Container $ContainerName -Blob "CustomerPortal.zip" -Context $ctx -Force

# The base URI where artifacts required by this template are located
$PathToWebApplicationPackages = ((Get-AzStorageContainer -Container $ContainerName -Context $ctx).CloudBlobContainer.uri.AbsoluteUri)

Write-host "Path to web application packages $PathToWebApplicationPackages"

#Parameter for ARM template, Make sure to add values for parameters before running the script.
$ARMTemplateParams = @{
   webAppNamePrefix             = "$WebAppNamePrefix"
   TenantID                     = "$TenantID"
   ADApplicationID              = "$ADApplicationID"
   ADApplicationSecret          = "$ADApplicationSecret"
   ADMTApplicationID            = "$ADMTApplicationID"
   SQLServerName                = "$SQLServerName"
   SQLAdminLogin                = "$SQLAdminLogin"
   SQLAdminLoginPassword        = "$SQLAdminLoginPassword"
   bacpacUrl                    = "$URLToBacpacFromStorage"
   SAASKeyForbacpac             = ""
   PublisherAdminUsers          = "$PublisherAdminUsers"
   PathToWebApplicationPackages = "$PathToWebApplicationPackages"
}


# Create RG if not exists
New-AzResourceGroup -Name $ResourceGroupForDeployment -Location $location -Force

Write-host "Deploying the ARM template to set up resources"
# Deploy resources using ARM template
New-AzResourceGroupDeployment -ResourceGroupName $ResourceGroupForDeployment -TemplateFile $PathToARMTemplate -TemplateParameterObject $ARMTemplateParams


Write-host "🧹 Cleaning things up!"
# Cleanup : Delete the temporary storage account and the resource group created to host the bacpac file.
Remove-AzResourceGroup -Name $resourceGroupForStorageAccount -Force 
Remove-Item –path $TempFolderToStoreBacpac –recurse 
Remove-Item -path ["..\..\Publish"] -recurse

Write-host "🏁 If the intallation completed without error complete the folllowing checklist:\n"

Write-host "__ Add The following URLs to the multi-tenant AAD App Registration in Azure Portal:"
Write-host "   https://$webAppNamePrefix-portal.azurewebsites.net"
Write-host "   https://$webAppNamePrefix-portal.azurewebsites.net/"
Write-host "   https://$webAppNamePrefix-portal.azurewebsites.net/Home/Index"
Write-host "   https://$webAppNamePrefix-portal.azurewebsites.net/Home/Index/"
Write-host "__ Verify ID Tokens checkbox has been checked-out ✅"

Write-host "__ Add The following URLs to the single-tenant AAD App Registration in Azure Portal:"
Write-host "   https://$webAppNamePrefix-admin.azurewebsites.net"
Write-host "   https://$webAppNamePrefix-admin.azurewebsites.net/"
Write-host "   https://$webAppNamePrefix-admin.azurewebsites.net/Home/Index"
Write-host "   https://$webAppNamePrefix-admin.azurewebsites.net/Home/Index/"
Write-host "__ Verify ID Tokens checkbox has been checked-out ✅"

Write-host "__ Add The following URL in PartnerCenter SaaS Technical Configuration->Landing Page section"
Write-host "   https://$webAppNamePrefix-portal.azurewebsites.net/"
Write-host "__ Add The following URL in PartnerCenter SaaS Technical Configuration->Connection Webhook section"
Write-host "   https://$webAppNamePrefix-portal.azurewebsites.net/api/AzureWebhook"
Write-host "__ Add The following TenantID in PartnerCenter SaaS Technical Configuration Tenant ID"
Write-host "   $TenantID"
Write-host "__ Add The following ApplicationID in PartnerCenter SaaS Technical Configuration->AAD Application ID section"
Write-host "   $ADApplicationID"

