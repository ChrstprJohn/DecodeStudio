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

Adds eight entries per collection and demo-user set, plus one studio record and
local sample images. Preserves existing matching data and your admin account.
See [sample-data documentation](../docs/sample-data.md) for demo login details.

## Checks

- Test-Home.ps1: check the running landing/login page and stylesheet.
- Test-Accounts.ps1: exercise authentication with a temporary account and remove
  that test account afterward. See ../docs/authentication.md for setup.
- Test-Portals.ps1: verify content CRUD, admin access, account activation, profile,
  photo, and password changes using temporary data. See ../docs/features.md.
- `node scripts/Test-Design.mjs`: verify gallery pause/reduced-motion/visibility
  behavior and mobile-menu closing without starting a browser or changing data.

- Added `Add-ServiceImages.ps1` (idempotent local schema upgrade) and `Set-ServiceSampleImages.ps1` (backfill generated service artwork paths). The service image upgrade is already applied locally.

