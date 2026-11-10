param(
    [Parameter(Mandatory)]
    [string]$Email
)

$ErrorActionPreference = 'Stop'
$emailAddress = $Email.Trim().ToLowerInvariant()
$parsedAddress = $null
if (-not [System.Net.Mail.MailAddress]::TryCreate($emailAddress, [ref]$parsedAddress)) {
    throw 'Enter a valid email address.'
}

$secretPath = Join-Path $env:APPDATA 'Microsoft/UserSecrets/decode-studio-portfolio-local/secrets.json'
$settings = Get-Content -LiteralPath $secretPath -Raw | ConvertFrom-Json
$connection = [System.Data.SqlClient.SqlConnection]::new($settings.'ConnectionStrings:Portfolio')

try {
    $connection.Open()
    $command = $connection.CreateCommand()
    $command.CommandText = @'
UPDATE dbo.Users
SET Role = N'Admin'
OUTPUT inserted.Email, inserted.Role
WHERE Email = @Email;
'@
    [void]$command.Parameters.Add('@Email', [System.Data.SqlDbType]::NVarChar, 256)
    $command.Parameters['@Email'].Value = $emailAddress

    $reader = $command.ExecuteReader()
    try {
        if (-not $reader.Read()) {
            throw 'No account matches that email. Register the account first.'
        }

        $savedEmail = $reader.GetString(0)
        $savedRole = $reader.GetString(1)
        if ($reader.Read() -or $savedRole -ne 'Admin') {
            throw 'Unexpected account promotion result.'
        }

        "Verified: $savedEmail has role $savedRole. Log in again to refresh the session."
    }
    finally { $reader.Dispose(); $command.Dispose() }
}
finally { $connection.Dispose() }
