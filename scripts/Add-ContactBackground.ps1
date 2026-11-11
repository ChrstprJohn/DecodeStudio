$ErrorActionPreference = 'Stop'
$settings = Get-Content (Join-Path $env:APPDATA 'Microsoft/UserSecrets/decode-studio-portfolio-local/secrets.json') -Raw | ConvertFrom-Json
$connection = [System.Data.SqlClient.SqlConnection]::new($settings.'ConnectionStrings:Portfolio')
try {
    $connection.Open()
    $command = $connection.CreateCommand()
    try {
        $command.CommandText = Get-Content (Join-Path $PSScriptRoot 'database/add-contact-background.sql') -Raw
        [void]$command.ExecuteNonQuery()
        $command.CommandText = "SELECT COL_LENGTH('dbo.SiteContent', 'ContactBackgroundImagePath')"
        if ($command.ExecuteScalar() -ne 1000) { throw 'Contact background column was not verified.' }
        'Verified: SiteContent.ContactBackgroundImagePath is available.'
    }
    finally { $command.Dispose() }
}
finally { $connection.Dispose() }
