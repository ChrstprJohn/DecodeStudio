# Decode Studio website plan

Status: draft for user review, not implemented. This document records the website
scope and proposed database before entity classes or migrations are created.

## Business and audience

The website represents DCD / Decode Studio, a web design and development studio.
Use Decode Studio as the working display name; branding can be refined later.
The site showcases completed websites, explains services, introduces the owner
and team, and provides contact information.

## Website content

Start with one landing page containing these sections. Separate content pages
can be added if the amount of content eventually justifies them.

| Section | Content managed by the admin | Assignment requirement |
| --- | --- | --- |
| Hero | Heading, introduction, optional image, and primary link | Business/company introduction |
| About | Studio description and business information | About the Business/Company |
| Website showcase | Website/project title, description, preview image, and live website link | Products |
| Services | Web design, web development, deployment, and other actual services | Services |
| Contact | Display email, phone if provided, and contact/social links | Contact Information |
| Owner and team | Owner/team names, roles, biographies, and photos | Owners Information |
| Footer | Studio identity, contact/social links, and section navigation | Supporting website content |

The website showcase is a portfolio of work, not an ecommerce store. No cart,
checkout, payments, or product inventory is planned. Contact information is in
scope; a contact form and message storage have not been requested.

## Accounts and permissions

- Admin owns the studio website and can edit all website content.
- Admin can list and view accounts, deactivate/reactivate users, and see
  dashboard statistics. Deactivation is the initial user-removal approach.
- Admin views other users' profile information without editing their personal
  details. Passwords are never displayed.
- Registered users can edit only their own profile, including separate name fields,
  profile image, and password through the custom account service.
- Account profiles are separate from the public owner/team entries. A visitor
  registering an account does not become a studio team member.
- An inactive account must lose access, including an existing signed-in session.

Use a custom Users table, ASP.NET Core cookie authentication, and Microsoft's password hasher. Each account has an Admin or User role; full Identity account management is no longer planned.

## School demo and public deployment

There are two intended access modes:

| Mode | Portfolio content | Profile and admin functions |
| --- | --- | --- |
| Local school demo | Login required, except registration/login and required public assets | Login required; admin functions require the Admin role |
| Public deployment | Visitors can browse without an account | Remain protected; admin functions require the Admin role |

Plan one explicit configuration setting, `Portfolio:RequireLogin`, to control
access to portfolio content. Set it to `true` for the local school demo and
explicitly to `false` for the public deployment. This is a proposed setting and
does not exist in the application yet.

Enforce this on the server. Hiding login buttons or navigation alone does not
change access permissions. Public mode must never disable admin authorization
or allow anonymous profile editing. Login can remain reachable for the owner
even if it is not prominent in the public navigation.

The assignment requires login before viewing the website. Demonstrate and submit
that mode for grading; public deployment is a separate intended configuration.

## Proposed database

Use one application database with one Users table and a small set of
content tables. Final SQL types, lengths, and validation rules will be decided
with the entity implementation. The names below are planned entity names.

### Users

Store accounts in one table:

| Field | Purpose |
| --- | --- |
| Id, Email, PasswordHash, Role | Generated account ID, unique email, hashed password, and Admin/User role |
| FirstName, MiddleName, LastName | Separate profile name fields; middle name optional |
| ProfileImagePath | Optional uploaded profile-image reference |
| IsActive | Admin-controlled account access |
| CreatedAtUtc | Registration date for account statistics |

The custom account service handles login and password changes. No separate role, claim, token, or external-login tables are needed.

### SiteContent — one studio record

| Fields | Purpose |
| --- | --- |
| Id | Primary key; application maintains one studio record |
| StudioName, Tagline | Studio identity and short description |
| HeroTitle, HeroDescription, HeroImagePath | Landing introduction; image optional |
| HeroLinkText, HeroLinkUrl | Primary landing link |
| AboutTitle, AboutDescription | Business/company section |
| ContactEmail, ContactPhone | Display contact information; phone optional |
| FooterText | Optional footer copy |

Keep shared contact details here and reuse them in the contact section/footer.
Do not duplicate them in multiple records. Fixed section navigation can remain
in Razor markup; this scope does not require a full page-builder system.

### Project — repeatable website showcase entries

| Field | Purpose |
| --- | --- |
| Id | Primary key |
| Title, Description | Showcase information |
| ImagePath | Website preview image |
| WebsiteUrl | Link to the actual website |
| SortOrder | Admin-selected display order |
| IsPublished | Whether visitors can see the entry |

### Service — repeatable service entries

| Field | Purpose |
| --- | --- |
| Id | Primary key |
| Title, Description | E.g. web design, development, or deployment |
| SortOrder | Display order |
| IsPublished | Whether visitors can see the entry |

### TeamMember — owner and team information

| Field | Purpose |
| --- | --- |
| Id | Primary key |
| FirstName, MiddleName, LastName, RoleTitle, Bio | Owner/team introduction; middle name optional |
| PhotoPath | Optional member photo |
| IsOwner | Identifies the owner entry or entries |
| SortOrder | Display order |
| IsPublished | Whether visitors can see the entry |

These entries do not need account IDs. Public team information is managed by the
admin independently of registered user accounts.

### SocialLink — shared contact links

| Field | Purpose |
| --- | --- |
| Id | Primary key |
| Label, Url | E.g. GitHub, LinkedIn, or another studio contact link |
| SortOrder | Display order |

Reuse these links in contact and footer areas. Separate team-member social links
are outside the initial scope.

### Relationships and file handling

Each account has one Role value. The studio content tables belong
to this single website and do not need artificial relationships to user accounts
or a multi-company structure.

Store image references in SQL, rather than image bytes. The actual file storage
location will be selected during upload implementation. Validate file size/type,
generate safe filenames, and ensure deployed storage persists uploaded files.
Validate website/social URLs before rendering them as links.

## Analytics

Start the admin dashboard with database-derived counts:

- Total, active, and inactive registered users.
- Registrations grouped by date using `CreatedAtUtc`.
- Published project and service counts.

These do not need an analytics table. Traffic metrics such as visits, unique
visitors, referrers, and project-link clicks require separate tracking and are
not part of the initial schema. Their scope remains to be decided; account
counts must not be presented as visitor analytics.

## Implementation sequence

1. Inspect the existing Docker SQL Server schema without modifying it. Decide
   whether to use a separate database or integrate with existing tables.
2. Implement the agreed entities, EF context, and custom cookie authentication.
3. Run the reviewed table script manually, then map EF to the existing tables. Do not apply an initial create-tables migration over them.
4. Implement accounts, roles, deactivation enforcement, and the two access modes.
5. Implement admin editing for studio content, showcase, services, team, and links.
6. Render the landing page from saved content and implement profile editing.
7. Add the initial dashboard counts and verify permission boundaries.

The reviewable SQL table script is in [database/schema.sql](database/schema.sql),
with exact types and naming explained in [database/README.md](database/README.md).
The [page inventory](pages.md) lists the planned screens and sections.
The SQL has not been executed; no migration, connection, or feature is implemented.
