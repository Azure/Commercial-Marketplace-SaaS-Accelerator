BeforeAll {
    $deployment = Split-Path $PSScriptRoot -Parent
    . (Join-Path $deployment 'DeploymentHelpers.ps1')

    # Stubs make it impossible for these tests to contact Azure or SQL.
    function az { throw 'Unmocked Azure CLI call' }
    function dotnet { throw 'Unmocked dotnet call' }
    function dotnet-ef { throw 'Unmocked EF call' }
    function git { throw 'Unmocked git call' }
    function Get-AzTenant { }
    function Get-AzSubscription { param($TenantId) }
    function Get-AzSqlServer { param($ServerName, $ResourceGroupName) }
    function Invoke-Sqlcmd {
        [CmdletBinding()]
        param($ServerInstance, $Database, $Username, $Password, $AccessToken, $InputFile,
            $Query, [switch]$AbortOnError, [switch]$DisableVariables, $Encrypt, [switch]$TrustServerCertificate)
        throw 'Unmocked SQL call'
    }
}

Describe 'SQL connection string parsing and authentication' {
    It 'supports <ServerKey> and <DatabaseKey> aliases and quoted special characters' -ForEach @(
        @{ ServerKey = 'Server'; DatabaseKey = 'Database' }
        @{ ServerKey = 'Data Source'; DatabaseKey = 'Initial Catalog' }
        @{ ServerKey = 'Address'; DatabaseKey = 'Database' }
        @{ ServerKey = 'Addr'; DatabaseKey = 'Database' }
        @{ ServerKey = 'Network Address'; DatabaseKey = 'Database' }
    ) {
        $config = Get-DeploymentSqlConfiguration "$ServerKey=tcp:example.database.windows.net,1433;$DatabaseKey=example;UID=operator;PWD='test;pass`"word';"
        $config.Server | Should -Be 'tcp:example.database.windows.net,1433'
        $config.Database | Should -Be 'example'
        $config.Mode | Should -Be 'Sql'
        $config.Password | Should -Be 'test;pass"word'
    }

    It 'selects SQL credentials with explicit SQL Password authentication' {
        $config = Get-DeploymentSqlConfiguration 'Server=example;Database=example;User ID=operator;Password=test-password;Authentication=Sql Password;Encrypt=True;'
        $parameters = Get-DeploymentSqlParameters $config
        $parameters.Username | Should -Be 'operator'
        $parameters.Password | Should -Be 'test-password'
        $parameters.ContainsKey('AccessToken') | Should -BeFalse
        $parameters.Encrypt | Should -Be 'Mandatory'
        $parameters.AbortOnError | Should -BeTrue
        $parameters.ErrorAction | Should -Be 'Stop'
    }

    It 'selects the CLI operator for <Authentication>' -ForEach @(
        @{ Authentication = 'Active Directory Managed Identity' }
        @{ Authentication = 'Active Directory MSI' }
        @{ Authentication = 'Active Directory Default' }
        @{ Authentication = 'Active Directory Interactive' }
    ) {
        Mock az { $global:LASTEXITCODE = 0; 'test-token' }
        $config = Get-DeploymentSqlConfiguration "Server=example;Database=example;Authentication=$Authentication;"
        $parameters = Get-DeploymentSqlParameters $config
        $parameters.AccessToken | Should -Be 'test-token'
        $parameters.ContainsKey('Password') | Should -BeFalse
        Should -Invoke az -Times 1 -Exactly -ParameterFilter {
            $args -contains 'https://database.windows.net/'
        }
    }

    It 'does not use a managed identity client ID as the SQL operator' {
        $config = Get-DeploymentSqlConfiguration 'Server=example;Database=example;Authentication=Active Directory Managed Identity;UID=application-client-id;'
        $config.Mode | Should -Be 'Entra'
    }

    It 'rejects invalid or ambiguous configuration without disclosing it: <Name>' -ForEach @(
        @{ Name = 'missing database'; Value = 'Server=example;UID=operator;PWD=test-password' }
        @{ Name = 'missing server'; Value = 'Database=example;UID=operator;PWD=test-password' }
        @{ Name = 'missing authentication'; Value = 'Server=example;Database=example' }
        @{ Name = 'missing password'; Value = 'Server=example;Database=example;UID=operator' }
        @{ Name = 'password without username'; Value = 'Server=example;Database=example;PWD=test-password' }
        @{ Name = 'Entra password'; Value = 'Server=example;Database=example;Authentication=Active Directory Password;UID=operator;PWD=test-password' }
        @{ Name = 'Entra with SQL credentials'; Value = 'Server=example;Database=example;Authentication=Active Directory Default;UID=operator;PWD=test-password' }
        @{ Name = 'unknown mode'; Value = 'Server=example;Database=example;Authentication=Unknown;UID=operator;PWD=test-password' }
        @{ Name = 'blank explicit mode'; Value = 'Server=example;Database=example;Authentication=" ";UID=operator;PWD=test-password' }
        @{ Name = 'Windows authentication'; Value = 'Server=example;Database=example;Integrated Security=SSPI' }
        @{ Name = 'conflicting aliases'; Value = 'Server=example;Data Source=other;Database=example;UID=operator;PWD=test-password' }
        @{ Name = 'invalid syntax'; Value = 'Server="unterminated;PWD=test-password' }
        @{ Name = 'invalid encryption'; Value = 'Server=example;Database=example;UID=operator;PWD=test-password;Encrypt=invalid' }
    ) {
        { Get-DeploymentSqlConfiguration $Value } | Should -Throw
        try { Get-DeploymentSqlConfiguration $Value } catch {
            $_.Exception.Message | Should -Not -Match 'test-password'
        }
    }
}

Describe 'Shared command and prerequisite guards' {
    It 'detects actual native process failures and suppresses sensitive diagnostics' {
        $executable = (Get-Process -Id $PID).Path
        { Invoke-DeploymentCommand $executable @('-NoProfile', '-Command', '[Console]::Error.WriteLine("test-password"); exit 7') 'Test operation failed' } |
            Should -Throw '*Test operation failed (exit code 7)*'
    }

    It 'rejects a SqlServer module without access token support' {
        Mock Get-Command {
            [pscustomobject]@{ Parameters = @{ ServerInstance = $null; Database = $null; Username = $null; Password = $null } }
        } -ParameterFilter { $Name -eq 'Invoke-Sqlcmd' }
        { Assert-DeploymentPrerequisites } | Should -Throw '*SqlServer PowerShell module*'
    }

    It 'rejects empty SQL tokens' {
        Mock az { $global:LASTEXITCODE = 0; '' }
        { Get-DeploymentSqlParameters @{ Mode = 'Entra'; Server = 'example'; Database = 'example' } } |
            Should -Throw '*empty Azure SQL access token*'
    }

    It 'rejects a mismatched subscription or tenant' {
        Mock az {
            $global:LASTEXITCODE = 0
            if ($args[1] -eq 'show') { '{"id":"other-subscription","tenantId":"test-tenant"}' }
        }
        { Set-DeploymentSubscription 'test-subscription' 'test-tenant' } | Should -Throw '*verification failed*'
    }

    It 'refuses a stale migration script without invoking EF' {
        $workspace = Join-Path $TestDrive 'stale-migration'
        New-Item -ItemType Directory $workspace | Out-Null
        Set-Content (Join-Path $workspace 'script.sql') 'stale'
        Mock dotnet-ef { throw 'Must not run' }
        { New-DeploymentMigration -RepositoryRoot $TestDrive -Workspace $workspace } | Should -Throw '*already exists*'
        Should -Invoke dotnet-ef -Times 0 -Exactly
    }
}

Describe '<EntryPoint> orchestration' -ForEach @(
    @{ EntryPoint = 'Upgrade.ps1' }
    @{ EntryPoint = 'Deploy.ps1' }
) {
    BeforeEach {
        $script:fixture = Join-Path $TestDrive ([guid]::NewGuid().ToString('N'))
        $fixtureDeployment = Join-Path $script:fixture 'deployment'
        New-Item -ItemType Directory -Path $fixtureDeployment -Force | Out-Null
        Copy-Item (Join-Path $deployment '*.ps1') $fixtureDeployment
        $script:configPath = Join-Path $script:fixture 'src/AdminSite/appsettings.Development.json'
        New-Item -ItemType Directory -Path (Split-Path $script:configPath) -Force | Out-Null
        $script:originalConfig = '{"ConnectionStrings":{"DefaultConnection":"original-test-setting"}}'
        Set-Content -LiteralPath $script:configPath -Value $script:originalConfig -NoNewline
        $script:legacySql = Join-Path $fixtureDeployment 'script.sql'
        Set-Content $script:legacySql 'pre-existing SQL'
        $script:legacyPublish = Join-Path $script:fixture 'Publish'
        New-Item -ItemType Directory $script:legacyPublish | Out-Null
        $global:SaaSDeploymentTestState = @{
            Failure = ''
            Workspace = $null
            Events = [System.Collections.Generic.List[string]]::new()
            CliCalls = [System.Collections.Generic.List[string]]::new()
            Answers = [System.Collections.Generic.Queue[string]]::new()
            ConnectionString = 'Server=example.database.windows.net;Database=example;Authentication=Active Directory Managed Identity;'
            PackagesChecked = 0
        }
        foreach ($answer in @('Y', '', '', 'N')) { $global:SaaSDeploymentTestState.Answers.Enqueue($answer) }
        $script:oldConnection = [Environment]::GetEnvironmentVariable('ConnectionStrings__DefaultConnection', 'Process')
        $env:ConnectionStrings__DefaultConnection = 'original-test-environment'

        Mock Read-Host { $global:SaaSDeploymentTestState.Answers.Dequeue() }
        Mock Write-Host { }
        Mock Write-Warning { }
        Mock Get-AzTenant { }
        Mock Get-AzSubscription { }
        Mock Get-AzSqlServer { }
        Mock git { $global:LASTEXITCODE = 0; 'test-source-revision' }
        Mock New-Item {
            $global:SaaSDeploymentTestState.Workspace = $Path
            [System.IO.Directory]::CreateDirectory($Path)
        } -ParameterFilter { $Path -like '*saas-deployment-*' }
        Mock Invoke-WebRequest { [pscustomobject]@{ Content = '192.0.2.1' } }
        Mock az {
            $global:LASTEXITCODE = 0
            $call = $args -join ' '
            $global:SaaSDeploymentTestState.CliCalls.Add($call)
            if (($global:SaaSDeploymentTestState.Failure -eq 'subscription' -and $call -like 'account set *') -or
                ($global:SaaSDeploymentTestState.Failure -eq 'keyvault' -and $call -like 'keyvault secret show *') -or
                ($global:SaaSDeploymentTestState.Failure -eq 'token' -and $call -like 'account get-access-token *') -or
                ($global:SaaSDeploymentTestState.Failure -eq 'deployment' -and $call -like 'webapp deploy *') -or
                ($global:SaaSDeploymentTestState.Failure -eq 'customer-deployment' -and $call -like 'webapp deploy *--name example-portal *') -or
                ($global:SaaSDeploymentTestState.Failure -eq 'firewall-create' -and $call -like 'sql server firewall-rule create *') -or
                ($global:SaaSDeploymentTestState.Failure -eq 'firewall-delete' -and $call -like 'sql server firewall-rule delete *')) {
                $global:LASTEXITCODE = 1
                'sensitive-test-diagnostic'
                return
            }
            switch -Wildcard ($call) {
                'account show *' { '{"id":"test-subscription","tenantId":"test-tenant"}' }
                'keyvault secret show *' { $global:SaaSDeploymentTestState.ConnectionString }
                'account get-access-token *' { 'test-token' }
                'webapp deploy *' {
                    $package = $args[[array]::IndexOf($args, '--src-path') + 1]
                    $zip = [System.IO.Compression.ZipFile]::OpenRead($package)
                    try {
                        @($zip.Entries | Where-Object { $_.Name -eq 'appsettings.Development.json' }).Count | Should -Be 0
                        @($zip.Entries | Where-Object { $_.Name -eq 'application.txt' }).Count | Should -BeGreaterThan 0
                    }
                    finally { $zip.Dispose() }
                    $global:SaaSDeploymentTestState.PackagesChecked++
                    $global:SaaSDeploymentTestState.Events.Add('deploy')
                }
                'keyvault list *' { '[]' }
                'rest *' { '{"nameAvailable":true}' }
                'ad signed-in-user *' { 'test-user' }
            }
        }
        Mock dotnet {
            $global:LASTEXITCODE = 0
            if ($args[0] -eq '--version') { '10.0.400'; return }
            $global:SaaSDeploymentTestState.Events.Add($args[0])
            if (($global:SaaSDeploymentTestState.Failure -eq 'restore' -and $args[0] -eq 'restore') -or
                ($global:SaaSDeploymentTestState.Failure -eq 'build' -and $args[0] -eq 'publish') -or
                ($global:SaaSDeploymentTestState.Failure -eq 'scheduler' -and ($args -join ' ') -like '*MeteredTriggerJob.csproj*') -or
                ($global:SaaSDeploymentTestState.Failure -eq 'customer' -and ($args -join ' ') -like '*CustomerSite.csproj*')) {
                $global:LASTEXITCODE = 1
                return
            }
            if ($args[0] -eq 'publish') {
                $args | Should -Contain '--no-restore'
                $output = $args[[array]::IndexOf($args, '-o') + 1]
                [System.IO.Directory]::CreateDirectory($output) | Out-Null
                [System.IO.File]::WriteAllText((Join-Path $output 'application.txt'), 'test-package')
                [System.IO.File]::WriteAllText((Join-Path $output 'appsettings.Development.json'), 'do-not-publish')
            }
        }
        Mock dotnet-ef {
            $global:LASTEXITCODE = 0
            $global:SaaSDeploymentTestState.Events.Add('migration')
            $args | Should -Contain '--idempotent'
            $args | Should -Contain 'SaasKitContext'
            $args | Should -Contain '--no-build'
            ($args -join ' ') | Should -Match 'DataAccess.csproj'
            ($args -join ' ') | Should -Match 'AdminSite.csproj'
            $env:ConnectionStrings__DefaultConnection | Should -Be 'Server=localhost;Database=MigrationGeneration;Integrated Security=True;'
            if ($global:SaaSDeploymentTestState.Failure -eq 'ef') { $global:LASTEXITCODE = 1; return }
            if ($global:SaaSDeploymentTestState.Failure -eq 'missing-script') { return }
            $output = $args[[array]::IndexOf($args, '--output') + 1]
            $contents = if ($global:SaaSDeploymentTestState.Failure -eq 'empty-script') { '' } elseif ($global:SaaSDeploymentTestState.Failure -eq 'blank-script') { '   ' } else { 'SELECT 1;' }
            [System.IO.File]::WriteAllText($output, $contents)
        }
        Mock Invoke-Sqlcmd {
            if ($global:SaaSDeploymentTestState.Failure -eq 'sql' -or
                ($global:SaaSDeploymentTestState.Failure -eq 'migration-sql' -and $InputFile)) {
                throw 'sensitive-test-diagnostic'
            }
            if ($InputFile) {
                $InputFile | Should -Be (Join-Path $global:SaaSDeploymentTestState.Workspace 'script.sql')
                $global:SaaSDeploymentTestState.Events.Add('apply')
            }
            else { $global:SaaSDeploymentTestState.Events.Add('sql-query') }
        }
        $script:entryPath = Join-Path $fixtureDeployment $EntryPoint
        $script:entryArguments = @{ WebAppNamePrefix = 'example'; ResourceGroupForDeployment = 'example' }
        if ($EntryPoint -eq 'Deploy.ps1') {
            $script:entryArguments += @{
                Location = 'example-region'; PublisherAdminUsers = 'operator@example.invalid'
                TenantID = 'test-tenant'; AzureSubscriptionID = 'test-subscription'
                ADApplicationID = 'test-application'; ADApplicationSecret = 'test-secret'
                ADApplicationIDAdmin = 'test-admin'; ADMTApplicationIDPortal = 'test-portal'
                KeyVault = 'example-kv'
            }
        }
    }

    AfterEach {
        $env:ConnectionStrings__DefaultConnection | Should -Be 'original-test-environment'
        [Environment]::SetEnvironmentVariable('ConnectionStrings__DefaultConnection', $script:oldConnection, 'Process')
        if ($global:SaaSDeploymentTestState.Workspace) { Test-Path $global:SaaSDeploymentTestState.Workspace | Should -BeFalse }
        if ($script:originalConfig) {
            [System.IO.File]::ReadAllText($script:configPath) | Should -BeExactly $script:originalConfig
        }
        else {
            Test-Path $script:configPath | Should -BeFalse
        }
        (Get-Content $script:legacySql).Trim() | Should -Be 'pre-existing SQL'
        Test-Path $script:legacyPublish | Should -BeTrue
        Should -Invoke Write-Host -Times 0 -Exactly -ParameterFilter {
            ($Object -join ' ') -match 'test-secret|test-token|test;password|original-test-setting'
        }
        Remove-Variable SaaSDeploymentTestState -Scope Global
    }

    It 'restores once, builds three components, then generates and applies SQL before deploying' {
        & $script:entryPath @script:entryArguments
        ($global:SaaSDeploymentTestState.Events -join ',') | Should -BeLike 'restore,publish,publish,publish,migration,sql-query,*apply,*deploy,deploy'
        @($global:SaaSDeploymentTestState.Events | Where-Object { $_ -eq 'restore' }).Count | Should -Be 1
        $global:SaaSDeploymentTestState.PackagesChecked | Should -Be 2
        $env:ConnectionStrings__DefaultConnection | Should -Be 'original-test-environment'
        if ($EntryPoint -eq 'Upgrade.ps1') {
            ($global:SaaSDeploymentTestState.CliCalls -join "`n") | Should -Not -Match 'firewall-rule'
        }
        else {
            ($global:SaaSDeploymentTestState.CliCalls -join "`n") | Should -Match 'firewall-rule delete'
            ($global:SaaSDeploymentTestState.CliCalls -join "`n") | Should -Not -Match 'start-ip-address 0\.0\.0\.0'
        }
    }

    It 'stops before SQL on <Failure> failure' -ForEach @(
        @{ Failure = 'subscription'; Message = '*Azure CLI operation failed*' }
        @{ Failure = 'restore'; Message = '*NuGet restore failed*' }
        @{ Failure = 'build'; Message = '*Admin Portal build failed*' }
        @{ Failure = 'scheduler'; Message = '*Meter Scheduler build failed*' }
        @{ Failure = 'customer'; Message = '*Customer Portal build failed*' }
        @{ Failure = 'ef'; Message = '*EF Core migration generation failed*' }
        @{ Failure = 'missing-script'; Message = '*non-empty migration script*' }
        @{ Failure = 'empty-script'; Message = '*non-empty migration script*' }
        @{ Failure = 'blank-script'; Message = '*non-empty migration script*' }
        @{ Failure = 'token'; Message = '*Azure CLI operation failed*' }
    ) {
        $global:SaaSDeploymentTestState.Failure = $Failure
        { & $script:entryPath @script:entryArguments } | Should -Throw $Message
        Should -Invoke Invoke-Sqlcmd -Times 0 -Exactly
        @($global:SaaSDeploymentTestState.Events | Where-Object { $_ -eq 'deploy' }).Count | Should -Be 0
    }

    It 'stops without a success message or diagnostic disclosure on failed deployment' {
        $global:SaaSDeploymentTestState.Failure = 'deployment'
        try {
            & $script:entryPath @script:entryArguments
            throw 'Expected deployment to fail'
        }
        catch {
            $_.Exception.Message | Should -BeLike '*Azure CLI operation failed*'
            $_.Exception.Message | Should -Not -Match 'sensitive-test-diagnostic|test-secret|test-token'
        }
        Should -Invoke Write-Host -Times 0 -Exactly -ParameterFilter { ($Object -join ' ') -match 'completed successfully' }
    }

    It 'aborts on SQL failure without exposing the original error' {
        $global:SaaSDeploymentTestState.Failure = 'sql'
        { & $script:entryPath @script:entryArguments } | Should -Throw '*SQL operation failed*'
        @($global:SaaSDeploymentTestState.Events | Where-Object { $_ -eq 'deploy' }).Count | Should -Be 0
    }

    It 'does not deploy when migration execution fails' {
        $global:SaaSDeploymentTestState.Failure = 'migration-sql'
        { & $script:entryPath @script:entryArguments } | Should -Throw '*SQL operation failed*'
        Should -Invoke Invoke-Sqlcmd -Times 1 -Exactly -ParameterFilter { [bool]$InputFile }
        @($global:SaaSDeploymentTestState.Events | Where-Object { $_ -eq 'deploy' }).Count | Should -Be 0
    }

    It 'reports failure if the second application deployment fails' {
        $global:SaaSDeploymentTestState.Failure = 'customer-deployment'
        { & $script:entryPath @script:entryArguments } | Should -Throw '*Azure CLI operation failed*'
        @($global:SaaSDeploymentTestState.Events | Where-Object { $_ -eq 'deploy' }).Count | Should -Be 1
        Should -Invoke Write-Host -Times 0 -Exactly -ParameterFilter { ($Object -join ' ') -match 'completed successfully' }
    }

    It 'does not create development settings when none existed' {
        Remove-Item -LiteralPath $script:configPath
        $script:originalConfig = $null
        & $script:entryPath @script:entryArguments
        Test-Path $script:configPath | Should -BeFalse
    }

    It 'handles private upgrades without creating public firewall rules' -Skip:($EntryPoint -ne 'Upgrade.ps1') {
        $global:SaaSDeploymentTestState.Answers.Clear()
        foreach ($answer in @('Y', '', '', 'Y')) { $global:SaaSDeploymentTestState.Answers.Enqueue($answer) }
        & $script:entryPath @script:entryArguments
        ($global:SaaSDeploymentTestState.CliCalls -join "`n") | Should -Not -Match 'firewall-rule|public-network-access'
        $global:SaaSDeploymentTestState.Events | Should -Contain 'apply'
    }

    It 'accepts explicit subscription and UsePrivateEndpoints=<Private> without those prompts' -Skip:($EntryPoint -ne 'Upgrade.ps1') -ForEach @(
        @{ Private = $true }
        @{ Private = $false }
    ) {
        $global:SaaSDeploymentTestState.Answers.Clear()
        foreach ($answer in @('Y', '')) { $global:SaaSDeploymentTestState.Answers.Enqueue($answer) }
        & $script:entryPath @script:entryArguments -AzureSubscriptionID 'test-subscription' -UsePrivateEndpoints $Private
        $global:SaaSDeploymentTestState.Answers.Count | Should -Be 0
        Should -Invoke Read-Host -Times 0 -Exactly -ParameterFilter {
            $Prompt -like '*SubscriptionID*' -or $Prompt -like '*private endpoints*'
        }
        ($global:SaaSDeploymentTestState.CliCalls -join "`n") | Should -Match 'account set --subscription test-subscription'
        ($global:SaaSDeploymentTestState.CliCalls -join "`n") | Should -Not -Match 'firewall-rule|public-network-access'
        $global:SaaSDeploymentTestState.Events | Should -Contain 'apply'
        $expectedWarnings = if ($Private) { 1 } else { 0 }
        Should -Invoke Write-Warning -Times $expectedWarnings -Exactly -ParameterFilter { $Message -like 'Private endpoints require*' }
    }

    It 'uses the existing SQL credential flow for upgrades' -Skip:($EntryPoint -ne 'Upgrade.ps1') {
        $global:SaaSDeploymentTestState.ConnectionString = 'Address=example;Initial Catalog=example;UID=operator;PWD="test;password";'
        & $script:entryPath @script:entryArguments
        Should -Invoke Invoke-Sqlcmd -Times 3 -Exactly -ParameterFilter {
            $Username -eq 'operator' -and $Password -eq 'test;password' -and -not $AccessToken -and $AbortOnError -and $DisableVariables
        }
        ($global:SaaSDeploymentTestState.CliCalls -join "`n") | Should -Not -Match 'get-access-token'
    }

    It 'stops on failed Key Vault retrieval before any SQL' -Skip:($EntryPoint -ne 'Upgrade.ps1') {
        $global:SaaSDeploymentTestState.Failure = 'keyvault'
        { & $script:entryPath @script:entryArguments } | Should -Throw '*Azure CLI operation failed*'
        Should -Invoke Invoke-Sqlcmd -Times 0 -Exactly
    }

    It 'stops on empty Key Vault retrieval before any SQL' -Skip:($EntryPoint -ne 'Upgrade.ps1') {
        $global:SaaSDeploymentTestState.ConnectionString = ''
        { & $script:entryPath @script:entryArguments } | Should -Throw '*empty DefaultConnection*'
        Should -Invoke Invoke-Sqlcmd -Times 0 -Exactly
    }

    It 'cleans up an attempted firewall creation on failure' -Skip:($EntryPoint -ne 'Deploy.ps1') {
        $global:SaaSDeploymentTestState.Failure = 'firewall-create'
        { & $script:entryPath @script:entryArguments } | Should -Throw '*Azure CLI operation failed*'
        ($global:SaaSDeploymentTestState.CliCalls -join "`n") | Should -Match 'firewall-rule delete'
        Should -Invoke Invoke-Sqlcmd -Times 0 -Exactly
    }

    It 'reports firewall cleanup failure and still cleans local files' -Skip:($EntryPoint -ne 'Deploy.ps1') {
        $global:SaaSDeploymentTestState.Failure = 'firewall-delete'
        { & $script:entryPath @script:entryArguments } | Should -Throw '*Azure CLI operation failed*'
        ($global:SaaSDeploymentTestState.CliCalls -join "`n") | Should -Match 'firewall-rule delete'
        Should -Invoke Write-Host -Times 0 -Exactly -ParameterFilter { ($Object -join ' ') -match 'completed successfully' }
    }
}

Describe 'PowerShell syntax' {
    It 'parses <Name>' -ForEach @(
        @{ Name = 'Deploy.ps1' }; @{ Name = 'Upgrade.ps1' }; @{ Name = 'DeploymentHelpers.ps1' }
    ) {
        $tokens = $null
        $parseErrors = $null
        [System.Management.Automation.Language.Parser]::ParseFile((Join-Path $deployment $Name), [ref]$tokens, [ref]$parseErrors) | Out-Null
        $parseErrors | Should -BeNullOrEmpty
    }
}
