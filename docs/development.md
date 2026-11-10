# Development guide

## First setup

Install the .NET 10 SDK and Node.js with npm. From the repository root:

```powershell
cd MyPortfolio
npm install
dotnet restore
dotnet tool restore
dotnet run
```

Open the URL printed in the terminal. The HTTP launch profile uses
`http://localhost:5038`; the HTTPS profile also defines `https://localhost:7061`.
For HTTPS development, trust the development certificate if needed with
`dotnet dev-certs https --trust`.

Run servers and watchers in foreground terminals. Stop each with Ctrl+C before
ending the session. Do not remove or manually modify `node_modules` to clean up.

## Build and style

```powershell
dotnet build
```

The project runs `npm run css:build` before building. This also applies to normal
Visual Studio builds. npm dependencies must be installed first.

For CSS-only work:

```powershell
npm run css:build
```

For live CSS compilation, use a second terminal:

```powershell
npm run css:watch
```

Tailwind scans `Views/` and `wwwroot/js/`, as configured in `Styles/input.css`.
Use complete utility names such as `bg-blue-600`; do not construct fragments
such as `bg-@color-600`, which the compiler cannot reliably detect.

Edit utilities in Razor views rather than editing `wwwroot/css/site.css`, which
is regenerated. The project explicitly includes that generated file in its
static asset manifest and publish output.

## Page JavaScript

Add scripts only when there is real browser behavior. Match page folders:

```text
Views/Profile/Edit.cshtml
wwwroot/js/profile/edit.js
```

Load the script from that view:

```cshtml
@section Scripts {
    <script src="~/js/profile/edit.js" asp-append-version="true"></script>
}
```

If multiple pages need the same behavior, add a shared script and load it in
`Views/Shared/_Layout.cshtml`. There is no shared script yet.

Normal links and form submissions do not need custom JavaScript. Future forms
must validate input on the server; browser validation is optional feedback and
does not replace server validation. jQuery validation plugins were removed.

## Dependencies

| Dependency | Version | Purpose |
| --- | --- | --- |
| Microsoft.EntityFrameworkCore.SqlServer | 10.0.14 | SQL Server access through EF Core |
| Microsoft.EntityFrameworkCore.Design | 10.0.14 | EF design-time tooling |
| System.Security.Cryptography.Pkcs | 10.0.14 | Patched override of a transitive SQL-client dependency |
| dotnet-ef | 10.0.14 | Project-local migration command |
| tailwindcss | 4.3.5 | Utility CSS compiler |
| @tailwindcss/cli | 4.3.5 | Tailwind build command |

Versions above describe the initial cleanup. The project files and npm lockfile
are the source of truth if dependencies change.

## Local database connection

The app reads the portfolio database on 127.0.0.1,1433. Existing tables are
created separately with docs/database/schema.sql. This app does not create or
migrate them automatically. Full Identity EF stores have been removed.

This machine's connection is saved in .NET User Secrets under
ConnectionStrings:Portfolio. Passwords are not in source files or documentation.
On another machine, configure that secret before running from MyPortfolio:

```powershell
dotnet user-secrets set "ConnectionStrings:Portfolio" "Server=127.0.0.1,1433;Database=portfolio;User Id=sa;Password=YOUR_LOCAL_PASSWORD;Encrypt=True;TrustServerCertificate=True;"
```

The certificate-trust option is for the local SQL container. For deployment,
configure ConnectionStrings__Portfolio through the host environment and use the
host's SQL/certificate settings. User Secrets are loaded in Development.

An empty SiteContent table is supported: the view model defaults to Decode Studio with no
hero description. To display saved content, create row Id = 1 using all required
columns in the documented schema. The app only reads this row; it does not seed it.

## Home smoke check

While the app is running, from the repository root:

```powershell
./scripts/Test-Home.ps1
```

The check verifies the landing section containers in public mode, or the login form after the school-mode redirect, plus CSS. Use [Test-Accounts](authentication.md) for authenticated database checks.
Stop the server/watchers with Ctrl+C after testing.

## Image files

All hero/project/team/profile image forms use file pickers. Selected files are
copied into MyPortfolio/wwwroot/uploads/images. The database stores their relative
paths. The folder is created in the project and recreated by the upload service
if needed. Git tracks its empty placeholder, not uploaded images. Back up this
folder with the database and provide persistent storage for deployment.

## Visual design and motion

DESIGN.md records the current reference and shared tokens. Styles/input.css owns
the Tailwind theme, base form styles, component utilities, self-hosted Inter font,
and gallery keyframes. Views remain Razor templates with Tailwind classes.

wwwroot/js/site.js handles the mobile menu and gallery pause/play. It honors
reduced-motion preferences and pauses the gallery offscreen/in background tabs.
Run `node scripts/Test-Design.mjs` to check these behaviors without a server.
