param([string]$BaseUrl = 'http://localhost:5041')

# Uses two temporary accounts/content entries; cleanup targets only this run's IDs.
$ErrorActionPreference = 'Stop'
$marker = 'portal-test-' + [guid]::NewGuid().ToString('N')
$adminEmail = $marker + '-admin@example.test'
$userEmail = $marker + '-user@example.test'
$password = 'Portal-' + [guid]::NewGuid().ToString('N')
$newPassword = 'Changed-' + [guid]::NewGuid().ToString('N')
$secretPath = Join-Path $env:APPDATA 'Microsoft/UserSecrets/decode-studio-portfolio-local/secrets.json'
$settings = Get-Content $secretPath -Raw | ConvertFrom-Json
$db = [System.Data.SqlClient.SqlConnection]::new($settings.'ConnectionStrings:Portfolio')
$created = [System.Collections.Generic.List[object]]::new()
$clients = [System.Collections.Generic.List[System.Net.Http.HttpClient]]::new()
$studioCreated = $false
$uploadedPath = $null
$testImages = [System.Collections.Generic.List[string]]::new()
$pngBytes = [Convert]::FromBase64String('iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+jRZkAAAAASUVORK5CYII=')

function Assert($condition, [string]$message) { if (-not $condition) { throw $message } }
function New-Client {
    $handler = [System.Net.Http.HttpClientHandler]::new()
    $handler.AllowAutoRedirect = $false
    $client = [System.Net.Http.HttpClient]::new($handler)
    $clients.Add($client)
    return $client
}
function Sql([string]$query, [hashtable]$parameters = @{}) {
    $command = $db.CreateCommand(); $command.CommandText = $query
    foreach ($key in $parameters.Keys) { [void]$command.Parameters.AddWithValue('@' + $key, $parameters[$key]) }
    $table = [System.Data.DataTable]::new()
    try {
        $reader = $command.ExecuteReader()
        try { $table.Load($reader) } finally { $reader.Dispose() }
        return ,$table
    } finally { $command.Dispose() }
}
function Page($client, [string]$path) {
    $response = $client.GetAsync($BaseUrl + $path).GetAwaiter().GetResult()
    try {
        return @{ Status = [int]$response.StatusCode; Html = $response.Content.ReadAsStringAsync().GetAwaiter().GetResult(); Location = [string]$response.Headers.Location }
    } finally { $response.Dispose() }
}
function Token($client, [string]$path) {
    $page = Page $client $path
    Assert ($page.Status -eq 200) "GET $path failed: $($page.Status)"
    $inputMatch = [regex]::Match($page.Html, '<input[^>]*name="__RequestVerificationToken"[^>]*>')
    $value = [regex]::Match($inputMatch.Value, 'value="([^"]+)"')
    Assert $value.Success "Missing anti-forgery token on $path"
    return [System.Net.WebUtility]::HtmlDecode($value.Groups[1].Value)
}
function Post($client, [string]$path, [hashtable]$fields, [string]$tokenPage) {
    $dictionary = [System.Collections.Generic.Dictionary[string,string]]::new()
    foreach ($key in $fields.Keys) { if ($key -ne "ImageFile") { $dictionary.Add($key, [string]$fields[$key]) } }
    if ($tokenPage) { $dictionary['__RequestVerificationToken'] = Token $client $tokenPage }
    if ($fields.ContainsKey('ImageFile')) {
        $body = [System.Net.Http.MultipartFormDataContent]::new()
        foreach ($key in $dictionary.Keys) { $body.Add([System.Net.Http.StringContent]::new($dictionary[$key]), $key) }
        $file = [System.Net.Http.ByteArrayContent]::new([byte[]]$fields.ImageFile)
        $file.Headers.ContentType = [System.Net.Http.Headers.MediaTypeHeaderValue]::new('image/png')
        $body.Add($file, 'ImageFile', 'test.png')
    }
    else { $body = [System.Net.Http.FormUrlEncodedContent]::new($dictionary) }
    try {
        $response = $client.PostAsync($BaseUrl + $path, $body).GetAwaiter().GetResult()
        try { return @{ Status = [int]$response.StatusCode; Html = $response.Content.ReadAsStringAsync().GetAwaiter().GetResult(); Location = [string]$response.Headers.Location } }
        finally { $response.Dispose() }
    } finally { $body.Dispose() }
}
function Register($client, [string]$email) {
    $result = Post $client '/Account/Register' @{ FirstName='Portal'; LastName='Test'; Email=$email; Password=$password; ConfirmPassword=$password } '/Account/Register'
    Assert ($result.Status -eq 302) 'Temporary account registration failed'
}
function Login($client, [string]$email, [string]$pass) {
    return Post $client '/Account/Login' @{ Email=$email; Password=$pass } '/Account/Login'
}
function Capture($client, [string]$path, [string]$name) {
    $page = Page $client $path
    Assert ($page.Status -eq 200) "Could not capture $path"
    $folder = Join-Path (Get-Location) '.taste-skill/portals'
    [void](New-Item -ItemType Directory -Force -Path $folder)
    $html = $page.Html.Replace('href="/css/site.css', 'href="' + $BaseUrl + '/css/site.css')
    Set-Content (Join-Path $folder ($name + '.html')) $html
}

try {
    $db.Open()
    $admin = New-Client; $user = New-Client; $otherSession = New-Client; $anonymous = New-Client
    Register $admin $adminEmail; Register $user $userEmail
    [void](Sql 'UPDATE dbo.Users SET Role = N''Admin'' WHERE Email = @Email' @{ Email=$adminEmail })
    $adminId = (Sql 'SELECT Id FROM dbo.Users WHERE Email = @Email' @{ Email=$adminEmail }).Rows[0].Id
    $userId = (Sql 'SELECT Id FROM dbo.Users WHERE Email = @Email' @{ Email=$userEmail }).Rows[0].Id
    Assert ((Login $admin $adminEmail $password).Location -eq '/Admin') 'Admin login must land on analytics'
    Assert ((Login $user $userEmail $password).Location -eq '/') 'User login must land on home'
    Assert ((Login $otherSession $userEmail $password).Status -eq 302) 'Second user session failed'
    foreach ($path in @('/Admin','/Studio','/Projects','/Offerings','/Team','/Contact','/Links','/Accounts')) {
        Assert ((Page $admin $path).Status -eq 200) "Admin cannot read $path"
        $denied = Page $user $path
        Assert ($denied.Status -eq 302 -and $denied.Location.Contains('/Account/AccessDenied')) "User can reach $path"
    }
    Assert ((Page $anonymous '/Profile').Status -eq 302) 'Anonymous profile access must require login'
    Assert ((Post $admin '/Projects/Create' @{Title='missing-token'} '').Status -eq 400) 'Admin writes must require anti-forgery tokens'
    $denied = Post $user '/Projects/Create' @{ Title='forbidden' } '/Profile'
    Assert ($denied.Status -eq 302 -and $denied.Location.Contains('/Account/AccessDenied')) 'User can submit admin content'

    $cases = @(
        @{ Controller='Projects'; Table='Projects'; Key='Title'; ImageColumn='ImagePath'; Fields=@{Title=$marker+'-project';Description='Project description';ImageFile=$pngBytes;WebsiteUrl='https://example.com';SortOrder=1;IsPublished='true'} },
        @{ Controller='Offerings'; Table='Services'; Key='Title'; ImageColumn='ImagePath'; Fields=@{ImageFile=$pngBytes;Title=$marker+'-service';Description='Service description';SortOrder=1;IsPublished='true'} },
        @{ Controller='Team'; Table='TeamMembers'; Key='FirstName'; ImageColumn='PhotoPath'; Fields=@{ImageFile=$pngBytes;FirstName=$marker+'-team';LastName='Member';RoleTitle='Developer';Bio='Team biography';SortOrder=1;IsOwner='true';IsPublished='true'} },
        @{ Controller='Links'; Table='SocialLinks'; Key='Label'; Fields=@{Label=$marker+'-link';Url='https://example.com';SortOrder=1} }
    )
    foreach ($case in $cases) {
        $controller=$case.Controller; $table=$case.Table; $key=$case.Key; $fields=$case.Fields
        $response = Post $admin "/$controller/Create" $fields "/$controller/Create"
        Assert ($response.Status -eq 302) "$controller create failed: $($response.Status)"
        $row = (Sql "SELECT Id FROM dbo.$table WHERE $key = @Value" @{Value=$fields[$key]}).Rows[0]
        Assert ($null -ne $row) "$controller row not saved"
        $id=[int]$row.Id; $created.Add(@{Table=$table;Id=$id})
        $contentImage = $null
        if ($case.ContainsKey('ImageColumn')) {
            $column=$case.ImageColumn
            $contentImage=(Sql "SELECT $column AS Image FROM dbo.$table WHERE Id=@Id" @{Id=$id}).Rows[0].Image
            Assert ($contentImage.StartsWith('/uploads/images/')) "$controller did not copy image to the shared folder"
            $testImages.Add($contentImage)
            Assert ((Page $anonymous $contentImage).Status -eq 200) "$controller uploaded image was not served"
            [void]$fields.Remove('ImageFile')
        }
        Assert ((Page $admin "/$controller").Html.Contains($fields[$key])) "$controller read failed"
        Assert ((Page $user '/').Html.Contains($fields[$key])) "$controller published content missing on landing"
        if ($fields.ContainsKey('IsPublished')) {
            $fields.IsPublished='false'; $fields.Id=$id
            Assert ((Post $admin "/$controller/Edit" $fields "/$controller/Edit/$id").Status -eq 302) "$controller unpublish failed"
            Assert (-not (Page $user '/').Html.Contains($fields[$key])) "$controller draft leaked on landing"
            $fields.IsPublished='true'
        }
        $fields[$key] += '-edited'; $fields.Id=$id
        Assert ((Post $admin "/$controller/Edit" $fields "/$controller/Edit/$id").Status -eq 302) "$controller edit failed"
        Assert ((Page $user '/').Html.Contains($fields[$key])) "$controller edit not visible"
        if ($case.ContainsKey('ImageColumn')) {
            $column=$case.ImageColumn
            Assert ((Sql "SELECT $column AS Image FROM dbo.$table WHERE Id=@Id" @{Id=$id}).Rows[0].Image -eq $contentImage) "$controller must keep existing image without new upload"
            $fields.ImageFile=$pngBytes
            Assert ((Post $admin "/$controller/Edit" $fields "/$controller/Edit/$id").Status -eq 302) "$controller image replacement failed"
            [void]$fields.Remove('ImageFile')
            $replacement=(Sql "SELECT $column AS Image FROM dbo.$table WHERE Id=@Id" @{Id=$id}).Rows[0].Image
            $testImages.Add($replacement)
            Assert ($replacement -ne $contentImage) "$controller replacement did not get its own filename"
            Assert (-not (Test-Path -LiteralPath (Join-Path (Get-Location) ('MyPortfolio/wwwroot' + $contentImage)))) 'Old content image was not removed'
            $contentImage=$replacement
        }
        Assert ((Page $admin "/$controller/Delete/$id").Status -eq 200) "$controller delete confirmation missing"
        Assert ((Sql "SELECT Id FROM dbo.$table WHERE Id=@Id" @{Id=$id}).Rows.Count -eq 1) 'GET delete mutated data'
        Assert ((Post $admin "/$controller/Delete" @{id=$id;confirmed='true'} "/$controller/Delete/$id").Status -eq 302) "$controller delete failed"
        Assert ((Sql "SELECT Id FROM dbo.$table WHERE Id=@Id" @{Id=$id}).Rows.Count -eq 0) "$controller delete did not persist"
        if ($contentImage) { Assert (-not (Test-Path -LiteralPath (Join-Path (Get-Location) ('MyPortfolio/wwwroot' + $contentImage)))) "$controller deleted record left its image" }
        Assert ((Page $admin "/$controller/Edit/$id").Status -eq 404) "$controller missing row should be 404"
    }
    $badProject=@{Title=$marker;Description='Invalid';ImageFile=$pngBytes;WebsiteUrl='javascript:alert(1)';SortOrder=0}
    Assert ((Post $admin '/Projects/Create' $badProject '/Projects/Create').Status -eq 200) 'Invalid project should return validation errors'
    Assert ((Sql 'SELECT Id FROM dbo.Projects WHERE Title=@Title' @{Title=$marker}).Rows.Count -eq 0) 'Unsafe URL was saved'

    if ((Sql 'SELECT Id FROM dbo.SiteContent WHERE Id=1').Rows.Count -eq 0) {
        $studio=@{ImageFile=$pngBytes;StudioName=$marker;HeroTitle=$marker+' hero';HeroDescription='Hero description';AboutTitle='About';AboutDescription='About description'}
        Assert ((Post $admin '/Studio/Edit' $studio '/Studio').Status -eq 302) 'Studio create failed'
        $studioCreated=$true
        $heroImage=(Sql 'SELECT HeroImagePath FROM dbo.SiteContent WHERE Id=1').Rows[0].HeroImagePath
        $testImages.Add($heroImage)
        Assert ($heroImage.StartsWith('/uploads/images/')) 'Hero upload not saved in shared images folder'
        [void]$studio.Remove('ImageFile')
        Assert ((Page $user '/').Html.Contains($studio.HeroTitle)) 'Studio content missing from home'
        $studio.HeroTitle += ' edited'
        Assert ((Post $admin '/Studio/Edit' $studio '/Studio').Status -eq 302) 'Studio update failed'
        Assert ((Page $user '/').Html.Contains($studio.HeroTitle)) 'Updated studio content missing'
        Assert ((Post $admin '/Contact' @{Email='studio@example.test';Phone='555-0100'} '/Contact').Status -eq 302) 'Contact save failed'
        Assert ((Page $user '/').Html.Contains('mailto:studio@example.test')) 'Contact email missing from home'
        Assert ((Post $admin '/Contact' @{Email='';Phone=''} '/Contact').Status -eq 302) 'Contact clear failed'
        Assert (-not (Page $user '/').Html.Contains('mailto:studio@example.test')) 'Cleared contact email still visible'
        Assert ((Post $admin '/Studio/Delete' @{confirmed='true'} '/Studio/Delete').Status -eq 302) 'Studio delete failed'
        Assert ((Sql 'SELECT Id FROM dbo.SiteContent WHERE Id=1').Rows.Count -eq 0) 'Studio record not deleted'
        Assert (-not (Test-Path -LiteralPath (Join-Path (Get-Location) ('MyPortfolio/wwwroot' + $heroImage)))) 'Hero image not cleaned up after deletion' 
    }

    Assert ((Page $admin "/Accounts/Details/$userId").Status -eq 200) 'Admin account details failed'
    Assert ((Page $user "/Accounts/Details/$userId").Status -eq 302) 'User can access account management'
    Assert ((Post $admin '/Accounts/SetActive' @{id=$userId;isActive='false'} "/Accounts/Details/$userId").Status -eq 302) 'Deactivate failed'
    Assert ((Page $user '/Profile').Status -eq 302) 'Deactivated session still works'
    Assert ((Post $admin '/Accounts/SetActive' @{id=$userId;isActive='true'} "/Accounts/Details/$userId").Status -eq 302) 'Reactivate failed'
    Assert ((Post $admin '/Accounts/SetActive' @{id=$adminId;isActive='false'} "/Accounts/Details/$adminId").Status -eq 302) 'Self-deactivation response failed'
    Assert ((Sql 'SELECT IsActive FROM dbo.Users WHERE Id=@Id' @{Id=$adminId}).Rows[0].IsActive) 'Admin deactivated self'
    Assert ((Page $admin "/Accounts/Delete/$userId").Status -eq 404) 'Account deletion must not exist'
    [void](Login $user $userEmail $password)
    [void](Login $otherSession $userEmail $password)

    $profile=@{FirstName='Changed';MiddleName='Middle';LastName='User';Email=$userEmail;Id=$adminId;Role='Admin'}
    Assert ((Post $user '/Profile/Edit' $profile '/Profile').Status -eq 302) 'Profile update failed'
    $saved=(Sql 'SELECT FirstName, Role FROM dbo.Users WHERE Id=@Id' @{Id=$userId}).Rows[0]
    Assert ($saved.FirstName -eq 'Changed' -and $saved.Role -eq 'User') 'Profile update/overposting protection failed'
    Assert ((Sql 'SELECT FirstName FROM dbo.Users WHERE Id=@Id' @{Id=$adminId}).Rows[0].FirstName -eq 'Portal') 'Profile changed another user'
    $profile.Email=$adminEmail
    Assert ((Post $user '/Profile/Edit' $profile '/Profile').Status -eq 200) 'Duplicate profile email should be rejected'
    $profile.Email=$userEmail

    $badUpload=[System.Net.Http.MultipartFormDataContent]::new()
    foreach ($key in @('FirstName','LastName','Email')) { $badUpload.Add([System.Net.Http.StringContent]::new([string]$profile[$key]),$key) }
    $badUpload.Add([System.Net.Http.StringContent]::new((Token $user '/Profile')),'__RequestVerificationToken')
    $badFile=[System.Net.Http.ByteArrayContent]::new([System.Text.Encoding]::UTF8.GetBytes('<svg>not an allowed raster image</svg>'))
    $badUpload.Add($badFile,'Photo','fake.png')
    $response=$user.PostAsync($BaseUrl+'/Profile/Edit',$badUpload).GetAwaiter().GetResult()
    try {
        $html=$response.Content.ReadAsStringAsync().GetAwaiter().GetResult()
        Assert ([int]$response.StatusCode -eq 200 -and $html.Contains('Choose a PNG')) 'Invalid photo signature was accepted'
    } finally { $response.Dispose(); $badUpload.Dispose() }

    $token = Token $user '/Profile'
    $multipart=[System.Net.Http.MultipartFormDataContent]::new()
    foreach ($key in @('FirstName','LastName','Email')) { $multipart.Add([System.Net.Http.StringContent]::new([string]$profile[$key]),$key) }
    $multipart.Add([System.Net.Http.StringContent]::new($token),'__RequestVerificationToken')
    $png=[Convert]::FromBase64String('iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+jRZkAAAAASUVORK5CYII=')
    $image=[System.Net.Http.ByteArrayContent]::new($png)
    $image.Headers.ContentType=[System.Net.Http.Headers.MediaTypeHeaderValue]::new('image/png')
    $multipart.Add($image,'Photo','profile.png')
    $response=$user.PostAsync($BaseUrl+'/Profile/Edit',$multipart).GetAwaiter().GetResult()
    try { Assert ([int]$response.StatusCode -eq 302) 'Photo upload failed' } finally {$response.Dispose();$multipart.Dispose()}
    $uploadedPath=(Sql 'SELECT ProfileImagePath FROM dbo.Users WHERE Id=@Id' @{Id=$userId}).Rows[0].ProfileImagePath
    Assert ($uploadedPath.StartsWith('/uploads/images/')) 'Photo reference not saved'
    Assert ((Page $anonymous $uploadedPath).Status -eq 200) 'Uploaded photo not served'
    $profile.RemovePhoto='true'
    Assert ((Post $user '/Profile/Edit' $profile '/Profile').Status -eq 302) 'Remove photo failed'
    Assert ((Sql 'SELECT ProfileImagePath FROM dbo.Users WHERE Id=@Id' @{Id=$userId}).Rows[0].IsNull('ProfileImagePath')) 'Photo reference not cleared'
    Assert ((Page $anonymous $uploadedPath).Status -ne 200) 'Removed photo file still served'
    Assert (-not (Test-Path -LiteralPath (Join-Path (Get-Location) ('MyPortfolio/wwwroot' + $uploadedPath)))) 'Removed photo file still exists'

    Capture $admin '/Admin' 'analytics'
    Capture $admin '/Accounts' 'accounts'
    Capture $admin '/Projects/Create' 'project-form'
    Capture $user '/Profile' 'profile'
    $change=@{CurrentPassword='incorrect';NewPassword=$newPassword;ConfirmPassword=$newPassword}
    Assert ((Post $user '/Profile/ChangePassword' $change '/Profile/ChangePassword').Status -eq 200) 'Incorrect current password should fail'
    $change.CurrentPassword=$password
    Assert ((Post $user '/Profile/ChangePassword' $change '/Profile/ChangePassword').Status -eq 302) 'Password change failed'
    Assert ((Page $otherSession '/Profile').Status -eq 302) 'Password change must invalidate another session'
    Assert ((Login $user $userEmail $password).Status -eq 200) 'Old password must fail'
    Assert ((Login $user $userEmail $newPassword).Status -eq 302) 'New password must work'
    'PASS: admin sidebar/routes, all content CRUD, draft visibility, role boundaries, account activation, no account deletion, own-profile updates, photos, password changes, and session revocation.'
}
finally {
    if ($db.State -eq 'Open') {
        foreach ($entry in $created) { [void](Sql "DELETE FROM dbo.$($entry.Table) WHERE Id=@Id" @{Id=$entry.Id}) }
        if ($studioCreated) { [void](Sql 'DELETE FROM dbo.SiteContent WHERE Id=1 AND StudioName=@Name' @{Name=$marker}) }
        [void](Sql 'DELETE FROM dbo.Users WHERE Email IN (@Admin, @User)' @{Admin=$adminEmail;User=$userEmail})
        $db.Dispose()
    }
    if ($uploadedPath -and $uploadedPath.StartsWith('/uploads/images/')) {
        $file=Join-Path (Get-Location) ('MyPortfolio/wwwroot' + $uploadedPath)
        if (Test-Path -LiteralPath $file) { Remove-Item -LiteralPath $file }
    }
    foreach ($imagePath in $testImages) {
        if ($imagePath.StartsWith('/uploads/images/')) {
            $file=Join-Path (Get-Location) ('MyPortfolio/wwwroot' + $imagePath)
            if (Test-Path -LiteralPath $file) { Remove-Item -LiteralPath $file }
        }
    }
    foreach ($client in $clients) { $client.Dispose() }
}




