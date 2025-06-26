# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See LICENSE file in the project root for license information.

#
# Powershell script to deploy the resources - Customer portal, Publisher portal and the Azure SQL Database
#

Param(  
   [string][Parameter(Mandatory)]$WebAppNamePrefix, # Prefix used for creating web applications
   [string][Parameter(Mandatory)]$ResourceGroupForDeployment # Name of the resource group to deploy the resources
)

# Define the message
$message = @"
The SaaS Accelerator is offered under the MIT License as open source software and is not supported by Microsoft.

If you need help with the accelerator or would like to report defects or feature requests use the Issues feature on the GitHub repository at https://aka.ms/SaaSAccelerator

Do you agree? (Y/N)
"@

# Display the message in yellow
Write-Host $message -ForegroundColor Yellow

# Prompt the user for input
$response = Read-Host


# Check the user's response
if ($response -ne 'Y' -and $response -ne 'y') {
    Write-Host "You did not agree. Exiting..." -ForegroundColor Red
    exit
}

# Proceed if the user agrees
Write-Host "Thank you for agreeing. Proceeding with the script..." -ForegroundColor Green


#Get TenantID if not set as argument
	$currentContext = az account show | ConvertFrom-Json
	$currentTenant = $currentContext.tenantId
	$currentSubscription = $currentContext.id

	Get-AzTenant | Format-Table
	$TenantID = $null
    if (!($TenantID = Read-Host "⌨  Type your TenantID or press Enter to accept your current one [$currentTenant]")) { $TenantID = $currentTenant }  
	
	#Get Azure Subscription if not set as argument
	Get-AzSubscription -TenantId $TenantID | Format-Table
	$AzureSubscriptionID = $null
	if (!($AzureSubscriptionID = Read-Host "⌨  Type your SubscriptionID or press Enter to accept your current one [$currentSubscription]")) { $AzureSubscriptionID = $currentSubscription }
	#Set the AZ Cli context

	az account set -s $AzureSubscriptionID
	Write-Host "🔑 Azure Subscription '$AzureSubscriptionID' selected."

#endregion

Function String-Between
{
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory=$true)][String]$Source,
		[Parameter(Mandatory=$true)][String]$Start,
		[Parameter(Mandatory=$true)][String]$End
	)
	$sIndex = $Source.indexOf($Start) + $Start.length
	$eIndex = $Source.indexOf($End, $sIndex)
	return $Source.Substring($sIndex, $eIndex-$sIndex)
}

# Setting variables
$ErrorActionPreference = "Stop"
$WebAppNameAdmin=$WebAppNamePrefix+"-admin"
$WebAppNamePortal=$WebAppNamePrefix+"-portal"
$KeyVault=$WebAppNamePrefix+"-kv"
$SQLDatabaseName = $WebAppNamePrefix +"AMPSaaSDB"
$SQLServerName = $WebAppNamePrefix + "-sql"
$ServerUri = $SQLServerName+".database.windows.net"
$ServerUriPrivate = $SQLServerName+".privatelink.database.windows.net"

#region Deploy Database

# Ask user if their env is private end point protected and run a if else based on the response
$isPEenv = Read-Host "Is your environment setup with private endpoints? (Y/N)"

#### THIS SECTION DEPLOYS CODE AND DATABASE CHANGES
Write-host "#### STEP 1 Database deployment start####"

if ($isPEenv -ne 'Y' -and $isPEenv -ne 'y') {
	
	Write-host "## STEP 1.1 Retrieved ConnectionString from KeyVault"
	$ConnectionString = az keyvault secret show `
		--vault-name $KeyVault `
		--name "DefaultConnection" `
		--query "{value:value}" `
		--output tsv

	#Extract components from ConnectionString since Invoke-Sqlcmd needs them separately
	$Server = String-Between -source $ConnectionString -start "Data Source=" -end ";"
	$Database = String-Between -source $ConnectionString -start "Initial Catalog=" -end ";"
	$User = String-Between -source $ConnectionString -start "User Id=" -end ";"
	$Pass = String-Between -source $ConnectionString -start "Password=" -end ";"

	Write-host "## STEP 1.2 Update connection string to the Adminsite project"
	Set-Content -Path ../src/AdminSite/appsettings.Development.json -value "{`"ConnectionStrings`": {`"DefaultConnection`":`"$ConnectionString`"}}"

	Write-host "## STEP 1.3 START Generating migration script"	
	dotnet-ef migrations script `
		--idempotent `
		--context SaaSKitContext `
		--project ../src/DataAccess/DataAccess.csproj `
		--startup-project ../src/AdminSite/AdminSite.csproj `
		--output script.sql

    $compatibilityScript = "
	IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL 
	-- No __EFMigrations table means Database has not been upgraded to support EF Migrations
	BEGIN
		CREATE TABLE [__EFMigrationsHistory] (
			[MigrationId] nvarchar(150) NOT NULL,
			[ProductVersion] nvarchar(32) NOT NULL,
			CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
		);

		IF (SELECT TOP 1 VersionNumber FROM DatabaseVersionHistory ORDER BY CreateBy DESC) = '2.10'
			INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) 
				VALUES (N'20221118045814_Baseline_v2', N'6.0.1');

		IF (SELECT TOP 1 VersionNumber FROM DatabaseVersionHistory ORDER BY CreateBy DESC) = '5.00'
			INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])  
				VALUES (N'20221118045814_Baseline_v2', N'6.0.1'), (N'20221118203340_Baseline_v5', N'6.0.1');

		IF (SELECT TOP 1 VersionNumber FROM DatabaseVersionHistory ORDER BY CreateBy DESC) = '6.10'
			INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])  
				VALUES (N'20221118045814_Baseline_v2', N'6.0.1'), (N'20221118203340_Baseline_v5', N'6.0.1'), (N'20221118211554_Baseline_v6', N'6.0.1');
	END;
	GO"

	
	Write-host "## STEP 1.4 Running compatibility script"
	Invoke-Sqlcmd -query $compatibilityScript -ServerInstance $Server -database $Database -Username $User -Password $Pass


	Write-host "## STEP 1.5 START: Run migration against database"
	Invoke-Sqlcmd -inputFile script.sql -ServerInstance $Server -database $Database -Username $User -Password $Pass
	
} else
{
	Write-host "## STEP 1.1 Constructing connection string with AAD auth"
	$ConnectionString="Server=tcp:"+$ServerUriPrivate+";Database="+$SQLDatabaseName+";TrustServerCertificate=True;Authentication=Active Directory Default;"

	Write-host "## STEP 1.2 Update connection string to the Adminsite project"
	Set-Content -Path ../src/AdminSite/appsettings.Development.json -value "{`"ConnectionStrings`": {`"DefaultConnection`":`"$ConnectionString`"}}"

	Write-host "## STEP 1.3 START Generating migration script"	
	dotnet-ef migrations script `
		--idempotent `
		--context SaaSKitContext `
		--project ../src/DataAccess/DataAccess.csproj `
		--startup-project ../src/AdminSite/AdminSite.csproj `
		--output script.sql

	Write-Host "## STEP 1.4 Getting the IP"
	$currentIP = (Invoke-WebRequest -Uri "http://ifconfig.me/ip").Content.Trim()
	
	Write-Host "## STEP 1.5 Add the current IP to the SQL server firewall rules"
	az sql server firewall-rule create `
		--resource-group $ResourceGroupForDeployment `
		--server $SQLServerName `
		--name "SAAllowCurrentIP" `
		--start-ip-address $currentIP `
		--end-ip-address $currentIP

	Write-Host "## STEP 1.6 Current IP added to SQL server firewall rules." -ForegroundColor Green

	Write-host "## STEP 1.7 ➡️ Execute SQL schema/data script"
	Invoke-Sqlcmd -InputFile ./script.sql -ConnectionString $ConnectionString

	Write-host "## STEP 1.8 START: Removing the client IP which was added at 1.5"
	az sql server firewall-rule delete `
		--resource-group $ResourceGroupForDeployment `
		--server $SQLServerName `
		--name "SAAllowCurrentIP" `
}

Remove-Item -Path ../src/AdminSite/appsettings.Development.json
Remove-Item -Path script.sql

Write-host "#### Database Deployment complete ####"	


#endregion Deploy Database

#region Deploy code

Write-host "#### STEP 2 Deploying new code ####" 

Write-host "## STEP 2.1 Building Admin Portal" 
dotnet publish ../src/AdminSite/AdminSite.csproj -v q -c release -o ../Publish/AdminSite/

Write-host "## STEP 2.2 Building Meter Scheduler"
dotnet publish ../src/MeteredTriggerJob/MeteredTriggerJob.csproj -c release -o ../Publish/AdminSite/app_data/jobs/triggered/MeteredTriggerJob/ --runtime win-x64 --self-contained true -p:PublishReadyToRun=false

Write-host "## STEP 2.3 Building Customer Portal" 
dotnet publish ../src/CustomerSite/CustomerSite.csproj -v q -c release -o ../Publish/CustomerSite

Write-host "## STEP 2.4 Compress packages." 
Compress-Archive -Path ../Publish/CustomerSite/* -DestinationPath ../Publish/CustomerSite.zip -Force
Compress-Archive -Path ../Publish/AdminSite/* -DestinationPath ../Publish/AdminSite.zip -Force

Write-host "## STEP 2.5 Deploying code to Admin Portal"
az webapp deploy `
	--resource-group $ResourceGroupForDeployment `
	--name $WebAppNameAdmin `
	--src-path "../Publish/AdminSite.zip" `
	--type zip
Write-host "## Deployed code to Admin Portal"

Write-host "## STEP 2.6 Deploying code to Customer Portal"
az webapp deploy `
	--resource-group $ResourceGroupForDeployment `
	--name $WebAppNamePortal `
	--src-path "../Publish/CustomerSite.zip"  `
	--type zip
Write-host "## Deployed code to Customer Portal"

#endregion Deploy code

Remove-Item -Path ../Publish -recurse -Force
Write-host "#### Code deployment complete ####" 
Write-host ""
Write-host "#### The upgrade process has completed successfully ####" 
Write-host ""
Write-host "#### Warning!!! ####"
Write-host "#### If the upgrade is to >=7.5.0, MeterScheduler feature is pre-enabled and changed to DB config instead of the App Service configuration. Please update the IsMeteredBillingEnabled value accordingly in the Admin portal -> Settings page. ####"
Write-host "#### "
