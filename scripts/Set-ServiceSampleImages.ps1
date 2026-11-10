# Backfill generated artwork for the original sample services, preserving uploaded images.
$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path $PSScriptRoot
$samples = Get-Content (Join-Path $PSScriptRoot 'seed/sample-data.json') -Raw | ConvertFrom-Json
$settings = Get-Content (Join-Path $env:APPDATA 'Microsoft/UserSecrets/decode-studio-portfolio-local/secrets.json') -Raw | ConvertFrom-Json
$connection = [System.Data.SqlClient.SqlConnection]::new($settings.'ConnectionStrings:Portfolio')
$transaction = $null
try {
    $connection.Open()
    $transaction = $connection.BeginTransaction()
    for ($index = 0; $index -lt $samples.Services.Count; $index++) {
        $service = $samples.Services[$index]
        $file = Join-Path $projectRoot ('MyPortfolio/wwwroot' + $service.ImagePath)
        if (-not (Test-Path -LiteralPath $file)) { throw "Missing image: $($service.ImagePath)" }
        $command = $connection.CreateCommand()
        $command.Transaction = $transaction
        $command.CommandText = @'
UPDATE dbo.Services SET ImagePath = @Image
WHERE Title = @Title AND
    (ImagePath IS NULL OR ImagePath = N'' OR ImagePath = @Image OR ImagePath = @OldImage);
'@
        [void]$command.Parameters.AddWithValue('@Title', $service.Title)
        [void]$command.Parameters.AddWithValue('@Image', $service.ImagePath)
        [void]$command.Parameters.AddWithValue('@OldImage', "/images/services/service-$($index + 1).svg")
        try { $count = $command.ExecuteNonQuery(); "$($service.Title): $count image path updated/verified." }
        finally { $command.Dispose() }
    }
    $transaction.Commit()
}
catch { if ($transaction) { $transaction.Rollback() }; throw }
finally { if ($transaction) { $transaction.Dispose() }; $connection.Dispose() }
