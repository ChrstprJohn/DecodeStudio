# MyPortfolio

ASP.NET Core MVC (.NET 10), Razor views, and Tailwind CSS 4.

## Documentation

See [docs](docs/README.md) for the architecture and folder structure, development
guide, and a dated record of completed work and pending features.

See the [branch workflow](docs/branch-workflow.md) for both variants, database sharing, and how to carry changes across.

## Run locally

Requires the .NET 10 SDK and Node.js/npm.

```powershell
cd MyPortfolio
npm install
dotnet restore
dotnet tool restore
dotnet run
```

Open the localhost URL printed in the terminal. Stop the app with Ctrl+C.
Normal .NET builds also compile Tailwind, including builds from Visual Studio.

For live styling changes, run `npm run css:watch` in a second foreground terminal.
Stop it with Ctrl+C when finished. Otherwise, restart the app to rebuild styles.
`npm run css:build` compiles styles once without starting the app.

## Structure

- `Controllers/`: HTTP requests and view responses.
- `Models/Entities/`: database entities when database work begins.
- `Models/ViewModels/`: form inputs and page display models.
- `Services/`: business logic when features need it.
- `Data/`: EF Core context when the database is configured.
- `Views/`: Razor markup and shared layout.
- `Styles/input.css`: Tailwind entry point; scans views and browser scripts.
- `wwwroot/css/site.css`: generated stylesheet; edit Tailwind classes instead.
- `wwwroot/js/`: add page-specific scripts as needed, e.g. `profile/edit.js`.

Load page scripts using `@section Scripts` in the corresponding view. Add a
shared script to the layout only when there is shared browser behavior.
Use complete Tailwind class names so the compiler can detect them.

## Prepared dependencies

- EF Core SQL Server provider and design tools.
- Full Identity EF stores were removed; custom account authentication uses cookies and the standalone Microsoft password hasher.
- Project-local `dotnet-ef` tool for future migrations.
- Tailwind CSS and CLI as build-only npm dependencies.

The home page reads SiteContent row 1 from portfolio through EF Core. The local connection is configured in .NET User Secrets. See docs/development.md for setup on another machine. No content is inserted automatically. Login/signup/logout are implemented; content CRUD and profile/account portals are implemented; analytics is blank and migrations are not used. Anonymous home requests redirect to /Account/Login by default. See [account documentation](docs/authentication.md).

Bootstrap, jQuery, and the demo Privacy page have been removed. Forms should use
server-side validation; jQuery client validation is no longer included.

See [implemented portal features](docs/features.md) for routes, permissions, and CRUD tests.

See [DESIGN.md](DESIGN.md) for the shared public/account/admin visual language.
