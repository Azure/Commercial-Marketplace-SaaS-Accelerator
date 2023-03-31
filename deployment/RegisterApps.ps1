# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See LICENSE file in the project root for license information.

#
# Powershell script to create App registrations in AAD to allow access to  and SSO
#

#.\RegisterApps.ps1 `
# -WebAppNamePrefix "amp_saas_accelerator_<unique>" `

Param(  
   [string][Parameter(Mandatory)]$WebAppNamePrefix, # Prefix used for creating web applications
   [string][Parameter()]$TenantID # The value should match the value provided for Active Directory TenantID in the Technical Configuration of the Transactable Offer in Partner Center
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

$currentContext = az account show | ConvertFrom-Json
if ($LASTEXITCODE) { throw "Last command returned $LASTEXITCODE exit code, terminating ..." }
$currentTenant = $currentContext.tenantId

#Get TenantID if not set as argument
if(!($TenantID)) {    
    Get-AzTenant | Format-Table
    if (!($TenantID = Read-Host "⌨  Type your TenantID or press Enter to accept your current one [$currentTenant]")) { $TenantID = $currentTenant }    
}
else {
    Write-Host "🔑 Tenant provided: $TenantID"
}

#endregion

#region Create AAD App Registrations

#Create App Registration for authenticating calls to the Marketplace API
Write-Host "🔑 Creating Fulfilment API App Registration"
try {   
    $ADApplicationDisplayName = "$WebAppNamePrefix-FulfillmentAppReg"
    $ADApplication = az ad app create --only-show-errors --display-name $ADApplicationDisplayName | ConvertFrom-Json
    if ($LASTEXITCODE) { throw "Last command returned $LASTEXITCODE exit code, terminating ..." }
    $ADObjectID = $ADApplication.id
    $ADApplicationID = $ADApplication.appId
    sleep 5 #this is to give time to AAD to register
    $ADApplicationSecret = az ad app credential reset --id $ADObjectID --append --display-name 'SaaSAPI' --years 2 --query password --only-show-errors --output tsv
            
    Write-Host "   🔵 FulfilmentAPI App Registration created."
    Write-Host "      ➡️ Application Display Name:" $ADApplicationDisplayName
    Write-Host "      ➡️ Application ID:" $ADApplicationID
    Write-Host "      ➡️ App Secret:" $ADApplicationSecret
}
catch [System.Net.WebException],[System.IO.IOException] {
    Write-Host "🚨🚨   $PSItem.Exception"
    break;
}

#Create Multi-Tenant App Registration for Landing Page User Login
Write-Host "🔑 Creating Landing Page SSO App Registration"
try {
    $ADMTApplicationDisplayName = "$WebAppNamePrefix-LandingpageAppReg"
    $appCreateRequestBodyJson = @"
{
	"displayName" : "$ADMTApplicationDisplayName",
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
    if ($LASTEXITCODE) { throw "Last command returned $LASTEXITCODE exit code, terminating ..." }
    
    $ADMTApplicationID = $landingpageLoginAppReg.appId

    Write-Host "   🔵 Landing Page SSO App Registration created."
    Write-Host "      ➡️ Application Display Name:" $ADMTApplicationDisplayName
    Write-Host "      ➡️ Application Id: $ADMTApplicationID"

}
catch [System.Net.WebException],[System.IO.IOException] {
    Write-Host "🚨🚨   $PSItem.Exception"
    break;
}

#endregion
