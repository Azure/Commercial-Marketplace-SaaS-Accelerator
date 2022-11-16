# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See LICENSE file in the project root for license information.

#
# Powershell script to deploy code - Customer portal, Publisher portal and the Azure SQL Database
#
Param(  
   [string][Parameter(Mandatory)]$WebAppNamePrefix, # Prefix used for creating web applications
   [string][Parameter(Mandatory)]$ResourceGroupForDeployment, # Name of the resource group to deploy the resources
   [switch][Parameter()]$TryFixDatabase #if upgrading from an older version of the accelerator, set to true to attempt to bring database to latest.
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

Write-host "#### Deploying new database ####" 
$ConnectionString = az keyvault secret show `
	--vault-name $KeyVault `
	--name "DefaultConnection" `
	--query "{value:value}" `
	--output tsv
	
#Extract components from ConnectionString since Invoke-Sqlcmd needs them separately. This won't work if password contains ; or = :(
#$dbData = @{}
#$ConnectionString.split(';') | Foreach-object {$kv = $_.split('='); $dbData.Add($kv[0], $kv[1])}
$Server = String-Between -source $ConnectionString -start "Data Source=" -end ";"
$Database = String-Between -source $ConnectionString -start "Initial Catalog=" -end ";"
$User = String-Between -source $ConnectionString -start "User Id=" -end ";"
$Pass = String-Between -source $ConnectionString -start "Password=" -end ";"

Write-host "## Retrieved ConnectionString from KeyVault"
$devJson = "{`"ConnectionStrings`": {`"DefaultConnection`":`"$ConnectionString`"}}"
Set-Content -Path ../src/AdminSite/appsettings.Development.json -value $devJson

dotnet-ef migrations script `
	--idempotent `
	--context SaaSKitContext `
	--project ../src/DataAccess/DataAccess.csproj `
	--startup-project ../src/AdminSite/AdminSite.csproj `
	-o script.sql
	
Write-host "## Generated migration script"	

if ($TryFixDatabase -eq $true){
	#manually create the migrations table and initialize the db to the initial migration
	Write-host "## !!!Attempting to upgrade database to migration compatibility.!!!"	
	$createTable = "CREATE TABLE [__EFMigrationsHistory] ([MigrationId] nvarchar(150) NOT NULL,[ProductVersion] nvarchar(32) NOT NULL,CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId]));"
	$updateTable = "INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20221116194140_Initial', N'6.0.1');"

	Invoke-Sqlcmd -query $createTable -ServerInstance $Server -database $Database -Username $User -Password $Pass
	Invoke-Sqlcmd -query $updateTable -ServerInstance $Server -database $Database -Username $User -Password $Pass
}

Invoke-Sqlcmd -ServerInstance $Server -database $Database -Username $User -Password $Pass -InputFile ./script.sql

Write-host "## Ran migration against database"	

Remove-Item -Path ../src/AdminSite/appsettings.Development.json
Remove-Item -Path script.sql
Write-host "#### Database Deployment complete ####"	


Write-host "#### Deploying new code ####" 

dotnet publish ../src/AdminSite/AdminSite.csproj -v q -c debug -o ../Publish/AdminSite/
Write-host "## Admin Portal built" 
if ($MeteredSchedulerSupport -eq $true)
{ 
    dotnet publish ../src/MeteredTriggerJob/MeteredTriggerJob.csproj -v q -c debug -o ../Publish/AdminSite/app_data/jobs/triggered/MeteredTriggerJob --runtime win-x64 --self-contained true 
    Write-host "## Metered Scheduler to Admin Portal Built"
}

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