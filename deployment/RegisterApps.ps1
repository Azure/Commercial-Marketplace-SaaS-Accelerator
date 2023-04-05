# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See LICENSE file in the project root for license information.

#
# Powershell script to create App registrations in AAD to allow access to  and SSO
#

<#
.SYNOPSIS
Creates App Registrations:
- One for authenticating calls to the Marketplace API with the specified prefix and additional configuration options.
- Second Multi-Tenant App Registration for Landing Page User Login.

.PARAMETER WebAppNamePrefix
The prefix used for creating web applications. This parameter is mandatory.

.PARAMETER TenantID
ID of a tenant in which the WebApp registration needs to be created.

.PARAMETER LogoURLpng
URI to a PNG file with logo for uploading to the WebAppReg.

.EXAMPLE
.\RegisterApps.ps1 -WebAppNamePrefix "amp_saas_accelerator_<unique>" -TenantID 04a1fb8a-922c-4808-8b05-6aad85609349

This example creates 2 web app registrations with the prefix "MyWebApp" in Teant with ID: 04a1fb8a-922c-4808-8b05-6aad85609349.
#>

Param(  
   [string][Parameter(Mandatory)]$WebAppNamePrefix, # Prefix used for creating web applications
   [string][Parameter()]$TenantID, # The value should match the value provided for Active Directory TenantID in the Technical Configuration of the Transactable Offer in Partner Center
   [string][Parameter()]$LogoURLpng
   )

# Make sure to install Az Module before running this script
# Install-Module Az
# Install-Module -Name AzureAD

$ErrorActionPreference = "Stop"

#region Validate Parameters

if($WebAppNamePrefix.Length -gt 21) {
    Throw "🛑 Web name prefix must be less than 21 characters."
    Exit
}

#endregion 

Write-Host "Starting SaaS Accelerator Deployment..."

#region Select Tenant / Subscription for deployment

$currentContext = az account tenant list | ConvertFrom-Json
if ($LASTEXITCODE) { throw "Last command returned $LASTEXITCODE exit code, terminating ..." }
$currentTenant = $currentContext.tenantId

#Get TenantID if not set as argument
if(!($TenantID)) {    
    Get-AzTenant | Format-Table
    if (!($TenantID = Read-Host "⌨  Type your TenantID or press Enter to accept your current one [$currentTenant]")) { $TenantID = $currentTenant }    
}
else {
    Write-Host "🔑 Tenant provided for Web App Registrations: $TenantID"
}

#endregion
Import-Module (Join-Path $PSScriptRoot "./azure-deploy/RegisterAppsFunctions.ps1") -Force
$result = Set-FulfillmentWebAppRegistration -WebAppNamePrefix $WebAppNamePrefix
$result = Set-SsoWebAppRegistration -WebAppNamePrefix $WebAppNamePrefix `
    -LogoURLpng $LogoURLpng
