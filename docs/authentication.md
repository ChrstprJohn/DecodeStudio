# Accounts, login, and registration

## Implemented

Plain Razor forms use the existing Users table. No schema change, migration, or
seed account is required. The landing page now renders saved published content in a plain layout.

| Request | Action |
| --- | --- |
| GET /Account/Register | Display the sign-up form |
| POST /Account/Register | Validate and create a User account, then redirect to login |
| GET /Account/Login | Display login, or signed-in status and logout |
| POST /Account/Login | Verify credentials, issue a session cookie, redirect by role |
| POST /Account/Logout | Clear the login cookie and redirect to login |
| GET /Account/AccessDenied | Display a 403 page |

The shared navigation exposes My portal, admin navigation for admins, and logout. Forms submit through Razor tag helpers and require anti-forgery
tokens. Invalid input returns the form with server-side validation messages.
No JavaScript or jQuery is required.

## Files and request flow

```text
Views/Account/Register.cshtml or Login.cshtml
    → AccountController
    → AccountService
    → ApplicationDbContext.Users
    → SQL Server portfolio.dbo.Users
```

- Models/Entities/User.cs maps the existing table.
- Models/ViewModels/RegisterViewModel.cs accepts first/middle/last names, email,
  password, and confirmation. Role, IsActive, and PasswordHash are not form inputs.
- Models/ViewModels/LoginViewModel.cs accepts email and password.
- AccountService trims names, lowercases/trims emails, checks duplicates, hashes
  passwords, and verifies login credentials. It also handles concurrent duplicate
  registration via the existing SQL unique-email constraint.
- AccountController validates forms and manages redirects, claims, and login/logout.
- AccountCookieEvents rechecks active status and role on authenticated requests.
- Program.cs registers the service, standalone password hasher, and cookie handler.

Microsoft's PasswordHasher is used as a standalone framework utility. This does
not restore full Identity account management or add AspNet account tables.

## Rules

- All new accounts are active Users, including the first account. The server
  supplies the role, activation status, and UTC registration date.
- Email is unique and normalized to lowercase by the application.
- Passwords are 8–128 characters and must match the confirmation. Only hashes
  are stored. Hashes needing an upgrade are refreshed after successful login.
- Login requires an active account with User or Admin role.
- Login cookies are HTTP-only session cookies with an eight-hour ticket lifetime.
  Production requires HTTPS cookies; local HTTP is allowed in Development.
- Deactivating/deleting an account or changing its role rejects its old cookie
  on the next authenticated request. Role changes require logging in again. Password changes also invalidate old sessions through a password-version claim.
- Admin login goes to /Admin (blank analytics); User login goes home. No user-supplied return URL is followed.

## Local school mode and public deployment

appsettings.Development.json sets Portfolio:RequireLogin to true. Anonymous home
requests redirect to Login. Account entry pages, the error page, and static assets
remain available anonymously.

appsettings.json defaults the setting to false for public browsing. Account
logout still explicitly requires authentication. Future admin actions must use
Authorize(Roles = "Admin") regardless of this setting. Admin pages and the user portal are documented in [features.md](features.md).

## Admin promotion

Register your intended account normally. No account is automatically promoted.
Use the local maintenance script after choosing the account:

```powershell
./scripts/Promote-Admin.ps1 -Email "your-account@example.com"
```

It updates only that account's Role and verifies the saved value. Log in again
after changing the role. The requested owner account has now been promoted;
registration still always creates User accounts.

## Verification

Build first. From MyPortfolio, run a temporary Development server:

```powershell
$env:ASPNETCORE_ENVIRONMENT = 'Development'
dotnet bin/Verification/MyPortfolio.dll --urls http://localhost:5041
```

That output is produced by:

```powershell
dotnet build --no-restore -p:UseAppHost=false -p:OutDir=bin/Verification/
```

From the repository root, run:

```powershell
./scripts/Test-Accounts.ps1
```

The script reads this machine's User Secrets, creates one uniquely named temporary
account, tests forms and database behavior, and deletes only its test account in
cleanup. It covers validation, duplicate emails, forced User role, stored hash,
wrong/correct passwords, logout, anonymous access, deactivation, and role changes.
It temporarily changes only its own test account's activation and role.
Stop the test server with Ctrl+C afterward.

## Deferred

Profile editing, password changes, account activation, and content management are implemented. Password recovery, email verification, and disabling registration for public deployment remain deferred. Login
throttling/lockout is also deferred and should be added before public exposure
of these account endpoints. See [features.md](features.md) for current portal endpoints and tests.
