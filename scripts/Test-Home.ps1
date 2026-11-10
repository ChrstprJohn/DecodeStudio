param(
    [string]$BaseUrl = 'http://localhost:5038',
    [string]$ExpectedHeroTitle
)

$ErrorActionPreference = 'Stop'

$response = Invoke-WebRequest "$BaseUrl/"
if ($response.StatusCode -ne 200) {
    throw 'The home page did not respond successfully.'
}

foreach ($marker in @('aria-label="Main navigation"', 'id="hero"', 'id="hero-title"')) {
    if (-not $response.Content.Contains($marker)) {
        throw "Missing home page markup: $marker"
    }
}

if ($ExpectedHeroTitle) {
    $encodedTitle = [System.Net.WebUtility]::HtmlEncode($ExpectedHeroTitle)
    if ($response.Content -notmatch ('id="hero-title"[^>]*>' + [regex]::Escape($encodedTitle) + '</h1>')) {
        throw 'The displayed hero title does not match the expected content.'
    }
}

$stylesheet = Invoke-WebRequest "$BaseUrl/css/site.css"
if ($stylesheet.Content -notmatch 'tailwindcss') {
    throw 'The generated Tailwind stylesheet was not served.'
}

'PASS: database-backed home page, navigation, hero, and Tailwind stylesheet.'
