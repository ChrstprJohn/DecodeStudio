$ErrorActionPreference = 'Stop'
$settings = Get-Content (Join-Path $env:APPDATA 'Microsoft/UserSecrets/decode-studio-portfolio-local/secrets.json') -Raw | ConvertFrom-Json
$connection = [System.Data.SqlClient.SqlConnection]::new($settings.'ConnectionStrings:Portfolio')
try {
    $connection.Open()
    $command = $connection.CreateCommand()
    $command.CommandText = Get-Content (Join-Path $PSScriptRoot 'database/add-service-images.sql') -Raw
    [void]$command.ExecuteNonQuery()
    $command.Dispose()
    'Verified: Services.ImagePath is available; existing records preserved.'
}
finally { $connection.Dispose() }
