param(
    [string]$BaseUrl = 'http://localhost:5038'
)

$ErrorActionPreference = 'Stop'

$response = Invoke-WebRequest "$BaseUrl/"
if ($response.StatusCode -ne 200) {
    throw 'The home page did not respond successfully.'
}

$markers = @('aria-label="Main navigation"', 'id="footer"')
$sectionIds = @('hero', 'about', 'services', 'projects', 'team', 'contact')
$previousPosition = -1
foreach ($sectionId in $sectionIds) {
    $position = $response.Content.IndexOf('id="' + $sectionId + '"')
    if ($position -le $previousPosition) {
        throw "Missing or out-of-order landing section: $sectionId"
    }
    $previousPosition = $position
    if ($response.Content -notmatch ('href="/?#' + $sectionId + '"')) {
        throw "Missing landing navigation anchor: $sectionId"
    }
}
if ($response.Content -match '(?i)(href|action)="/(?:admin|account|profile)(?:/|"|\?)') {
    throw 'The public page links to account routes.'
}

$handler = [System.Net.Http.HttpClientHandler]::new()
$handler.AllowAutoRedirect = $false
$client = [System.Net.Http.HttpClient]::new($handler)
try {
    foreach ($path in '/admin', '/Studio', '/Projects', '/Offerings', '/Team', '/Contact', '/Links', '/Profile') {
        $result = $client.GetAsync("$BaseUrl$path").GetAwaiter().GetResult()
        if ([int]$result.StatusCode -ne 302 -or $result.Headers.Location.OriginalString -notmatch '/admin/login\?') {
            throw "$path did not redirect to the admin sign-in."
        }
    }
    foreach ($path in '/Account/Login', '/Account/Register', '/Accounts') {
        $result = $client.GetAsync("$BaseUrl$path").GetAwaiter().GetResult()
        if ([int]$result.StatusCode -ne 404) {
            throw "$path should be unavailable on the public branch."
        }
    }
}
finally {
    $client.Dispose()
}

$login = Invoke-WebRequest "$BaseUrl/admin/login"
if ($login.Content -notmatch 'action="/admin/login"' -or $login.Content -notmatch 'name="__RequestVerificationToken"') {
    throw 'Missing admin sign-in form or antiforgery token.'
}

foreach ($marker in $markers) {
    if (-not $response.Content.Contains($marker)) {
        throw "Missing home page markup: $marker"
    }
}

$stylesheet = Invoke-WebRequest "$BaseUrl/css/site.css"
if ($stylesheet.Content -notmatch 'tailwindcss') {
    throw 'The generated Tailwind stylesheet was not served.'
}

'PASS: public landing, admin route protection, and Tailwind stylesheet.'

