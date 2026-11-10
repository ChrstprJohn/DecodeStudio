# Work log

## 2026-11-10 — Local Git repository

- Initialized Git at the project root with main as the initial branch.
- Reviewed tracked candidates: local SQL credentials remain in User Secrets;
  documentation contains only a placeholder password.
- Existing ignore rules exclude IDE state, build output, node_modules, environment
  files, user-specific project settings, and local visual inspection artifacts.
- Captured the current project and documentation in the initial local commit.
  No remote repository or push was configured.

## 2026-11-10 — Moved database configuration into Data

- Moved connection-string lookup and SQL Server context registration into
  Data/DatabaseConfiguration.cs. Program.cs calls AddDatabase once.
- Credentials remain in User Secrets. Database behavior and service registration
  are unchanged. Verified compilation with a separate output folder.

## 2026-11-10 — Removed template cache attribute

- Removed ResponseCache from the error action at the user's request. It had
  instructed clients not to store error responses; there was no home-page cache.
- Left the default MVC route unchanged: / resolves to HomeController.Index.
- Verified compilation using a separate output folder to avoid the running app.

## 2026-11-10 — Simplified home method signatures

- Removed explicit CancellationToken parameters from the home action, service,
  and EF query call to simplify the beginner-facing request flow.
- Queries remain asynchronous. Abandoned browser requests no longer explicitly
  cancel this query. Verified with a build; no server or database changes needed.

## 2026-11-10 — Minimal database-backed home page

- Added SiteContent entity, ApplicationDbContext, HomeService, and HomeViewModel.
- HomeController now queries SiteContent row 1 asynchronously through the service.
- Added a minimal Tailwind navigation and hero; removed the starter footer.
- Empty content shows Decode Studio with no description. Database errors are
  not silently replaced with fallback content.
- Configured the local portfolio connection through .NET User Secrets. Old
  project credentials failed; the user explicitly authorized reading current
  Docker SQL credentials, which were stored privately without printing them.
- Removed the no-longer-needed Identity EF store dependency.
- Added scripts/Test-Home.ps1 as a repeatable HTTP smoke check.
- Verified a zero-warning/error build, direct read-only SQL query, HTTP home/CSS
  checks, and desktop/mobile screenshots (tablet also rendered). SiteContent had
  zero rows; no data was seeded or changed. Populated-row rendering was not tested.
- The design inspector flagged missing media/motion and sparse copy; these are
  intentional because the requested page is blank apart from navigation/hero.
- Stopped the test server and verified no matching MVC/Node workers remained.
- Updated architecture, setup, and current implementation documentation.

## 2026-11-10 — Named database creation

- Updated schema.sql to create portfolio if absent and select it before table
  creation. Existing databases are reused; existing tables are not overwritten.
- Documented GO batch support and database-creation permission requirements.
- Script was checked as text, not executed against SQL Server.

## 2026-11-10 — Custom account schema replaces Identity draft

- Replaced seven Identity table definitions with one Users table: separate name
  fields, unique email, password hash, image, role, activation, and UTC creation date.
- Kept all five content table definitions unchanged. The SQL only creates tables
  with primary/uniqueness keys; no sample rows or separate index/default/check SQL.
- Updated current plans to custom accounts using cookie authentication and
  Microsoft's password hasher. Earlier Identity decisions below are historical.
- Dependencies remain unchanged in this schema-only step.
- Statically checked the script and documentation links. SQL was not executed;
  database connection work follows the user's manual execution.

## 2026-11-10 — Simplified table-only SQL

- Removed explicit indexes, default values, check constraints, and transaction
  commands at the user's request. Kept table definitions, primary keys, and
  foreign keys. SQL Server automatically indexes primary keys.
- Updated the database notes: the app will supply required field values and
  maintain the single studio record. SQL has not been executed.

## 2026-11-10 — Reviewable SQL schema and page inventory

- Added `database/schema.sql` for manual SQL Server execution: five content tables
  plus seven standard Identity tables, keys, indexes, and defaults.
- Used PascalCase naming and separate FirstName, optional MiddleName, and LastName
  for account profiles and public team members; no combined full-name column.
- Added database naming/relationship notes and the requested page/section inventory.
- SQL is a draft for review and was not run against the user's database.
- Manual table creation will need a matching EF model and migration baseline later.

## 2026-11-10 — Decode Studio scope and database planning

- Recorded DCD / Decode Studio as the working studio name.
- Planned a landing page with hero, About, website showcase, services, contact,
  owner/team, and footer content.
- Mapped database-managed content to the assignment's four content requirements.
- Documented admin content/account management and user-only profile editing.
- Recorded login-required school mode and public-browsing deployment mode, with
  admin/profile authorization preserved in both.
- Proposed Identity accounts plus SiteContent, Project, Service, TeamMember, and
  SocialLink entities; no migrations or database changes were made.
- Defined initial analytics as account/content counts; traffic tracking remains
  undecided and is not included in the proposed schema.
- Checked the documentation links; no runtime checks were needed for this step.

## 2026-11-10 — MVC starter cleanup

### Completed

- Kept the existing .NET 10 MVC project, controller-based routing, shared layout,
  and error handling.
- Removed Bootstrap, jQuery, their bundled files, the validation script partial,
  the Privacy demo page/action, and template styles/scripts.
- Replaced demo home content with a minimal under-construction page.
- Added Tailwind CSS 4 and its CLI, pinned npm dependencies, and a lockfile.
- Added one-shot and watch commands for CSS compilation.
- Connected Tailwind compilation to normal .NET builds and explicitly included
  generated CSS in static assets/publish content.
- Created `Models/Entities`, `Models/ViewModels`, `Services`, and `Data`.
- Moved `ErrorViewModel` into `Models/ViewModels` and updated its namespace/users.
- Installed EF Core SQL Server, EF design tools, and Identity EF stores.
- Added the matching project-local EF CLI tool.
- Pinned a patched cryptography dependency after NuGet reported vulnerabilities
  in the transitive version.
- Added source-control exclusions and quick-start instructions.

### Verification recorded during cleanup

- .NET build succeeded with zero warnings and zero errors.
- `dotnet ef --version` returned 10.0.14.
- HTTP smoke checks passed for the homepage and generated Tailwind stylesheet.
- Homepage response contained no Bootstrap or jQuery references.
- Generated CSS was confirmed in the static asset build manifest.
- Test servers were stopped; process checks found no matching project Node workers.

### Still pending

- Inspect the existing SQL Server schema and choose database/table strategy.
- Configure the EF context, connection secrets, and Identity.
- Implement registration, login, role checks, and inactive-account enforcement.
- Implement profile editing, admin user management/statistics, and website content.
- Add migrations and feature tests alongside those implementations.

No existing database was modified. The current starter does not enforce login.

## 2026-11-10 — Project documentation

- Added this work log, an architecture/folder guide, and a development guide under
  `docs/`, with an index linked from the root README.
- Documented current behavior separately from planned database/account features.
- Checked local documentation links. No application code changed in this step;
  runtime/build checks were not repeated for Markdown-only additions.
