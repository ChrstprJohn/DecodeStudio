# Database schema

Status: prepared for your manual execution, not run against your server.
[Open schema.sql](schema.sql). This replaces the earlier Identity-based draft.

## Run

Execute the whole script in a SQL Server editor supporting GO batches, such as
SSMS. It creates the database `portfolio` if missing, selects it, and creates six
tables in dbo. Your SQL login needs database-creation permission if it is missing.
It does not change/drop existing tables, insert content, or create accounts. Existing tables
with these names will cause errors; this is not a rerunnable migration script.

## Tables

| Table | Purpose |
| --- | --- |
| Users | Accounts, profile names/photo, role, activation, registration date |
| SiteContent | One studio record: hero, About, contact details, footer |
| Projects | Website showcase images, descriptions, and live website links |
| Services | Studio services |
| TeamMembers | Public owner/team names, roles, biographies, and photos |
| SocialLinks | Shared studio contact/social links |

## Users columns

| Column | SQL type | Notes |
| --- | --- | --- |
| Id | int IDENTITY | Generated primary key |
| FirstName | nvarchar(100) | Required |
| MiddleName | nvarchar(100) | Optional |
| LastName | nvarchar(100) | Required |
| Email | nvarchar(256) | Required and unique |
| PasswordHash | nvarchar(500) | Microsoft's password hasher supplies this |
| ProfileImagePath | nvarchar(500) | Optional image reference |
| Role | nvarchar(20) | App supplies Admin or User |
| IsActive | bit | App supplies activation status |
| CreatedAtUtc | datetime2(7) | App supplies registration date in UTC |

One role per account is sufficient. There are no Identity role/claim/token tables.
Use ASP.NET Core cookie authentication and Microsoft's password hasher without
full Identity account management. The app will implement email normalization,
role validation, password changes, and deactivation/session enforcement.

## Conventions and relationships

- PascalCase descriptive names: FirstName rather than fname.
- Both Users and TeamMembers store FirstName, optional MiddleName, and LastName.
  Assemble full names for display rather than storing duplicate combined names.
- Primary keys use Id; collection tables have plural names.
- SiteContent is intended for one record with Id = 1, maintained by the app.
- UTC dates end in Utc; booleans use Is prefixes.
- Image columns hold file references, not binary image data.

These are independent collections for one studio. Team members are content, not
registered accounts, so no foreign keys are needed between these six tables.
The script has CREATE TABLE statements, primary keys, and email uniqueness.
SQL Server automatically indexes primary/unique keys; there are no separate
CREATE INDEX statements, default values, or check constraints.

## Access and analytics

School mode requires login to browse; public mode allows portfolio browsing
while keeping admin/profile operations protected. This will be controlled by
configuration, with no table changes needed between modes.
Initial analytics use account/content counts and registration dates. Visitor
tracking is not included.

## Next: connect EF Core

Map entities to these existing tables after manual execution. Do not apply an
initial create-tables migration over them. A migration baseline can be added
later if migrations are adopted; this script creates no EF migration history.
