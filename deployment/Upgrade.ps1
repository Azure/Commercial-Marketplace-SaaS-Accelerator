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



Write-host "#### Deploying new code ####" 

dotnet publish ../src/AdminSite/AdminSite.csproj -v q -c release -o ../Publish/AdminSite/
Write-host "## Admin Portal built" 
dotnet publish ../src/MeteredTriggerJob/MeteredTriggerJob.csproj -v q -c release -o ../Publish/AdminSite/app_data/jobs/triggered/MeteredTriggerJob --runtime win-x64 --self-contained true 
Write-host "## Metered Scheduler to Admin Portal Built"
dotnet publish ../src/CustomerSite/CustomerSite.csproj -v q -c release -o ../Publish/CustomerSite
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
Write-host ""
Write-host "#### Warning!!! ####"
Write-host "#### If the upgrade is to >=7.5.0, MeterScheduler feature is pre-enabled and changed to DB config instead of the App Service configuration. Please update the IsMeteredBillingEnabled value accordingly in the Admin portal -> Settings page. ####"
Write-host "#### "
