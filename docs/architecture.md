# Architecture and folder structure

Follow the agreed [MVC coding conventions](coding-conventions.md) when adding
controllers, actions, services, and forms.

## Approach

One ASP.NET Core MVC project targeting .NET 10. Controllers handle requests,
Razor views produce HTML, and Tailwind styles those views. These are MVC Razor
views (`.cshtml`), not the separate Razor Pages programming model.

Use plain JavaScript for browser interactions when needed. There is no React
application, Bootstrap, or jQuery.

## Current folders

```text
MVC_Portfolio/
├── docs/                         Project documentation
├── README.md                     Quick start
└── MyPortfolio/
    ├── Controllers/
    │   └── HomeController.cs
    ├── Models/
    │   ├── Entities/             All six database entities
    │   └── ViewModels/
    │       └── ErrorViewModel.cs
    ├── Services/                 Concrete services for home, accounts, content, and profiles
    ├── Data/                     ApplicationDbContext.cs
    ├── Views/
    │   ├── Home/Index.cshtml
    │   ├── Home/Sections/        One partial per landing section
    │   ├── Shared/
    │   │   ├── _Layout.cshtml
    │   │   └── Error.cshtml
    │   ├── _ViewImports.cshtml
    │   └── _ViewStart.cshtml
    ├── Styles/input.css          Tailwind entry point
    ├── wwwroot/
    │   ├── css/site.css          Generated Tailwind output
    │   ├── js/                   Reserved for browser scripts
    │   └── favicon.ico
    ├── Properties/launchSettings.json
    ├── Program.cs                Application setup and request pipeline
    ├── appsettings.json          Non-secret configuration
    ├── appsettings.Development.json
    ├── MyPortfolio.csproj        .NET dependencies and CSS build hook
    ├── MyPortfolio.slnx          Visual Studio solution
    ├── package.json              Frontend build dependencies and commands
    ├── package-lock.json         Locked npm dependency versions
    └── dotnet-tools.json         Project-local EF migration tool
```

Reserved folders currently contain `.gitkeep` files so they can be tracked.
`bin/`, `obj/`, `node_modules/`, and `.vs/` are generated or local tooling folders;
they are excluded from source control.

## Where code belongs

| Location | Responsibility |
| --- | --- |
| Controllers | Accept requests, check form validation, call services, select views or redirects. |
| Models/Entities | Represent database records. Add these when the schema is agreed. |
| Models/ViewModels | Define allowed form inputs or data needed by a particular view. |
| Services | Apply business rules and use the EF context for feature operations. |
| Data | Hold the EF Core database context and database setup code. |
| Views | Render HTML, forms, and validation messages. Avoid database queries here. |
| wwwroot | Serve public assets. Never put credentials or private documents here. |

Entities and view models stay in separate subfolders within `Models`, as agreed.
For example, a future `EditProfileViewModel` can accept a name and bio without
accepting account roles or activation status. The service copies permitted
values to the database entity for the signed-in user.

Keep concrete services simple. No generic repository layer, base services, or
interfaces for every class are needed at this stage.

## Request flow

Currently:

```text
Browser → HomeController → HomeService → ApplicationDbContext → portfolio.SiteContent
              ↓
          HomeViewModel → Razor view + shared layout → HTML response
```

When database features are implemented:

```text
Browser → Controller → Service → EF Core context → SQL Server
              ↓
          Razor view → HTML response
```

The default route is `{controller=Home}/{action=Index}/{id?}`. The shared layout
loads the generated CSS and supports optional page scripts through a `Scripts`
section. The shared error view displays a request ID when available.

## Current implementation and next steps

ApplicationDbContext maps all six existing tables. DatabaseConfiguration reads
local User Secrets and registers SQL Server without creating or migrating tables.

HomeService loads studio content and published collections for the designed landing
page, including the decorative hero gallery. Each content feature has its own concrete service and controller. Admin
controllers enforce the Admin role and share _AdminLayout. ProfileController uses
the signed-in ID and a separate ProfileService for personal account changes.

Cookie authentication, activation/role/password-version validation, account forms,
content CRUD, account activation, and profile/photo/password updates are implemented.
See [features.md](features.md) for the current endpoint inventory and tests.

The shared design language is documented in [DESIGN.md](../DESIGN.md). Analytics
is deliberately blank. Deployment upload storage, password
recovery, email verification, and login throttling remain future work. No automatic
migrations, table creation, or content seeding run.
