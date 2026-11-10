#requires -Version 7.0
param()

# Additive local sample data. Existing matching rows and owner accounts are preserved.
$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path $PSScriptRoot -Parent
$dataPath = Join-Path $PSScriptRoot 'seed/sample-data.json'
$data = Get-Content -LiteralPath $dataPath -Raw | ConvertFrom-Json -AsHashtable
$imagesDirectory = Join-Path $projectRoot 'MyPortfolio/wwwroot/uploads/images'
[void](New-Item -ItemType Directory -Force -Path $imagesDirectory)
$demoPassword = 'DemoStudio2026!'

function Write-DemoImage([string]$imagePath, [string]$title, [string]$subtitle, [string]$background, [string]$foreground, [bool]$avatar = $false) {
    $filename = [System.IO.Path]::GetFileName($imagePath)
    if (-not $filename.StartsWith('demo-') -or -not $filename.EndsWith('.svg')) {
        throw 'Unexpected sample image filename.'
    }
    $file = Join-Path $imagesDirectory $filename
    if (Test-Path -LiteralPath $file) { return }
    $title = [System.Security.SecurityElement]::Escape($title)
    $subtitle = [System.Security.SecurityElement]::Escape($subtitle)
    if ($avatar) {
        $svg = @"
<svg xmlns="http://www.w3.org/2000/svg" width="480" height="480" viewBox="0 0 480 480">
  <rect width="480" height="480" fill="$background"/>
  <circle cx="240" cy="195" r="110" fill="$foreground" opacity="0.12"/>
  <text x="240" y="226" text-anchor="middle" font-family="Arial,sans-serif" font-size="86" fill="$foreground">$title</text>
  <text x="240" y="370" text-anchor="middle" font-family="Arial,sans-serif" font-size="23" fill="$foreground">$subtitle</text>
  <text x="240" y="420" text-anchor="middle" font-family="Arial,sans-serif" font-size="15" fill="$foreground">DEMO AVATAR</text>
</svg>
"@
    }
    else {
        $svg = @"
<svg xmlns="http://www.w3.org/2000/svg" width="1280" height="800" viewBox="0 0 1280 800">
  <rect width="1280" height="800" fill="$background"/>
  <rect x="60" y="55" width="1160" height="690" rx="10" fill="white"/>
  <text x="110" y="120" font-family="Arial,sans-serif" font-size="24" font-weight="bold" fill="$foreground">$title</text>
  <text x="1000" y="120" font-family="Arial,sans-serif" font-size="17" fill="$foreground">Work · About</text>
  <text x="110" y="275" font-family="Arial,sans-serif" font-size="52" font-weight="bold" fill="$foreground">$title</text>
  <text x="110" y="325" font-family="Arial,sans-serif" font-size="25" fill="$foreground">$subtitle</text>
  <rect x="110" y="370" width="200" height="50" fill="$foreground"/>
  <text x="143" y="403" font-family="Arial,sans-serif" font-size="18" fill="white">Explore project</text>
  <rect x="710" y="190" width="450" height="360" fill="$background"/>
  <rect x="755" y="235" width="165" height="270" fill="$foreground" opacity="0.85"/>
  <rect x="940" y="295" width="175" height="210" fill="$foreground" opacity="0.35"/>
  <text x="110" y="650" font-family="Arial,sans-serif" font-size="18" fill="$foreground">DECODE STUDIO / DEMO WEBSITE PREVIEW</text>
</svg>
"@
    }
    [System.IO.File]::WriteAllText($file, $svg)
}

Write-DemoImage $data.SiteContent.HeroImagePath 'Decode Studio' 'Design. Develop. Deploy.' '#e8ece8' '#263c32'
foreach ($item in $data.Projects) {
    Write-DemoImage $item.ImagePath ($item.Title.Replace(' (Demo)', '')) 'A sample website concept' $item.Background $item.Foreground
}
for ($index = 0; $index -lt $data.TeamMembers.Count; $index++) {
    $member = $data.TeamMembers[$index]
    $initials = $member.FirstName.Substring(0, 1) + $member.LastName.Substring(0, 1)
    $palette = $data.Projects[$index]
    Write-DemoImage $member.PhotoPath $initials ($member.FirstName + ' ' + $member.LastName) $palette.Background $palette.Foreground $true
}

# Use the same framework password hasher as the app, not custom hashing code.
$runtime = @(dotnet --list-runtimes | Where-Object { $_ -match '^Microsoft.AspNetCore.App 10\.' })[-1]
if (-not $runtime -or $runtime -notmatch '^Microsoft.AspNetCore.App (\S+) \[(.+)\]$') {
    throw 'Install the ASP.NET Core 10 runtime before seeding demo accounts.'
}
$runtimeDirectory = Join-Path $Matches[2] $Matches[1]
foreach ($assembly in @('Microsoft.Extensions.Options.dll', 'Microsoft.AspNetCore.Cryptography.KeyDerivation.dll', 'Microsoft.Extensions.Identity.Core.dll')) {
    [void][System.Reflection.Assembly]::LoadFrom((Join-Path $runtimeDirectory $assembly))
}
$hasher = [Microsoft.AspNetCore.Identity.PasswordHasher[object]]::new()
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

function Add-SampleRow([string]$table, [string[]]$keys, [System.Collections.IDictionary]$values) {
    # Identifiers come only from this script and its checked-in JSON; values are parameters.
    $columns = @($values.Keys)
    foreach ($identifier in @($table) + $columns + $keys) {
        if ($identifier -notmatch '^[A-Za-z][A-Za-z0-9]*$') { throw 'Invalid seed identifier.' }
    }
    $conditions = ($keys | ForEach-Object { "[$_] = @$_" }) -join ' AND '
    $columnList = ($columns | ForEach-Object { "[$_]" }) -join ', '
    $parameterList = ($columns | ForEach-Object { "@$_" }) -join ', '
    $sql = "IF NOT EXISTS (SELECT 1 FROM dbo.[$table] WITH (UPDLOCK, HOLDLOCK) WHERE $conditions) INSERT INTO dbo.[$table] ($columnList) VALUES ($parameterList);"
    $count = Execute $sql $values
    if ($count -gt 0) { $inserted[$table] += $count }
}

function Count-SampleRows([string]$table, [string[]]$keys, [System.Collections.IDictionary]$values) {
    $command = $connection.CreateCommand()
    $command.Transaction = $transaction
    $conditions = ($keys | ForEach-Object { "[$_] = @$_" }) -join ' AND '
    $command.CommandText = "SELECT COUNT(*) FROM dbo.[$table] WHERE $conditions"
    foreach ($key in $keys) { [void]$command.Parameters.AddWithValue('@' + $key, $values[$key]) }
    try { return [int]$command.ExecuteScalar() } finally { $command.Dispose() }
}

try {
    $connection.Open()
    if ($connection.Database -ne 'portfolio') { throw 'This sample script expects the portfolio database.' }
    $transaction = $connection.BeginTransaction([System.Data.IsolationLevel]::Serializable)
    foreach ($table in @('SiteContent', 'Projects', 'Services', 'TeamMembers', 'SocialLinks', 'Users')) { $inserted[$table] = 0 }

    Add-SampleRow 'SiteContent' @('Id') $data.SiteContent
    # Fill empty singleton fields, while keeping any existing nonempty custom copy.
    foreach ($column in $data.SiteContent.Keys) {
        if ($column -eq 'Id') { continue }
        [void](Execute "UPDATE dbo.SiteContent SET [$column] = @Value WHERE Id = 1 AND ([$column] IS NULL OR LTRIM(RTRIM([$column])) = N'');" @{ Value = $data.SiteContent[$column] })
    }
    foreach ($table in @('Projects', 'Services', 'TeamMembers', 'SocialLinks')) {
        foreach ($item in $data[$table]) {
            $values = [ordered]@{}
            foreach ($key in $item.Keys) { if ($key -notin @('Background', 'Foreground')) { $values[$key] = $item[$key] } }
            $keys = switch ($table) { 'TeamMembers' { @('FirstName', 'LastName') }; 'SocialLinks' { @('Label') }; default { @('Title') } }
            Add-SampleRow $table $keys $values
            if ((Count-SampleRows $table $keys $values) -ne 1) { throw "Sample verification failed for $table." }
        }
        if ($data[$table].Count -lt 8) { throw "Expected at least eight sample rows for $table." }
    }
    for ($index = 0; $index -lt $data.Users.Count; $index++) {
        $user = $data.Users[$index]
        $values = [ordered]@{}
        foreach ($key in $user.Keys) { $values[$key] = $user[$key] }
        $values.PasswordHash = $hasher.HashPassword([object]::new(), $demoPassword)
        $values.CreatedAtUtc = [DateTime]::UtcNow.AddDays(-(($index + 1) * 4))
        Add-SampleRow 'Users' @('Email') $values
        if ((Count-SampleRows 'Users' @('Email') $values) -ne 1) { throw 'Demo user verification failed.' }
    }
    if ($data.Users.Count -lt 8) { throw 'Expected eight demo users.' }
    foreach ($path in @($data.SiteContent.HeroImagePath) + @($data.Projects.ImagePath) + @($data.Services.ImagePath) + @($data.TeamMembers.PhotoPath)) {
        if (-not (Test-Path -LiteralPath (Join-Path $projectRoot ('MyPortfolio/wwwroot' + $path)))) {
            throw "Missing sample image: $path"
        }
    }
    $transaction.Commit()
    'Sample data verified and committed. Newly inserted rows:'
    foreach ($table in @('SiteContent', 'Projects', 'Services', 'TeamMembers', 'SocialLinks', 'Users')) { "${table}: $($inserted[$table])" }
    'Demo login: demo01@decodestudio.example through demo06@decodestudio.example'
    "Demo password: $demoPassword"
    'demo07 and demo08 are inactive examples. Existing matching records/passwords are unchanged.'
}
catch {
    if ($transaction) { $transaction.Rollback() }
    throw
}
finally {
    if ($transaction) { $transaction.Dispose() }
    $connection.Dispose()
}

