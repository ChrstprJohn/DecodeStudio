#requires -Version 7.0
param()

# Local content seed. Existing collections and all accounts are left alone.
$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path $PSScriptRoot -Parent
$dataPath = Join-Path $PSScriptRoot 'seed/sample-data.json'
$data = Get-Content -LiteralPath $dataPath -Raw | ConvertFrom-Json -AsHashtable
$secretPath = Join-Path $env:APPDATA 'Microsoft/UserSecrets/decode-studio-portfolio-local/secrets.json'
$settings = Get-Content -LiteralPath $secretPath -Raw | ConvertFrom-Json
$connection = [System.Data.SqlClient.SqlConnection]::new($settings.'ConnectionStrings:Portfolio')
$transaction = $null
$inserted = @{}

function Execute([string]$sql, [System.Collections.IDictionary]$parameters = @{}) {
    $command = $connection.CreateCommand()
    $command.Transaction = $transaction
    $command.CommandText = $sql
    foreach ($key in $parameters.Keys) {
        $value = $parameters[$key]
        if ($null -eq $value) { $value = [DBNull]::Value }
        [void]$command.Parameters.AddWithValue('@' + $key, $value)
    }
    try { return $command.ExecuteNonQuery() } finally { $command.Dispose() }
}

try {
    $connection.Open()
    if ($connection.Database -ne 'portfolio') { throw 'This sample script expects the portfolio database.' }
    $transaction = $connection.BeginTransaction([System.Data.IsolationLevel]::Serializable)
    $tables = @('SiteContent', 'Projects', 'Services', 'TeamMembers', 'SocialLinks')
    foreach ($table in $tables) {
        $inserted[$table] = 0
        $rows = @($data[$table])
        foreach ($item in $rows) {
            $columns = @($item.Keys)
            if ($columns -match '^[^A-Za-z]|[^A-Za-z0-9]') { throw "Invalid seed column for $table." }
            foreach ($path in @($item.ImagePath, $item.PhotoPath) | Where-Object { $_ -like '/images/*' }) {
                if (-not (Test-Path -LiteralPath (Join-Path $projectRoot ('MyPortfolio/wwwroot' + $path)) -PathType Leaf)) {
                    throw "Missing seed image: $path"
                }
            }
        }

        # Seed an empty collection as a unit. Preserve deliberate removals and edits.
        $command = $connection.CreateCommand()
        $command.Transaction = $transaction
        $command.CommandText = "SELECT COUNT(*) FROM dbo.[$table] WITH (UPDLOCK, HOLDLOCK);"
        try { $hasRows = $command.ExecuteScalar() -gt 0 } finally { $command.Dispose() }
        if ($hasRows) { continue }

        foreach ($item in $rows) {
            $columnList = ($item.Keys | ForEach-Object { "[$_]" }) -join ', '
            $parameterList = ($item.Keys | ForEach-Object { "@$_" }) -join ', '
            $inserted[$table] += Execute "INSERT INTO dbo.[$table] ($columnList) VALUES ($parameterList);" $item
        }
    }
    $transaction.Commit()
    'Content seed completed. Newly inserted rows:'
    foreach ($table in $tables) { "${table}: $($inserted[$table])" }
}
catch {
    if ($transaction) { $transaction.Rollback() }
    throw
}
finally {
    if ($transaction) { $transaction.Dispose() }
    $connection.Dispose()
}
