param(
    [string]$BaseUrl = 'http://localhost:5038'
)

$ErrorActionPreference = 'Stop'

$response = Invoke-WebRequest "$BaseUrl/"
if ($response.StatusCode -ne 200) {
    throw 'The home page did not respond successfully.'
}

$markers = @('aria-label="Main navigation"', 'id="footer"')
if ($response.Content.Contains('id="hero"')) {
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
}
else {
    $markers += @('action="/Account/Login"', 'name="Email"', 'name="Password"')
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

'PASS: landing or school-mode login page, shared layout, and Tailwind stylesheet.'

