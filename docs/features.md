# Functional admin and user portals

The current UI uses the shared gray/white/charcoal design language while preserving
working MVC forms, database operations, authorization, and navigation.

## Entry points

- Admin login redirects to /Admin, the blank analytics page with a sidebar.
- User login redirects to /. Shared navigation offers My portal and logout.
- /Profile lets a signed-in user edit their own account, including admins.
- School mode requires portfolio login; public mode permits anonymous browsing.
  Admin and profile operations remain protected in both modes.

## Content endpoints

All content-management controllers require the Admin role.

| Controller / URL prefix | Content | Service |
| --- | --- | --- |
| Studio | Studio identity, hero, About, contact, footer | SiteContentService |
| Projects | Website showcase | ProjectService |
| Offerings | Service entries in the Services table | ServiceOfferingService |
| Team | Owner/team information | TeamMemberService |
| Links | Studio social/contact links | SocialLinkService |

Projects, Offerings, Team, and Links each implement:

| Method and path | Behavior |
| --- | --- |
| GET /Controller/Index | List records, including drafts |
| GET /Controller/Create | Display an empty form |
| POST /Controller/Create | Validate and insert |
| GET /Controller/Edit/{id} | Read the record into the editing form |
| POST /Controller/Edit | Validate and update the specified record |
| GET /Controller/Delete/{id} | Confirm without deleting |
| POST /Controller/Delete | Delete only with confirmed=true |

Replace Controller with the concrete name above. These are conventional MVC
routes, not JSON APIs. POST forms require anti-forgery tokens. Missing records
return 404; invalid input redisplays the form with errors.

Studio is a singleton: GET /Studio shows its form; POST /Studio/Edit creates or
updates row 1. GET /Studio/Delete confirms clearing it; POST /Studio/Delete clears
that record with confirmed=true. Other content tables are unaffected.

## Image files

All image fields use file pickers: studio hero, project preview, team photo, and
profile photo. ImageUploadService copies PNG/JPEG/WebP files up to 2 MB into
MyPortfolio/wwwroot/uploads/images using generated filenames. SQL stores only
the relative path. No editable image URL/path fields remain.

Projects require an image on creation. Hero/team photos are optional and can be
removed. Editing without a new file preserves the current image. Replacing or
deleting a record removes its generated image after the database save succeeds;
failed saves remove the new file. Legacy profile-image paths remain supported.
Seed SVGs use stable shared names and are retained when replacing demo content.

Uploads are public assets and excluded from Git. Back up images with the database
and provide persistent storage on deployment. Static-file middleware serves files
created after builds. Website/social links accept http/https; hero links also
allow a local /path or #section. Razor encodes saved text.

## Landing reads and section skeleton

HomeService reads SiteContent and the content collections. Projects, services,
and team include only published entries, ordered by SortOrder then Id. Social
links use the same ordering. Drafts stay in admin screens.

Home/Index.cshtml composes these partials under Views/Home/Sections:

1. _Hero.cshtml
2. _About.cshtml
3. _Services.cshtml
4. _Projects.cshtml (products / website showcase)
5. _Contact.cshtml
6. _Team.cshtml

Each receives HomeViewModel, with no database queries in the view. Containers and
headings stay available when data is empty. Optional images/links are conditional.
The shared layout adds section anchor links on the landing page and renders the
footer afterward. The public sections use editorial headings, alternating light/dark fields, and image-led layouts.
See [pages.md](pages.md) for the section-to-file and data mapping.

## Account management

- GET /Accounts: all users, with active/inactive filters.
- GET /Accounts/Details/{id}: read-only names, email, photo, role, status, and UTC
  registration date. Password hashes are never included in the view model.
- POST /Accounts/SetActive: activate/deactivate the selected account.
- Admins cannot deactivate themselves. Status changes invalidate affected cookies
  on the next authenticated request.
- No account delete action, personal-detail editing of other users, or in-app role
  assignment exists. Use the local promotion script for the selected owner account.

## User portal

- GET /Profile: own names, email, and photo.
- POST /Profile/Edit: validate/save only the signed-in user's data. Email remains
  unique. Submitted account IDs, roles, status, and image paths are ignored.
- Profile photos can be uploaded, replaced, or removed.
- GET/POST /Profile/ChangePassword: verify the current password and hash the new
  password, then sign out. Password-version claims invalidate other sessions on
  their next request, without an extra database column.

## Organization and verification

Controllers call concrete services; services perform EF operations. Form view
models accept permitted fields; entities map the existing six tables. The shared
_AdminLayout supplies the sidebar. No generic CRUD controllers, repository layer,
new schema tables, startup migrations, or automatic content seeding were added.

With the Development test server on port 5041, run scripts/Test-Portals.ps1 from
the root. It creates two uniquely named temporary accounts and content entries,
tests CRUD, publication, role boundaries, activation, profile/photo/password
operations and session revocation, then removes its records/files. Studio write
tests run only if row 1 is initially absent, preserving existing studio content.
The script captures rendered HTML under ignored .taste-skill/portals for checks.

Test-Accounts.ps1 covers authentication regression. Test-Home.ps1 checks the
landing section order/anchors or school-mode login form, plus CSS. Sample data
is added only by the explicit [seed script](sample-data.md).

Analytics calculations, password recovery, email verification, login throttling,
and disabling public registration for deployment remain separate work.
