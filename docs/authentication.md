# Admin sign-in

Visitors can browse the portfolio anonymously. Open /admin directly to reach
the admin sign-in form at /admin/login. The public navigation has no account
links. Only active Admin accounts can sign in; registration and account
management routes are absent on this branch.

The admin sidebar includes profile editing, password changes, and logout.
Admin sessions use a separate cookie from the school site. The app checks
the account's role, active status, and password version on each authenticated
request. Login and other POST forms require antiforgery tokens.

Use the existing Admin account from the configured SQL database. If you need
to create an admin for a new database, register it through the school branch
and run [Promote-Admin.ps1](../scripts/README.md) against that database.

See the [branch workflow](branch-workflow.md) for database sharing and how to
carry shared changes across.
