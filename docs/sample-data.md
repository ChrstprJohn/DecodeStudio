# Sample website content

Run from the repository root in PowerShell 7:

```powershell
./scripts/Seed-SampleData.ps1
```

The script uses the local portfolio connection from .NET User Secrets. It seeds
empty content tables from `scripts/seed/sample-data.json`: Website Content,
Product Showcase, Services, Team, and Social Links. Populated tables and all
accounts are left alone. Writes run in a single transaction.

Project and service images are stored under `MyPortfolio/wwwroot/images/` and
referenced by path in the seed. Uploaded images are stored separately under
`MyPortfolio/wwwroot/uploads/images/`. The seed does not require uploaded files
or generate demo avatars. Team entries without photos use the site's fallback.
