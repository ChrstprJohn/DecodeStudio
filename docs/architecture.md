# Architecture and folder structure

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
    │   ├── Entities/             SiteContent.cs maps the studio record
    │   └── ViewModels/
    │       └── ErrorViewModel.cs
    ├── Services/                 HomeService.cs loads home content
    ├── Data/                     ApplicationDbContext.cs
    ├── Views/
    │   ├── Home/Index.cshtml
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

The home page reads studio name, hero title, and hero description from
SiteContent row 1 using an async, read-only EF query. An empty table displays
Decode Studio with no description. Connection/query errors remain errors rather
than silently showing a successful-looking fallback page.

DatabaseConfiguration.cs in Data reads the connection setting and registers the
SQL Server context. Program.cs calls DatabaseConfiguration.AddDatabase once.

ApplicationDbContext currently maps SiteContent only. HomeService performs the
query and maps to HomeViewModel. Program.cs registers both through dependency
injection. Credentials come from ConnectionStrings:Portfolio in User Secrets
for development, or environment configuration for deployment.

The page has a studio-name link, Home navigation, and a hero. Other sections,
images, footer, account screens, and admin features are intentionally not built.
Full Identity EF stores have been removed. Custom accounts with cookie
authentication and Microsoft's password hasher remain planned.

No startup migrations, table creation, or content seeding run. Map the remaining
entities when implementing their features.
