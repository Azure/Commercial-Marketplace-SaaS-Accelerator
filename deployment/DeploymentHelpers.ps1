# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See LICENSE file in the project root for license information.

function Invoke-DeploymentCommand {
    param(
        [Parameter(Mandatory)][string]$Command,
        [string[]]$Arguments = @(),
        [Parameter(Mandatory)][string]$FailureMessage
    )

    # Native stderr can contain secrets. Report the operation, never its arguments or output.
    $PSNativeCommandUseErrorActionPreference = $false
    try {
        $output = & $Command @Arguments 2>&1
        $exitCode = $LASTEXITCODE
    }
    catch {
        throw $FailureMessage
    }
    if ($exitCode -ne 0) {
        throw "$FailureMessage (exit code $exitCode)."
    }
    $output | ForEach-Object { if ($_ -isnot [System.Management.Automation.ErrorRecord]) { "$_" } }
}

function Invoke-AzureCli {
    Invoke-DeploymentCommand -Command az -Arguments $args -FailureMessage 'Azure CLI operation failed. Check sign-in, permissions and network access'
}

function Get-DeploymentAzureContext {
    $context = Invoke-AzureCli account show --output json
    try {
        $result = ($context -join "`n") | ConvertFrom-Json -ErrorAction Stop
    }
    catch {
        throw 'Azure CLI returned an invalid account context. Run az login and try again.'
    }
    if (-not $result.id -or -not $result.tenantId) {
        throw 'Azure CLI account context is missing a subscription or tenant. Run az login.'
    }
    $result
}

function Set-DeploymentSubscription {
    param([string]$SubscriptionId, [string]$TenantId)

    Invoke-AzureCli account set --subscription $SubscriptionId | Out-Null
    $context = Get-DeploymentAzureContext
    if ($context.id -ne $SubscriptionId -or $context.tenantId -ne $TenantId) {
        throw 'Azure subscription context verification failed. Check the selected subscription and tenant.'
    }
}

function Get-DeploymentConnectionString {
    param([string]$VaultName)

    $value = Invoke-AzureCli keyvault secret show --vault-name $VaultName --name DefaultConnection --query value --output tsv
    if ([string]::IsNullOrWhiteSpace($value)) {
        throw 'Key Vault returned an empty DefaultConnection secret.'
    }
    $value
}

function Get-ConnectionStringValue {
    param([System.Data.Common.DbConnectionStringBuilder]$Builder, [string[]]$Keys)

    $values = @($Keys | Where-Object { $Builder.ContainsKey($_) } | ForEach-Object { [string]$Builder[$_] })
    if (@($values | Select-Object -Unique).Count -gt 1) {
        throw 'Connection string contains conflicting aliases.'
    }
    if ($values.Count) { $values[0] }
}

function Get-DeploymentSqlConfiguration {
    param([Parameter(Mandatory)][string]$ConnectionString)

    $builder = [System.Data.Common.DbConnectionStringBuilder]::new()
    try {
        # Property assignment would add a dictionary key instead of setting this .NET property.
        $builder.set_ConnectionString($ConnectionString)
    }
    catch {
        throw 'DefaultConnection is not a valid SQL connection string.'
    }
    $server = Get-ConnectionStringValue $builder @('Server', 'Data Source', 'Address', 'Addr', 'Network Address')
    $database = Get-ConnectionStringValue $builder @('Database', 'Initial Catalog')
    $user = Get-ConnectionStringValue $builder @('User ID', 'UID')
    $password = Get-ConnectionStringValue $builder @('Password', 'PWD')
    $authentication = Get-ConnectionStringValue $builder @('Authentication')
    $integrated = Get-ConnectionStringValue $builder @('Integrated Security', 'Trusted_Connection')
    $encrypt = Get-ConnectionStringValue $builder @('Encrypt')
    $trust = Get-ConnectionStringValue $builder @('TrustServerCertificate', 'Trust Server Certificate')

    if ([string]::IsNullOrWhiteSpace($server) -or [string]::IsNullOrWhiteSpace($database)) {
        throw 'DefaultConnection must contain a server and database.'
    }
    if ($integrated -and $integrated -notin @('false', 'no')) {
        throw 'Integrated Security / Trusted_Connection is not supported. Use SQL Password or a supported Active Directory authentication mode.'
    }
    if ($trust -and $trust -notin @('true', 'false', 'yes', 'no')) {
        throw 'Invalid TrustServerCertificate setting in DefaultConnection.'
    }
    if ($encrypt -and $encrypt -notin @('true', 'false', 'yes', 'no', 'mandatory', 'optional', 'strict')) {
        throw 'Invalid Encrypt setting in DefaultConnection.'
    }
    $hasUser = -not [string]::IsNullOrWhiteSpace($user)
    $hasPassword = -not [string]::IsNullOrEmpty($password)
    if ($builder.ContainsKey('Authentication') -and [string]::IsNullOrWhiteSpace($authentication)) {
        throw 'Authentication must not be empty when specified in DefaultConnection.'
    }
    if ((-not $authentication -or $authentication -eq 'Sql Password') -and $hasUser -and $hasPassword) {
        $mode = 'Sql'
    }
    elseif ($authentication -in @('Active Directory Managed Identity', 'Active Directory MSI', 'Active Directory Default', 'Active Directory Interactive') -and
        -not $hasPassword -and (-not $hasUser -or $authentication -in @('Active Directory Managed Identity', 'Active Directory MSI'))) {
        # A managed identity client ID belongs to the application, not the interactive operator.
        $mode = 'Entra'
    }
    else {
        throw 'Unsupported or ambiguous SQL authentication. Use SQL credentials (optionally Authentication=Sql Password), or Active Directory Managed Identity/MSI, Default or Interactive without a password.'
    }
    @{
        Server = $server
        Database = $database
        Mode = $mode
        Username = $user
        Password = $password
        Encrypt = $encrypt
        TrustServerCertificate = $trust -in @('true', 'yes')
    }
}

function Assert-DeploymentPrerequisites {
    foreach ($name in @('az', 'dotnet', 'dotnet-ef')) {
        if (-not (Get-Command $name -ErrorAction SilentlyContinue)) {
            throw "Required command '$name' was not found. Check installation and PATH."
        }
    }
    $command = Get-Command Invoke-Sqlcmd -ErrorAction SilentlyContinue
    foreach ($parameter in @('ServerInstance', 'Database', 'Username', 'Password', 'AccessToken', 'InputFile', 'Query', 'AbortOnError', 'DisableVariables', 'Encrypt', 'TrustServerCertificate')) {
        if (-not $command -or -not $command.Parameters.ContainsKey($parameter)) {
            throw 'Install or update the SqlServer PowerShell module (version 22 or later): Invoke-Sqlcmd must support AccessToken, AbortOnError, DisableVariables and Encrypt.'
        }
    }
}

function Get-DeploymentSqlParameters {
    param([hashtable]$Configuration)

    $parameters = @{
        ServerInstance = $Configuration.Server
        Database = $Configuration.Database
        ErrorAction = 'Stop'
        AbortOnError = $true
        DisableVariables = $true
    }
    if ($Configuration.TrustServerCertificate) {
        $parameters.TrustServerCertificate = $true
    }
    if ($Configuration.Encrypt) {
        $parameters.Encrypt = switch ($Configuration.Encrypt) {
            { $_ -in @('true', 'yes', 'mandatory') } { 'Mandatory' }
            { $_ -in @('false', 'no', 'optional') } { 'Optional' }
            'strict' { 'Strict' }
        }
    }
    if ($Configuration.Mode -eq 'Sql') {
        $parameters.Username = $Configuration.Username
        $parameters.Password = $Configuration.Password
    }
    elseif ($Configuration.Mode -eq 'Entra') {
        Write-Host "Using the Azure CLI operator's Entra identity, not the web application's Managed Identity."
        $token = Invoke-AzureCli account get-access-token --resource https://database.windows.net/ --query accessToken --output tsv
        if ([string]::IsNullOrWhiteSpace($token)) {
            throw 'Azure CLI returned an empty Azure SQL access token. Run az login.'
        }
        $parameters.AccessToken = $token
    }
    else {
        throw 'Unsupported SQL authentication mode.'
    }
    $parameters
}

function Invoke-DeploymentSql {
    param([hashtable]$SqlParameters, [string]$Query, [string]$InputFile)

    try {
        if ($InputFile) {
            Invoke-Sqlcmd @SqlParameters -InputFile $InputFile | Out-Null
        }
        else {
            Invoke-Sqlcmd @SqlParameters -Query $Query | Out-Null
        }
    }
    catch {
        throw 'SQL operation failed. Check database connectivity, authentication and permissions (db_owner or CONTROL DATABASE). For private endpoints, use a privately connected host. Database changes may already have been applied; inspect the database before retrying.'
    }
}

function Test-DeploymentSqlAccess {
    param([hashtable]$SqlParameters)

    Invoke-DeploymentSql -SqlParameters $SqlParameters -Query @"
IF COALESCE(IS_MEMBER('db_owner'), 0) <> 1 AND COALESCE(HAS_PERMS_BY_NAME(DB_NAME(), 'DATABASE', 'CONTROL'), 0) <> 1
    THROW 51000, 'The deployment operator requires db_owner or CONTROL DATABASE.', 1;
"@
}

function New-DeploymentWorkspace {
    $path = Join-Path ([System.IO.Path]::GetTempPath()) ("saas-deployment-" + [guid]::NewGuid().ToString('N'))
    New-Item -ItemType Directory -Path $path -ErrorAction Stop | Out-Null
    $path
}

function New-DeploymentPackages {
    param([string]$RepositoryRoot, [string]$Workspace)

    $solution = Join-Path $RepositoryRoot 'src/SaaSAccelerator.sln'
    Invoke-DeploymentCommand dotnet @('restore', $solution) 'NuGet restore failed. Deployment aborted before database changes' | Out-Null
    $admin = Join-Path $Workspace 'AdminSite'
    $customer = Join-Path $Workspace 'CustomerSite'
    Invoke-DeploymentCommand dotnet @('publish', (Join-Path $RepositoryRoot 'src/AdminSite/AdminSite.csproj'), '--no-restore', '-c', 'Release', '-o', $admin) 'Admin Portal build failed' | Out-Null
    Invoke-DeploymentCommand dotnet @('publish', (Join-Path $RepositoryRoot 'src/MeteredTriggerJob/MeteredTriggerJob.csproj'), '--no-restore', '-c', 'Release', '-o', (Join-Path $admin 'app_data/jobs/triggered/MeteredTriggerJob'), '--runtime', 'win-x64', '--self-contained', 'true', '-p:PublishReadyToRun=false') 'Meter Scheduler build failed' | Out-Null
    Invoke-DeploymentCommand dotnet @('publish', (Join-Path $RepositoryRoot 'src/CustomerSite/CustomerSite.csproj'), '--no-restore', '-c', 'Release', '-o', $customer) 'Customer Portal build failed' | Out-Null

    # These are generated copies only. Never publish developer-specific configuration.
    Get-ChildItem -LiteralPath $Workspace -Filter appsettings.Development.json -Recurse -File |
        Remove-Item -Force -ErrorAction Stop
}

function New-DeploymentMigration {
    param([string]$RepositoryRoot, [string]$Workspace)

    $path = Join-Path $Workspace 'script.sql'
    if (Test-Path -LiteralPath $path) {
        throw 'Migration output already exists. Use a new deployment workspace.'
    }
    $previousConnection = [Environment]::GetEnvironmentVariable('ConnectionStrings__DefaultConnection', 'Process')
    try {
        # EF script generation is offline. No real credentials or temporary appsettings file are needed.
        $env:ConnectionStrings__DefaultConnection = 'Server=localhost;Database=MigrationGeneration;Integrated Security=True;'
        Invoke-DeploymentCommand dotnet-ef @('migrations', 'script', '--idempotent', '--context', 'SaasKitContext', '--project', (Join-Path $RepositoryRoot 'src/DataAccess/DataAccess.csproj'), '--startup-project', (Join-Path $RepositoryRoot 'src/AdminSite/AdminSite.csproj'), '--configuration', 'Release', '--no-build', '--output', $path) 'EF Core migration generation failed. Database deployment aborted' | Out-Null
        if (-not (Test-Path -LiteralPath $path -PathType Leaf) -or
            [string]::IsNullOrWhiteSpace([System.IO.File]::ReadAllText($path))) {
            throw 'EF Core did not generate a new, non-empty migration script. Database deployment aborted.'
        }
    }
    finally {
        [Environment]::SetEnvironmentVariable('ConnectionStrings__DefaultConnection', $previousConnection, 'Process')
    }
    $path
}

function Compress-DeploymentPackages {
    param([string]$Workspace)

    foreach ($site in @('AdminSite', 'CustomerSite')) {
        Compress-Archive -Path (Join-Path $Workspace "$site/*") -DestinationPath (Join-Path $Workspace "$site.zip") -ErrorAction Stop
    }
}

function Publish-DeploymentPackages {
    param([string]$Workspace, [string]$ResourceGroup, [string]$AdminApp, [string]$CustomerApp)

    Invoke-AzureCli webapp deploy --resource-group $ResourceGroup --name $AdminApp --src-path (Join-Path $Workspace 'AdminSite.zip') --type zip --output none | Out-Null
    Invoke-AzureCli webapp deploy --resource-group $ResourceGroup --name $CustomerApp --src-path (Join-Path $Workspace 'CustomerSite.zip') --type zip --output none | Out-Null
}
