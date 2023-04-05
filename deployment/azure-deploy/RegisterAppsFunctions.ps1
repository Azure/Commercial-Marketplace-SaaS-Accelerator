# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See LICENSE file in the project root for license information.

#
# Powershell module with functions to create App registrations in AAD to allow access to SaaS Fulfillment API and SSO
#

<#
.SYNOPSIS
Creates App Registration for calls to the SaaS Fulfillment API.

.PARAMETER WebAppNamePrefix
The prefix used for creating web applications. This parameter is mandatory.

.OUTPUTS
Returns an object with these fields:
- ADApplicationID: The value should match the value provided for Active Directory Application ID in the Technical Configuration of the Transactable Offer in Partner Center.
- ADApplicationSecret: Secret key of the AD Application.

.EXAMPLE
Import-Module RegisterAppsFunctions.ps1
Set-FulfillmentWebAppRegistration -WebAppNamePrefix "MyWebApp"

This example creates a web app registration with the "MyWebApp-" prefix.

#>
function Set-FulfillmentWebAppRegistration() {
    Param(
        [string][Parameter(Mandatory)]$WebAppNamePrefix
    )

    Write-Host "🔑 Creating Fulfillment API App Registration"
    try {   
        $ADApplicationDisplayName = "$WebAppNamePrefix-FulfillmentAppReg"
        $ADApplication = az ad app create --only-show-errors --display-name $ADApplicationDisplayName | ConvertFrom-Json
        if ($LASTEXITCODE) { throw "Creating $ADApplicationDisplayName App Registration returned $LASTEXITCODE exit code, terminating ..." }
        $ADObjectID = $ADApplication.id
        $ADApplicationID = $ADApplication.appId
        sleep 5 #this is to give time to AAD to register
        $ADApplicationSecret = az ad app credential reset --id $ADObjectID --append --display-name 'SaaSAPI' --years 2 --query password --only-show-errors --output tsv
                
        Write-Host "   🔵 FulfillmentAPI App Registration created."
        Write-Host "      ➡️ Application Display Name:" $ADApplicationDisplayName
        Write-Host "      ➡️ Application ID:" $ADApplicationID
        Write-Host "      ➡️ App Secret:" $ADApplicationSecret
    }
    catch [System.Net.WebException],[System.IO.IOException] {
        Write-Host "🚨🚨   $PSItem.Exception"
        throw;
    }

    return [PSCustomObject]@{
        ADApplicationID = $ADApplicationID
        ADApplicationSecret = $ADApplicationSecret
    }
}

<#
.SYNOPSIS
Creates Multi-Tenant App Registration allowing SSO.

.PARAMETER WebAppNamePrefix
The prefix used for creating web applications. This parameter is mandatory.

.PARAMETER LogoURLpng
URI to a PNG file with logo for uploading to the WebAppReg.

.OUTPUTS
Returns an object with one field:
- ADMTApplicationID: Multi-Tenant Active Directory Application ID.

.EXAMPLE
Import-Module RegisterAppsFunctions.ps1
Set-SsoWebAppRegistration -WebAppNamePrefix "MyWebApp" -LogoURLpng "https://cdn2.steamgriddb.com/file/sgdb-cdn/logo/3e49d4ec10aad7c623659c37e011665a.png"

This example creates a web app registration with the "MyWebApp-" prefix and uploads the Application Branding logo.

#>
function Set-SsoWebAppRegistration() {
    Param(  
        [string][Parameter(Mandatory)]$WebAppNamePrefix,
        [string][Parameter()]$LogoURLpng
    )
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
        if ($LASTEXITCODE) { throw "Creating $ADMTApplicationDisplayName App Registration returned $LASTEXITCODE exit code, terminating ..." }

        $ADMTApplicationID = $landingpageLoginAppReg.appId
        $ADMTObjectID = $landingpageLoginAppReg.id

        Write-Host "   🔵 Landing Page SSO App Registration created."
        Write-Host "      ➡️ Application Display Name:" $ADMTApplicationDisplayName
        Write-Host "      ➡️ Application Id: $ADMTApplicationID"

        # Download Publisher's AppRegistration logo
        if($LogoURLpng) { 
            Write-Host "   🔵 Logo image provided. Setting the Application branding logo"
            Write-Host "      ➡️ Setting the Application branding logo"
            $token=(az account get-access-token --resource "https://graph.microsoft.com" --query accessToken --output tsv)
            $logoWeb = Invoke-WebRequest $LogoURLpng
            $logoContentType = $logoWeb.Headers["Content-Type"]
            $logoContent = $logoWeb.Content
            
            $uploaded = Invoke-WebRequest `
                -Uri "https://graph.microsoft.com/v1.0/applications/$ADMTObjectID/logo" `
                -Method "PUT" `
                -Header @{"Authorization"="Bearer $token";"Content-Type"="$logoContentType";} `
                -Body $logoContent
            
            Write-Host "      ➡️ Application branding logo set from $LogoURLpng"
        }
        
    }
    catch [System.Net.WebException],[System.IO.IOException] {
        Write-Host "🚨🚨   $PSItem.Exception"
        throw;
    }

    return [PSCustomObject]@{
        ADMTApplicationID = $ADMTApplicationID
    }
}
