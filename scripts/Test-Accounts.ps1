param(
    [string]$BaseUrl = 'http://localhost:5041'
)

# Creates one uniquely named test account and removes only that account afterward.
$ErrorActionPreference = 'Stop'
$secretPath = Join-Path $env:APPDATA 'Microsoft/UserSecrets/decode-studio-portfolio-local/secrets.json'
$settings = Get-Content $secretPath -Raw | ConvertFrom-Json
$connection = [System.Data.SqlClient.SqlConnection]::new($settings.'ConnectionStrings:Portfolio')
$handler = [System.Net.Http.HttpClientHandler]::new()
$handler.AllowAutoRedirect = $false
$client = [System.Net.Http.HttpClient]::new($handler)
$email = 'auth-test-' + [guid]::NewGuid().ToString('N') + '@example.test'
$password = 'Test-' + [guid]::NewGuid().ToString('N')

function Assert-True($condition, [string]$message) {
    if (-not $condition) { throw $message }
}

function Get-Page([string]$path) {
    return $client.GetAsync($BaseUrl + $path).GetAwaiter().GetResult()
}

function Get-Token([string]$path) {
    $page = Get-Page $path
    try {
        Assert-True ($page.StatusCode -eq 200) "Could not open $path"
        $html = $page.Content.ReadAsStringAsync().GetAwaiter().GetResult()
        $inputMatch = [regex]::Match($html, '<input[^>]*name="__RequestVerificationToken"[^>]*>')
        $valueMatch = [regex]::Match($inputMatch.Value, 'value="([^"]+)"')
        Assert-True $valueMatch.Success "Missing anti-forgery token on $path"
        return [System.Net.WebUtility]::HtmlDecode($valueMatch.Groups[1].Value)
    }
    finally { $page.Dispose() }
}

function Post-Form([string]$path, [hashtable]$fields) {
    $dictionary = [System.Collections.Generic.Dictionary[string,string]]::new()
    foreach ($key in $fields.Keys) { $dictionary.Add($key, [string]$fields[$key]) }
    $body = [System.Net.Http.FormUrlEncodedContent]::new($dictionary)
    try { return $client.PostAsync($BaseUrl + $path, $body).GetAwaiter().GetResult() }
    finally { $body.Dispose() }
}

function Read-TestUser {
    $command = $connection.CreateCommand()
    $command.CommandText = 'SELECT Id, FirstName, MiddleName, LastName, Email, PasswordHash, Role, IsActive, CreatedAtUtc FROM dbo.Users WHERE Email = @Email'
    [void]$command.Parameters.AddWithValue('@Email', $email)
    $table = [System.Data.DataTable]::new()
    $reader = $command.ExecuteReader()
    try { $table.Load($reader) } finally { $reader.Dispose(); $command.Dispose() }
    return ,$table
}

function Set-TestAccount([bool]$active, [string]$role) {
    $command = $connection.CreateCommand()
    $command.CommandText = 'UPDATE dbo.Users SET IsActive = @Active, Role = @Role WHERE Email = @Email'
    [void]$command.Parameters.AddWithValue('@Active', $active)
    [void]$command.Parameters.AddWithValue('@Role', $role)
    [void]$command.Parameters.AddWithValue('@Email', $email)
    try { Assert-True ($command.ExecuteNonQuery() -eq 1) 'Test account update failed' }
    finally { $command.Dispose() }
}

try {
    $connection.Open()
    $response = Get-Page '/'
    Assert-True ($response.StatusCode -eq 302 -and $response.Headers.Location.OriginalString.Contains('/Account/Login')) 'Anonymous home should require login in school mode'
    $response.Dispose()

    $response = Get-Page '/css/site.css'
    Assert-True ($response.StatusCode -eq 200) 'Login styles must be available anonymously'
    $response.Dispose()

    $response = Post-Form '/Account/Register' @{ FirstName = 'Test' }
    Assert-True ($response.StatusCode -eq 400) 'Missing anti-forgery token should be rejected'
    $response.Dispose()

    $registration = @{
        FirstName = 'Test'; MiddleName = ''; LastName = 'Account'; Email = $email
        Password = $password; ConfirmPassword = 'does-not-match'; Role = 'Admin'
        __RequestVerificationToken = (Get-Token '/Account/Register')
    }
    $response = Post-Form '/Account/Register' $registration
    Assert-True ($response.StatusCode -eq 200 -and (Read-TestUser).Rows.Count -eq 0) 'Invalid registration should not save a user'
    $response.Dispose()

    $registration.ConfirmPassword = $password
    $registration.__RequestVerificationToken = Get-Token '/Account/Register'
    $response = Post-Form '/Account/Register' $registration
    Assert-True ($response.StatusCode -eq 302) 'Registration should redirect to login'
    $response.Dispose()
    $userTable = Read-TestUser
    Assert-True ($userTable.Rows.Count -eq 1) 'Expected one saved test account'
    $user = $userTable.Rows[0]
    Assert-True ($user.Role -eq 'User' -and $user.IsActive) 'Registration must not accept an Admin role from the form'
    Assert-True ($user.FirstName -eq 'Test' -and $user.LastName -eq 'Account') 'Name fields were not saved'
    Assert-True ($user.PasswordHash -ne $password -and $user.PasswordHash.Length -gt 50) 'Password must be hashed'

    $registration.Email = $email.ToUpperInvariant()
    $registration.__RequestVerificationToken = Get-Token '/Account/Register'
    $response = Post-Form '/Account/Register' $registration
    $html = $response.Content.ReadAsStringAsync().GetAwaiter().GetResult()
    Assert-True ($response.StatusCode -eq 200 -and $html.Contains('already registered') -and (Read-TestUser).Rows.Count -eq 1) 'Duplicate email should be rejected'
    $response.Dispose()

    $login = @{ Email = $email; Password = 'incorrect-password'; __RequestVerificationToken = (Get-Token '/Account/Login') }
    $response = Post-Form '/Account/Login' $login
    $html = $response.Content.ReadAsStringAsync().GetAwaiter().GetResult()
    Assert-True ($response.StatusCode -eq 200 -and $html.Contains('Invalid email or password')) 'Wrong password should fail'
    $response.Dispose()

    $login.Password = $password
    $login.__RequestVerificationToken = Get-Token '/Account/Login'
    $response = Post-Form '/Account/Login' $login
    Assert-True ($response.StatusCode -eq 302) 'Correct password should log in'
    $response.Dispose()
    $response = Get-Page '/'
    Assert-True ($response.StatusCode -eq 200) 'Signed-in user should reach home'
    $response.Dispose()

    $response = Get-Page '/Account/Logout'
    Assert-True ([int]$response.StatusCode -in @(404, 405)) 'Logout must not accept GET'
    $response.Dispose()
    $response = Post-Form '/Account/Logout' @{ __RequestVerificationToken = (Get-Token '/Account/Login') }
    Assert-True ($response.StatusCode -eq 302) 'Logout should redirect'
    $response.Dispose()
    $response = Get-Page '/'
    Assert-True ($response.StatusCode -eq 302) 'Logged-out user must lose home access'
    $response.Dispose()

    $login.__RequestVerificationToken = Get-Token '/Account/Login'
    $response = Post-Form '/Account/Login' $login
    Assert-True ($response.StatusCode -eq 302) 'Second login should succeed'
    $response.Dispose()
    Set-TestAccount $false 'User'
    $response = Get-Page '/'
    Assert-True ($response.StatusCode -eq 302) 'Deactivation must invalidate an existing session'
    $response.Dispose()
    $login.__RequestVerificationToken = Get-Token '/Account/Login'
    $response = Post-Form '/Account/Login' $login
    Assert-True ($response.StatusCode -eq 200) 'Inactive account must not log in'
    $response.Dispose()

    Set-TestAccount $true 'User'
    $login.__RequestVerificationToken = Get-Token '/Account/Login'
    $response = Post-Form '/Account/Login' $login
    Assert-True ($response.StatusCode -eq 302) 'Reactivated account should log in'
    $response.Dispose()
    Set-TestAccount $true 'Admin'
    $response = Get-Page '/'
    Assert-True ($response.StatusCode -eq 302) 'Role changes should invalidate the old session'
    $response.Dispose()

    'PASS: registration, validation, duplicate email, forced User role, password hashing, login/logout, access control, inactive accounts, and role-change invalidation.'
}
finally {
    if ($connection.State -eq 'Open') {
        $command = $connection.CreateCommand()
        $command.CommandText = 'DELETE FROM dbo.Users WHERE Email = @Email'
        [void]$command.Parameters.AddWithValue('@Email', $email)
        try { [void]$command.ExecuteNonQuery() }
        finally { $command.Dispose(); $connection.Dispose() }
    }
    $client.Dispose()
}
