# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See LICENSE file in the project root for license information.

#
# Powershell script to deploy the resources - Customer portal, Publisher portal and the Azure SQL Database
#

Param(  
   [string][Parameter(Mandatory)]$WebAppNamePrefix, # Prefix used for creating web applications
   [string][Parameter(Mandatory)]$ResourceGroupForDeployment # Name of the resource group to deploy the resources
)

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

$ErrorActionPreference = "Stop"
$WebAppNameAdmin=$WebAppNamePrefix+"-admin"
$WebAppNamePortal=$WebAppNamePrefix+"-portal"
$KeyVault=$WebAppNamePrefix+"-kv"

#### THIS SECTION DEPLOYS CODE AND DATABASE CHANGES
Write-host "#### Deploying new database ####" 
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

Write-host "## Retrieved ConnectionString from KeyVault"
Set-Content -Path ../src/AdminSite/appsettings.Development.json -value "{`"ConnectionStrings`": {`"DefaultConnection`":`"$ConnectionString`"}}"

dotnet-ef migrations script `
	--idempotent `
	--context SaaSKitContext `
	--project ../src/DataAccess/DataAccess.csproj `
	--startup-project ../src/AdminSite/AdminSite.csproj `
	--output script.sql
	
Write-host "## Generated migration script"	

Write-host "## !!!Attempting to upgrade database to migration compatibility.!!!"	
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

Invoke-Sqlcmd -query $compatibilityScript -ServerInstance $Server -database $Database -Username $User -Password $Pass
Write-host "## Ran compatibility script against database"
Invoke-Sqlcmd -inputFile script.sql -ServerInstance $Server -database $Database -Username $User -Password $Pass
Write-host "## Ran migration against database"	

Remove-Item -Path ../src/AdminSite/appsettings.Development.json
Remove-Item -Path script.sql
Write-host "#### Database Deployment complete ####"	



Write-host "#### Deploying new code ####" 

dotnet publish ../src/AdminSite/AdminSite.csproj -v q -c debug -o ../Publish/AdminSite/
Write-host "## Admin Portal built" 
dotnet publish ../src/MeteredTriggerJob/MeteredTriggerJob.csproj -v q -c debug -o ../Publish/AdminSite/app_data/jobs/triggered/MeteredTriggerJob --runtime win-x64 --self-contained true 
Write-host "## Metered Scheduler to Admin Portal Built"
dotnet publish ../src/CustomerSite/CustomerSite.csproj -v q -c debug -o ../Publish/CustomerSite
Write-host "## Customer Portal Built" 

Compress-Archive -Path ../Publish/CustomerSite/* -DestinationPath ../Publish/CustomerSite.zip -Force
Compress-Archive -Path ../Publish/AdminSite/* -DestinationPath ../Publish/AdminSite.zip -Force
Write-host "## Code packages prepared." 

Write-host "## Deploying code to Admin Portal"
az webapp deploy `
	--resource-group $ResourceGroupForDeployment `
	--name $WebAppNameAdmin `
	--src-path "../Publish/AdminSite.zip" `
	--type zip
Write-host "## Deployed code to Admin Portal"

Write-host "## Deploying code to Customer Portal"
az webapp deploy `
	--resource-group $ResourceGroupForDeployment `
	--name $WebAppNamePortal `
	--src-path "../Publish/CustomerSite.zip"  `
	--type zip
Write-host "## Deployed code to Customer Portal"

Remove-Item -Path ../Publish -recurse -Force
Write-host "#### Code deployment complete ####" 
