# Local maintenance scripts

Run scripts from the repository root in PowerShell. Database scripts read the
local portfolio connection from .NET User Secrets; no passwords are stored here.

## Promote a registered account

```powershell
./scripts/Promote-Admin.ps1 -Email "your-account@example.com"
```

Updates only the matching account's Role to Admin and verifies the returned row.
It does not create an account or change its password, name, or activation status.
Running it again for an existing admin is safe. Log in again afterward because
role changes invalidate the previous login cookie.

## Populate sample data

```powershell
./scripts/Seed-SampleData.ps1
```

Populates empty website content tables. See [sample-data documentation](../docs/sample-data.md).

## Upgrade an existing deployment database

Back up the VM database and run [upgrade-public-portfolio.sql](database/upgrade-public-portfolio.sql)
before deploying the updated app. For a fresh database, use
[schema.sql](../docs/database/schema.sql).

## Checks

- Test-Home.ps1: check the public landing page, stylesheet, and admin sign-in routes.
- Test-Accounts.ps1 and Test-Portals.ps1 exercise the school variant; run them on the school branch against a test database.
- `node scripts/Test-Design.mjs`: verify gallery pause/reduced-motion/visibility
  behavior and mobile-menu closing without starting a browser or changing data.

- Added `Add-ServiceImages.ps1` (idempotent local schema upgrade) and `Set-ServiceSampleImages.ps1` (backfill generated service artwork paths). The service image upgrade is already applied locally.

