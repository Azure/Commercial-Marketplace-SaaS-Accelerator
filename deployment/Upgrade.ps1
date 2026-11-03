#!/usr/bin/env pwsh
#Requires -Version 7.0
#Requires -PSEdition Core
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See LICENSE file in the project root for license information.

Param(
    [string][Parameter(Mandatory)]$WebAppNamePrefix,
    [string][Parameter(Mandatory)]$ResourceGroupForDeployment,
    [string][Parameter()]$TenantID,
    [string][Parameter()]$AzureSubscriptionID,
    [bool][Parameter()]$UsePrivateEndpoints
)

. (Join-Path $PSScriptRoot 'DeploymentHelpers.ps1')
$ErrorActionPreference = 'Stop'

Write-Host @"
The SaaS Accelerator is offered under the MIT License as open source software and is not supported by Microsoft.

If you need help with the accelerator or would like to report defects or feature requests use the Issues feature on the GitHub repository at https://aka.ms/SaaSAccelerator

Do you agree? (Y/N)
"@ -ForegroundColor Yellow
if ((Read-Host) -ne 'Y') {
    Write-Host 'You did not agree. Exiting...' -ForegroundColor Red
    return
}

Assert-DeploymentPrerequisites
$currentContext = Get-DeploymentAzureContext
$currentTenant = $currentContext.tenantId
$currentSubscription = $currentContext.id
if (-not $TenantID) {
    Get-AzTenant -ErrorAction Stop | Format-Table
    $TenantID = Read-Host "Type your TenantID or press Enter to accept your current one [$currentTenant]"
    if (-not $TenantID) { $TenantID = $currentTenant }
}
if (-not $AzureSubscriptionID) {
    Get-AzSubscription -TenantId $TenantID -ErrorAction Stop | Format-Table
    $AzureSubscriptionID = Read-Host "Type your SubscriptionID or press Enter to accept your current one [$currentSubscription]"
    if (-not $AzureSubscriptionID) { $AzureSubscriptionID = $currentSubscription }
}
Set-DeploymentSubscription -SubscriptionId $AzureSubscriptionID -TenantId $TenantID

if (-not $PSBoundParameters.ContainsKey('UsePrivateEndpoints')) {
    $isPEenv = Read-Host 'Is your environment setup with private endpoints? (Y/N)'
    if ($isPEenv -notin @('Y', 'N')) {
        throw 'Specify Y or N for private endpoint connectivity.'
    }
    $UsePrivateEndpoints = $isPEenv -eq 'Y'
}
if ($UsePrivateEndpoints) {
    Write-Warning 'Private endpoints require private DNS and network connectivity to SQL, Key Vault and the deployment endpoints. Ordinary public Cloud Shell cannot reach private-only resources. No public firewall rules will be added.'
}

$RepositoryRoot = Split-Path $PSScriptRoot -Parent
$Workspace = New-DeploymentWorkspace
try {
    Write-Host 'Restoring dependencies and building all application components before database changes.'
    New-DeploymentPackages -RepositoryRoot $RepositoryRoot -Workspace $Workspace
    $migration = New-DeploymentMigration -RepositoryRoot $RepositoryRoot -Workspace $Workspace
    Compress-DeploymentPackages -Workspace $Workspace

    $ConnectionString = Get-DeploymentConnectionString -VaultName ($WebAppNamePrefix + '-kv')
    $configuration = Get-DeploymentSqlConfiguration -ConnectionString $ConnectionString
    $SqlParams = Get-DeploymentSqlParameters -Configuration $configuration
    Test-DeploymentSqlAccess -SqlParameters $SqlParams

    # Preserve pre-EF migration history compatibility for existing deployments.
    $compatibilityScript = @"
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
    GO
"@
    Write-Host 'Applying migration history compatibility and idempotent database migrations.'
    Invoke-DeploymentSql -SqlParameters $SqlParams -Query $compatibilityScript
    Invoke-DeploymentSql -SqlParameters $SqlParams -InputFile $migration

    Write-Host 'Deploying application packages.'
    Publish-DeploymentPackages -Workspace $Workspace -ResourceGroup $ResourceGroupForDeployment `
        -AdminApp ($WebAppNamePrefix + '-admin') -CustomerApp ($WebAppNamePrefix + '-portal')
}
finally {
    Remove-Item -LiteralPath $Workspace -Recurse -Force -ErrorAction Stop
}

Write-Host 'The upgrade process has completed successfully.'
Write-Warning 'If upgrading to >=7.5.0, MeterScheduler is pre-enabled and configured in the database instead of App Service. Review IsMeteredBillingEnabled in Admin portal -> Settings.'
