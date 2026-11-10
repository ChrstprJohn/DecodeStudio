# Work log

## 2026-11-10 — Studio.Design direction across all pages

- Inspected the live reference in a browser, including desktop/mobile hero motion,
  scroll transitions, portfolio presentation, and navigation.
- Replaced the orange hero with a gray editorial canvas and looping diagonal
  gallery of the site's own published project images, plus pause/reduced-motion
  controls and a responsive copy/CTA layout.
- Applied the new theme to every landing section, login/signup/status forms,
  profile/password forms, admin sidebar, content forms, and account tables.
- Preserved conventional routes, authentication, anti-forgery, data binding, CRUD,
  and account permissions. Analytics remains blank.
- Self-hosted Inter and its license from the official font repository. Added
  DESIGN.md/PRODUCT.md and updated the current visual documentation.
- Build and authentication/CRUD/portal regression checks passed; test records
  were cleaned up. Added assert-based checks for motion and menu behavior.
- Reviewed the live landing/mobile menu/profile and captured admin form/table
  screens. Verified no horizontal overflow in the desktop/mobile landing views
  and no browser console errors. Saved previews under ignored .taste-skill.
- Stopped both temporary preview servers and confirmed no matching Node workers.

## 2026-11-10 — Orange/black decode hero reference

- Applied the supplied screenshot's composition to the hero only: orange ground,
  black navigation, dotted studio mark, deliberate open space, and giant decode.
- Confirmed decode as the requested word. Kept the database headline and placed
  supporting description/CTA below the first-screen canvas.
- Moved the hero outside the capped landing wrapper and avoided duplicate headers.
  Preserved account actions and all section anchors; other sections are unchanged.
- Retained the stored hero-image field; this typography-led composition omits it.
- Verified zero-warning/error build, landing smoke check, and browser captures at
  desktop, tablet, and mobile widths. No database content/schema changes.

## 2026-11-10 — Landing section skeleton and rendering order

- Organized Home/Index into six readable partials under Home/Sections, ordered
  Hero, About, Services, Products/Showcase, Contact, and Team.
- Kept database rendering, optional image/link handling, publication filtering,
  and the shared footer. Added semantic section headings and landing anchor links.
- Kept styling minimal for future design work; no admin/account features changed.
- Updated page/architecture/feature docs and smoke-check assertions for section
  order and navigation. No database content or schema changes.
- Build passed without warnings/errors. Verified section order, anchor links,
  stylesheet, and all eight seeded entries per landing collection over HTTP.

## 2026-11-10 — Sample content for landing-page design

- Added Seed-SampleData.ps1 and seed/sample-data.json with eight projects,
  services, team members, social links, and demo users; SiteContent stays one row.
- Generated one hero, eight project-preview SVGs, and eight initials avatars
  locally under wwwroot/uploads/images. Demo graphics are clearly identified.
- Executed the script against portfolio. Existing admin data was preserved.
- Reran it and verified zero new rows, confirming idempotence for sample keys.
- Checked all sample content and 17 image URLs through the app; verified active
  demo login and rejection of an inactive demo account.
- Known demo credentials are documented for local testing, not public deployment.

## 2026-11-10 — File pickers for every image

- Replaced hero, project, and team image URL/path fields with real file pickers.
- Generalized the profile uploader into ImageUploadService. All new image files
  go to wwwroot/uploads/images; SQL keeps only generated relative paths.
- Kept existing images on ordinary edits, added removal for optional images, and
  cleaned generated files after replacement/deletion or a failed database save.
- Preserved compatibility with earlier /uploads/profiles images.
- Created a tracked folder placeholder while ignoring uploaded binary files.
- Verified zero-warning/error build and full CRUD/portal integration tests,
  including hero/team/project uploads, preservation/replacement/deletion, profile
  upload/removal, and rejection of a fake image. Test data/files were removed.
- Reran the owner promotion script and verified the requested account is Admin.

## 2026-11-10 — Plain admin and user portals with CRUD

- Added concrete entities/services/controllers/forms for Projects, Offerings
  (Services table), Team, Links, and singleton Studio content.
- Added an admin sidebar and blank Analytics default page. Admin login redirects
  there; regular users still land on Home.
- Added read-only account list/details with active/inactive filters and activation
  forms. No account delete endpoint. Self-deactivation is prevented.
- Added own-profile editing, unique-email checks, validated photo upload/removal,
  and current-password-verified password changes. Account IDs/roles cannot be
  changed through profile forms.
- Added password-version cookie checks so password changes invalidate existing
  sessions. Name changes refresh the cookie name on the next request.
- Added plain database-backed landing output for published content and account
  navigation. Kept analytics calculations and visual design deferred.
- Mapped all existing tables; no schema changes or migrations were applied.
- Build and existing account tests passed. Test-Portals.ps1 exercised all content
  CRUD, permissions, activation, profile/photo/password operations and cleanup.
  No real owner account or existing content was edited by tests.

## 2026-11-10 — Owner account promoted

- Added scripts/Promote-Admin.ps1 and a scripts README for local maintenance.
- The script uses User Secrets and a parameterized query, updates only the
  matching email's role, and checks the saved role returned by SQL Server.
- Ran it for the account explicitly named by the user and verified Admin.
- Names, passwords, and activation status were not modified. The owner must log
  in again to refresh the role in the cookie. Signup still creates User accounts.

## 2026-11-10 — Database-backed account forms

- Added User entity, registration/login view models, AccountService, and
  AccountController with conventional Login/Register/Logout actions.
- Added plain Razor sign-up, login, and access-denied views. Landing sections
  remain blank. Account creation always assigns User; admin promotion is deferred.
- Used Microsoft's standalone password hasher and ASP.NET Core cookies, without
  full Identity stores. No database schema changes or seed accounts.
- Added server validation, anti-forgery checks, email normalization/uniqueness,
  password rehash support, and per-request activation/role revalidation.
- Development requires login for the portfolio; public-mode default permits
  browsing. Static assets/account entry pages remain anonymously accessible.
- Build passed with no warnings/errors. Test-Accounts.ps1 passed against SQL,
  covering registration, invalid/duplicate data, role overposting, password
  verification, logout, deactivation, and stale-role cookies. Temporary account
  cleanup completed; no real user was promoted.
- Updated docs and the home smoke check for the school-mode login redirect.
- Verified public-mode anonymous home access and the updated home smoke check.
- Reviewed login/signup renders at desktop and mobile sizes; tablet renders were
  also captured. Marketing-style inspector warnings about large headlines/media
  are outside the requested plain-form scope.
- Confirmed no temporary test accounts remained and stopped the test server and
  project Node workers. No admin account was promoted.

## 2026-11-10 — Blank landing-page scaffold

- Removed visible landing content and layout styling for later frontend work.
- Left empty hero, about, projects, services, contact, and team sections in
  Views/Home/Index.cshtml, with navigation/footer placeholders in the layout.
- Preserved database loading, model, Tailwind setup, and keyboard skip link.
- Updated the smoke check to expect section containers rather than hero copy.
- Scope is landing page only; no admin/account pages or database changes.

## 2026-11-10 — Agreed MVC implementation conventions

- Documented conventional /Controller/Action routing, method names as action
  names, GET links and POST forms, CRUD naming, and view discovery.
- Recorded explicit constructor injection, scoped concrete services, controller/
  service/data responsibilities, and server-rendered Razor as project conventions.
- Linked the guide from the documentation index and architecture guide.
- Documentation-only update; verified local links without repeating app tests.

## 2026-11-10 — Explicit constructors for learning

- Replaced primary constructors in HomeController and HomeService with explicit
  constructors and private readonly fields, making received dependencies visible.
- Dependency injection and request/database behavior remain unchanged.
- Verified compilation using the separate verification output folder.

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

- Refined the home hero against Studio.Design: viewport-based diagonal gallery, square image-color panels, fixed frosted-glass navigation, and dedicated GSAP motion code. Kept existing database content and other pages. Added resize-continuity coverage to the motion check.

- Extended the reference-led landing layout to Services (including optional image upload), the two-row website showcase, tall grayscale people cards, and the dark grouped footer. Updated section ordering and regression coverage; applied the non-destructive Services.ImagePath column upgrade.

- Created eight distinct navigation-and-hero-only HTML samples, captured optimized WebP images, and used them in the main hero without updating project records. Replaced generated people with empty spaces/still-life assets. Added a short GSAP entrance sequence with reduced-motion support. Sources and prompts: docs/hero-assets.md.

- Matched reference sizes for the hero, nav, and Services; removed the visible gallery pause button; aligned image/square rows and centered their diagonal stripe. Added a proper SVG slash wordmark. Replaced the staggered showcase with three equal columns using taller captures, centered the team heading and portrait grid, and generated eight service WebP images. Saved ImagePath values in seed JSON and the eight local sample service records; verified upload/replacement CRUD and removed unused SVG service artwork.

- Shortened the database/seed hero copy to a two-line headline and compact description. Matched the reference reveal curve and heading → navigation/copy → gallery sequence; increased the loop speed and removed hover/focus pauses. Changed service CTAs to Learn more with matching dimensions and cropped the featured artwork flush into the card edge.

- Hero reload: hide content before the first paint, then wait 0.5 seconds before the existing entrance sequence. Reduced motion shows immediately; a four-second fallback reveals content if GSAP fails.

- Added SEO Setup as the ninth sample service with /images/services/seo-setup.webp, seed JSON and database row. Added separate section-animation.js for 1.6-second viewport reveals (32px rise, wall tiles at 110% scale with stagger), respecting reduced motion. Tightened the shared logo slash.

- Showcase projects now render in groups of six, each a three-column/two-row desktop wall, with the studio badge between groups. Empty slots keep the last group aligned and fill automatically as projects are published; mobile hides empty slots.

- Mobile showcase keeps three columns in a centered 144%-wide wall, clipping the outer cards at the viewport edges. Empty slots remain aligned and the badge stays centered between groups.
